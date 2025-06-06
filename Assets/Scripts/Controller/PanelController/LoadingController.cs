using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : PanelController
{
    // 현재 ProgressIcon이 회전 중인지 회전 여부 확인
    private bool isSpinning = true;
    [SerializeField] private Image progressIcon;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text miniLoadingText;
    [SerializeField] private Image background;
    // ProgressIcon 회전 속도
    public float rotationSpeed = 100f;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _miniLoadingPanel;
    [SerializeField] private GameObject _warningPanel;
    [SerializeField] private Button[] _confirmButton;
    

    private void Start()
    {
        DataController.instance.OnDataLoadEnd -= DeactivateLoadingPanel;
        DataController.instance.OnDataLoadEnd += DeactivateLoadingPanel;
        DataController.instance.OnDataUpdateEnd -= DeactivateMiniLoadingPanel;
        DataController.instance.OnDataUpdateEnd += DeactivateMiniLoadingPanel;
    }

    private void OnEnable()
    {
        InitializeListener();
        ActivateLoadingPanel();
    }

    private void Update()
    {
        // ProgressIcon 회전
        if (isSpinning && progressIcon != null)
        {
            progressIcon.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
    }

    private void InitializeListener()
    {
        foreach (var button in _confirmButton)
        {
            button.onClick.RemoveListener(DeactivateWarningPopupPanel);
            button.onClick.AddListener(DeactivateWarningPopupPanel);
        }
    }

    public void SetLoadingPanelOpaque(bool isOpaque)
    {
        if (!isOpaque)
        {
            Color color = background.color;
            color.a = 0f;
            background.color = color;
        }
        else
        {
            Color color = background.color;
            color.a = 1f;
            background.color = color;
        }
    }
    public void SetLoadingMessage(string message)
    {
        loadingText.text = message;
    }

    public void SetMiniLoaingMessage(string message)
    {
        miniLoadingText.text = message;
    }
    public void ActivateLoadingPanel()
    {
        _loadingPanel.SetActive(true);
        StartSpinner();
    }
    private void DeactivateLoadingPanel()
    {
        // 회전 멈추기
        StopSpinner();
        _loadingPanel.SetActive(false);
    }

    public void ActivateMiniLoadingPanel()
    {
        _miniLoadingPanel.SetActive(true);
    }
    public void DeactivateMiniLoadingPanel()
    {
        _miniLoadingPanel.SetActive(false);
    }
    
    public void ActivateWarningPopupPanel()
    {
        _warningPanel.SetActive(true);
    }
    
    private void DeactivateWarningPopupPanel()
    {
        _warningPanel.SetActive(false);
    }
    
    private void StopSpinner()
    {
        isSpinning = false;
    }
    private void StartSpinner()
    {
        isSpinning = true;
    }
    
    
    
}
