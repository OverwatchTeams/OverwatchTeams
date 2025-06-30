using System;
using System.Collections;
using System.Collections.Generic;
using RainbowArt.CleanFlatUI;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelController : PanelController
{
    [SerializeField] private Button _teamMakingButton;
    [SerializeField] private Button _gameResultButton;
    [SerializeField] private Button _gameRecordButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _exitAcceptButton;
    [SerializeField] private Button _dailyRecordButton;
    [SerializeField] private ModalWindow _warningPopup;

    private void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _teamMakingButton.onClick.RemoveListener(OnClickTeamMakingButton);
        _gameResultButton.onClick.RemoveListener(OnClickGameResultButton);
        _gameRecordButton.onClick.RemoveListener(OnClickGameRecordButton);
        _exitButton.onClick.RemoveListener(OnClickExitButton);
        _exitAcceptButton.onClick.RemoveListener(OnClickExitAcceptButton);
        _dailyRecordButton.onClick.RemoveListener(OnClickDailyRecordButton);
        
        _teamMakingButton.onClick.AddListener(OnClickTeamMakingButton);
        _gameResultButton.onClick.AddListener(OnClickGameResultButton);
        _gameRecordButton.onClick.AddListener(OnClickGameRecordButton);
        _exitButton.onClick.AddListener(OnClickExitButton);
        _exitAcceptButton.onClick.AddListener(OnClickExitAcceptButton);
        _dailyRecordButton.onClick.AddListener(OnClickDailyRecordButton);
    }

    private void OnClickTeamMakingButton()
    {
        //OpenPanel("[PopupPanel] MakeTeamPanel");
    }
    private void OnClickGameResultButton()
    {
        if (DataController.instance._isLoading)
        {
            _warningPopup.ShowModalWindow();
            return;
        }
        OpenPanel("[PopupPanel] GameResultPanel");
    }
    private void OnClickGameRecordButton()
    {
        if (DataController.instance._isLoading)
        {
            _warningPopup.ShowModalWindow();
            return;
        }
        OpenPanel("[PopupPanel] RecordsPannel");
    }

    private void OnClickDailyRecordButton()
    {
        OpenPanel("[PopupPanel] DailyRecord");
    }
    
    private void OnClickExitButton()
    {
        OpenPanel("[PopupPanel] FitterPopup");
        Debug.Log(FitterMessage.Instance);
        Debug.Log(this.gameObject);
        FitterMessage.Instance.SetOwner(this.gameObject);
        FitterMessage.Instance.SetDescription("OverwatchTeams를 종료하시겠습니까?");
    }

    private void OnClickExitAcceptButton()
    {
        if(FitterMessage.Instance.IsOwner(this.gameObject))
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    
    }
    
}
