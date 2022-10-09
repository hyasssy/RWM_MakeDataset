using UnityEngine;
using UnityEngine.Serialization;

namespace RWM.VR
{
    public class InputReceiver : MonoBehaviour
    {
        [SerializeField] AddLine addLines;

        [FormerlySerializedAs("makeGridData")] [SerializeField]
        private MakePrimaryData makePrimaryData;

        public void PushExec()
        {
            makePrimaryData.Execute();
        }

        public void PushAddLine()
        {
            addLines.AddLineFunction();
        }
    }
}