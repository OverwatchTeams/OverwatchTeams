using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubNameButtonPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text _subNameText;

    public void ChangeSubNameText(string subName)
    {
        _subNameText.text = subName;
    }
}
