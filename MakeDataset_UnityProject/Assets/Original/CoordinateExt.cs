using UnityEngine;
using System.Collections.Generic;

public static class CoordinateExt
{
    /// <summary>緯度経度をメートルのx,y座標(double)で返します。x=lng,y=lat</summary>
    public static (double x, double y) Coord2Meter(Coordinate coord)
    {
        // int isEastHemis = coord.Longitude > 0 ? 1 : -1; // TODO いるのか謎。。要調査
        var x = coord.Longitude * Coord2MeterList.GetLngCoef(coord.Latitude);// * isEastHemis;
        var y = coord.Latitude * Coord2MeterList.GetLatCoef();
        return (x, y);
    }

    ///<summary>
    ///指定したベクトル位置のdoubleの緯度経度を返します(lat, lng)
    ///</summary>
    public static Coordinate GetTargetLocationToVec(Coordinate coord, Vector2 targetDir)
    {
        // Debug.Log(coord.Latitude + ", " + coord.Longitude);
        var lat = coord.Latitude + targetDir.y / Coord2MeterList.GetLatCoef();
        var lng = coord.Longitude + targetDir.x / Coord2MeterList.GetLngCoef(coord.Latitude);
        var result = new Coordinate(lat, lng);
        return result;
    }

    /// <summary>ある地点(緯度経度)からある地点(緯度経度)のベクトルをメートル換算Vector2で返します。</summary>
    static Vector2 Coord2CoordVec(Coordinate fromCoord, Coordinate toCoord)
    {
        var delta = new Vector2((float)(Coord2Meter(toCoord).x - Coord2Meter(fromCoord).x), (float)(Coord2Meter(toCoord).y - Coord2Meter(fromCoord).y));
        // Debug.Log("delta" + delta.x + "," + delta.y);
        return delta;
    }
    /// <summary>現在のセッションにおける指定した座標のTransformPositionを返す</summary>
    public static Vector2 Coord2RelativePos(Coordinate targetPosCoord, Coordinate areaOriginCoord)
    {
        var targetPos = Coord2CoordVec(areaOriginCoord, targetPosCoord);
        return targetPos;
    }
    /// <summary>現在のセッション位におけるTransformPositionを、緯度経度に変換する</summary>
    public static Coordinate RelativePos2Coord(Vector2 relativePos, Coordinate originLocation)
    {
        var originLocation2Meter = Coord2Meter(originLocation);
        var lat = ((double)relativePos.y + originLocation2Meter.y) / Coord2MeterList.GetLatCoef();
        var lng = ((double)relativePos.x + originLocation2Meter.x) / Coord2MeterList.GetLngCoef(lat);
        Debug.Log($"originLocation = {originLocation2Meter}, relativePos = {relativePos}");
        Debug.Log($"originCoord = {originLocation.Latitude}-{originLocation.Longitude}, relativePos = {relativePos.x}-{relativePos.y}");
        var originCoorddashLat = originLocation2Meter.y / Coord2MeterList.GetLatCoef();
        var originCoorddashLng = originLocation2Meter.x / Coord2MeterList.GetLngCoef(originCoorddashLat);
        Debug.Log($"originCoord' = {originCoorddashLat}, {originCoorddashLng}");
        Debug.Log($"result = {lat}, {lng}");
        return new Coordinate(lat, lng);
    }
    public static string CoordToString(Coordinate coord)
    {
        return coord.Latitude.ToString() + "," + coord.Longitude.ToString();
    }
    public static Coordinate StringToCoord(string coordText)
    {
        var firstLength = coordText.IndexOf(",");
        var lat = double.Parse(coordText.Substring(0, firstLength));
        var lng = double.Parse(coordText.Substring(firstLength + 1, coordText.Length - firstLength - 1));
        return new Coordinate(lat, lng);
    }



    static class Coord2MeterList
    {
        // 計算式がよくわからないため緯度経度をメートルに換算する係数をリストにしておく。
        // 大体これを経度にかければ、東京でも99%ぐらいの精度が出る。（緯度が上がるとめちゃくちゃになるのは仕方がない。）
        // リスト参照もと
        // https://www.wingfield.gr.jp/archives/9721#:~:text=%E7%B7%AF%E5%BA%A60%E5%BA%A6%E3%80%81%E3%81%A4%E3%81%BE%E3%82%8A%E8%B5%A4%E9%81%93,%E8%B7%9D%E9%9B%A2%E3%81%AF%E7%B4%8491km%E3%81%A7%E3%81%99%E3%80%82
        // 精度検証用
        // https://vldb.gsi.go.jp/sokuchi/surveycalc/surveycalc/bl2stf.html
        // Latの方も補正すると僅かに精度が上がるが、せいぜい1%程度だから無視しても良い
        public static float GetLatCoef()
        {
            return latCoef;
        }
        static float latCoef = 111319.44f;
        public static float GetLngCoef(double lat)
        {
            if (!lngCoefDict.ContainsKey((int)Mathf.Abs((float)lat))) Debug.LogError("経度係数リストに値がありません。トライしたのは" + lat);
            return lngCoefDict[(int)Mathf.Abs((float)lat)];
        }
        ///<summary>
        /// 緯度によって、距離あたりの経度を座標変換していく係数が変わってくるその係数計算。
        ///</summary>
        static Dictionary<int, float> lngCoefDict = new Dictionary<int, float>() {
            {0, 111319.4908f},
            {1, 111302.6493f},
            {2, 111252.1298f},
            {3, 111167.9466f},
            {4, 111050.124f},
            {5, 110898.6955f},
            {6, 110713.7047f},
            {7, 110495.2045f},
            {8, 110243.2576f},
            {9, 109957.9362f},
            {10, 109639.3221f},
            {11, 109287.5068f},
            {12, 108902.5913f},
            {13, 108484.6862f},
            {14, 108033.9117f},
            {15, 107550.3973f},
            {16, 107034.2822f},
            {17, 106485.7152f},
            {18, 105904.8544f},
            {19, 105291.8673f},
            {20, 104646.9309f},
            {21, 103970.2317f},
            {22, 103261.9655f},
            {23, 102522.3372f},
            {24, 101751.5613f},
            {25, 100949.8614f},
            {26, 100117.4705f},
            {27, 99254.6306f},
            {28, 98361.5928f},
            {29, 97438.6175f},
            {30, 96485.9741f},
            {31, 95503.9408f},
            {32, 94492.805f},
            {33, 93452.8629f},
            {34, 92384.4194f},
            {35, 91287.7885f},
            {36, 90163.2924f},
            {37, 89011.2625f},
            {38, 87832.0385f},
            {39, 86625.9685f},
            {40, 85393.4091f},
            {41, 84134.7255f},
            {42, 82850.2908f},
            {43, 81540.4864f},
            {44, 80205.7019f},
            {45, 78846.3347f},
            {46, 77462.7903f},
            {47, 76055.482f},
            {48, 74624.8405f},
            {49, 73171.2644f},
            {50, 71695.2196f},
            {51, 70197.1396f},
            {52, 68677.4748f},
            {53, 67136.683f},
            {54, 65575.229f},
            {55, 63993.5843f},
            {56, 62392.2272f},
            {57, 60771.6427f},
            {58, 59132.3223f},
            {59, 57474.7635f},
            {60, 55799.4704f},
            {61, 54106.9529f},
            {62, 52397.7267f},
            {63, 50672.3134f},
            {64, 48931.2401f},
            {65, 47175.0392f},
            {66, 45404.2485f},
            {67, 43619.4106f},
            {68, 41821.0731f},
            {69, 40009.7884f},
            {70, 38186.1133f},
            {71, 36350.6089f},
            {72, 34503.8407f},
            {73, 32646.3777f},
            {74, 30778.7931f},
            {75, 28901.6635f},
            {76, 27015.5689f},
            {77, 25121.0924f},
            {78, 23218.8202f},
            {79, 21309.3412f},
            {80, 19393.2468f},
            {81, 17471.1309f},
            {82, 15543.5895f},
            {83, 13611.2206f},
            {84, 11674.6236f},
            {85, 9734.4f},
            {86, 7791.152f},
            {87, 5845.4833f},
            {88, 3897.9983f},
            {89, 1949.302f},
            {90, -1f},// Error

        };
    }

}
public class Coordinate
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public Coordinate(
        double lat = 0f,
        double lng = 0f
    )
    {
        // -90~90、緯度
        if (lat > 90)
        {
            lat = 180 - lat;
        }
        else if (lat < -90)
        {
            lat = -180 - lat;
        }

        // -180~180、経度
        if (lng > 180)
        {
            lng = lng - 360;
        }
        else if (lng < -180)
        {
            lng = lng + 360;
        }
        this.Latitude = lat;
        this.Longitude = lng;
    }
}