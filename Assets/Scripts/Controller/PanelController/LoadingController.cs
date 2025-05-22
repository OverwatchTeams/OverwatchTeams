using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : PanelController
{
    // 현재 ProgressIcon이 회전 중인지 회전 여부 확인
    private bool isSpinning = true;
    public Image progressIcon;
    // ProgressIcon 회전 속도
    public float rotationSpeed = 100f;
    // Start is called before the first frame update
    private void Start()
    {
        DataManager.instance.OnDataLoaded += ChangeToMainPanel;
    }

    // Update is called once per frame
    private void Update()
    {
        // ProgressIcon 회전
        if (isSpinning && progressIcon != null)
        {
            progressIcon.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
    }
    
    private void ChangeToMainPanel()
    {
        // 회전 멈추기
        StopSpinner();
        this.gameObject.SetActive(false);
    }
    
    private void StopSpinner()
    {
        isSpinning = false;
    }
    
    
    
}
