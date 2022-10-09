using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.VisualScripting;

namespace RWM.VR
{


    public class MakeSearchGridData : MonoBehaviour
    {
        string DownloadsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Downloads/";
        string DataFolderPath => DownloadsPath + "data/";
        private string VideosFolderPath => DataFolderPath + "videos/";
        private string ResultFolderPath => DataFolderPath + "search/";
        private string ResultIndexFilePath => ResultFolderPath + "searchIndex.json";

        /// <summary>
        /// Execボタンから起動
        /// </summary>
        public void Execute(string playAreaName)
        {
            var townVideoData = ReadPlayAreaJson(playAreaName);
            var videoIdList = GetVideoIdList(townVideoData);
            var allVideoData = GetAllVideoData(videoIdList);

            // まず全部見て、最小cellのリストを作る
            var cellList = MakeCellList(allVideoData);
            // 次にregionのリストを作る
            var regionList = MakeRegionList(cellList);

            var searchIndex = new SearchIndexJson();
            searchIndex.playArea = townVideoData.playArea;
            searchIndex.edittedAt = TimeStampExt.DT2TS(DateTime.Now);
            // jsonにまとめあげる
            regionList.ForEach(region =>
            {
                searchIndex.searchRegions.Add(region.regionId); // Indexに追加
                // regionの個数、jsonファイルを作成する
                var regionJsonData = new CellsInRegionJson();
                regionJsonData.cells = region.cells;
                var jsonString = JsonUtility.ToJson(regionJsonData, true);
                if (!Directory.Exists(ResultFolderPath)) Directory.CreateDirectory(ResultFolderPath);
                SaveJson(jsonString, ResultFolderPath + region.regionId + ".json");
            });
            var jsonString = JsonUtility.ToJson(searchIndex, true);
            SaveJson(jsonString, ResultIndexFilePath);
        }

        //TODO なんか美しい構造にできないかなあ~~~ちょっとわかりづらい
        List<CellJson> MakeCellList(List<VideoMetaJson> allData)
        {
            var result = new List<CellJson>();
            allData.ForEach(singleVideoMetaData =>
            {
                singleVideoMetaData.locationLogs.ForEach(locationLog =>
                {
                    //全部のデータの緯度経度情報の数の分処理
                    var cellId = GetCellId(locationLog.lat, locationLog.lng);
                    bool flag = false;
                    if (result.Count != 0)
                    {
                        result.ForEach(resultCellData =>
                        {
                            // もし存在してたら、その要素に位置情報を追加する
                            if (resultCellData.cellId == cellId)
                            {
                                flag = true;
                                AddLocationData(ref resultCellData.videoFrameList, singleVideoMetaData.videoId,
                                    locationLog.frameNumber);
                            }
                        });
                    }

                    if (!flag)
                    {
                        // なかったら作って位置情報を書き込む
                        var cell = new CellJson();
                        cell.cellId = cellId;
                        AddLocationData(ref cell.videoFrameList, singleVideoMetaData.videoId, locationLog.frameNumber);

                        result.Add(cell);
                    }
                });
            });
            return result;
        }

        void AddLocationData(ref List<VideoFrameData> resultCell, string videoId, int frameNumber)
        {
            bool flag = false;
            if (resultCell.Count != 0)
            {
                resultCell.ForEach(videoInCell =>
                {
                    // videoIdが存在していたら、その要素に追加
                    if (videoInCell.videoId == videoId)
                    {
                        flag = true;
                        videoInCell.frameNumbers.Add(frameNumber);
                    }
                });
            }

            if (!flag)
            {
                // videoIdがまだ存在していなかったら、作ってNumberも追加
                var videoFrameData = new VideoFrameData();
                videoFrameData.videoId = videoId;
                videoFrameData.frameNumbers.Add(frameNumber);
                resultCell.Add(videoFrameData);
            }
        }

        List<SearchRegion> MakeRegionList(List<CellJson> cellList)
        {
            var result = new List<SearchRegion>();
            // RegionがなければRegionを作って要素を追加、あればそこに要素を追加
            cellList.ForEach(cell =>
            {
                var regionId = GetRegionId(cell.cellId);
                bool flag = false;
                if (result.Count != 0)
                {
                    result.ForEach(resultRegion =>
                    {
                        if (resultRegion.regionId == regionId)
                        {
                            // すでにregionがある
                            flag = true;
                            resultRegion.cells.Add(cell);
                        }
                    });
                }
                if (!flag)
                {
                    // まだregionがないので作って追加
                    var newRegion = new SearchRegion();
                    newRegion.regionId = regionId;
                    newRegion.cells.Add(cell);
                    result.Add(newRegion);
                }
            });
            return result;
        }

        string LatLngToString(double lat, double lng)
        {
            return lat.ToString() + "_" + lng.ToString();
        }

        (double lat, double lng) StringToLatLng(string latLng)
        {
            var firstLength = latLng.IndexOf("_"); // ,があるindexを取得
            var lat = double.Parse(latLng.Substring(0, firstLength));
            var lng = double.Parse(latLng.Substring(firstLength + 1, latLng.Length - firstLength - 1));
            return (lat, lng);
        }

        (double flooredLat, double flooredLng) GetFlooredLatLng(double lat, double lng, int digits)
        {
            var flooredLat = Math.Floor(lat * digits) / digits;
            var flooredLng = Math.Floor(lng * digits) / digits;
            return (flooredLat, flooredLng);
        }

        string GetCellId(double lat, double lng)
        {
            var floored = GetFlooredLatLng(lat, lng, GridDigits.CELL_DIGITS);
            return LatLngToString(floored.flooredLat, floored.flooredLng);
        }

        string GetRegionId(double lat, double lng)
        {
            var floored = GetFlooredLatLng(lat, lng, GridDigits.REGION_DIGITS);
            return LatLngToString(floored.flooredLat, floored.flooredLng);
        }
        string GetRegionId(string latLngString)
        {
            var (lat, lng) = StringToLatLng(latLngString);
            var floored = GetFlooredLatLng(lat, lng, GridDigits.REGION_DIGITS);
            return LatLngToString(floored.flooredLat, floored.flooredLng);
        }

        string GetFlooredLatLngString(double lat, double lng, int digits)
        {
            var floored = GetFlooredLatLng(lat, lng, digits);
            return LatLngToString(floored.flooredLat, floored.flooredLng);
        }


        TownVideoDataJson ReadPlayAreaJson(string playAreaName)
        {
            StreamReader reader = new StreamReader(DataFolderPath + playAreaName + ".json"); //受け取ったパスのファイルを読み込む
            var datastr = reader.ReadToEnd(); //ファイルの中身をすべて読み込む
            reader.Close(); //ファイルを閉じる
            return JsonUtility.FromJson<TownVideoDataJson>(datastr);
        }

        List<string> GetVideoIdList(TownVideoDataJson townVideoDataJson)
        {
            var videoIdList = new List<string>();
            townVideoDataJson.videos.ForEach(videoData => { videoIdList.Add(videoData.videoId); });
            return videoIdList;
        }

        List<VideoMetaJson> GetAllVideoData(List<string> videoIdList)
        {
            var metadataList = new List<VideoMetaJson>();
            videoIdList.ForEach(videoId =>
            {
                StreamReader reader = new StreamReader(VideosFolderPath + videoId + ".json"); //受け取ったパスのファイルを読み込む
                var datastr = reader.ReadToEnd(); //ファイルの中身をすべて読み込む
                reader.Close(); //ファイルを閉じる
                var metaData = JsonUtility.FromJson<VideoMetaJson>(datastr);
                metadataList.Add(metaData);
            });
            return metadataList;
        }

        void SaveJson(string json, string path)
        {
            StreamWriter writer = new StreamWriter(path, false); //初めに指定したデータの保存先を開く
            writer.WriteLine(json); //JSONデータを書き込み
            writer.Flush(); //バッファをクリアする
            writer.Close(); //ファイルをクローズする
        }
    }
}