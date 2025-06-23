using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTypeDataManager : DataManager<MapTypeData>
{
    public static MapTypeDataManager instance;

    #region Override Methods
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "MapType/";
            base.Awake();
        }
        else
            Destroy(gameObject);
    }
    
    #region AddData
    protected override IEnumerator AddDataCoroutine(Action<bool> OnCompleted, MapTypeData data, string requestUrl)
    {
        Debug.Log($"name : {data.name}, index : {data.index}");
        return base.AddDataCoroutine(OnCompleted, data, url + "CreateMapType");
    }
    #endregion
    
    #region DeleteData
    
    protected override IEnumerator DeleteDataCoroutine(Action<bool> OnCompleted, int id, string requestUrl)
    {
        return base.DeleteDataCoroutine(OnCompleted, id, url +$"DeleteMapType?id={id}");
    }
    #endregion
    
    #region GetAllData

    protected override IEnumerator GetAllDataCoroutine(Action<bool> OnCompleted, Action<MapTypeData[]> OnCompletedDatas, string requestUrl)
    {
        return base.GetAllDataCoroutine(OnCompleted, OnCompletedDatas, url + "GetAllMapTypes");
    }
    #endregion
    
    #endregion
}
