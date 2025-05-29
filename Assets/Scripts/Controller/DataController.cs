using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;
    public Action OnDataLoadEnd;

    private MapData[] _maps;
    private MapTypeData[] _maptypes;

    public MapData[] Maps => _maps;
    public MapTypeData[] MapTypes => _maptypes;
    [SerializeField] private LoadingController _loadingController;

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
        _loadingController.gameObject.SetActive(true);
    }

    private IEnumerator UpdateData()
    {
        StartLoading();
        //맵 데이터 캐싱
        yield return StartCoroutine(MapDataManager.instance.GetAllData(OnMapLoaded));
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllData(OnMapTypeLoaded));
        OnDataLoadEnd?.Invoke();
    }

    private void OnMapLoaded(MapData[] maps)
    {
        _maps = maps;
    }

    private void OnMapTypeLoaded(MapTypeData[] maptypes)
    {
        _maptypes = maptypes;
    }

    public IEnumerator CreateMatch(Action<bool> OnCreated, RefinedMatchData match)
    {
        StartLoading();
        bool isCreated = false;
        yield return StartCoroutine(MatchDataManager.instance.AddData(success => isCreated = success, match));
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }

    public IEnumerator CreateMap(Action<bool> OnCreated, MapData map)
    {
        StartLoading();
        bool isCreated = false;
        yield return StartCoroutine(MapDataManager.instance.AddData(success => isCreated = success, map));
        yield return StartCoroutine(MapDataManager.instance.GetAllData(OnMapLoaded));
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }
    public IEnumerator CreateMapType(Action<bool> OnCreated, MapTypeData maptype)
    {
        StartLoading();
        bool isCreated = false;
        yield return StartCoroutine(MapTypeDataManager.instance.AddData(success => isCreated = success, maptype));
        yield return StartCoroutine(MapTypeDataManager.instance.GetAllData(OnMapTypeLoaded));
        OnCreated?.Invoke(isCreated);
        OnDataLoadEnd?.Invoke();
    }

}
