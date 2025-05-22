using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OverwatchMatchData;
using UGS;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public Action OnDataLoaded;
    public bool isLoadDone = false;

    private bool isDataDownloaded = false;
    private bool isMapDownloaded = false;
    private bool isMapTypeDownloaded = false;
    private bool isAttackDefenseTypeDownloaded = false;
    

    #region public

    //GoogleSheetData.Data
    public List<Data> DataList { get; set; }
    public Dictionary<int, Data> DataDictionary { get; set; }

    //GoogleSheetData.Map
    public List<Map> MapList { get; set; }
    public Dictionary<int, Map> MapDictionary { get; set; }

    //GoogleSheetData.MapType
    public List<MapType> MapTypeList { get; set; }
    public Dictionary<int, MapType> MapTypeDictionary { get; set; }

    //GoogleSheetData.AttackDefenseType
    public List<AttackDefenseType> AttackDefenseTypeList { get; set; }
    public Dictionary<int, AttackDefenseType> AttackDefenseTypeDictionary { get; set; }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            UnityGoogleSheet.LoadAllData();
            StartCoroutine(LoadAllData());
        }
        else
            Destroy(gameObject);
    }

    private IEnumerator LoadAllData()
    {
        yield return StartCoroutine(LoadDataFromDB());

        yield return StartCoroutine(LoadMapFromDB());
        
        yield return StartCoroutine(LoadMapTypeFromDB());

        yield return StartCoroutine(LoadAttackDefenseTypeFromDB());

        isLoadDone = true;
        Debug.Log("LoadFinished");
        OnDataLoaded?.Invoke();
    }
    
    #region LoadFromDB

    public IEnumerator LoadDataFromDB()
    {
        isDataDownloaded = false;
        DataList = OverwatchMatchData.Data.DataList;
        DataDictionary = OverwatchMatchData.Data.DataMap;
        isDataDownloaded = true;
        yield return new WaitUntil(() => isDataDownloaded);
    }

    public IEnumerator LoadMapFromDB()
    {
        isMapDownloaded = false;
        MapList = OverwatchMatchData.Map.MapList;
        MapDictionary = OverwatchMatchData.Map.MapMap;
        isMapDownloaded = true;
        
        yield return new WaitUntil(() => isMapDownloaded);
    }

    public IEnumerator LoadMapTypeFromDB()
    {
        isMapTypeDownloaded = false;
        MapTypeList = OverwatchMatchData.MapType.MapTypeList;
        MapTypeDictionary = OverwatchMatchData.MapType.MapTypeMap;
        isMapTypeDownloaded = true;
        yield return new WaitUntil(() => isMapTypeDownloaded);
    }

    public IEnumerator LoadAttackDefenseTypeFromDB()
    {
        isAttackDefenseTypeDownloaded = false;
        AttackDefenseTypeList = OverwatchMatchData.AttackDefenseType.AttackDefenseTypeList;
        AttackDefenseTypeDictionary = OverwatchMatchData.AttackDefenseType.AttackDefenseTypeMap;
        isAttackDefenseTypeDownloaded = true;
        yield return new WaitUntil(() => isAttackDefenseTypeDownloaded);
    }

    #endregion
}
