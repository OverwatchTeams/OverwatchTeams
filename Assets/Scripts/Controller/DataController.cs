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

    public MapData[] Maps => _maps;
    public MapTypeData[] MapTypes => _maptypes;
    public PlayerData[] Players => _players;
    [SerializeField] private LoadingController _loadingController;
    private bool _isLoading = true;

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
        _loadingController.ActivateLoadingPanel();
    }
    private void StartMiniLoading()
    {
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
    }

    private IEnumerator UpdateMainData()
    {
        bool isUpdated = false;
        if (!_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            yield break;
        }
        StartMiniLoading();
        _loadingController.SetMiniLoaingMessage("아재길드 히스토리를 분석 중입니다....");
        yield return StartCoroutine(MainDataManager.instance.UpdateMainDocument(success => isUpdated = success));
        _isLoading = true;
        OnDataUpdateEnd.Invoke();
    }
    
    private IEnumerator UpdatePlayerDatas()
    {
        bool isUpdated = false;
        if (!_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            yield break;
        }
        StartMiniLoading();
        _loadingController.SetMiniLoaingMessage("플레이어 통계를 분석 중입니다....");
        yield return StartCoroutine(PlayerDataManager.instance.UpdatePlayerDocuments(success => isUpdated = success));
        _loadingController.SetMiniLoaingMessage("플레이어 정보를 불러오는 중입니다....");
        yield return StartCoroutine(PlayerDataManager.instance.GetAllData(OnPlayerLoaded));
        _isLoading = true;
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
        if (!_isLoading)
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
        _isLoading = true;
        UpdateMainAndPlayerDatas();
    }

    public IEnumerator CreateMap(Action<bool> OnCreated, MapData map)
    {
        bool isCreated = false;
        if (!_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnCreated?.Invoke(isCreated);
            yield break;
        }
        StartLoading();
        _loadingController.SetLoadingMessage("맵을 추가하는 중입니다....");
        yield return StartCoroutine(MapDataManager.instance.AddData(success => isCreated = success, map));
        yield return StartCoroutine(MapDataManager.instance.GetAllData(OnMapLoaded));
        _isLoading = true;
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }
    public IEnumerator CreatePlayer(Action<bool> OnCreated, PlayerData player)
    {
        bool isCreated = false;
        if (!_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnCreated?.Invoke(isCreated);
            yield break;
        }
        StartLoading();
        _loadingController.SetLoadingMessage("플레이어를 추가하는 중입니다....");
        yield return StartCoroutine(PlayerDataManager.instance.AddData(success => isCreated = success, player));
        yield return StartCoroutine(PlayerDataManager.instance.GetAllData(OnPlayerLoaded));
        _isLoading = true;
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }
    
    public IEnumerator CreateMapType(Action<bool> OnCreated, MapTypeData maptype)
    {
        bool isCreated = false;
        if (!_isLoading)
        {
            _loadingController.ActivateWarningPopupPanel();
            OnCreated?.Invoke(isCreated);
            yield break;
        }
        StartLoading();
        _loadingController.SetLoadingMessage("맵타입을 추가하는 중입니다....");
        yield return StartCoroutine(MapTypeDataManager.instance.AddData(success => isCreated = success, maptype));
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllData(OnMapTypeLoaded));
        _isLoading = true;
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }

}
