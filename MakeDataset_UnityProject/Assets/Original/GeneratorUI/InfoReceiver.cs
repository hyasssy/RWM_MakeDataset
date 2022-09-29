using UnityEngine;
using TMPro;

public class InfoReceiver : MonoBehaviour
{

    public string GetInfo()
    {
        var inputField = gameObject.GetComponent<TMP_InputField>();
        return inputField.text;
    }
}
