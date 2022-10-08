using UnityEngine;
using System;
using System.IO;
using UnityEngine.PlayerLoop;

/// <summary>
/// Jsonの読み込みや保存、Renameした動画データの保存、すでにJsonファイルが存在しているかどうかのチェックなどする 
/// </summary>
public class IODataHandler
{
    string DownloadsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Downloads/";
    //Downloadsフォルダの中に下記名のフォルダを用意し、mp4たちをいれておく。すでにjsonがあるならそれも入れる。
    string _originalDataFolderName = "data";
    public string OriginalDataFolderPath => DownloadsPath + _originalDataFolderName + "/";
    private string _resultDataFolderName = "data/result";
    private string ResultDataFolderPath => DownloadsPath + _resultDataFolderName + "/";
    private string ResultVideoFolderPath => ResultDataFolderPath + "videos_mov/";
    private string ResultVideoJsonFolderPath => ResultDataFolderPath + "videos/";
    
    private string _playArea;
    private string PrimaryJsonDataPath => OriginalDataFolderPath + _playArea + ".json";
    private string ResultJsonDataPath => ResultDataFolderPath + _playArea + ".json";
    
    public IODataHandler(string playArea)
    {
        _playArea = playArea;

        int i = 0;
        while (Directory.Exists(ResultDataFolderPath))
        {
            i++;
            _resultDataFolderName = "data/result_" + i.ToString();
        }
        Directory.CreateDirectory(ResultDataFolderPath);
    }

    /// <summary>
    /// すでにjsonが存在しているかをチェックする
    /// </summary>
    public bool CheckExistJson()
    {
        return File.Exists(PrimaryJsonDataPath);
    }

    /// <summary>
    /// 存在しているJsonのデータを取得する。ちゃんとNullチェックしてから使うこと
    /// </summary>
    public TownVideoDataJson GetExistJsonData()
    {
        StreamReader reader = new StreamReader(PrimaryJsonDataPath); //受け取ったパスのファイルを読み込む
        var datastr = reader.ReadToEnd();//ファイルの中身をすべて読み込む
        reader.Close();//ファイルを閉じる
        return JsonUtility.FromJson<TownVideoDataJson>(datastr);
    }

    public void SaveResultJson(TownVideoDataJson mainJson)
    {
        var j = JsonUtility.ToJson(mainJson, true);
        SaveJson(j, ResultJsonDataPath);
    }

    void SaveJson(string json, string path)
    {
        StreamWriter writer = new StreamWriter(path, false);//初めに指定したデータの保存先を開く
        writer.WriteLine(json);//JSONデータを書き込み
        writer.Flush();//バッファをクリアする
        writer.Close();//ファイルをクローズする
    }

    public void SaveRenamedVideo(string originalVideoFileName, string newVideoId, VideoMetaJson videoMetaJson)
    {
        if (!Directory.Exists(ResultVideoFolderPath)) Directory.CreateDirectory(ResultVideoFolderPath);
        if (!Directory.Exists(ResultVideoJsonFolderPath)) Directory.CreateDirectory(ResultVideoJsonFolderPath);
        File.Copy(OriginalDataFolderPath + originalVideoFileName, ResultVideoFolderPath + newVideoId + ".mov");
        
        var j = JsonUtility.ToJson(videoMetaJson, true);
        SaveJson(j, ResultVideoJsonFolderPath + videoMetaJson.videoId + ".json");
    }
}
