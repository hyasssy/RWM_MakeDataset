using System;
using System.Collections.Generic;

// 1次データ
[Serializable]
public class TownVideoDataJSON{
    public string playArea;
    public TimeStamp edittedAt;
    public LocationCoordJSON originLocation;
    public List<StreetVideoJSON> videos = new List<StreetVideoJSON>();
}
[Serializable]
public class LocationCoordJSON{
    public double lat;
    public double lng;
    public double height;
}
[Serializable]
public class StreetVideoJSON{
    public string streetId;
    public string videoId;
    public VideoFileNameJSON fileName;
    public TimeStamp shootedAt;
    public TimeZone timeZone;
    public Weather weather;
    public int frameLength;
    public bool flag;
    public List<LocationLogJSON> locationLogs = new List<LocationLogJSON>();

}
[Serializable]
public class VideoFileNameJSON{
    public string originFileName;
    public string lightFileName;
}

[Serializable]
public class LocationLogJSON{
    public int frameNumber;
    public double lat;
    public double lng;
    public double height;
    public float rotation;
}




// 検索用2次データ
[Serializable]
public class SearchDataJSON{
    public string playArea;
    public TimeStamp edittedAt;
    public LocationCoordJSON originLocation;
    public List<DataInGridJSON> latLngGridDatas = new List<DataInGridJSON>();
}
[Serializable]
public class DataInGridJSON{
    public string gridId;
    public List<VideoFrameData> locations = new List<VideoFrameData>();
}
[Serializable]
public class VideoFrameData{
    public string videoId;
    public LocationLogJSON locationLog;
}




// Obsolete

// [Serializable]
// public class SingleStreetJSON{
//     public string streetId;
//     public string streetNum;
//     public string playArea;
//     public TimeStamp edittedAt;
//     public StreetVideoJSON video;
// }
// [Serializable]
// public class StreetVideoJSON{
//     public string fileName;
//     public TimeStamp shootedAt;
//     public string timeZone;
//     public string weather;
//     public int frameLength;
//     public bool flag;
//     public List<LocationJSON> posLog;
// }
// [Serializable]
// public class LocationJSON{
//     public int frameNumber;
//     public double lat;
//     public double lng;
//     public double height;
//     public float rotation;
// }





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