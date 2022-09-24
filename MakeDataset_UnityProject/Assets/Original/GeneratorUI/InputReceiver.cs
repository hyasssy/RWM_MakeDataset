using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    [SerializeField]
    AddLine _addLines;
    [SerializeField]
    GeneratePosData _generatePosData;
    public void PushExec()
    {
        _generatePosData.ExecFunction();
    }
    public void PushAddLine()
    {
        _addLines.AddLineFunction();
    }
}
