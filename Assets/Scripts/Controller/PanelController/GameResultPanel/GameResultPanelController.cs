using System;
using System.Collections;
using System.Collections.Generic;
using RainbowArt.CleanFlatUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultPanelController : PanelController
{
    [SerializeField] private Text _date;
    [SerializeField] private TMP_Text _round;
    [SerializeField] private Button _mapButton;
    [SerializeField] private GameObject[] _players;
    [SerializeField] private Button[] _teamCrownButtons;
    [SerializeField] private Image[] _teamCrownIcons;
    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _confirmCreateMatchButton;
    [SerializeField] private GameObject _calendar;
    
    [SerializeField] private FindPlayerPanelController _findPlayerPanelController;
    [SerializeField] private FindMapPanelController _findMapPanelController;
    private MatchData _latestMatchData;
    private GameObject _tempClickedPlayer;
    private int currentCycle;
    private int beginIndex;

    #region Initialization
    private void OnEnable()
    {
        _findPlayerPanelController.OnPlayerClicked -= ChangePlayer;
        _findPlayerPanelController.OnPlayerClicked += ChangePlayer;
        _findMapPanelController.OnMapClicked -= ChangeMap;
        _findMapPanelController.OnMapClicked += ChangeMap;
        
        StartCoroutine(MatchDataManager.instance.GetLastMatch(ResponseHandlers.Create<MatchData>(
            response =>
            {
                _latestMatchData = response;
                currentCycle = response.round + 1;
                beginIndex = response.index + 1;
                _round.text = currentCycle.ToString() + " 번째 사이클";
            },
            error => { })));
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
        foreach (var player in _players)
        {
            player.GetComponentInChildren<TMP_Text>().text = "-";
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
        
        DateTime utcNow = DateTime.UtcNow; // 현재 UTC 시간
        TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        DateTime estNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, estZone);
        
        _date.text = estNow.ToString("yyyy-MM-dd");
        _calendar.GetComponent<CalendarController>()._calendarPanel.SetActive(false);
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        foreach (var player in _players)
        {
            player.GetComponent<Button>().onClick.RemoveListener(delegate { PopupFindPlayerTab(player); });
            player.GetComponent<Button>().onClick.AddListener(delegate { PopupFindPlayerTab(player); });
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
        _confirmCreateMatchButton.onClick.RemoveListener(CreateMatch);
        _confirmCreateMatchButton.onClick.AddListener(CreateMatch);
    }
    #endregion

    private void ChangeMap(string mapName)
    {
        _mapButton.GetComponentInChildren<TMP_Text>().text = mapName;
        CloseAllPanel();
    }
    
    private void ChangePlayer(string playerName)
    {
        _tempClickedPlayer.GetComponentInChildren<TMP_Text>().text = playerName;
        CloseAllPanel();
    }

    private RefinedMatchData SetRefinedMatchData()
    {
        
        List<(string, Role)> playerList = new List<(string, Role)>();
        for (int i = 0; i < _players.Length; i++)
        {
            (string, Role) player;
            player.Item1 = _players[i].GetComponentInChildren<TMP_Text>().text;
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
            beginIndex = beginIndex,
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
    
    
    private void PopupFindPlayerTab(GameObject player)
    {
        _tempClickedPlayer = player;
        OpenPanel("[Popup] FindPlayer");
        _findPlayerPanelController.FilterSelectedPlayer(_players);
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
            OpenPanel("[PopupPanel] ErrorPopup", false);
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("맵을 선택해주세요");
        }
        else if (!CheckWinnerSavable())
        {
            OpenPanel("[PopupPanel] ErrorPopup", false);
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("승자를 선택해주세요");
        }
        else if (!CheckPlayerSavable())
        {
            OpenPanel("[PopupPanel] ErrorPopup", false);
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("플레이어를 모두 선택해주세요");
        }
        else
        {
            OpenPanel("[PopupPanel] FitterPopup", false);
            FitterMessage.Instance.SetOwner(this.gameObject);
            FitterMessage.Instance.SetDescription("이대로 저장하겠습니까?");
        }
    }

    private void CreateMatch()
    {
        if (!FitterMessage.Instance.IsOwner(this.gameObject)) return;
        RefinedMatchData data = SetRefinedMatchData();
        
        OpenPanel("[PopupPanel] NetworkingPopup");
        NetworkingMessage.Instance.SetOwner(this.gameObject);
        NetworkingMessage.Instance.SetDescription("저장 중 입니다.");
        StartCoroutine(DataController.instance.CreateMatchWithLoading(result =>
        {
            NetworkingMessage.Instance.gameObject.SetActive(false);
            if (result)
            {
                OpenPanel("[PopupPanel] FinishPopup");
                FinishMessage.Instance.SetOwner(this.gameObject);
                FinishMessage.Instance.SetDescription("저장이 완료되었습니다."+ "\n<color=#FF8080>데이터베이스 업데이트 중에는 다른 통신이 불가할 수 있습니다.</color>");
            }
            else
            {
                OpenPanel("[PopupPanel] ErrorPopup");
                ErrorMessage.Instance.SetOwner(this.gameObject);
                ErrorMessage.Instance.SetDescription("통신 중 문제가 발생하였습니다.");
            }
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
        foreach (var player in _players)
        {
            if (player.GetComponentInChildren<TMP_Text>().text == "-")
                return false;
        }

        return true;
    }
    #endregion
}
