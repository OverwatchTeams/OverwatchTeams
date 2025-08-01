using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class UserDataManager : DataManager<UserData>
{
    public static UserDataManager instance;

    #region Override Methods
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "User/";
            base.Awake();
        }
        else
            Destroy(gameObject);
    }
    
    #region AddData
    public override IEnumerator AddData(Action<bool> OnCompleted, UserData data)
    {
        return base.AddDataCoroutine(OnCompleted, data, url + "CreateUser");
    }
    #endregion
    
    #region DeleteData
    
    protected override IEnumerator DeleteDataCoroutine(Action<bool> OnCompleted, int id, string requestUrl)
    {
        return base.DeleteDataCoroutine(OnCompleted, id, url +$"DeleteUser?id={id}");
    }
    #endregion
    #endregion
    
    #region GetPlayerData

    public IEnumerator GetUserData(Action<RequestResponse<UserData>> OnCompleted, string email, string password)
    {
        return GetUserDataCoroutine(OnCompleted, url + "GetUserData?email=" + email + "&password=" + password);
    }

    private IEnumerator GetUserDataCoroutine(Action<RequestResponse<UserData>> OnCompleted, string requestUrl)
    {
        RequestResponse<UserData> newResponse = new RequestResponse<UserData>();
        if (string.IsNullOrEmpty(requestUrl))
        {
            newResponse.responseCode = 404;
            newResponse.responseText = "Not Found : Bad Request";
            newResponse.responseData = null;
            OnCompleted?.Invoke(newResponse);
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                UserData result = JsonConvert.DeserializeObject<UserData>(json);
                newResponse.responseCode = www.responseCode;
                newResponse.responseText = www.error;
                newResponse.responseData = result;
                OnCompleted?.Invoke(newResponse);
            }
            else
            {
                newResponse.responseCode = www.responseCode;
                newResponse.responseText = www.error;
                newResponse.responseData = null;
                OnCompleted?.Invoke(newResponse);
            }
        }
    }
    #endregion
}
