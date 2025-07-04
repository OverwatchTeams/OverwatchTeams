using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchSupportPanelController : PanelController
{
    [SerializeField] private Button _dailyRecordButton;
    [SerializeField] private DailyRecordPanelController _dailyRecordPanel;
    private string[] _players;
    
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
        _dailyRecordButton.onClick.RemoveListener(OnClickDailyRecordButton);
        _dailyRecordButton.onClick.AddListener(OnClickDailyRecordButton);
    }

    public void UpdatePlayers(string[] players)
    {
        _players = players;
    }
    
    private void OnClickDailyRecordButton()
    {
        OpenPanel("[PopupPanel] DailyRecord");
        _dailyRecordPanel.SetDailyMatchSupporter(_players);
    }
}
