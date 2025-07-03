using System;
using System.Collections;
using System.Collections.Generic;
using RainbowArt.CleanFlatUI;
using Unity.VisualScripting;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;

    private MapData[] _maps;
    private MapTypeData[] _maptypes;
    private PlayerData[] _clanPlayers;
    private PlayerData[] _allPlayers;
    private RecordDropdownDate _recordDropdownDates;
    private WinRateData _winRateData;
    private GameDatas _gameDatas;
    private DailyGameData _dailyGameData;

    public MapData[] Maps => _maps.Clone() as MapData[];
    public MapTypeData[] MapTypes => _maptypes.Clone() as MapTypeData[];
    public PlayerData[] ClanPlayers => _clanPlayers.Clone() as PlayerData[];
    public PlayerData[] AllPlayers => _allPlayers.Clone() as PlayerData[];
    public RecordDropdownDate RecordDropdownDates => _recordDropdownDates;
    public WinRateData WinRateData => _winRateData;
    public GameDatas GameDatas => _gameDatas;
    public DailyGameData DailyGameData => _dailyGameData;
    
    [SerializeField] private ModalWindowProgressBarLoop _defaultProgressPopup;
    [SerializeField] private ProgressBarLoop _miniProgressPopup;
    [SerializeField] private ProgressBarLoop _circularProgressPopup;
    [SerializeField] private ModalWindow _warningPopup;
    
    private InformMessage _informMessage;
    public bool _isLoading;

    #region Initiaize
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _informMessage = new InformMessage();
        StartCoroutine(InitializeData());
    }

    private void StartLoading()
    {
        _isLoading = true;
        _defaultProgressPopup.ShowModalWindow();
    }
    private void StartMiniLoading()
    {
        _isLoading = true;
        _miniProgressPopup.gameObject.SetActive(true);
    }
    private void StartCircularLoading()
    {
        _isLoading = true;
        _circularProgressPopup.gameObject.SetActive(true);
    }
    
    private void FinishLoading()
    {
        _isLoading = false;
        _defaultProgressPopup.HideModalWindow();
    }
    
    private void FinishMiniLoading()
    {
        _isLoading = false;
        _miniProgressPopup.gameObject.SetActive(false);
    }

    private void ErrorOccured(string errorCode)
    {
        _warningPopup.DescriptionValue = errorCode;
        _warningPopup.ShowModalWindow();
    }
    
    private void FinishCircularLoading()
    {
        _isLoading = false;
        _circularProgressPopup.gameObject.SetActive(false);
    }

    private IEnumerator InitializeData()
    {
        bool isSucceed = false;
        StartLoading();
        //맵 데이터 캐싱
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(PlayerDataManager.instance.GetAllPlayers(success => isSucceed = success, OnClanPlayersLoaded, SetClanPlayerURLData()));
        if (!isSucceed) ErrorOccured("[901] Get ClanPlayers Failed : InitializeData");
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MapDataManager.instance.GetAllData(success => isSucceed = success, OnMapLoaded));
        if (!isSucceed) ErrorOccured("[902] Get Maps Failed");
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllData(success => isSucceed = success, OnMapTypeLoaded));
        if (!isSucceed) ErrorOccured("[903] Get MapTypes Failed");
        FinishLoading();
    }
    #endregion
    
    #region MainData
    private void OnLeaderBoardLoaded(WinRateData winRateData)
    {
        _winRateData = winRateData;
    }
    private void OnGameDatasLoaded(GameDatas gameDatas)
    {
        _gameDatas = gameDatas;
    }
    
    private void OnDailyGameDataLoaded(DailyGameData dailyGameData)
    {
        _dailyGameData = dailyGameData;
    }
    
    private IEnumerator UpdateMainData()
    {
        bool isSucceed = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            yield break;
        }
        StartMiniLoading();
        _miniProgressPopup.TextValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MainDataManager.instance.UpdateMainDocument(success => isSucceed = success));
        FinishMiniLoading();
        if (!isSucceed) ErrorOccured("[904] Update MainDocument Failed");
    }
    public IEnumerator GetMainLeaderBoardData(string category, string date)
    {
        bool isSucceed = false;
        if (_isLoading || (category != "year" && category != "month" && category != "day"))
        {
            _warningPopup.ShowModalWindow();
            yield break;
        } 
        StartCircularLoading();
        yield return StartCoroutine(MainDataManager.instance.GetLeaderBoard(success => isSucceed = success, OnLeaderBoardLoaded, category, date));
        FinishCircularLoading();
        if (!isSucceed) ErrorOccured("[905] Get LeaderBoard Failed");
    }
    public IEnumerator GetMainGameDatas(string category, string date)
    {
        bool isSucceed = false;
        if (_isLoading || (category != "year" && category != "month" && category != "day"))
        {
            _warningPopup.ShowModalWindow();
            yield break;
        } 
        StartCircularLoading();
        yield return StartCoroutine(MainDataManager.instance.GetGameDatas(success => isSucceed = success, OnGameDatasLoaded, category, date));
        FinishCircularLoading();
        if (!isSucceed) ErrorOccured("[906] Get GameDatas Failed");
    }

    public IEnumerator GetMainDailyData(string date)
    {
        bool isSucceed = false;
        StartCircularLoading();
        yield return StartCoroutine(MainDataManager.instance.GetMainDailyData(success => isSucceed = success, OnDailyGameDataLoaded,date));
        FinishCircularLoading();
        if (!isSucceed) ErrorOccured("[906] Get GameDatas Failed");
    }
    #endregion
        
    #region PlayerData

    private void OnClanPlayersLoaded(PlayerData[] players)
    {
        _clanPlayers = players;
    }
    private void OnAllPlayersLoaded(PlayerData[] players)
    {
        _allPlayers = players;
    }
    
    private PlayerURLData SetClanPlayerURLData()
    {
        PlayerURLData playerURLData = new PlayerURLData();
        playerURLData.isClanMember = "true";
        playerURLData.fields = new List<string> { "player", "scores" , "subNames", "dates"};
        playerURLData.sortType = "recent";
        return playerURLData;
    }
    
    private PlayerURLData SetAllPlayerURLData()
    {
        PlayerURLData playerURLData = new PlayerURLData();
        playerURLData.isClanMember = "false";
        playerURLData.fields = new List<string> { "player", "scores", "isClanMember", "subNames", "dates" };
        playerURLData.sortType = "recent";
        return playerURLData;
    }
    
    private IEnumerator UpdatePlayerDatas()
    {
        bool isSucceed = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            yield break;
        }
        StartMiniLoading();
        _miniProgressPopup.TextValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(PlayerDataManager.instance.UpdatePlayerDocuments(success => isSucceed = success));
        if (!isSucceed) ErrorOccured("[907] UpdatePlayerDocuments Failed");
        _miniProgressPopup.TextValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(PlayerDataManager.instance.GetAllPlayers(success => isSucceed = success, OnClanPlayersLoaded, SetClanPlayerURLData()));
        FinishMiniLoading();
        if (!isSucceed) ErrorOccured("[901] Get ClanPlayers Failed : UpdatePlayeDatas");
    }
    
    public IEnumerator CreatePlayer(Action<bool> OnFinished, PlayerData player)
    {
        bool isSucceed = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnFinished?.Invoke(false);
            yield break;
        }
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(PlayerDataManager.instance.AddData(success => isSucceed = success, player));
        if (!isSucceed) ErrorOccured("[908] Add Data Failed");
        yield return StartCoroutine(PlayerDataManager.instance.GetAllPlayers(success => isSucceed = success, OnClanPlayersLoaded, SetClanPlayerURLData()));
        yield return StartCoroutine(PlayerDataManager.instance.GetAllPlayers(success => isSucceed = success, OnAllPlayersLoaded, SetAllPlayerURLData()));
        FinishLoading();
        OnFinished?.Invoke(isSucceed);
        if (!isSucceed) ErrorOccured("[901] Get ClanPlayers Failed : CreatePlayer");
    }
    
    public IEnumerator GetClanPlayerDatas()
    {
        bool isSucceed = false;
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(PlayerDataManager.instance.GetAllPlayers(success => isSucceed = success, OnClanPlayersLoaded, SetClanPlayerURLData()));
        FinishLoading();
        if (!isSucceed) ErrorOccured("[901] Get ClanPlayers Failed : GetClanPlayerDatas");
    }
    
    public IEnumerator GetAllPlayerDatas(PlayerURLData playerURLData)
    {
        bool isSucceed = false;
        StartCircularLoading();
        playerURLData.isClanMember = "false";
        yield return StartCoroutine(PlayerDataManager.instance.GetAllPlayers(success => isSucceed = success, OnAllPlayersLoaded, SetAllPlayerURLData()));
        FinishCircularLoading();
        if (!isSucceed) ErrorOccured("[910] Get AllPlayers Failed");
    }

    public IEnumerator GetPlayerData(Action<bool> OnCompleted, Action<PlayerData> OnPlayerLoaded, string playerName)
    {
        StartCircularLoading();
        yield return StartCoroutine(PlayerDataManager.instance.GetPlayerData(OnCompleted, OnPlayerLoaded, playerName));
        FinishCircularLoading();
    }
    #endregion
    
    #region MatchData
    public IEnumerator CreateMatch(Action<bool> OnCreated, RefinedMatchData match)
    {
        bool isSucceed = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnCreated?.Invoke(false);
            yield break;
        }
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MatchDataManager.instance.AddData(success => isSucceed = success, match));
        if (!isSucceed) ErrorOccured("[911] CreateMatch Failed");
        OnCreated?.Invoke(isSucceed);
        Debug.Log($"Add Data {isSucceed}");
        FinishLoading();
        UpdateMainAndPlayerDatas();
    }
    private void UpdateMainAndPlayerDatas()
    {
        StartCoroutine(UpdateMainAndPlayeDatasEnumerator());
    }

    private IEnumerator UpdateMainAndPlayeDatasEnumerator()
    {
        yield return StartCoroutine(UpdateMainData());
        yield return StartCoroutine(UpdatePlayerDatas());
    }
    
    #endregion

    #region MapData
    private void OnMapLoaded(MapData[] maps)
    {
        _maps = maps;
    }
    private void OnMapTypeLoaded(MapTypeData[] maptypes)
    {
        _maptypes = maptypes;
    }
    public IEnumerator CreateMap(Action<bool> OnFinished, MapData map)
    {
        bool isSucceed = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnFinished?.Invoke(false);
            yield break;
        }
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MapDataManager.instance.AddData(success => isSucceed = success, map));
        if (!isSucceed) ErrorOccured("[912] Add Map Failed");
        yield return StartCoroutine(MapDataManager.instance.GetAllData(success => isSucceed = success, OnMapLoaded));
        OnFinished?.Invoke(isSucceed);
        FinishLoading();
        if (!isSucceed) ErrorOccured("[913] Get AllMap Failed");
    }
    public IEnumerator CreateMapType(Action<bool> OnFinished, MapTypeData maptype)
    {
        bool isSucceed = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnFinished?.Invoke(false);
            yield break;
        }
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MapTypeDataManager.instance.AddData(success => isSucceed = success, maptype));
        if (!isSucceed) ErrorOccured("[914] Add MapType Failed");
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllData(success => isSucceed = success, OnMapTypeLoaded));
        OnFinished?.Invoke(isSucceed);
        FinishLoading();
        if (!isSucceed) ErrorOccured("[915] Get AllMapType Failed");
    }
    #endregion

    #region DropDownData
    public void OnDropdownDatesLoaded(RecordDropdownDate dropdownDates)
    {
        _recordDropdownDates = dropdownDates;
    }
    
    public IEnumerator GetDropdownDate(Action<bool> OnFinished)
    {
        bool isSucceed = false;
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MainDataManager.instance.GetDropDownDates(success => isSucceed = success, OnDropdownDatesLoaded));
        OnFinished?.Invoke(isSucceed);
        FinishLoading();
        if (!isSucceed) ErrorOccured("[916] Get DropDownDates Failed");
    }
    #endregion
}
