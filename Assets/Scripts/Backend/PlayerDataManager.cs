using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDataManager : DataManager<PlayerData>
{
    public static PlayerDataManager instance;

    #region Override Methods
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "Player/";
            base.Awake();
        }
        else
            Destroy(gameObject);
    }
    
    #region AddData
    protected override IEnumerator AddDataCoroutine(Action<bool> OnCompleted, PlayerData data, string requestUrl)
    {
        return base.AddDataCoroutine(OnCompleted, data, url + "CreatePlayer");
    }
    #endregion
    
    /*#region DeleteData
    
    protected override IEnumerator DeleteDataCoroutine(Action<bool> OnCompleted, int id, string requestUrl)
    {
        return base.DeleteDataCoroutine(OnCompleted, id, url +$"DeletePlayer?id={id}");
    }
    #endregion*/
    
    #region GetAllData

    public IEnumerator GetAllPlayers(Action<bool> OnCompleted, Action<PlayerData[]> OnCompletedDatas, PlayerURLData playerURLData)
    {
        string detailUrl = "GetAllPlayers";
        detailUrl += "?ClanMember=" + playerURLData.isClanMember;
        detailUrl += "&Fields=" + string.Join(",", playerURLData.fields);
        detailUrl += "&Sort=" + playerURLData.sortType;
        Debug.Log(detailUrl);
        return base.GetAllDataCoroutine(OnCompleted, OnCompletedDatas, url + detailUrl);
    }
    
    #endregion
    #endregion
    
    #region UpdatePlayerDocument
    public IEnumerator UpdatePlayerDocuments(Action<bool> OnCompleted)
    {
        yield return StartCoroutine(UpdatePlayerDocumentsCoroutine(OnCompleted));
    }
   
    protected virtual IEnumerator UpdatePlayerDocumentsCoroutine(Action<bool> OnCompleted)
    {
        string requestUrl = url + "UpdatePlayerDocuments";
        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
        {
            // DownloadHandler 추가
            request.downloadHandler = new DownloadHandlerBuffer();
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("PlayerDocuments 업데이트 성공: " + request.downloadHandler.text);
                OnCompleted?.Invoke(true);
            }
            else
            {
                Debug.LogError("PlayerDocuments 업데이트 실패: " + request.error);
                OnCompleted?.Invoke(false);
            }
        }
    }
    #endregion
}
