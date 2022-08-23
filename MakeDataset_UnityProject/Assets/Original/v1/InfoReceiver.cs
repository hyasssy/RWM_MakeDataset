using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;
using TMPro;

public class InfoReceiver : MonoBehaviour
{

    public string GetInfo()
    {
        var inputField = gameObject.GetComponent<TMP_InputField>();
        return inputField.text;
    }
}
