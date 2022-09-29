using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine.Serialization;

public class MakeGridData : MonoBehaviour
{
    // すでにjsonがある場合には、追加したものを~_2.jsonとして保存するようにする。（少しずつ作業できるようにする）
    // データがあるか探索し、無い場合やどう見ても重複してるだろ、っていうやつを消すのも欲しい...
    [SerializeField] private GetInputs getInputs;

    TownVideoDataJson _resultTownVideoData = new TownVideoDataJson();
    private TownVideoDataJson _previousTownVideoData;
    private IODataHandler _ioDataHandler;


    // Execボタンから起動
    public void Execute()
    {
        var inputDatas = getInputs.GetInputsFunction();
        _ioDataHandler = new IODataHandler(inputDatas.PlayArea);
        if (_ioDataHandler.CheckExistJson())
        {
            //すでに存在しているので、追加するだけでおけ
            _previousTownVideoData = _ioDataHandler.GetExistJsonData();
            // AddDatas(inputDatas);
        }
        else
        {// まだ存在していないので、新規で作成
            MakePrimaryDataJSON(inputDatas);
        }
    }

    void AddDatas(InputDatas inputDatas)
    {
        _previousTownVideoData.edittedAt =TimeStampExt.DT2TS(DateTime.Now);
        // ここに追加
    }

    void MakePrimaryDataJSON(InputDatas inputDatas)
    {
        _resultTownVideoData.playArea = inputDatas.PlayArea;
        _resultTownVideoData.edittedAt = TimeStampExt.DT2TS(DateTime.Now);
        var tmpOrigin = new LocationCoordJson();
        tmpOrigin.lat = 0;
        tmpOrigin.lng = 0;
        tmpOrigin.height = 0;
        _resultTownVideoData.originLocation = tmpOrigin; // あとで手動で入れる想定
        
        for(int i=0;i<inputDatas.videoDatas.Count;i++)
        {
            var videoData = new StreetVideoJson();
            videoData.streetId = inputDatas.videoDatas[i].streetId;
            videoData.videoId = GetUniqueVideoId(_resultTownVideoData, videoData.streetId);
            videoData.fileName.standardFileName = videoData.videoId + ".mp4";
            videoData.fileName.lightFileName = videoData.videoId + "_light.mp4";
            
            _resultTownVideoData.videos.Add(videoData);
            
            SetEachVideoData(
                inputDatas.videoDatas[i].fileName,
                videoData.videoId,
                inputDatas.videoDatas[i].startLat,
                inputDatas.videoDatas[i].startLng,
                inputDatas.videoDatas[i].endLat,
                inputDatas.videoDatas[i].endLng,
                inputDatas.ShootedAt,
                inputDatas.TimeZone,
                inputDatas.Weather
            ).Forget();
        }
        _ioDataHandler.SaveResultJson(_resultTownVideoData);
    }

    /// <summary>
    /// それぞれのビデオデータをRenameし、metadataを制作する
    /// </summary>
    /// <param name="targetJson"></param>
    /// <param name="videoId"></param>
    /// <param name="shootedAt"></param>
    /// <param name="timeZone"></param>
    /// <param name="weather"></param>
    async UniTask SetEachVideoData(
        string originFileName, 
        string videoId, 
        double startLat,
        double startLng,
        double endLat,
        double endLng,
        TimeStamp shootedAt,
        TimeZone timeZone,
        Weather weather)
    {
        var videoMetaJson = new VideoMetaJson();
        videoMetaJson.videoId = videoId;
        videoMetaJson.shootedAt = shootedAt;
        videoMetaJson.timeZone = timeZone;
        videoMetaJson.weather = weather;
        var frameLength = await CountVideoFrame(_ioDataHandler.OriginalDataFolderPath + originFileName + ".mp4");
        videoMetaJson.frameLength = frameLength;
        videoMetaJson.flag = true;
        videoMetaJson.locationLogs = GetPosLogList(
            startLat,startLng,endLat,endLng,frameLength
        );
        
        _ioDataHandler.SaveRenamedVideo(originFileName, videoId, videoMetaJson);
    }

    string GetUniqueVideoId(TownVideoDataJson videoData, string streetId){
        while(true){
            string tmpName = streetId +"_"+ UnityEngine.Random.Range(0,1000).ToString();
            bool isAlreadyExist = false;
            videoData.videos.ToList().ForEach(video =>
            {
                if(video.videoId == tmpName) isAlreadyExist = true;
            });
            if(!isAlreadyExist) return tmpName;
        }
    }
    async UniTask<int> CountVideoFrame(string videoFilePath){
        var vp = this.gameObject.AddComponent<VideoPlayer>();
        vp.url = videoFilePath;
        while(vp.frameCount == 0){
            await UniTask.Yield();
        }
        int result = (int)vp.frameCount;
        Destroy(vp); //用無しは消しとく。
        return result;
    }
    List<LocationLogJson> GetPosLogList(double startLat, double startLng, double endLat, double endLng, int frameLength){
         var resultList = new List<LocationLogJson>();
         for (var i = 0; i < frameLength; i++)
         {
             var result = new LocationLogJson();
             result.frameNumber = i;
             result.lat = startLat + (endLat - startLat) / (frameLength - 1) * i;
             result.lng = startLng + (endLng - startLng) / (frameLength - 1) * i;
             result.height = 0;
             if (i != 0)
             {
                 var start = CoordinateExt.Coord2Meter(new Coordinate(resultList[i - 1].lat, resultList[i - 1].lng));
                 var end = CoordinateExt.Coord2Meter(new Coordinate(result.lat, result.lng));
                 var direction = new Vector2((float)(end.x - start.x), (float)(end.y - start.y));
                 result.rotation = Vector2.SignedAngle(Vector2.up, direction);
                 Debug.Log("start" + start + ", end" + end + "angle" + result.rotation);
                 if(i == 1){
                     resultList[0].rotation = result.rotation;
                 }
             }
             resultList.Add(result);

         }
         return resultList;
     }
}