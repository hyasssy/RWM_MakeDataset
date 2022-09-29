using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageScroll : MonoBehaviour
{
    [SerializeField] private bool isInverse = false;
    [SerializeField] private GameObject scrollRoot;
    [SerializeField] private int scrollSpeed = 1;
    private Vector3 rootOriginPos;
    private void Start()
    {
        rootOriginPos = scrollRoot.transform.position;
    }

    void Update()
    {
        var scrollY = Input.mouseScrollDelta.y;
        if (scrollY != 0)
        {
            scrollRoot.transform.position += Vector3.up * scrollY;
        }

        if (scrollRoot.transform.position.y < rootOriginPos.y)
        {
            scrollRoot.transform.position = rootOriginPos;
        }
    }
}
