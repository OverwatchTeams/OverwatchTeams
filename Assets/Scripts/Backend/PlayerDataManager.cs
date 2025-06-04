using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
