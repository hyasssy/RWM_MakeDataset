using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>InputFieldの情報を集計する</summary>
public class GetInputs : MonoBehaviour
{
    [SerializeField]
    GameObject _numRoot;
    [SerializeField]
    GameObject _latlngRoot;


    public (List<string> nums, List<string> latlngs) GetInputsFunction(){
        var nums = GetInfosFromRoot(_numRoot);
        var latlngs = GetInfosFromRoot(_latlngRoot);
        return (nums, latlngs);
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
