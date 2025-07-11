using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordsPanelController : PanelController
{
    [SerializeField] private Button _leaderBoardButton;
    [SerializeField] private Button _statisticsButton;
    [SerializeField] private Button _playerStatisticsButton;

    private void OnEnable()
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
        _leaderBoardButton.onClick.RemoveListener(OnClickLeaderBoardButton);
        _statisticsButton.onClick.RemoveListener(OnClickStatisticsButton);
        _playerStatisticsButton.onClick.RemoveListener(OnClickPlayerStatisticsButton);
        
        _leaderBoardButton.onClick.AddListener(OnClickLeaderBoardButton);
        _statisticsButton.onClick.AddListener(OnClickStatisticsButton);
        _playerStatisticsButton.onClick.AddListener(OnClickPlayerStatisticsButton);
    }

    private void OnClickLeaderBoardButton()
    {
        OpenPanel("[PopupPanel] LeaderBoard");
    }

    private void OnClickStatisticsButton()
    {
        OpenPanel("[PopupPanel] Statistics");
    }

    private void OnClickPlayerStatisticsButton()
    {
        OpenPanel("[PopupPanel] PersonalRecord");
    }
}
