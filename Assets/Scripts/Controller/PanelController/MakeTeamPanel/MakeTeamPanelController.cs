using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakeTeamPanelController : PanelController
{
    [SerializeField] private Text _date;
    [SerializeField] private Button _mapButton;
    [SerializeField] private Button[] _playerButtons;
    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _confirmCreateMatchButton;
    [SerializeField] private GameObject _calendar;
    [SerializeField] private Button _openMatchSupportButton;
    [SerializeField] private Button _openFindPlayerButton;

    [SerializeField] private FindPlayerGroupPanelController _findPlayerGroupPanelController;
    [SerializeField] private FindMapPanelController _findMapPanelController;
    [SerializeField] private MatchSupportPanelController _matchSupportPanelController;
    private MatchData _latestMatchData;
    private Button _tempClickedPlayerButton;
    private int currentCycle;
    private int beginIndex;

    #region Initialization

    private void OnEnable()
    {
        _findMapPanelController.OnMapClicked -= ChangeMap;
        _findMapPanelController.OnMapClicked += ChangeMap;
        _findPlayerGroupPanelController.OnPoolUpdated -= UpdatePlayerPool;
        _findPlayerGroupPanelController.OnPoolUpdated += UpdatePlayerPool;
        
        StartCoroutine(MatchDataManager.instance.GetLastMatch(result =>
        {
            if (result != null)
            {
                _latestMatchData = result;
                currentCycle = result.round + 1;
                beginIndex = result.index + 1;
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
        foreach (var button in _playerButtons)
        {
            //드래그 드랍으로 바꿀 예정
            /*button.onClick.RemoveListener();
            button.onClick.AddListener();*/
        }

        _mapButton.onClick.RemoveListener(PopupFindMapTab);
        _finishButton.onClick.RemoveListener(PopupFinish);
        _mapButton.onClick.AddListener(PopupFindMapTab);
        _finishButton.onClick.AddListener(PopupFinish);
        _confirmCreateMatchButton.onClick.RemoveListener(CreateMatch);
        _confirmCreateMatchButton.onClick.AddListener(CreateMatch);
        _openMatchSupportButton.onClick.RemoveListener(OpenMatchSupport);
        _openMatchSupportButton.onClick.AddListener(OpenMatchSupport);
        _openFindPlayerButton.onClick.RemoveListener(OpenFindPlayer);
        _openFindPlayerButton.onClick.AddListener(OpenFindPlayer);
    }

    #endregion

    private void ChangeMap(string mapName)
    {
        _mapButton.GetComponentInChildren<TMP_Text>().text = mapName;
        CloseAllPanel();
    }

    private void OpenMatchSupport()
    {
        OpenPanel("[Panel] MatchSupport");
        string[] names = _playerButtons
            .Select(btn => btn.gameObject.GetComponentInChildren<TMP_Text>().text)
            .ToArray();
        _matchSupportPanelController.UpdatePlayers(names);
        
    }

    //FinPlayer 창 Open
    private void OpenFindPlayer()
    {
        OpenPanel("[Popup] FindPlayer");
        Button[] buttons = new[] { 0, 5, 1, 6, 2, 7, 3, 8, 4, 9 }
            .Select(i => _playerButtons[i])
            .ToArray();
        _findPlayerGroupPanelController.InitializePlayerPool(buttons);
    }

    //FindPlayer에서 업데이트된 Pool정보를 동기화
    private void UpdatePlayerPool(string[] foundPlayers)
    {
        string[] playerPool = new[] { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 }
            .Select(i => foundPlayers[i])
            .ToArray();
        
        for (int i = 0; i < _playerButtons.Length; i++)
        {
            _playerButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text = playerPool[i];
        }
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

        //WinnerTeam winnerTeam;
        //winnerTeam = GetWinner();
        return new RefinedMatchData
        {
            beginIndex = beginIndex,
            date = Convert.ToDateTime(_date.text),
            round = currentCycle,
            map = _mapButton.GetComponentInChildren<TMP_Text>().text,
            players = playerList,
            //winner = winnerTeam
        };
    }

    /*private WinnerTeam GetWinner()
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
    }*/

    private void PopupFindMapTab()
    {
        OpenPanel("[Popup] FindMap");
    }

    private void PopupFinish()
    {
        if (!CheckMapSavable())
        {
            OpenPanel("[PopupPanel] ErrorPopup", false);
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("맵을 선택해주세요");
        }
        /*else if (!CheckWinnerSavable())
        {
            OpenPanel("[PopupPanel] ErrorPopup", false);
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("승자를 선택해주세요");
        }*/
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
        StartCoroutine(DataController.instance.CreateMatch(result =>
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

    /*private bool CheckWinnerSavable()
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
    }*/

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
