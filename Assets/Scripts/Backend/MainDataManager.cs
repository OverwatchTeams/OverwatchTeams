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
   
   #region UpdateMainDocument
   public IEnumerator UpdateMainDocument(Action<bool> OnCompleted)
   {
       yield return StartCoroutine(UpdateMainDocumentCoroutine(OnCompleted));
   }
   
   private IEnumerator UpdateMainDocumentCoroutine(Action<bool> OnCompleted)
   {
       string requestUrl = url + "UpdateMainDocument";
       using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
       {
           // DownloadHandler 추가
           request.downloadHandler = new DownloadHandlerBuffer();

           yield return request.SendWebRequest();

           if (request.result == UnityWebRequest.Result.Success)
           {
               Debug.Log("MainDocument 업데이트 성공: " + request.downloadHandler.text);
               OnCompleted?.Invoke(true);
           }
           else
           {
               Debug.LogError("MainDocument 업데이트 실패: " + request.error);
               OnCompleted?.Invoke(false);
           }
       }
   }
   #endregion
   
   public IEnumerator GetDropDownDates(Action<bool> OnCompleted, Action<RecordDropdownDate> OnCompletedDatas)
   {
       yield return StartCoroutine(GetDropDownDatesCoroutine(OnCompleted, OnCompletedDatas));
   }
   
   private IEnumerator GetDropDownDatesCoroutine(Action<bool> OnCompleted, Action<RecordDropdownDate> OnCompletedDatas)
   {
       string requestUrl = url + "GetDropDownDates";
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetDropDownDates 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(false);
           OnCompletedDatas?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               RecordDropdownDate result = JsonConvert.DeserializeObject<RecordDropdownDate>(json);
               Debug.Log($"데이터 조회 성공");
               OnCompleted?.Invoke(true);
               OnCompletedDatas?.Invoke(result);
           }
           else
           {
               Debug.LogError($"데이터 전체 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(false);
               OnCompletedDatas?.Invoke(null);
           }
       }
   }
   #region GetGameDatas
   
   public IEnumerator GetGameDatas(Action<bool> OnCompleted, Action<GameDatas> OnCompletedDatas, string yearOrMonth, string date)
   {
       yield return StartCoroutine(GetGameDatasCoroutine(OnCompleted, OnCompletedDatas, yearOrMonth, date));
   }

   private IEnumerator GetGameDatasCoroutine(Action<bool> OnCompleted, Action<GameDatas> OnCompletedDatas, string yearOrMonth, string date)
   {
       string requestUrl = null;
       switch (yearOrMonth)
       {
           case "year":
               requestUrl = url + "GetMainGameDatasByYear?year=" + date;
               break;
           case "month":
               requestUrl = url + "GetMainGameDatasByMonth?month=" + date;
               break;
       }
       
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetMainGameDatas 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(false);
           OnCompletedDatas?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               GameDatas result = JsonConvert.DeserializeObject<GameDatas>(json);
               Debug.Log($"{date} 게임 데이터 조회 성공");
               OnCompleted?.Invoke(true);
               OnCompletedDatas?.Invoke(result);
           }
           else
           {
               Debug.LogError($"{date} 게임 데이터 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(false);
               OnCompletedDatas?.Invoke(null);
           }
       }
   }
   #endregion
   
   #region GetGameDatas
   
   public IEnumerator GetLeaderBoard(Action<bool> OnCompleted,Action<WinRateData> OnCompletedDatas, string yearOrMonth, string date)
   {
       yield return StartCoroutine(GetLeaderBoardCoroutine(OnCompleted, OnCompletedDatas, yearOrMonth, date));
   }

   private IEnumerator GetLeaderBoardCoroutine(Action<bool> OnCompleted,Action<WinRateData> OnCompletedDatas, string yearOrMonth, string date)
   {
       string requestUrl = null;
       switch (yearOrMonth)
       {
            case "year":
                requestUrl = url + "GetMainLeaderBoardByYear?year=" + date;
                break;
            case "month":
                requestUrl = url + "GetMainLeaderBoardByMonth?month=" + date;
                break;
       }
       
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetLeaderBoard 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(false);
           OnCompletedDatas?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               WinRateData result = JsonConvert.DeserializeObject<WinRateData>(json);
               Debug.Log($"{date} 랭크 데이터 조회 성공");
               OnCompleted?.Invoke(true);
               OnCompletedDatas?.Invoke(result);
           }
           else
           {
               Debug.LogError($"{date} 랭크 데이터 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(false);
               OnCompletedDatas?.Invoke(null);
           }
       }
   }
   #endregion
}
