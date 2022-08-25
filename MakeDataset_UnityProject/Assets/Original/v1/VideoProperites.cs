using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VideoProperites : MonoBehaviour
{
    //ここのUI作るのちょっとダルかったからInspectorからの入力に甘んじることとする
    [field:SerializeField]
    public string PlayArea{ get; private set; }
    [field:SerializeField]
    public TimeZone TimeZone{ get; private set; }
    [field:SerializeField]
    public Weather Weather{ get; private set; }
    [field: SerializeField]
    public TimeStamp ShootedAt{ get; private set; }
}