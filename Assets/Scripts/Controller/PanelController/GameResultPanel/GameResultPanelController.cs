using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultPanelController : PanelController
{
    [SerializeField] private Text _date;
    [SerializeField] private TMP_Text _round;
    [SerializeField] private Button _mapButton;
    [SerializeField] private Button[] _playerButtons;
    [SerializeField] private Button[] _teamCrownButtons;
    [SerializeField] private Image[] _teamCrownIcons;
    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _confirmCreateMatchButton;
    [SerializeField] private TMP_Text _errorText;
    
    [SerializeField] private FindMapPanelController _findMapPanelController;
    private MatchData _latestMatchData;
    private int currentCycle;

    #region Initialization
    private void OnEnable()
    {
        _findMapPanelController.OnMapClicked -= ChangeMap;
        _findMapPanelController.OnMapClicked += ChangeMap;
        StartCoroutine(MatchDataManager.instance.GetLastMatch(result => 
        {
            if (result != null)
            {
                _latestMatchData = result;
                currentCycle = result.round + 1;
                _round.text = currentCycle.ToString() + " 번째 사이클";
            }
        }));
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        //패널 초기화
        InitializePanel();

        //맵, 플레이어등 버튼리스너 초기화
        InitializeListeners();
    }

    private void InitializePanel()
    {
        //모든 플레이어 리셋
        foreach (var button in _playerButtons)
        {
            button.GetComponentInChildren<TMP_Text>().text = "-";
        }

        //모든 팀 크라운 컬러 변경
        foreach (var icon in _teamCrownIcons)
        {
            Color color = icon.color;
            color.a = 0;
            icon.color = color;
        }
        //맵 리셋
        _mapButton.GetComponentInChildren<TMP_Text>().text = "-";
        
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        foreach (var button in _playerButtons)
        {
            button.onClick.RemoveListener(PopupFindPlayerTab);
            button.onClick.AddListener(PopupFindPlayerTab);
        }

        foreach (var button in _teamCrownButtons)
        {
            button.onClick.RemoveListener(delegate { ChangeCrownTeam(button); });
            button.onClick.AddListener(delegate { ChangeCrownTeam(button); });
        }

        _mapButton.onClick.RemoveListener(PopupFindMapTab);
        _finishButton.onClick.RemoveListener(PopupFinish);
        _mapButton.onClick.AddListener(PopupFindMapTab);
        _finishButton.onClick.AddListener(PopupFinish);
    }
    #endregion

    private void ChangeMap(string mapName)
    {
        _mapButton.GetComponentInChildren<TMP_Text>().text = mapName;
        CloseAllPanel();
    }

    private RefinedMatchData SetRefinedMatchData()
    {
        
        List<(string, Role)> playerList = new List<(string, Role)>();
        for (int i = 0; i < _playerButtons.Length; i++)
        {
            (string, Role) player;
            player.Item1 = _playerButtons[i].GetComponentInChildren<TMP_Text>().text;
            //딜러 player들 index
            if (i == 0 || i == 1 || i == 5 || i == 6)
            {
                player.Item2 = Role.D;
            }
            //탱커 player들 index
            else if (i == 2 || i == 7)
            {
                player.Item2 = Role.T;
            }
            //힐러 player들 index
            else if (i == 3 || i == 4 || i == 8 || i == 9)
            {
                player.Item2 = Role.H;
            }
            else
            {
                player.Item2 = Role.Default;
            }
            playerList.Add(player);
        }

        WinnerTeam winnerTeam;
        winnerTeam = GetWinner();

        return new RefinedMatchData
        {
            date = Convert.ToDateTime(_date.text),
            round = currentCycle,
            map = _mapButton.GetComponentInChildren<TMP_Text>().text,
            players = playerList,
            winner = winnerTeam
        };
    }

    private WinnerTeam GetWinner()
    {
        for (int i = 0; i < _teamCrownIcons.Length; i++)
        {
            Color color =_teamCrownIcons[i].color;
            if (color.a != 0)
            {
                if (i == 0)
                {
                    return WinnerTeam.블루;
                }
                else if (i == 1)
                {
                    return WinnerTeam.레드;
                }
                break;
            }
        }
        return WinnerTeam.무승부;
    }
    
    
    private void PopupFindPlayerTab()
    {
        OpenPanel("[Popup] FindPlayer");
    }

    private void PopupFindMapTab()
    {
        OpenPanel("[Popup] FindMap");
    }

    private void ChangeCrownTeam(Button button)
    {
        for (int i = 0; i < _teamCrownButtons.Length; i++)
        {
            if (button == _teamCrownButtons[i])
            {
                Color color = _teamCrownIcons[i].color;
                color.a = 1;
                _teamCrownIcons[i].color = color;
            }
            else
            {
                Color color = _teamCrownIcons[i].color;
                color.a = 0;
                _teamCrownIcons[i].color = color;
            }
        }
    }

    private void PopupFinish()
    {
        if (!CheckMapSavable())
        {
            _errorText.text = "맵을 선택해주세요";
            OpenPanel("[Popup] SaveErrorMessage", false);
        }
        else if (!CheckWinnerSavable())
        {
            _errorText.text = "승자를 선택해주세요";
            OpenPanel("[Popup] SaveErrorMessage", false);
        }
        else if (!CheckPlayerSavable())
        {
            _errorText.text = "플레이어를 모두 선택해주세요";
            OpenPanel("[Popup] SaveErrorMessage", false);
        }
        else
        {
            OpenPanel("[Popup] SaveCheckMessage", false);
        }
    }

    private void CreateMatch()
    {
        RefinedMatchData data = SetRefinedMatchData();
        StartCoroutine(DataController.instance.CreateMatch(result =>
        {
            if (result)
                OpenPanel("[Popup] AddMapFinishMessage");
            else
                OpenPanel("[Popup] AddMapFailedMessage");
        }, data));
    }

    #region 이벤트-저장가능여부 확인
    private bool CheckMapSavable()
    {
        if (_mapButton.GetComponentInChildren<TMP_Text>().text != "-")
        {
            return true;
        }
        return false;
    }

    private bool CheckWinnerSavable()
    {
        int activeCrownCount = 0;
        foreach (var icon in _teamCrownIcons)
        {
            Color color = icon.color;
            if (color.a == 0) continue;
            activeCrownCount++;
        }

        if (activeCrownCount == 1)
        {
            return true;
        }
        return false;
    }

    private bool CheckPlayerSavable()
    {
        foreach (var playerbutton in _playerButtons)
        {
            if (playerbutton.GetComponentInChildren<TMP_Text>().text == "-")
                return false;
        }

        return true;
    }
    #endregion
}
