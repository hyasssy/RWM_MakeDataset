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
    string DownloadsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Downloads/";
    [SerializeField, Tooltip("Downloadsフォルダの中に下記名のフォルダを用意し、mp4をいれておく")]
    string folderName = "data";
    string FolderPath => DownloadsPath + folderName + "/";
    TownVideoDataJson _townVideoData = new TownVideoDataJson();
    
    async UniTask<TownVideoDataJson> MakePrimaryDataJSON(InputDatas inputDatas){
        _townVideoData.playArea = inputDatas.playArea;
        _townVideoData.edittedAt = TimeStampExt.DT2TS(DateTime.Now);
        // _townVideoData.originLocation = originLocation;//TODO
        // for(){
        //     var value = await MakeStreetVideoJSON(
        //         inputDatas
        //     );
        //     _townVideoData.videos.Add(value);
        // }
        // ここforでAddしていく
        return _townVideoData;
    }
    async UniTask<StreetVideoJson> MakeStreetVideoJSON(
        string videoFilePath,
        string streetId,
        string latLng,
        TimeStamp shootedAt,
        TimeZone timeZone,
        Weather weather,
        LocationLogJson locationLog
        // InputDatas inputDatas
    ){
        var streetVideoData = new StreetVideoJson();
        streetVideoData.streetId = streetId;
        var videoId = GetUniqueVideoId(_townVideoData, streetId);
        streetVideoData.videoId = videoId;
        var fileNameClass = new VideoFileNameJson();
        fileNameClass.standardFileName = videoId + ".mp4";
        fileNameClass.lightFileName = videoId + "_light.mp4";// これもさくっと作っておく。
        streetVideoData.fileName = fileNameClass;
        streetVideoData.shootedAt = shootedAt;
        streetVideoData.timeZone = timeZone;
        streetVideoData.weather = weather;
        streetVideoData.frameLength = await CountVideoFrame(FolderPath + videoId + ".mp4");
        streetVideoData.flag = true;
        // streetVideoData.locationLogs =
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
}