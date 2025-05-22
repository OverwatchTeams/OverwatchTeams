using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelController : PanelController
{
    [SerializeField] private Button _teamMakingButton;
    [SerializeField] private Button _gameResultButton;
    [SerializeField] private Button _gameRecordButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _exitAcceptButton;

    private void Start()
    {
        InitializeButtons();
        Initialize();
    }

    private void Initialize()
    {
        CloseAllPanel();
    }

    protected override void InitializeButtons()
    {
        base.InitializeButtons();
        _teamMakingButton.onClick.AddListener(OnClickTeamMakingButton);
        _gameResultButton.onClick.AddListener(OnClickGameResultButton);
        _gameRecordButton.onClick.AddListener(OnClickGameRecordButton);
        _exitButton.onClick.AddListener(OnClickExitButton);
        _exitAcceptButton.onClick.AddListener(OnClickExitAcceptButton);
    }

    private void OnClickTeamMakingButton()
    {
        OpenPanel("[PopupPanel] MakeTeamPanel");
    }
    private void OnClickGameResultButton()
    {
        OpenPanel("[PopupPanel] GameResultPanel");
    }
    private void OnClickGameRecordButton()
    {
        OpenPanel("[PopupPanel] RecordsPannel");
    }
    
    private void OnClickExitButton()
    {
        OpenPanel("[Popup] FinishCheck");
    }

    private void OnClickExitAcceptButton()
    {
    
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    
    }
    
}
