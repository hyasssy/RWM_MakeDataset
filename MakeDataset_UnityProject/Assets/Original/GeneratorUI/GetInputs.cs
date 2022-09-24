using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>InputFieldの情報を集計する</summary>
public class GetInputs : MonoBehaviour
{
    [SerializeField]
    GameObject _fileNameRoot;
    [SerializeField]
    GameObject _numRoot;
    [SerializeField]
    GameObject _latlngRoot;


    public Dictionary<string, (double lat, double lng)> GetInputsFunction(){
        var nums = GetInfosFromRoot(_numRoot);
        var latlngs = GetInfosFromRoot(_latlngRoot);
        var dict = new Dictionary<string, (double lat, double lng)>();
        for (var i = 0; i < nums.Count;i++){
            dict.Add(nums[i], ParseLatLng(latlngs[i]));
        }
        return dict;
    }
    (double lat, double lng) ParseLatLng(string latLng){
        // カンマで区切られた文字を取得
        var firstLength = latLng.IndexOf(","); // ,があるindexを取得
        var lat = double.Parse(latLng.Substring(0, firstLength)); // 12.12345
        var lng = double.Parse(latLng.Substring(firstLength + 1, latLng.Length - firstLength - 1)); // 98.98765
        return (lat, lng);
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
