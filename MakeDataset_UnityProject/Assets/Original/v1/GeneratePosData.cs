using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.IO;
using System.Runtime.InteropServices;


public class GeneratePosData : MonoBehaviour
{
    [SerializeField]
    GetInputs _getInputs;
    [SerializeField]
    VideoProperites _videoProperties;
    [SerializeField]
    string folderPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Downloads/data";


    public void ExecFunction(){
        if(!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        // (var nums, var latlngs) = _getInputs.GetInputsFunction();
        // var streetsJSON = new SingleStreetJSON();
        // streetsJSON

        // var json = JsonUtility.ToJson()
    }
    SingleStreetJSON PrepareStreetJSON(
        string filename,
        TimeStamp shootedAt,
        string timeZone,
        string weather,
        VideoClip videoClip){

        // videoファイルコピーして

        // var streetJSON = new SingleStreetJSON();
        // var streetVideoJSON = new StreetVideoJSON();
        // streetVideoJSON.filename = filename
        // streetVideoJSON.shootedAt =
        // streetVideoJSON.timeZone =
        // streetVideoJSON.weather =
        // streetVideoJSON.
        return null;
    }

}

[Serializable]
public class StreetsJSON{
    public List<SingleStreetJSON> street;
}

[Serializable]
public class SingleStreetJSON{
    public string streetId;
    public string num;
    public string playArea;
    public string edittedAt;
    public StreetVideoJSON video;
}
[Serializable]
public class StreetVideoJSON{
    public string filename;
    public TimeStamp shootedAt;
    public string timeZone;
    public string weather;
    public int frameLength;
    public bool flag = true;
    public List<LocationJSON> posLog;
}
[Serializable]
public class LocationJSON{
    public double lat;
    public double lng;
    public double height;
}

[Serializable]
public class TimeStamp
{
    public int year, month, day, hour, minute, second;
    // public DateTime dateTime { get
    //     {
    //         var result = new DateTime(year, month, day, hour, minute, second);
    //         return result;
    //     }
    // }
}