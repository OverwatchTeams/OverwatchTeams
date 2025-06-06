using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;
    public Action OnDataLoadEnd;
    public Action OnDataUpdateEnd;

    private MapData[] _maps;
    private MapTypeData[] _maptypes;
    private PlayerData[] _players;
    private RecordDropdownDate _recordDropdownDates;
    private WinRateData _winRateData;

    public MapData[] Maps => _maps;
    public MapTypeData[] MapTypes => _maptypes;
    public PlayerData[] Players => _players;
    public RecordDropdownDate RecordDropdownDates => _recordDropdownDates;
    public WinRateData WinRateData => _winRateData;
    [SerializeField] private LoadingController _loadingController;
    private bool _isLoading = false;

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
        StartCoroutine(UpdateData());
    }

    private void StartLoading()
    {
        _isLoading = true;
        _loadingController.ActivateLoadingPanel();
    }
    private void StartMiniLoading()
    {
        _isLoading = true;
        _loadingController.ActivateMiniLoadingPanel();
    }

    private IEnumerator UpdateData()
    {
        StartLoading();
        //맵 데이터 캐싱
        _loadingController.SetLoadingMessage("플레이어 정보를 불러오는 중입니다....");
        yield return StartCoroutine(PlayerDataManager.instance.GetAllData(OnPlayerLoaded));
        _loadingController.SetLoadingMessage("맵 정보를 불러오는 중입니다....");
        yield return StartCoroutine(MapDataManager.instance.GetAllData(OnMapLoaded));
        _loadingController.SetLoadingMessage("맵 타입 정보를 불러오는 중입니다....");
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllData(OnMapTypeLoaded));
        OnDataLoadEnd?.Invoke();
        _isLoading = false;
    }

    private IEnumerator UpdateMainData()
    {
        bool isUpdated = false;
        if (_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            yield break;
        }
        StartMiniLoading();
        _loadingController.SetMiniLoaingMessage("아재길드 히스토리를 분석 중입니다....");
        yield return StartCoroutine(MainDataManager.instance.UpdateMainDocument(success => isUpdated = success));
        _isLoading = false;
        OnDataUpdateEnd.Invoke();
    }
    
    private IEnumerator UpdatePlayerDatas()
    {
        bool isUpdated = false;
        if (_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            yield break;
        }
        StartMiniLoading();
        _loadingController.SetMiniLoaingMessage("플레이어 통계를 분석 중입니다....");
        yield return StartCoroutine(PlayerDataManager.instance.UpdatePlayerDocuments(success => isUpdated = success));
        _loadingController.SetMiniLoaingMessage("플레이어 정보를 불러오는 중입니다....");
        yield return StartCoroutine(PlayerDataManager.instance.GetAllData(OnPlayerLoaded));
        _isLoading = false;
        OnDataUpdateEnd.Invoke();
    }

    private void OnMapLoaded(MapData[] maps)
    {
        _maps = maps;
    }

    private void OnMapTypeLoaded(MapTypeData[] maptypes)
    {
        _maptypes = maptypes;
    }

    private void OnPlayerLoaded(PlayerData[] players)
    {
        _players = players;
    }

    private void OnLeaderBoardLoaded(WinRateData winRateData)
    {
        _winRateData = winRateData;
    }
    
    private void OnDropdownDatesLoaded(RecordDropdownDate dropdownDates)
    {
        _recordDropdownDates = dropdownDates;
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

    public IEnumerator CreateMatch(Action<bool> OnCreated, RefinedMatchData match)
    {
        bool isCreated = false;
        if (_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnCreated?.Invoke(isCreated);
            yield break;
        }
        StartLoading();
        _loadingController.SetLoadingMessage("매치를 저장중입니다....");
        yield return StartCoroutine(MatchDataManager.instance.AddData(success => isCreated = success, match));
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
        _isLoading = false;
        UpdateMainAndPlayerDatas();
    }

    public IEnumerator CreateMap(Action<bool> OnCreated, MapData map)
    {
        bool isCreated = false;
        if (_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnCreated?.Invoke(isCreated);
            yield break;
        }
        StartLoading();
        _loadingController.SetLoadingMessage("맵을 추가하는 중입니다....");
        yield return StartCoroutine(MapDataManager.instance.AddData(success => isCreated = success, map));
        yield return StartCoroutine(MapDataManager.instance.GetAllData(OnMapLoaded));
        _isLoading = false;
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }
    public IEnumerator CreatePlayer(Action<bool> OnCreated, PlayerData player)
    {
        bool isCreated = false;
        if (_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnCreated?.Invoke(isCreated);
            yield break;
        }
        StartLoading();
        _loadingController.SetLoadingMessage("플레이어를 추가하는 중입니다....");
        yield return StartCoroutine(PlayerDataManager.instance.AddData(success => isCreated = success, player));
        yield return StartCoroutine(PlayerDataManager.instance.GetAllData(OnPlayerLoaded));
        _isLoading = false;
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }
    
    public IEnumerator CreateMapType(Action<bool> OnCreated, MapTypeData maptype)
    {
        bool isCreated = false;
        if (_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnCreated?.Invoke(isCreated);
            yield break;
        }
        StartLoading();
        _loadingController.SetLoadingMessage("맵타입을 추가하는 중입니다....");
        yield return StartCoroutine(MapTypeDataManager.instance.AddData(success => isCreated = success, maptype));
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllData(OnMapTypeLoaded));
        _isLoading = false;
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }

    public IEnumerator GetDropdownDate(Action<bool> OnGet)
    {
        bool isGet = false;
        if (_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnGet?.Invoke(false);
            yield break;
        } 
        StartLoading();
        _loadingController.SetLoadingMessage("통계값을 불러오는 중입니다...");
        yield return StartCoroutine(MainDataManager.instance.GetDropDownDates(OnDropdownDatesLoaded));
        _isLoading = false;
        isGet = true;
        OnGet?.Invoke(isGet);
        OnDataLoadEnd?.Invoke();
    }
    public IEnumerator GetMainLeaderBoardData(string yearOrMonth, string date)
    {
        if (_isLoading || (yearOrMonth != "year" && yearOrMonth != "month"))
        {
            _loadingController.ActivateWarningPopupPanel();
            yield break;
        } 
        StartLoading();
        _loadingController.SetLoadingPanelOpaque(false);
        _loadingController.SetLoadingMessage("");
        yield return StartCoroutine(MainDataManager.instance.GetLeaderBoard(OnLeaderBoardLoaded, yearOrMonth, date));
        _isLoading = false;
        _loadingController.SetLoadingPanelOpaque(false);
        OnDataLoadEnd?.Invoke();
    }

}
