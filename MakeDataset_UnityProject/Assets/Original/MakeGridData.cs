using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using System.Linq;

public class MakeGridData : MonoBehaviour
{
    string _downloadsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Downloads/";
    [SerializeField, Tooltip("Downloadsフォルダの中に下記フォルダを用意し、mp4をいれる")]
    string _folderName = "data";
    string _folderPath => _downloadsPath + _folderName + "/";

    TownVideoDataJSON _townVideoData = new TownVideoDataJSON();
    async UniTask<TownVideoDataJSON> MakePrimaryDataJSON(
        // string playArea,
        // LocationCoordJSON originLocation,
        InputDatas inputDatas
    ){
        _townVideoData.playArea = inputDatas.playArea;
        _townVideoData.edittedAt = TimeStampExt.DT2TS(DateTime.Now);
        // _townVideoData.originLocation = originLocation;//TODO
        for(){
            var value = await MakeStreetVideoJSON(
                inputDatas
            );
            _townVideoData.videos.Add(value);
        }
        // ここforでAddしていく
        return _townVideoData;
    }
    async UniTask<StreetVideoJSON> MakeStreetVideoJSON(
        string videoFilePath,
        string streetId,
        string latLng,
        TimeStamp shootedAt,
        TimeZone timeZone,
        Weather weather,
        LocationLogJSON locationLog
        // InputDatas inputDatas
    ){
        var streetVideoData = new StreetVideoJSON();
        streetVideoData.streetId = streetId;
        var videoId = GetUniqueVideoId(_townVideoData, streetId);
        streetVideoData.videoId = videoId;
        var fileNameClass = new VideoFileNameJSON();
        fileNameClass.originFileName = videoId + ".mp4";
        fileNameClass.lightFileName = videoId + "_light.mp4";// これもさくっと作っておく。
        streetVideoData.fileName = fileNameClass;
        streetVideoData.shootedAt = shootedAt;
        streetVideoData.timeZone = timeZone;
        streetVideoData.weather = weather;
        streetVideoData.frameLength = await CountVideoFrame()
        streetVideoData.flag = true;
        // streetVideoData.locationLogs =
    }
    string GetUniqueVideoId(TownVideoDataJSON videoData, string streetId){
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
}