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
   
   #region GetGameDatas
   public IEnumerator GetGameDatas(Action<TotalGameData> OnCompleted)
   {
       yield return StartCoroutine(GetGameDatasCoroutine(OnCompleted));
   }
   
   public IEnumerator GetGameDatasByYear(Action<FilteredGameData> OnCompleted)
   {
       yield return StartCoroutine(GetGameDatasByYearCoroutine(OnCompleted));
   }
   
   public IEnumerator GetGameDatasByMonth(Action<FilteredGameData> OnCompleted)
   {
       yield return StartCoroutine(GetGameDatasByMonthCoroutine(OnCompleted));
   }

   private IEnumerator GetGameDatasCoroutine(Action<TotalGameData> OnCompleted)
   {
       string requestUrl = url + "GetGameDatas";
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetGameDatas 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               TotalGameData result = JsonConvert.DeserializeObject<TotalGameData>(json);
               Debug.Log($"데이터 조회 성공");
               OnCompleted?.Invoke(result);
           }
           else
           {
               Debug.LogError($"데이터 전체 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(null);
           }
       }
   }
   private IEnumerator GetGameDatasByYearCoroutine(Action<FilteredGameData> OnCompleted)
   {
       string requestUrl = url + "GetGameDatasByYear";
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetGameDatasByYear 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               FilteredGameData result = JsonConvert.DeserializeObject<FilteredGameData>(json);
               Debug.Log($"데이터 조회 성공");
               OnCompleted?.Invoke(result);
           }
           else
           {
               Debug.LogError($"데이터 전체 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(null);
           }
       }
   }
   
   private IEnumerator GetGameDatasByMonthCoroutine(Action<FilteredGameData> OnCompleted)
   {
       string requestUrl = url + "GetGameDatasByMonth";
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetGameDatasByMonth 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               FilteredGameData result = JsonConvert.DeserializeObject<FilteredGameData>(json);
               Debug.Log($"데이터 조회 성공");
               OnCompleted?.Invoke(result);
           }
           else
           {
               Debug.LogError($"데이터 전체 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(null);
           }
       }
   }
   #endregion
   
   #region GetGameDatas
   
   public IEnumerator GetLeaderBoardByYear(Action<FilteredLeaderBoardData> OnCompleted)
   {
       yield return StartCoroutine(GetLeaderBoardByYearCoroutine(OnCompleted));
   }
   
   public IEnumerator GetLeaderBoardByMonth(Action<FilteredLeaderBoardData> OnCompleted)
   {
       yield return StartCoroutine(GetLeaderBoardByMonthCoroutine(OnCompleted));
   }
   private IEnumerator GetLeaderBoardByYearCoroutine(Action<FilteredLeaderBoardData> OnCompleted)
   {
       string requestUrl = url + "GetLeaderBoardByYear";
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetLeaderBoardByYear 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               FilteredLeaderBoardData result = JsonConvert.DeserializeObject<FilteredLeaderBoardData>(json);
               Debug.Log($"데이터 조회 성공");
               OnCompleted?.Invoke(result);
           }
           else
           {
               Debug.LogError($"데이터 전체 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(null);
           }
       }
   }
   
   private IEnumerator GetLeaderBoardByMonthCoroutine(Action<FilteredLeaderBoardData> OnCompleted)
   {
       string requestUrl = url + "GetLeaderBoardByMonth";
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetLeaderBoardByMonth 요청 URL이 null이거나 비어있습니다.");
           OnCompleted?.Invoke(null);
           yield break;
       }

       using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
       {
           yield return www.SendWebRequest();

           if (www.result == UnityWebRequest.Result.Success)
           {
               string json = www.downloadHandler.text;
               FilteredLeaderBoardData result = JsonConvert.DeserializeObject<FilteredLeaderBoardData>(json);
               Debug.Log($"데이터 조회 성공");
               OnCompleted?.Invoke(result);
           }
           else
           {
               Debug.LogError($"데이터 전체 조회 실패: {www.responseCode} - {www.error}");
               OnCompleted?.Invoke(null);
           }
       }
   }
   #endregion
}
