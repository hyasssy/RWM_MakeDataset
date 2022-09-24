using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using System.Linq;



public class GeneratePosData : MonoBehaviour
{
    [SerializeField]
    GetInputs _getInputs;
    [SerializeField]
    InputDatas _videoProperties;
    string _downloadsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Downloads/";
    [SerializeField, Tooltip("Downloadsフォルダの中に下記フォルダを用意し、mp4をいれる")]
    string _folderName = "data";
    string _folderPath => _downloadsPath + _folderName + "/";

    VideoPlayer vp;

    public async void ExecFunction(){
        if(!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);


        var streetsJsonData = new StreetsJSON();

        var fileNames = Directory.GetFiles(_folderPath);
        var videoFileNames = new List<string>(); //フォルダ内のmp4を全部取得
        fileNames.ToList().ForEach(fileName =>
        {
            if(fileName.Contains("mp4")){
                var nameWithoutFormat = System.IO.Path.GetFileNameWithoutExtension(fileName);
                videoFileNames.Add(nameWithoutFormat);
            }
        });

        var inputsDict = _getInputs.GetInputsFunction();

        for (int i = 0; i < inputsDict.Count - 1;i++){
            var streetJsonData = await MakeNewStreetJSON(
                videoFileNames[i],
                _folderPath,
                _videoProperties,
                inputsDict[videoFileNames[i]].lat,
                inputsDict[videoFileNames[i]].lng,
                inputsDict[(Int32.Parse(videoFileNames[i])+1).ToString()].lat,
                inputsDict[(Int32.Parse(videoFileNames[i])+1).ToString()].lng
            );

            streetsJsonData.street.Add(streetJsonData);
        }


        var json = JsonUtility.ToJson(streetsJsonData, true);
        Debug.Log(json);
        var jsonFilePath = _folderPath + "result/" + _videoProperties.playArea + ".json";
        StreamWriter writer = new StreamWriter(jsonFilePath, false);//初めに指定したデータの保存先を開く
        writer.WriteLine(json);//JSONデータを書き込み
        writer.Flush();//バッファをクリアする
        writer.Close();//ファイルをクローズする
    }

    async UniTask<int> CountVideoFrame(string url){
        vp = this.gameObject.AddComponent<VideoPlayer>();
        vp.url = url;
        while(true){
            await UniTask.Yield();
            if(vp.frameCount != 0) return (int)vp.frameCount;
        }
    }
    async UniTask<SingleStreetJSON> MakeNewStreetJSON( //新しいものを作る。更新とかは考えてない
        string fromNum, //撮影番号。最初だからファイル名
        string folderPath,
        InputDatas vProperties,
        double startLat,
        double startLng,
        double endLat,
        double endLng
        ){

        // videoファイルコピーしてguid作成
        var uuid = System.Guid.NewGuid().ToString();
        var newName = uuid + "_01.mp4";
        var dPath = folderPath + "result/";
        if(!Directory.Exists(dPath)) Directory.CreateDirectory(dPath);
        File.Copy(folderPath + fromNum + ".mp4", dPath + newName);

        // 道の情報
        var streetJSON = new SingleStreetJSON();
        streetJSON.streetId = uuid;
        streetJSON.playArea = vProperties.playArea;
        streetJSON.edittedAt = TimeStampExt.DT2TS(DateTime.Now);
        streetJSON.streetNum = fromNum + "-"+(Int32.Parse(fromNum)+1).ToString();

        // とくにその撮影したものの情報
        var streetVideoJSON = new StreetVideoJSON();
        streetVideoJSON.fileName = newName;
        streetVideoJSON.shootedAt = vProperties.shootedAt;
        streetVideoJSON.timeZone = vProperties.timeZone.ToString();
        streetVideoJSON.weather = vProperties.weather.ToString();
        streetVideoJSON.frameLength = await CountVideoFrame(folderPath + fromNum + ".mp4");
        streetVideoJSON.flag = true;
        // 位置を補完して流し込み
        var posLog = GetPosLogList(startLat, startLng, endLat, endLng, streetVideoJSON.frameLength);
        streetVideoJSON.posLog = posLog;

        streetJSON.video = streetVideoJSON;

        return streetJSON;
    }
    List<LocationJSON> GetPosLogList(double startLat, double startLng, double endLat, double endLng, int frameCount){
        var resultList = new List<LocationJSON>();
        for (var i = 0; i < frameCount; i++)
        {
            var result = new LocationJSON();
            result.frameNumber = i;
            result.lat = startLat + (endLat - startLat) / (frameCount - 1) * i;
            result.lng = startLng + (endLng - startLng) / (frameCount - 1) * i;
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