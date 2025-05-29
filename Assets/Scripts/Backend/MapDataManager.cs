using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : DataManager<MapData>
{
    public static MapDataManager instance;

    #region Override Methods
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "Map/";
            base.Awake();
        }
        else
            Destroy(gameObject);
    }
    
    #region AddData
    protected override IEnumerator AddDataCoroutine(Action<bool> OnCompleted, MapData data, string requestUrl)
    {
        return base.AddDataCoroutine(OnCompleted, data, url + "CreateMap");
    }
    #endregion
    
    #region DeleteData
    
    protected override IEnumerator DeleteDataCoroutine(Action<bool> OnCompleted, int id, string requestUrl)
    {
        return base.DeleteDataCoroutine(OnCompleted, id, url +$"DeleteMap?id={id}");
    }
    #endregion
    
    #region GetAllData

    protected override IEnumerator GetAllDataCoroutine(Action<MapData[]> OnCompleted, string requestUrl)
    {
        return base.GetAllDataCoroutine(OnCompleted, url + "GetAllMaps");
    }
    #endregion
    
    #endregion
}
