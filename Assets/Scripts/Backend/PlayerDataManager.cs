using System;
using System.Collections;
using System.Collections.Generic;
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

    protected override IEnumerator GetAllDataCoroutine(Action<PlayerData[]> OnCompleted, string requestUrl)
    {
        return base.GetAllDataCoroutine(OnCompleted, url + "GetAllPlayersSortedByRecent");
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
