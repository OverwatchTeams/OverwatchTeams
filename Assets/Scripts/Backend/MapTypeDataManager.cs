using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTypeDataManager : MonoBehaviour
{
    public static MapTypeDataManager instance;
    
    private string url = "https://51g7o9m3xj.execute-api.ap-northeast-2.amazonaws.com/";
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "MapType/";
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
    public IEnumerator GetAllMapTypes(Action<Response<MapTypeData[]>> OnCompleted)
    {
        string requestUrl = url + "GetAllMapTypes";
        yield return DataUtility.GetData(OnCompleted, requestUrl);
    }
    
    public IEnumerator CreateMapType(Action<Response<string>> OnCompleted, MapTypeData mapTypeData)
    {
        string requestUrl = url + "CreateMapType";
        yield return DataUtility.AddData(OnCompleted, requestUrl, mapTypeData);
    }
    
    public IEnumerator DeleteMap(Action<Response<string>> OnCompleted, int id)
    {
        string requestUrl = url + $"DeleteMapType?id={id}";
        yield return DataUtility.DeleteData(OnCompleted, requestUrl);
    }
}
