using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RainbowArt.CleanFlatUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalRecordController : PanelController
{
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private Toggle _isClanPlayerToggle;
    [SerializeField] private Toggle _isVoiceAvailable;
    [SerializeField] private GameObject _subNamesContainer;
    [SerializeField] private GameObject _subNamePrefab;
    [SerializeField] private TMP_Text _firstAttendance;
    [SerializeField] private TMP_Text _lastAttendance;
    [SerializeField] private TMP_InputField _dealerInputField;
    [SerializeField] private TMP_InputField _tankerInputField;
    [SerializeField] private TMP_InputField _healerInputField;
    [SerializeField] private GameObject _editLayer;
    [SerializeField] private GameObject _scoreField;
    
    // 연도 월별 승률 테이블 셋
    [SerializeField] private GameObject _dateCategorySelector;
    [SerializeField] private TMP_Dropdown _dateDropdown;
    [SerializeField] private GameObject _mapWinRateRowPrefab;
    [SerializeField] private GameObject _mapWinRateContainer;
    [SerializeField] private GameObject _roleWinRateRowPrefab;
    [SerializeField] private GameObject _roleWinRateContainer;
    [SerializeField] private Button _mapWinRateTableMapNameButton;
    [SerializeField] private Button _mapWinRateTableWinButton;
    [SerializeField] private Button _mapWinRateTableDrawButton;
    [SerializeField] private Button _mapWinRateTableLoseButton;
    [SerializeField] private Button _mapWinRateTableRoundButton;
    [SerializeField] private Button _mapWinRateTableWinRateButton;
    
    // 시너지 테이블 셋
    [SerializeField] private GameObject _playerSelector;
    [SerializeField] private GameObject _opponentSelector;
    [SerializeField] private GameObject _teamSelector;
    [SerializeField] private GameObject _synergyRowPrefab;
    [SerializeField] private GameObject _synergyContainer;
    [SerializeField] private Button _synergyTableNameButton;
    [SerializeField] private Button _synergyTableRoundButton;
    [SerializeField] private Button _synergyTableWinRateButton;
    
    [SerializeField] private FindPlayerPanelController _findPlayerPanelController;
    [SerializeField] private ModalWindow _warningPopup;
    [SerializeField] private List<ScrollRect> _scrollRects;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _editFinishButton;
    
    public PlayerData _playerData;
    private List<string> _yearItems = new List<string>();
    private List<string> _monthItems = new List<string>();
    private string _winRateFilter = "map";
    private string _synergyFilter = "round";
    
    private string _yearOrMonthFilter;
    private string _dateFilter;
    private string _playerFilter = "딜러";
    private string _opponentFilter = "딜러";
    private string _teamFilter = "아군";
    private void OnEnable()
    {
        _playerData = new PlayerData();
        _findPlayerPanelController.OnPlayerClicked -= ChangePlayer;
        _findPlayerPanelController.OnPlayerClicked += ChangePlayer;
        _editButton.gameObject.SetActive(false);
        _editFinishButton.gameObject.SetActive(false);
        _editLayer.SetActive(false);
        _scoreField.SetActive(false);
        _isVoiceAvailable.interactable = false;
        _isClanPlayerToggle.interactable = true;
        Initialize();
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        InitializePanel();
        if (_playerData.player != null) return;
        OpenPanel("[Panel] FindPlayer");
    }

    private void InitializePanel()
    {
        InitializeCategoryDates();
        InitializeDefaultInfo();
        InitializeTables();
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _dateCategorySelector.GetComponent<Selector>().OnValueChanged.RemoveListener(OnDateCategoryChanged);
        _dateCategorySelector.GetComponent<Selector>().OnValueChanged.AddListener(OnDateCategoryChanged);
        _dateDropdown.onValueChanged.RemoveListener((OnDateDropdownChanged));
        _dateDropdown.onValueChanged.AddListener((OnDateDropdownChanged));
        _playerSelector.GetComponent<Selector>().OnValueChanged.RemoveListener(OnPlayerCategoryChanged);
        _playerSelector.GetComponent<Selector>().OnValueChanged.AddListener(OnPlayerCategoryChanged);
        _opponentSelector.GetComponent<Selector>().OnValueChanged.RemoveListener(OnOpponentCategoryChanged);
        _opponentSelector.GetComponent<Selector>().OnValueChanged.AddListener(OnOpponentCategoryChanged);
        _teamSelector.GetComponent<Selector>().OnValueChanged.RemoveListener(OnTeamCategoryChanged);
        _teamSelector.GetComponent<Selector>().OnValueChanged.AddListener(OnTeamCategoryChanged);
        
        _mapWinRateTableMapNameButton.onClick.RemoveListener(OnMapWinRateTableMapClicked);
        _mapWinRateTableMapNameButton.onClick.AddListener(OnMapWinRateTableMapClicked);
        _mapWinRateTableWinButton.onClick.RemoveListener(OnMapWinRateTableWinClicked);
        _mapWinRateTableWinButton.onClick.AddListener(OnMapWinRateTableWinClicked);
        _mapWinRateTableDrawButton.onClick.RemoveListener(OnMapWinRateTableDrawClicked);
        _mapWinRateTableDrawButton.onClick.AddListener(OnMapWinRateTableDrawClicked);
        _mapWinRateTableLoseButton.onClick.RemoveListener(OnMapWinRateTableLoseClicked);
        _mapWinRateTableLoseButton.onClick.AddListener(OnMapWinRateTableLoseClicked);
        _mapWinRateTableRoundButton.onClick.RemoveListener(OnMapWinRateTableRoundClicked);
        _mapWinRateTableRoundButton.onClick.AddListener(OnMapWinRateTableRoundClicked);
        _mapWinRateTableWinRateButton.onClick.RemoveListener(OnMapWinRateTableWinRateClicked);
        _mapWinRateTableWinRateButton.onClick.AddListener(OnMapWinRateTableWinRateClicked);
        _synergyTableNameButton.onClick.RemoveListener(OnSynergyTableNameClicked);
        _synergyTableNameButton.onClick.AddListener(OnSynergyTableNameClicked);
        _synergyTableRoundButton.onClick.RemoveListener(OnSynergyTableRoundClicked);
        _synergyTableRoundButton.onClick.AddListener(OnSynergyTableRoundClicked);
        _synergyTableWinRateButton.onClick.RemoveListener(OnSynergyTableWinRateClicked);
        _synergyTableWinRateButton.onClick.AddListener(OnSynergyTableWinRateClicked);
        
        _editButton.onClick.RemoveListener(OnEditButtonClicked);
        _editButton.onClick.AddListener(OnEditButtonClicked);
        _editFinishButton.onClick.RemoveListener(OnFinishEditButtonClicked);
        _editFinishButton.onClick.AddListener(OnFinishEditButtonClicked);
        
        _isClanPlayerToggle.onValueChanged.RemoveListener(OnClanValueChanged);
        _isClanPlayerToggle.onValueChanged.AddListener(OnClanValueChanged);
        
        _isVoiceAvailable.onValueChanged.RemoveListener(OnVoiceValueChanged);
        _isVoiceAvailable.onValueChanged.AddListener(OnVoiceValueChanged);
    }

    private void InitializeDefaultInfo()
    {
        //서브네임 변경
        for (int i = 0; i < _subNamesContainer.transform.childCount; i++)
        {
            Destroy(_subNamesContainer.transform.GetChild(i).gameObject);
        }
        if (_playerData.player == null)
        {
            _playerName.GetComponentInChildren<TMP_Text>().text = "-";
            _isClanPlayerToggle.isOn = true;
            _isClanPlayerToggle.isOn = true;
        }
        else
        {
            if (_playerData.subNames != null)
            {
                for (int i = 0; i < _playerData.subNames.Count; i++)
                {
                    GameObject go = Instantiate(_subNamePrefab, _subNamesContainer.transform, false);
                    go.GetComponent<SubNameButtonPrefab>().ChangeSubNameText(_playerData.subNames[i]);
                }
            }   
            _playerName.GetComponentInChildren<TMP_Text>().text = _playerData.player;
            _isClanPlayerToggle.SetIsOnWithoutNotify(_playerData.isClanMember ?? false);
            _isVoiceAvailable.SetIsOnWithoutNotify(_playerData.isVoiceAvailable ?? false);
            
            _firstAttendance.text = _playerData.dates.first.ToString("yyyy년 MM월 dd일");
            _lastAttendance.text = _playerData.dates.last.ToString("yyyy년 MM월 dd일");
        }
        //연도, 월별 선택 초기화
        _dateCategorySelector.GetComponent<Selector>().CurrentIndex = 1;
        _dateDropdown.value = 0;
        _playerSelector.GetComponent<Selector>().CurrentIndex = 0;
        _opponentSelector.GetComponent<Selector>().CurrentIndex = 0;
        _teamSelector.GetComponent<Selector>().CurrentIndex = 0;

        _yearOrMonthFilter = _dateCategorySelector.GetComponent<Selector>()
            .options[_dateCategorySelector.GetComponent<Selector>().CurrentIndex].optionText;
        _dateFilter = _dateDropdown.itemText.text;
        _playerFilter = _playerSelector.GetComponent<Selector>()
            .options[_playerSelector.GetComponent<Selector>().CurrentIndex].optionText;
        _opponentFilter = _opponentSelector.GetComponent<Selector>()
            .options[_opponentSelector.GetComponent<Selector>().CurrentIndex].optionText;
        _teamFilter = _teamSelector.GetComponent<Selector>()
            .options[_teamSelector.GetComponent<Selector>().CurrentIndex].optionText;
    }

    private void InitializeTables()
    {
        foreach (var scrollRect in _scrollRects)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void InitializeCategoryDates()
    {
        _yearItems.Clear();
        _monthItems.Clear();
        if (_playerData.player != null)
        {
            foreach (var winRate in _playerData.winRates.byYear)
            {
                _yearItems.Add(winRate.Key);
            }
            _yearItems = _yearItems
                .OrderByDescending(y => y)
                .ToList();
            
            foreach (var winRate in _playerData.winRates.byMonth)
            {
                _monthItems.Add(winRate.Key);
            }
            _monthItems = _monthItems
                .OrderByDescending(m => m)
                .ToList();
        }
    }
    private IEnumerator SetDropdown(string yearOrMonth)
    {
        _dateDropdown.ClearOptions();
        if (_playerData.player != null)
        {
            switch (yearOrMonth)
            {
                case "연도별":
                    foreach (var year in _yearItems)
                    {
                        _dateDropdown.options.Add(new TMP_Dropdown.OptionData(year));
                    }
                    break;

                case "월별":
                    foreach (var month in _monthItems)
                    {
                        _dateDropdown.options.Add(new TMP_Dropdown.OptionData(month));
                    }
                    break;
            }
        }
        _dateDropdown.value = 0;
        _dateDropdown.RefreshShownValue();
        
        Canvas.ForceUpdateCanvases();

        yield return null;
        if (_playerData.player != null)
        {
            _dateFilter = _dateDropdown.options[_dateDropdown.value].text;
            SetMapWinRateContainer();
            SetRoleWinRateContainer();   
        }
    }
    
    private void ErrorOccured(string errorCode)
    {
        _warningPopup.DescriptionValue = errorCode;
        _warningPopup.ShowModalWindow();
    }
    private void ChangePlayer(string playerName)
    {
        _playerName.text = playerName;
        StartCoroutine(SetSpecificRecord(playerName));
        if (UserData.instance.GetUserPermission() == Permission.ClanMember) return;
        _editButton.gameObject.SetActive(true);
    }

    private IEnumerator SetSpecificRecord(string playerName)
    {
        bool isSucceed = false;
        yield return StartCoroutine(DataController.instance.GetPlayerData(success => isSucceed = success, 
            playerData => _playerData = playerData, playerName));
        if (!isSucceed)
        {
            ErrorOccured("[911] Get Player Data Failed");
            yield break;
        }
        Initialize();
    }

    private void OnDateCategoryChanged(int value)
    {
        _yearOrMonthFilter = _dateCategorySelector.GetComponent<Selector>().options[value].optionText;
        InitializeTables();
        StartCoroutine(SetDropdown(_yearOrMonthFilter));
    }
    
    private void OnDateDropdownChanged(int value)
    {
        _dateFilter = _dateDropdown.options[value].text;
        InitializeTables();
        SetMapWinRateContainer();
        SetRoleWinRateContainer();
    }
    
    private void OnPlayerCategoryChanged(int value)
    {
        _playerFilter = _playerSelector.GetComponent<Selector>().options[value].optionText;
        InitializeTables();
        SetSynergyContainer();
    }
    
    private void OnOpponentCategoryChanged(int value)
    {
        _opponentFilter = _opponentSelector.GetComponent<Selector>().options[value].optionText;
        
        InitializeTables();
        SetSynergyContainer();
    }
    
    private void OnTeamCategoryChanged(int value)
    {
        _teamFilter = _teamSelector.GetComponent<Selector>().options[value].optionText;

        InitializeTables();
        SetSynergyContainer();
    }

    private void OnMapWinRateTableMapClicked()
    {
        _winRateFilter = "map";
        SetMapWinRateContainer();
    }
    private void OnMapWinRateTableWinClicked()
    {
        _winRateFilter = "win";
        SetMapWinRateContainer();
    }
    private void OnMapWinRateTableDrawClicked()
    {
        _winRateFilter = "draw";
        SetMapWinRateContainer();
    }
    private void OnMapWinRateTableLoseClicked()
    {
        _winRateFilter = "lose";
        SetMapWinRateContainer();
    }
    private void OnMapWinRateTableRoundClicked()
    {
        _winRateFilter = "round";
        SetMapWinRateContainer();
    }
    private void OnMapWinRateTableWinRateClicked()
    {
        _winRateFilter = "winRate";
        SetMapWinRateContainer();
    }
    private void OnSynergyTableNameClicked()
    {
        _synergyFilter = "name";
        SetSynergyContainer();
    }
    private void OnSynergyTableRoundClicked()
    {
        _synergyFilter = "round";
        SetSynergyContainer();
    }
    private void OnSynergyTableWinRateClicked()
    {
        _synergyFilter = "winRate";
        SetSynergyContainer();
    }

    private void OnEditButtonClicked()
    {
        _editButton.gameObject.SetActive(false);
        _editFinishButton.gameObject.SetActive(true);
        
        _scoreField.gameObject.SetActive(true);
        _dealerInputField.text = ((float)_playerData.scores.D / 100 % 1 == 0)
            ? ((int)((float)_playerData.scores.D / 100)).ToString()
            : string.Format($"{(float)_playerData.scores.D / 100:0.##}");
        _tankerInputField.text = ((float)_playerData.scores.T / 100 % 1 == 0)
            ? ((int)((float)_playerData.scores.T / 100)).ToString()
            : string.Format($"{(float)_playerData.scores.T / 100:0.##}");
        _healerInputField.text = ((float)_playerData.scores.H / 100 % 1 == 0)
            ? ((int)((float)_playerData.scores.H / 100)).ToString()
            : string.Format($"{(float)_playerData.scores.H / 100:0.##}");
        
        _isVoiceAvailable.interactable = true;
        _isClanPlayerToggle.interactable = true;
        _editLayer.SetActive(true);
    }
    
    private void OnFinishEditButtonClicked()
    {
        _editButton.gameObject.SetActive(true);
        _editFinishButton.gameObject.SetActive(false);
        
        _scoreField.gameObject.SetActive(false);
        _isVoiceAvailable.interactable = false;
        _isClanPlayerToggle.interactable = false;
        _editLayer.SetActive(false);
        _scoreField.gameObject.SetActive(false);

        if ((int)(float.Parse(_dealerInputField.text) * 100) != _playerData.scores.D ||
            (int)(float.Parse(_tankerInputField.text) * 100) != _playerData.scores.T ||
            (int)(float.Parse(_healerInputField.text) * 100) != _playerData.scores.H)
        {
            PlayerData playerData = new PlayerData();
            playerData.player = _playerData.player;
            playerData.scores = new PlayerData.Scores();
        
            playerData.scores.D = _playerData.scores.D = (int)(float.Parse(_dealerInputField.text) * 100);
            playerData.scores.T = _playerData.scores.T = (int)(float.Parse(_tankerInputField.text) * 100);
            playerData.scores.H = _playerData.scores.H = (int)(float.Parse(_healerInputField.text) * 100);
        
            playerData.isVoiceAvailable = _playerData.isVoiceAvailable;
            playerData.isClanMember = _playerData.isClanMember;
            StartCoroutine(ChangePlayerInfo(playerData));
        }
    }

    private IEnumerator ChangePlayerInfo(PlayerData playerData)
    {
        yield return StartCoroutine(DataController.instance.CreatePlayer(null, playerData));
        ChangePlayer(_playerData.player);
    }

    private void OnVoiceValueChanged(bool value)
    {
        _playerData.isVoiceAvailable = value;
    }
    
    private void OnClanValueChanged(bool value)
    {
        _playerData.isClanMember = value;
    }

    private void SetMapWinRateContainer()
    {
        if (_playerData.winRates == null) return;
        foreach (Transform child in _mapWinRateContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, PlayerData.WinRate.WinRateDetail.GameDetail> datas = new Dictionary<string, PlayerData.WinRate.WinRateDetail.GameDetail>();
        switch (_yearOrMonthFilter)
        {
            case "연도별":
                datas = _playerData.winRates.byYear[_dateFilter].map;
                break;
            case "월별":
                datas = _playerData.winRates.byMonth[_dateFilter].map;
                break;
        }
        List<KeyValuePair<string, PlayerData.WinRate.WinRateDetail.GameDetail>> sortedDatas = new List<KeyValuePair<string, PlayerData.WinRate.WinRateDetail.GameDetail>>();
        switch (_winRateFilter)
        {
            case "map":
                sortedDatas = datas
                    .OrderBy(map => map.Key)
                    .ToList();
                break;
            case "win":
                sortedDatas = datas
                    .OrderByDescending(map => map.Value.wins)
                    .ToList();
                break;
            case "draw":
                sortedDatas = datas
                    .OrderByDescending(map => map.Value.draws)
                    .ToList();
                break;
            case "lose":
                sortedDatas = datas
                    .OrderByDescending(map => map.Value.playedGames - map.Value.wins - map.Value.draws)
                    .ToList();
                break;
            case "round":
                sortedDatas = datas
                    .OrderByDescending(map => map.Value.playedGames)
                    .ToList();
                break;
            case "winRate":
                sortedDatas = datas
                    .OrderByDescending(map => map.Value.rate)
                    .ToList();
                break;
        }
        

        int i = 1;
        foreach (var mapData in sortedDatas)
        {
            GameObject go = Instantiate(_mapWinRateRowPrefab, _mapWinRateContainer.transform);
            MapWinRateRow mapInfo = go.GetComponent<MapWinRateRow>();
            if (i % 2 == 0)
            {
                foreach (var image in mapInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table04");
                }
            }
            else
            {
                foreach (var image in mapInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table03");
                }
            }
            mapInfo._map.text = mapData.Key;
            mapInfo._win.text = mapData.Value.wins.ToString();
            mapInfo._draw.text = mapData.Value.draws.ToString();
            mapInfo._lose.text = (mapData.Value.playedGames - mapData.Value.wins - mapData.Value.draws).ToString();
            mapInfo._round.text = mapData.Value.playedGames.ToString();
            mapInfo._winRate.text = ((int)Mathf.Round(mapData.Value.rate * 100)).ToString();
            i++;
        }
    }

    private void SetRoleWinRateContainer()
    {
        if (_playerData.winRates == null) return;
        foreach (Transform child in _roleWinRateContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, PlayerData.WinRate.WinRateDetail.GameDetail> datas = new Dictionary<string, PlayerData.WinRate.WinRateDetail.GameDetail>();
        switch (_yearOrMonthFilter)
        {
            case "연도별":
                datas.Add("D", _playerData.winRates.byYear[_dateFilter].role.D);
                datas.Add("T", _playerData.winRates.byYear[_dateFilter].role.T);
                datas.Add("H", _playerData.winRates.byYear[_dateFilter].role.H);
                break;
            case "월별":
                datas.Add("D", _playerData.winRates.byMonth[_dateFilter].role.D);
                datas.Add("T", _playerData.winRates.byMonth[_dateFilter].role.T);
                datas.Add("H", _playerData.winRates.byMonth[_dateFilter].role.H);
                break; 
        }

        int i = 1;
        foreach (var roleData in datas)
        {
            GameObject go = Instantiate(_roleWinRateRowPrefab, _roleWinRateContainer.transform);
            RoleWinRateRow roleInfo = go.GetComponent<RoleWinRateRow>();
            if (i % 2 == 0)
            {
                foreach (var image in roleInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table04");
                }
            }
            else
            {
                foreach (var image in roleInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table03");
                }
            }
            roleInfo._role.text = roleData.Key;
            roleInfo._win.text = roleData.Value.wins.ToString();
            roleInfo._draw.text = roleData.Value.draws.ToString();
            roleInfo._lose.text = (roleData.Value.playedGames - roleData.Value.wins - roleData.Value.draws).ToString();
            roleInfo._round.text = roleData.Value.playedGames.ToString();
            roleInfo._winRate.text = ((int)Mathf.Round(roleData.Value.rate * 100)).ToString();
            i++;
        }
    }
    
    private void SetSynergyContainer()
    {
        if (_playerData.synergy == null) return;
        foreach (Transform child in _synergyContainer.transform)
        {
            Destroy(child.gameObject);
        }

        switch (_playerFilter)
        {
            case "딜러":
                _playerFilter = "D";
                break;
            case "탱커":
                _playerFilter = "T";
                break;
            case "힐러":
                _playerFilter = "H";
                break;
        }
        switch (_opponentFilter)
        {
            case "딜러":
                _opponentFilter = "D";
                break;
            case "탱커":
                _opponentFilter = "T";
                break;
            case "힐러":
                _opponentFilter = "H";
                break;
        }
        
        Dictionary<string, PlayerData.PositionSet.Team.TeamDetail> datas = new Dictionary<string, PlayerData.PositionSet.Team.TeamDetail>();
        string positionSet = _playerFilter + "_" + _opponentFilter;
        switch (_teamFilter)
        {
            case "아군":
                foreach (var synergy in _playerData.synergy)
                    switch (positionSet)
                    {
                        case "D_D":
                            datas.Add(synergy.Key, synergy.Value.D_D.sameTeam);
                            break;
                        case "D_T":
                            datas.Add(synergy.Key, synergy.Value.D_T.sameTeam);
                            break;
                        case "D_H":
                            datas.Add(synergy.Key, synergy.Value.D_H.sameTeam);
                            break;
                        case "T_D":
                            datas.Add(synergy.Key, synergy.Value.T_D.sameTeam);
                            break;
                        case "T_T":
                            datas.Add(synergy.Key, synergy.Value.T_T.sameTeam);
                            break;
                        case "T_H":
                            datas.Add(synergy.Key, synergy.Value.T_H.sameTeam);
                            break;
                        case "H_D":
                            datas.Add(synergy.Key, synergy.Value.H_D.sameTeam);
                            break;
                        case "H_T":
                            datas.Add(synergy.Key, synergy.Value.H_T.sameTeam);
                            break;
                        case "H_H":
                            datas.Add(synergy.Key, synergy.Value.H_H.sameTeam);
                            break;
                    }

                break;
            case "적군":
                foreach (var synergy in _playerData.synergy)
                    switch (positionSet)
                    {
                        case "D_D":
                            datas.Add(synergy.Key, synergy.Value.D_D.oppositeTeam);
                            break;
                        case "D_T":
                            datas.Add(synergy.Key, synergy.Value.D_T.oppositeTeam);
                            break;
                        case "D_H":
                            datas.Add(synergy.Key, synergy.Value.D_H.oppositeTeam);
                            break;
                        case "T_D":
                            datas.Add(synergy.Key, synergy.Value.T_D.oppositeTeam);
                            break;
                        case "T_T":
                            datas.Add(synergy.Key, synergy.Value.T_T.oppositeTeam);
                            break;
                        case "T_H":
                            datas.Add(synergy.Key, synergy.Value.T_H.oppositeTeam);
                            break;
                        case "H_D":
                            datas.Add(synergy.Key, synergy.Value.H_D.oppositeTeam);
                            break;
                        case "H_T":
                            datas.Add(synergy.Key, synergy.Value.H_T.oppositeTeam);
                            break;
                        case "H_H":
                            datas.Add(synergy.Key, synergy.Value.H_H.oppositeTeam);
                            break;
                    }
                break;
        }
        List<KeyValuePair<string, PlayerData.PositionSet.Team.TeamDetail>> sortedDatas = new List<KeyValuePair<string, PlayerData.PositionSet.Team.TeamDetail>>();
        switch (_synergyFilter)
        {
            case "name":
                sortedDatas = datas
                    .OrderBy(map => map.Key)
                    .ToList();
                break;
            
            case "round":
                sortedDatas = datas
                    .OrderByDescending(map => map.Value.games)
                    .ToList();
                break;
            
            case "winRate":
                sortedDatas = datas
                    .OrderByDescending(map => (float)map.Value.wins / map.Value.games)
                    .ToList();
                break;
        }

        int i = 1;
        foreach (var synergyData in sortedDatas)
        {
            GameObject go = Instantiate(_synergyRowPrefab, _synergyContainer.transform);
            SynergyRow synergyInfo = go.GetComponent<SynergyRow>();
            if (i % 2 == 0)
            {
                foreach (var image in synergyInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table04");
                }
            }
            else
            {
                foreach (var image in synergyInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table03");
                }
            }
            synergyInfo._name.text = synergyData.Key;
            synergyInfo._round.text = synergyData.Value.games.ToString();
            if (synergyData.Value.games == 0) synergyInfo._winRate.text = "0";
            else
            {
                synergyInfo._winRate.text = ((int)Mathf.Round(((float)synergyData.Value.wins / synergyData.Value.games) * 100)).ToString();   
            }
            i++;
        }
    }
}
