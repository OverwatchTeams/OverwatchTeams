using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SwipdownTextAnimation : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _textContainers;
    private List<string> _texts = new List<string>();
    
    private int _textIndex = 0;
    private int _containerIndex = 0;

    private void OnEnable()
    {
        _textIndex = 0;
        _containerIndex = 0;
    }
    public void SetTexts(List<string> texts)
    {
        _texts = texts;
        _textContainers[_containerIndex].text = _texts[_textIndex];
    }
    
    public void ReplaceText()
    {
        _textIndex++;
        if (_texts.Count == _textIndex) _textIndex = 0;
        _containerIndex = 1 - _containerIndex;
        _textContainers[_containerIndex].text = _texts[_textIndex];
    }
}
