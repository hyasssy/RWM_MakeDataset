using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageScroll : MonoBehaviour
{
    [SerializeField] private bool isInverse = false;
    [SerializeField] private GameObject scrollRoot;
    [SerializeField] private int scrollSpeed = 20;
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
            var inverse = isInverse ? -1 : 1;
            scrollRoot.transform.position += Vector3.up * scrollY * scrollSpeed *inverse;
        }

        if (scrollRoot.transform.position.y < rootOriginPos.y)
        {
            scrollRoot.transform.position = rootOriginPos;
        }
    }
}
