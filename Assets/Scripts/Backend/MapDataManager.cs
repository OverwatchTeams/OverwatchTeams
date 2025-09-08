using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : MonoBehaviour
{
    public static MapDataManager instance;

    private string url = "https://51g7o9m3xj.execute-api.ap-northeast-2.amazonaws.com/";
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "Map/";
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
    

    public IEnumerator GetAllMaps(Action<Response<MapData[]>> OnCompleted)
    {
        string requestUrl = url + "GetAllMaps";
        yield return DataUtility.GetData(OnCompleted, requestUrl);
    }

    public IEnumerator CreateMap(Action<Response<string>> OnCompleted, MapData mapData)
    {
        string requestUrl = url + "CreateMap";
        yield return DataUtility.AddData(OnCompleted, requestUrl, mapData);
    }

    public IEnumerator DeleteMap(Action<Response<string>> OnCompleted, int id)
    {
        string requestUrl = url + $"DeleteMap?id={id}";
        yield return DataUtility.DeleteData(OnCompleted, requestUrl);
    }
}
