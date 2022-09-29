using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


/// <summary>InputFieldの情報を集計する</summary>
public class GetInputs : MonoBehaviour
{
    [SerializeField]
    TMP_InputField playArea;

    [SerializeField] private TMP_InputField timeZone;
    [SerializeField] private TMP_InputField weather;
    [SerializeField] private TMP_InputField shootedAt;

    [SerializeField]
    GameObject fileNameRoot;
    [SerializeField]
    GameObject streetIdRoot;
    [SerializeField]
    GameObject startLatLngRoot;

    [SerializeField] private GameObject endLatLngRoot;


    public InputDatas GetInputsFunction(){
        var streetIds = GetInfosFromRoot(streetIdRoot);
        var fileNames = GetInfosFromRoot(fileNameRoot);
        var startLatLngs = GetInfosFromRoot(startLatLngRoot);
        var endLatLngs = GetInfosFromRoot(endLatLngRoot);
        var list = new List<(string fileName, string streetId, double startLat, double startLng, double endLat, double endLng)>();
        for (var i = 0; i < fileNames.Count;i++)
        {
            var startLatLng = ParseLatLng(startLatLngs[i]);
            var endLatLng = ParseLatLng(endLatLngs[i]);
            list.Add((fileNames[i], streetIds[i], startLatLng.lat, startLatLng.lng, endLatLng.lat, endLatLng.lng));
        }

        TimeZone tz;
        Enum.TryParse(timeZone.text, out tz);
        Debug.Log(tz);
        Weather wt;
        Enum.TryParse(weather.text, out wt);
        Debug.Log(wt);
        TimeStamp ts = ParseShootedAt(shootedAt.text);
        Debug.Log(ts);
        
        var result = new InputDatas(
            playArea.text,
            tz,
            wt,
            ts,
            list
            );
        Debug.Log(playArea.text);
        return result;
    }
    (double lat, double lng) ParseLatLng(string latLng){
        // カンマで区切られた文字を取得
        var firstLength = latLng.IndexOf(","); // ,があるindexを取得
        var lat = double.Parse(latLng.Substring(0, firstLength)); // 12.12345
        var lng = double.Parse(latLng.Substring(firstLength + 1, latLng.Length - firstLength - 1)); // 98.98765
        return (lat, lng);
    }

    TimeStamp ParseShootedAt(string shootedAtString)
    {
        var indexes = shootedAtString.AllIndexesOf(",").ToList();
        var year = int.Parse(shootedAtString.Substring(0, indexes[0]));
        var month = int.Parse(shootedAtString.Substring(indexes[0] + 1, indexes[1] - indexes[0] - 1));
        var day = int.Parse(shootedAtString.Substring(indexes[1] + 1, shootedAtString.Length - indexes[1] - 1));
        var result = TimeStampExt.TSConstructor(year, month, day, 0, 0, 0);
        return result;
    }

    List<string> GetInfosFromRoot(GameObject root){
        var inputFields = root.GetComponentsInChildren<InfoReceiver>().ToList();
        var result = new List<string>();
        inputFields.ForEach(infoReceiver =>
        {
            result.Add(infoReceiver.GetInfo());
        });
        return result;
    }
}


public class InputDatas
{
    public string PlayArea { get; private set; }
    public TimeZone TimeZone { get; private set; }
    public Weather Weather { get; private set; }
    public TimeStamp ShootedAt { get; private set; }
    public List<(string fileName, string streetId, double startLat, double startLng, double endLat, double endLng)> videoDatas;

    public InputDatas(
        string input_playArea, 
        TimeZone input_timeZone,
        Weather input_weather,
        TimeStamp input_shootedAt,
        List<(string fileName, string streetId, double startLat, double startLng, double endLat, double endLng)> input_videoDatas)
    {
        PlayArea = input_playArea;
        TimeZone = input_timeZone;
        Weather = input_weather;
        ShootedAt = input_shootedAt;
        videoDatas = input_videoDatas;
    }
}