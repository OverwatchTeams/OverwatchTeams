using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoleStatisticPanelController : PanelController
{
    [SerializeField] private GameObject _playerScoreContainer;
    [SerializeField] private GameObject _playerScorePrefab;
    [SerializeField] private GameObject _currentWinRateContainer;
    [SerializeField] private GameObject _currentWinRatePrefab;
    [SerializeField] private Button _dealerButton;
    [SerializeField] private Button _tankerButton;
    [SerializeField] private Button _healerButton;
    [SerializeField] private Button _attendanceButton;
    [SerializeField] private Button _winRateButton;
    [SerializeField] private GameObject _editButton;
    [SerializeField] private GameObject _editFinishButton;
    [SerializeField] private ScrollRect _playerScoreScroll;
    [SerializeField] private ScrollRect _currentWinRateScroll;
    private string _role = "dealer";
    private string _filter = "attendance";
    private List<PlayerScorePrefab> _playerScores;
    private bool syncing = false;
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        _role = "dealer";
        _filter = "attendance";
        StartCoroutine(SetTable());
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _editButton.GetComponent<Button>().onClick.RemoveListener(OnEditButtonClicked);
        _editFinishButton.GetComponent<Button>().onClick.RemoveListener(OnEditFinishButtonClicked);
        _dealerButton.onClick.RemoveListener(OnDealerButtonClicked);
        _tankerButton.onClick.RemoveListener(OnTankerButtonClicked);
        _healerButton.onClick.RemoveListener(OnHealerButtonClicked);
        _attendanceButton.onClick.RemoveListener(OnAttendanceButtonClicked);
        _winRateButton.onClick.RemoveListener(OnWinRateButtonClicked);
        _editButton.GetComponent<Button>().onClick.AddListener(OnEditButtonClicked);
        _editFinishButton.GetComponent<Button>().onClick.AddListener(OnEditFinishButtonClicked);
        _dealerButton.onClick.AddListener(OnDealerButtonClicked);
        _tankerButton.onClick.AddListener(OnTankerButtonClicked);
        _healerButton.onClick.AddListener(OnHealerButtonClicked);
        _attendanceButton.onClick.AddListener(OnAttendanceButtonClicked);
        _winRateButton.onClick.AddListener(OnWinRateButtonClicked);
        _editButton.SetActive(true);
        _editFinishButton.SetActive(false);
        _playerScoreScroll.onValueChanged.RemoveListener(OnScrollPlayerScore);
        _currentWinRateScroll.onValueChanged.RemoveListener(OnScrollCurrentWinRate);
        _playerScoreScroll.onValueChanged.AddListener(OnScrollPlayerScore);
        _currentWinRateScroll.onValueChanged.AddListener(OnScrollCurrentWinRate);
    }

    private IEnumerator SetTable()
    {
        _playerScores = new List<PlayerScorePrefab>();
        //맵 선택률 컨테이너 초기화
        foreach (Transform child in _playerScoreContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _currentWinRateContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //랭크 불러오기 승률
        int index1 = 1;
        int index2 = 1;
        yield return StartCoroutine(DataController.instance.GetMainLeaderBoardData("month", DataController.instance.RecordDropdownDates.month.First()));
        List<KeyValuePair<string, WinRateData.WinRate.RoleWinRate.SimpleWinRate>> sortedRole = new List<KeyValuePair<string, WinRateData.WinRate.RoleWinRate.SimpleWinRate>>();
        Dictionary<string, WinRateData.WinRate.MapWinRate> recentWinRate = new Dictionary<string, WinRateData.WinRate.MapWinRate>();
        if (_filter == "attendance")
        {
            
        }
        if (_role == "dealer")
        {
            if (_filter == "attendance")
            {
                sortedRole = DataController.instance.WinRateData.winRate.role.D
                    .OrderByDescending(map => map.Value.wins + map.Value.losses + map.Value.draws)
                    .ToList();
            }
            else
            {
                sortedRole = DataController.instance.WinRateData.winRate.role.D
                    .OrderByDescending(map => map.Value.winRate)
                    .ToList();
            }
        }
        else if (_role == "tanker")
        {
            if (_filter == "attendance")
            {
                sortedRole = DataController.instance.WinRateData.winRate.role.T
                    .OrderByDescending(map => map.Value.wins + map.Value.losses + map.Value.draws)
                    .ToList();
            }
            else
            {
                sortedRole = DataController.instance.WinRateData.winRate.role.T
                    .OrderByDescending(map => map.Value.winRate)
                    .ToList();
            }
        }
        else if (_role == "healer")
        {
            if (_filter == "attendance")
            {
                sortedRole = DataController.instance.WinRateData.winRate.role.H
                    .OrderByDescending(map => map.Value.wins + map.Value.losses + map.Value.draws)
                    .ToList();
            }
            else
            {
                sortedRole = DataController.instance.WinRateData.winRate.role.H
                    .OrderByDescending(map => map.Value.winRate)
                    .ToList();
            }
        }
        
        foreach (var role in sortedRole)
        {
            foreach (var player in DataController.instance.ClanPlayers)
            {
                if (player.player != role.Key) continue;
                GameObject go = Instantiate(_playerScorePrefab, _playerScoreContainer.transform);
                PlayerScorePrefab playerInfo = go.GetComponent<PlayerScorePrefab>();
                _playerScores.Add(playerInfo);
                if (index1 % 2 == 0)
                {
                    foreach (var image in playerInfo.gameObject.GetComponentsInChildren<Image>())
                    {
                        image.sprite = Resources.Load<Sprite>("Images/backGround_table04");
                    }
                    if (_role == "dealer")
                    {
                        playerInfo._delerBackground.sprite = Resources.Load<Sprite>("Images/backGround_table05");
                    }
                    else if (_role == "tanker")
                    {
                        playerInfo._tankerBackground.sprite = Resources.Load<Sprite>("Images/backGround_table05");
                    }
                    else if (_role == "healer")
                    {
                        playerInfo._healerBackground.sprite = Resources.Load<Sprite>("Images/backGround_table05");
                    }
                }
                else
                {
                    foreach (var image in playerInfo.gameObject.GetComponentsInChildren<Image>())
                    {
                        image.sprite = Resources.Load<Sprite>("Images/backGround_table03");
                    }
                    if (_role == "dealer")
                    {
                        playerInfo._delerBackground.sprite = Resources.Load<Sprite>("Images/backGround_table02");
                    }
                    else if (_role == "tanker")
                    {
                        playerInfo._tankerBackground.sprite = Resources.Load<Sprite>("Images/backGround_table02");
                    }
                    else if (_role == "healer")
                    {
                        playerInfo._healerBackground.sprite = Resources.Load<Sprite>("Images/backGround_table02");
                    }
                }

                playerInfo._name.text = player.player;
                playerInfo._dealerScore.text = player.scores.D.ToString();
                playerInfo._tankerScore.text = player.scores.T.ToString();
                playerInfo._healerScore.text = player.scores.H.ToString();
                index1++;
            
                if (player.player != role.Key) continue;
                GameObject go2 = Instantiate(_currentWinRatePrefab, _currentWinRateContainer.transform);
                RecentWinRateRankPrefab playerInfo2 = go2.GetComponent<RecentWinRateRankPrefab>();
                if (index2 % 2 == 0)
                {
                    foreach (var image in playerInfo2.gameObject.GetComponentsInChildren<Image>())
                    {
                        image.sprite = Resources.Load<Sprite>("Images/backGround_table04");
                    }
                    playerInfo2._winRateBackground.sprite = Resources.Load<Sprite>("Images/backGround_table05");
                    playerInfo2._roundBackground.sprite = Resources.Load<Sprite>("Images/backGround_table05");
                    
                }
                else
                {
                    foreach (var image in playerInfo2.gameObject.GetComponentsInChildren<Image>())
                    {
                        image.sprite = Resources.Load<Sprite>("Images/backGround_table03");
                    }
                    playerInfo2._winRateBackground.sprite = Resources.Load<Sprite>("Images/backGround_table02");
                    playerInfo2._roundBackground.sprite = Resources.Load<Sprite>("Images/backGround_table02");
                    
                }

                playerInfo2._name.text = role.Key;
                playerInfo2._winRate.text = ((int)role.Value.winRate).ToString() + "%";
                playerInfo2._round.text = (role.Value.draws + role.Value.wins + role.Value.losses).ToString();
                index2++;
            }
        }
    }

    private void OnEditButtonClicked()
    {
        foreach (var playerScore in _playerScores)
        {
            playerScore._dealerScore.interactable = true;
            playerScore._tankerScore.interactable = true;
            playerScore._healerScore.interactable = true;
        }
        _editButton.SetActive(false);
        _editFinishButton.SetActive(true);
    }

    private void OnEditFinishButtonClicked()
    {
        foreach (var playerScore in _playerScores)
        {
            playerScore._dealerScore.interactable = false;
            playerScore._tankerScore.interactable = false;
            playerScore._healerScore.interactable = false;
        }
        _editButton.SetActive(true);
        StartCoroutine(UpdatePlayerScores());
        _editFinishButton.SetActive(false);
    }

    private IEnumerator UpdatePlayerScores()
    {
        foreach (var playerScore in _playerScores)
        {
            for(int index = 0; index < DataController.instance.ClanPlayers.Length; index++)
            {
                bool isChanged = false;
                if (playerScore._name.text != DataController.instance.ClanPlayers[index].player) continue;
                playerScore._dealerScore.text ??= "0";
                playerScore._tankerScore.text ??= "0";
                playerScore._healerScore.text ??= "0";
                
                //수정 됐는지 여부 확인
                if(playerScore._dealerScore.text != DataController.instance.ClanPlayers[index].scores.D.ToString()) isChanged = true;
                if(playerScore._tankerScore.text != DataController.instance.ClanPlayers[index].scores.T.ToString()) isChanged = true;
                if(playerScore._healerScore.text != DataController.instance.ClanPlayers[index].scores.H.ToString()) isChanged = true;
                //수정된 데이터는 업데이트
                if (isChanged)
                {
                    PlayerData player = new PlayerData();
                    player.scores = new PlayerData.Scores();
                    player.player = DataController.instance.ClanPlayers[index].player;
                    player.scores.D = Convert.ToInt32(playerScore._dealerScore.text);
                    player.scores.T = Convert.ToInt32(playerScore._tankerScore.text);
                    player.scores.H = Convert.ToInt32(playerScore._healerScore.text);
                    yield return StartCoroutine(DataController.instance.CreatePlayer(
                        result => {
                            if (!result) OpenPanel("[Popup] UpdatePlayerErrorMessage");
                        }, player)
                    );
                }
            }
        }
        //플레이어 정보 업데이트
        yield return StartCoroutine(DataController.instance.GetClanPlayerDatas());
        OpenPanel("[Popup] UpdatePlayerFinishMessage");
    }

    private void OnDealerButtonClicked()
    {
        _role = "dealer";
        StartCoroutine(SetTable());
    }

    private void OnTankerButtonClicked()
    {
        _role = "tanker";
        StartCoroutine(SetTable());
    }

    private void OnHealerButtonClicked()
    {
        _role = "healer";
        StartCoroutine(SetTable());
    }
    
    private void OnAttendanceButtonClicked()
    {
        _filter = "attendance";
        StartCoroutine(SetTable());
    }

    private void OnWinRateButtonClicked()
    {
        _filter = "winRate";
        StartCoroutine(SetTable());
    }

    private void OnScrollPlayerScore(Vector2 pos)
    {
        if (syncing) return;
        syncing = true;
        _currentWinRateScroll.verticalNormalizedPosition = _playerScoreScroll.verticalNormalizedPosition;
        syncing = false;
    }

    private void OnScrollCurrentWinRate(Vector2 pos)
    {
        if (syncing) return;
        syncing = true;
        _playerScoreScroll.verticalNormalizedPosition = _currentWinRateScroll.verticalNormalizedPosition;
        syncing = false;
    }
    
}
