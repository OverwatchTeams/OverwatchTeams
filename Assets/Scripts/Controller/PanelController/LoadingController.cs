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
    [SerializeField] private TMP_Text progressText;
    // ProgressIcon 회전 속도
    public float rotationSpeed = 100f;

    private void Start()
    {
        DataController.instance.OnDataLoadEnd -= DeactivateLoadingPanel;
        DataController.instance.OnDataLoadEnd += DeactivateLoadingPanel;
    }

    private void OnEnable()
    {
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

    private void ActivateLoadingPanel()
    {
        StartSpinner();
    }
    private void DeactivateLoadingPanel()
    {
        // 회전 멈추기
        StopSpinner();
        this.gameObject.SetActive(false);
    }
    
    private void StopSpinner()
    {
        isSpinning = false;
    }
    private void StartSpinner()
    {
        isSpinning = true;
    }

    public void ChangeDescription(string description)
    {
        
    }
    
    
    
}
