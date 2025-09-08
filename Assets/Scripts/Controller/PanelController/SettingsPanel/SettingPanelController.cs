using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : PanelController
{
    [SerializeField] private Slider _scrollSpeedSlider;
    [SerializeField] private Button _refreshButton;
    
    private void OnEnable()
    {
        Initialize();
        float saved = PlayerPrefs.GetFloat("ScrollSpeed", 15f);
        _scrollSpeedSlider.value = saved;
        GameManager.instance.UpdateScrollSensitivity(saved);
        _scrollSpeedSlider.onValueChanged.RemoveListener(GameManager.instance.UpdateScrollSensitivity);
        _scrollSpeedSlider.onValueChanged.AddListener(GameManager.instance.UpdateScrollSensitivity);
        _refreshButton.onClick.RemoveListener(OnClickRefresh);
        _refreshButton.onClick.AddListener(OnClickRefresh);
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
    }

    private void OnClickRefresh()
    {
        StartCoroutine(OnClickRefreshCoroutine());
    }

    private IEnumerator OnClickRefreshCoroutine()
    {
        yield return StartCoroutine(DataController.instance.InitializeDataWithLoading());
        this.gameObject.SetActive(false);
    }
}
