using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : PanelController
{
    [SerializeField] private Slider _scrollSpeedSlider;
    
    private void OnEnable()
    {
        Initialize();
        float saved = PlayerPrefs.GetFloat("ScrollSpeed", 15f);
        _scrollSpeedSlider.value = saved;
        GameManager.instance.UpdateScrollSensitivity(saved);
        _scrollSpeedSlider.onValueChanged.RemoveListener(GameManager.instance.UpdateScrollSensitivity);
        _scrollSpeedSlider.onValueChanged.AddListener(GameManager.instance.UpdateScrollSensitivity);
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
    }
}
