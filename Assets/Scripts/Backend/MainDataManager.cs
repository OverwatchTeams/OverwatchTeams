using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class MainDataManager : MonoBehaviour
{ 
   public static MainDataManager instance;
   
   private string url = "https://51g7o9m3xj.execute-api.ap-northeast-2.amazonaws.com/";
   
   private void Awake()
   {
       if (instance == null)
       {
           instance = this;
           url += "Main/";
           DontDestroyOnLoad(this);
       }
       else
           Destroy(gameObject);
   }

   public IEnumerator UpdateMainDocument(Action<Response<bool>> OnCompleted)
   {
       string requestUrl = url + "UpdateMainDocument";
       yield return DataUtility.UpdateData(OnCompleted, requestUrl);
   }
   
   public IEnumerator GetDropDownDates(Action<Response<RecordDropdownDate>> OnCompleted)
   {
       string requestUrl = url + "GetDropDownDates";
       yield return DataUtility.GetData(OnCompleted, requestUrl);
   }
   
   public IEnumerator GetGameDatas(Action<Response<GameDatas>> OnCompleted, string category, string date)
   {
       string requestUrl = url + "GetMainGameDatas?category=" + category + "&date=" + date;
       yield return DataUtility.GetData(OnCompleted, requestUrl);
   }
   
   public IEnumerator GetDailyGameData(Action<Response<DailyGameData>> OnCompleted, string date)
   {
       string requestUrl = url + "GetMainDailyData?date=" + date;
       yield return DataUtility.GetData(OnCompleted, requestUrl);
   }
   
   public IEnumerator GetLeaderBoard(Action<Response<WinRateData>> OnCompleted, string category, string date)
   {
       string requestUrl = url + "GetMainLeaderBoard?category=" + category + "&date=" + date;
       yield return DataUtility.GetData(OnCompleted, requestUrl);
   }
}
