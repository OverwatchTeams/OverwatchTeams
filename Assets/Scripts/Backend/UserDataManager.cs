using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    private string url = "https://51g7o9m3xj.execute-api.ap-northeast-2.amazonaws.com/";
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "User/";
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
    
    public IEnumerator GetUserData(Action<Response<UserData>> OnCompleted, string email, string password)
    {
        url += "GetUserData?email=" + email + "&password=" + password;
        yield return DataUtility.GetData(OnCompleted, url);
    }
    
    public IEnumerator CreateUserData(Action<Response<bool>> OnCompleted, UserData data)
    {
        url += "CreateUser";
        yield return DataUtility.AddData(OnCompleted, url, data);
    }
}
