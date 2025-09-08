using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    private string url = "https://51g7o9m3xj.execute-api.ap-northeast-2.amazonaws.com/";
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "Player/";
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
    
    public IEnumerator UpdatePlayerDocument(Action<Response<bool>> OnCompleted)
    {
        string requestUrl = url + "UpdatePlayerDocuments";
        yield return DataUtility.UpdateData(OnCompleted, requestUrl);
    }
    
    public IEnumerator GetPlayersData(Action<Response<PlayerData[]>> OnCompleted, PlayerURLData playerURLData)
    {
        string requestUrl = url + "GetAllPlayers";
        requestUrl += "?ClanMember=" + playerURLData.isClanMember;
        requestUrl += "&Fields=" + string.Join(",", playerURLData.fields);
        requestUrl += "&Sort=" + playerURLData.sortType;
        yield return DataUtility.GetData(OnCompleted, requestUrl);
    }
    
    public IEnumerator GetPlayerData(Action<Response<PlayerData>> OnCompleted, string playerName)
    {
        string requestUrl = url + "GetPlayerData?Name=" + playerName;
        yield return DataUtility.GetData(OnCompleted, requestUrl);
    }
    
    public IEnumerator CreatePlayer(Action<Response<string>> OnCompleted, PlayerData data)
    {
        string requestUrl = url + "CreatePlayer";
        yield return DataUtility.AddData(OnCompleted, requestUrl, data);
    }
    
    public IEnumerator DeleteMap(Action<Response<string>> OnCompleted, int id)
    {
        string requestUrl = url +$"DeletePlayer?id={id}";
        yield return DataUtility.DeleteData(OnCompleted, requestUrl);
    }
}
