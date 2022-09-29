using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// 1次データ PlayArea全体
[Serializable]
public class TownVideoDataJson{
    public string playArea;
    public TimeStamp edittedAt;
    public LocationCoordJson originLocation; // 手動で入れる
    public List<StreetVideoJson> videos = new List<StreetVideoJson>();
}
[Serializable]
public class LocationCoordJson{
    public double lat;
    public double lng;
    public double height;
}
[Serializable]
public class StreetVideoJson{
    public string streetId;
    public string videoId;
    public VideoFileNameJson fileName;
}
[Serializable]
public class VideoFileNameJson{
    public string standardFileName;
    public string lightFileName;
}

// 1次データ 各Videoのメタデータ 全フレームのデータを保持
[Serializable]
public class VideoMetaJson
{
    public string videoId;
    public TimeStamp shootedAt;
    public TimeZone timeZone;
    public Weather weather;
    public int frameLength;
    public bool flag;
    public List<LocationLogJson> locationLogs = new List<LocationLogJson>();
}

[Serializable]
public class LocationLogJson{
    public int frameNumber;
    public double lat;
    public double lng;
    public float height;
    public float rotation;
}




// 検索用2次データ
[Serializable]
public class SearchDataJson{
    public string playArea;
    public TimeStamp edittedAt;
    public List<CellRegionJson> latLngGrid = new List<CellRegionJson>();
}
[Serializable]
public class CellRegionJson{
    public string cellRegionId;
    public List<CellJson> cells = new List<CellJson>();     
}

[Serializable]
public class CellJson
{
    public string cellId;
    public List<VideoFrameData> locations = new List<VideoFrameData>();
}

[Serializable]
public class VideoFrameData{
    public string videoId;
    public List<int> frameNumbers = new List<int>();
}




// Json用型定義

[Serializable]
public class TimeStamp
{
    public int year, month, day, hour, minute, second;
}
public static class TimeStampExt{
    public static TimeStamp TSConstructor(int year, int month, int day, int hour, int minute, int second){
        var result = new TimeStamp();
        result.year = year;
        result.month = month;
        result.day = day;
        result.hour = hour;
        result.minute = minute;
        result.second = second;
        return result;
    }
    public static TimeStamp DT2TS(DateTime dateTime){
        var result = TSConstructor(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        return result;
    }
    public static DateTime TS2DT(TimeStamp timeStamp){
        var result = new DateTime(timeStamp.year, timeStamp.month, timeStamp.day, timeStamp.hour, timeStamp.minute, timeStamp.second);
        return result;
    }
}

public enum TimeZone{
    Morning,
    Noon,
    Afternoon,
    Evening,
    Night
}
public enum Weather{
    Sunny,
    Cloudy,
    Rainy,
    Snowy
}