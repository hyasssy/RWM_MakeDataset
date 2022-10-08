using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.VisualScripting;

public class MakeSearchGridData : MonoBehaviour
{
    string DownloadsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Downloads/";
    string DataFolderPath => DownloadsPath + "data/";
    private string VideosFolderPath => DataFolderPath + "videos/";
    private string ResultFolderPath => DataFolderPath + "search/";

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
        // 次にdistrictsのリストを作る
        var districtsList = MakeDistrictList(cellList);
        // 次にregionのリストを作る
        var regionList = MakeRegionList(districtsList);
        
        // jsonにまとめあげる
        regionList.ForEach(region =>
        {
            var SearchData = new SearchDataJson();
            SearchData.playArea = townVideoData.playArea;
            SearchData.edittedAt = TimeStampExt.DT2TS(DateTime.Now);
            SearchData.districts = region.districtList;
            var jsonString = JsonUtility.ToJson(SearchData, true);
            if (!Directory.Exists(ResultFolderPath)) Directory.CreateDirectory(ResultFolderPath);
            SaveJson(jsonString, ResultFolderPath + region.regionId +".json");
        });
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
                if (result.Count != 0){
                    result.ForEach(resultCellData =>
                    {
                        // もし存在してたら、その要素に位置情報を追加する
                        if (resultCellData.cellId == cellId)
                        {
                            flag = true;
                            AddLocationData(ref resultCellData.videoFrameList, singleVideoMetaData.videoId, locationLog.frameNumber);
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
        // videoFrameData.frameNumbers //ここはまた分解
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
        {// videoIdがまだ存在していなかったら、作ってNumberも追加
            var videoFrameData = new VideoFrameData();
            videoFrameData.videoId = videoId;
            videoFrameData.frameNumbers.Add(frameNumber);
            resultCell.Add(videoFrameData);
        }
    }

    List<CellDistrictJson> MakeDistrictList(List<CellJson> cellList)
    {
        var result = new List<CellDistrictJson>();
        cellList.ForEach(cell =>
        {
            var districtId = GetDistrictId(cell.cellId);
            bool flag = false;
            if (result.Count != 0)
            {
                result.ForEach(resultDistrict =>
                {
                    if (resultDistrict.districtId == districtId)
                    {
                        // 存在してたら、その要素に追加
                        resultDistrict.cells.Add(cell);
                        flag = true;
                    }
                });
            }

            if (!flag)
            {
                // なかったら作って情報書き込む
                var district = new CellDistrictJson();
                district.districtId = GetDistrictId(cell.cellId);
                district.cells.Add(cell);
                result.Add(district);
            }
        });
        return result;
    }

    List<Region> MakeRegionList(List<CellDistrictJson> districtList)
    {
        var result = new List<Region>();
        // Regionがあればそれに追加、なければRegionを作って追加
        districtList.ForEach(district =>
        {
            var regionId = GetRegionId(district.districtId);
            bool flag = false;
            if (result.Count != 0)
            {
                result.ForEach(region =>
                {
                    if (region.regionId == regionId)
                    {// すでにregionがある
                        flag = true;
                        region.districtList.Add(district);
                    }
                });
            }

            if (!flag)
            {// まだregionがないので作って追加
                var region = new Region();
                region.regionId = regionId;
                region.districtList.Add(district);
                result.Add(region);
            }
        });
        return result;
    }

    
    string GetCellId(double lat, double lng)
    {
        // いま位は適当
        var flooredLat = Mathf.Floor((float)(lat * 10000)) / 10000;
        var flooredLng = Mathf.Floor((float)(lng * 10000)) / 10000;
        return flooredLat.ToString() + "_" + flooredLng.ToString();
    }

    string GetDistrictId(string cellId)
    {
        var firstLength = cellId.IndexOf("_"); // ,があるindexを取得
        var lat = float.Parse(cellId.Substring(0, firstLength));
        var lng = float.Parse(cellId.Substring(firstLength + 1, cellId.Length - firstLength - 1));
        lat = Mathf.Floor(lat * 1000) / 1000;
        lng = MathF.Floor(lng * 1000) / 1000;
        return lat.ToString() + "_" + lng.ToString();
    }
    string GetRegionId(string districtId)
    {
        var firstLength = districtId.IndexOf("_"); // ,があるindexを取得
        var lat = float.Parse(districtId.Substring(0, firstLength));
        var lng = float.Parse(districtId.Substring(firstLength + 1, districtId.Length - firstLength - 1));
        lat = Mathf.Floor(lat * 100) / 100;
        lng = MathF.Floor(lng * 100) / 100;
        return lat.ToString() + "_" + lng.ToString();
    }

    TownVideoDataJson ReadPlayAreaJson(string playAreaName)
    {
        StreamReader reader = new StreamReader(DataFolderPath + playAreaName + ".json"); //受け取ったパスのファイルを読み込む
        var datastr = reader.ReadToEnd();//ファイルの中身をすべて読み込む
        reader.Close();//ファイルを閉じる
        return JsonUtility.FromJson<TownVideoDataJson>(datastr);
    }
    List<string> GetVideoIdList(TownVideoDataJson townVideoDataJson)
    {
        var videoIdList = new List<string>();
        townVideoDataJson.videos.ForEach(videoData =>
        {
            videoIdList.Add(videoData.videoId);
        });
        return videoIdList;
    }

    List<VideoMetaJson> GetAllVideoData(List<string> videoIdList)
    {
        var metadataList = new List<VideoMetaJson>();
        videoIdList.ForEach(videoId =>
        {
            StreamReader reader = new StreamReader(VideosFolderPath + videoId + ".json"); //受け取ったパスのファイルを読み込む
            var datastr = reader.ReadToEnd();//ファイルの中身をすべて読み込む
            reader.Close();//ファイルを閉じる
            var metaData = JsonUtility.FromJson<VideoMetaJson>(datastr);
            metadataList.Add(metaData);
        });
        return metadataList;
    }

    void SaveJson(string json, string path)
    {
        StreamWriter writer = new StreamWriter(path, false);//初めに指定したデータの保存先を開く
        writer.WriteLine(json);//JSONデータを書き込み
        writer.Flush();//バッファをクリアする
        writer.Close();//ファイルをクローズする
    }
}
