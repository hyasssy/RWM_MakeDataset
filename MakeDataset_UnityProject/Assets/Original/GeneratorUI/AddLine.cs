using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;


public class AddLine : MonoBehaviour
{
    [SerializeField]
    List<ButtonSet> _buttonSets;

    [Serializable]
    public class ButtonSet
    {
        public GameObject buttonPrefab;
        public Transform rootTransform;
    }

    public void AddLineFunction()
    {
        // Prefab作ってRootに移動させる
        _buttonSets.ForEach(buttonSet =>
        {
            var newButton = Instantiate(buttonSet.buttonPrefab);
            newButton.transform.SetParent(buttonSet.rootTransform);
            newButton.transform.localScale = Vector3.one;
        });
    }
}
