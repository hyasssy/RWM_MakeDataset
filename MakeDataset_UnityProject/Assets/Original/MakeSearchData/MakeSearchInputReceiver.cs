using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

namespace RWM.VR
{
    public class MakeSearchInputReceiver : MonoBehaviour
    {
        //Inputから起動
        [SerializeField] private MakeSearchGridData makeSearchGridData;

        public void PushExec()
        {
            makeSearchGridData.Execute(playAreaIF.text);
            Debug.Log("Exec!!!");
        }

        //Inputを参照
        [SerializeField] private TMP_InputField playAreaIF;
    }
}