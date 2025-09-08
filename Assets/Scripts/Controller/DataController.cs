using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<RefinedMatchData> _refinedMatches;
    private string _latestMatchDate;

    public MapData[] Maps => _maps.Clone() as MapData[];
    public MapTypeData[] MapTypes => _maptypes.Clone() as MapTypeData[];
    public PlayerData[] ClanPlayers => _clanPlayers.Clone() as PlayerData[];
    public PlayerData[] AllPlayers => _allPlayers.Clone() as PlayerData[];
    public RecordDropdownDate RecordDropdownDates => _recordDropdownDates;
    public WinRateData WinRateData => _winRateData;
    public GameDatas GameDatas => _gameDatas;
    public DailyGameData DailyGameData => _dailyGameData;
    public List<RefinedMatchData> RefinedMatches => _refinedMatches;
    
    [SerializeField] private ModalWindowProgressBarLoop _defaultProgressPopup;
    [SerializeField] private ProgressBarLoop _miniProgressPopup;
    [SerializeField] private ProgressBarLoop _circularProgressPopup;
    [SerializeField] private ModalWindow _warningPopup;
    
    private InformMessage _informMessage;
    public bool _isLoading;
    public Action OnCreatePlayerFininshed;
    
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
        StartCoroutine(InitializeDataWithLoading());
    }
    
    private IEnumerator RunCoroutine(IEnumerator coroutine, Action onCompleted)
    {
        yield return StartCoroutine(coroutine);
        onCompleted?.Invoke();
    }

    private void StartLoading()
    {
        _isLoading = true;
        _defaultProgressPopup.ShowModalWindow();
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

    private IEnumerator GetAllPlayerData()
    {
        yield return StartCoroutine(PlayerDataManager.instance.GetPlayersData(
            ResponseHandlers.Create<PlayerData[]>(OnAllPlayersLoaded, ErrorOccured), SetAllPlayerURLData()));
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
    }

    private IEnumerator GetAllDropDownDate()
    {
        yield return StartCoroutine(MainDataManager.instance.GetDropDownDates(
            ResponseHandlers.Create<RecordDropdownDate>(OnDropdownDatesLoaded, ErrorOccured)));
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
    }

    private IEnumerator GetAllClanPlayerData()
    {
        yield return StartCoroutine(PlayerDataManager.instance.GetPlayersData(
            ResponseHandlers.Create<PlayerData[]>(OnClanPlayersLoaded, ErrorOccured), SetClanPlayerURLData()));
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
    }

    private IEnumerator GetAllMapData()
    {
        yield return StartCoroutine(MapDataManager.instance.GetAllMaps(
            ResponseHandlers.Create<MapData[]>(OnMapLoaded, ErrorOccured)));
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
    }
    
    private IEnumerator GetAllMapTypeData()
    {
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllMapTypes(
            ResponseHandlers.Create<MapTypeData[]>(OnMapTypeLoaded, ErrorOccured)));
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
    }

    private IEnumerator GetDailyMatchData()
    {
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MatchDataManager.instance.GetDailyMatches(
            ResponseHandlers.Create<List<RefinedMatchData>>(OnDailyMatchDataLoaded, ErrorOccured), _latestMatchDate));
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MainDataManager.instance.GetDailyGameData(
            ResponseHandlers.Create<DailyGameData>(OnDailyGameDataLoaded, ErrorOccured), _latestMatchDate));
    }

    private IEnumerator GetStatisticData(string category, string date)
    {
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MainDataManager.instance.GetLeaderBoard(
            ResponseHandlers.Create<WinRateData>(OnLeaderBoardLoaded, ErrorOccured), category, date));
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MainDataManager.instance.GetGameDatas(
            ResponseHandlers.Create<GameDatas>(OnGameDataLoaded, ErrorOccured), category, date));
    }

    private IEnumerator UpdateMainData()
    {
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MainDataManager.instance.UpdateMainDocument(
            ResponseHandlers.Create<bool>(_ => {}, ErrorOccured)));
    }

    private IEnumerator UpdatePlayerData()
    {
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(PlayerDataManager.instance.UpdatePlayerDocument(
            ResponseHandlers.Create<bool>(_ => {}, ErrorOccured)));
    }

    private IEnumerator UpdateMatchData()
    {
        //병렬 실행
        yield return StartCoroutine(UpdateMainData());
        yield return StartCoroutine(UpdatePlayerData());
        yield return StartCoroutine(GetAllDropDownDate());
        yield return StartCoroutine(GetDailyMatchData());
    }

    private IEnumerator InitializeData()
    {
        //구분 연, 월 캐싱
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(GetAllDropDownDate());
        
        //모든 플레이어 정보 캐싱
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(GetAllPlayerData());

        //Permission이 ClanMember이면 위 정보까지만 캐싱
        if (UserData.instance.GetUserPermission() != Permission.ClanMember)
        {
            //클랜 플레이어 정보 캐싱
            _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
            yield return StartCoroutine(GetAllClanPlayerData());
            //모든 맵 정보 캐싱
            _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
            yield return StartCoroutine(GetAllMapData());
            //모든 맵 타입 정보 캐싱
            _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
            yield return StartCoroutine(GetAllMapTypeData());
            //최근 경기 기록 캐싱
            _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
            yield return StartCoroutine(GetDailyMatchData());
        }
    }
    
    public IEnumerator InitializeDataWithLoading()
    {
        StartLoading();
        yield return StartCoroutine(InitializeData());
        FinishLoading();
    }
    private void OnLeaderBoardLoaded(WinRateData winRateData)
    {
        _winRateData = winRateData;
    }
    private void OnGameDataLoaded(GameDatas gameDatas)
    {
        _gameDatas = gameDatas;
    }
    
    private void OnDailyGameDataLoaded(DailyGameData dailyGameData)
    {
        _dailyGameData = dailyGameData;
    }
    private void OnDailyMatchDataLoaded(List<RefinedMatchData> matches)
    {
        _refinedMatches = matches;
    }
    
    public IEnumerator GetMainDataWithLoading(string category, string date)
    {
        if (_isLoading || (category != "year" && category != "month" && category != "day"))
        {
            _warningPopup.ShowModalWindow();
            yield break;
        } 
        StartCircularLoading();
        yield return StartCoroutine(GetStatisticData(category, date));
        FinishCircularLoading(); 
    }
        
    #region PlayerData

    private void OnClanPlayersLoaded(PlayerData[] players)
    {
        _clanPlayers = players;
    }
    private void OnAllPlayersLoaded(PlayerData[] players)
    {
        _allPlayers = players;
        _clanPlayers = players
            .Where(p => p.isClanMember.GetValueOrDefault())
            .ToArray();
    }
    
    private PlayerURLData SetClanPlayerURLData()
    {
        PlayerURLData playerURLData = new PlayerURLData();
        playerURLData.isClanMember = "true";
        playerURLData.fields = new List<string> { "player", "dates", "scores" ,"winRates", "subNames", "isVoiceAvailable"};
        playerURLData.sortType = "recent";
        return playerURLData;
    }
    
    private PlayerURLData SetAllPlayerURLData()
    {
        PlayerURLData playerURLData = new PlayerURLData();
        playerURLData.isClanMember = "false";
        playerURLData.fields = new List<string> { "player", "dates", "isClanMember", "subNames"};
        playerURLData.sortType = "recent";
        return playerURLData;
    }
    
    public IEnumerator CreatePlayerWithLoading(Action<bool> OnFinished, PlayerData player)
    {
        bool success = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnFinished?.Invoke(false);
            yield break;
        }
        StartCircularLoading();
        
        //플레이어 데이터 생성
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(PlayerDataManager.instance.CreatePlayer(
            ResponseHandlers.Create<string>(
                _ =>
                {
                    success = true;
                }, 
                error => 
                { 
                    ErrorOccured(error); 
                    success = false; 
                }), player));
        
        //클랜 플레이어 정보 다시 캐싱
        if (success)
            yield return StartCoroutine(GetAllClanPlayerData());
        
        FinishCircularLoading();
        OnFinished?.Invoke(success);
        OnCreatePlayerFininshed?.Invoke();
    }
    
    public IEnumerator GetAllClanPlayerDataWithLoading()
    {
        StartLoading();
        //클랜 플레이어 정보 캐싱
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(GetAllClanPlayerData());
        FinishLoading();
    }
    
    public IEnumerator GetAllPlayerDataWithLoading(PlayerURLData playerURLData)
    {
        StartCircularLoading();
        playerURLData.isClanMember = "false";
        yield return StartCoroutine(GetAllPlayerData());
        FinishCircularLoading();
    }

    public IEnumerator GetPlayerDataWithLoading(Action<bool> OnCompleted, Action<PlayerData> OnPlayerLoaded, string playerName)
    {
        bool success = false;
        StartCircularLoading();
        yield return StartCoroutine(PlayerDataManager.instance.GetPlayerData(
            ResponseHandlers.Create<PlayerData>(data=>
            {
                OnPlayerLoaded(data);
                success = true;
            }, error =>
            {
                ErrorOccured(error);
                success = false;
            }), playerName));
        FinishCircularLoading();
        OnCompleted?.Invoke(success);
    }
    #endregion
    
    #region MatchData
    public IEnumerator CreateMatchWithLoading(Action<bool> OnCompleted, RefinedMatchData match)
    {
        bool success = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnCompleted?.Invoke(false);
            yield break;
        }
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MatchDataManager.instance.CreateMatch(
            ResponseHandlers.Create<int>(
                _ =>
                {
                    success = true;
                },
                error =>
                {
                    success = false;
                    ErrorOccured(error);
                }), match));
        yield return StartCoroutine(UpdateMatchData());
        OnCompleted?.Invoke(success);
        FinishLoading();
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
        bool success = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnFinished?.Invoke(false);
            yield break;
        }
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MapDataManager.instance.CreateMap(ResponseHandlers.Create<string>(
            _ =>
            {
                success = true;
            },
            error =>
            {
                success = false;
                ErrorOccured(error);
            }), map));
        yield return StartCoroutine(GetAllMapData());
        OnFinished?.Invoke(success);
        FinishLoading();
    }
    public IEnumerator CreateMapType(Action<bool> OnFinished, MapTypeData maptype)
    {
        bool success = false;
        if (_isLoading)
        {
            _warningPopup.ShowModalWindow();
            OnFinished?.Invoke(false);
            yield break;
        }
        StartLoading();
        _defaultProgressPopup.DescriptionValue = _informMessage.GetRandomMessage();
        yield return StartCoroutine(MapTypeDataManager.instance.CreateMapType(ResponseHandlers.Create<string>(
            _ =>
            {
                success = true;
            },
            error =>
            {
                success = false;
                ErrorOccured(error);
            }), maptype));
        yield return StartCoroutine(GetAllMapTypeData());
        OnFinished?.Invoke(success);
        FinishLoading();
    }
    #endregion

    #region DropDownData
    public void OnDropdownDatesLoaded(RecordDropdownDate dropdownDates)
    {
        _recordDropdownDates = dropdownDates;
        _latestMatchDate = _recordDropdownDates.day[0].ToString();
    }
    #endregion
}
