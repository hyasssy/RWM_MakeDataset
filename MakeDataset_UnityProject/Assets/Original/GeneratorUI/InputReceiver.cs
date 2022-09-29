using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    [SerializeField] AddLine addLines;

    [SerializeField] private MakeGridData makeGridData;
    public void PushExec()
    {
        makeGridData.Execute();
    }
    public void PushAddLine()
    {
        addLines.AddLineFunction();
    }
}
