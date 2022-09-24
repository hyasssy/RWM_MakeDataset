using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputDatas : MonoBehaviour
{
    // 設計甘めに適当にpublicにしてる
    // [field: SerializeField]
    public string playArea;// { get; private set; }
    public TimeZone timeZone;
    [field: SerializeField]
    public Weather weather;
    [field: SerializeField]
    public TimeStamp shootedAt;


}