using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager<T> : MonoBehaviour
{
    //url은 각 Data별로 변경하여야 함
   protected string url = "https://51g7o9m3xj.execute-api.ap-northeast-2.amazonaws.com/";
   
   protected virtual void Awake()
   {
       DontDestroyOnLoad(this);
   }

   #region AddData
   public virtual IEnumerator AddData(Action<bool> OnCompleted, T data)
   {
       yield return StartCoroutine(AddDataCoroutine(OnCompleted, data, null));
   }

   protected virtual IEnumerator AddDataCoroutine(Action<bool> OnCompleted, T data, string requestUrl)
   {
       string json = JsonConvert.SerializeObject(data);
       byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
       using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
       {
           request.uploadHandler = new UploadHandlerRaw(bodyRaw);
           request.downloadHandler = new DownloadHandlerBuffer();
           request.SetRequestHeader("Content-Type", "application/json");

           yield return request.SendWebRequest();

           if (request.result == UnityWebRequest.Result.Success)
           {
               Debug.Log("데이터 추가 성공: " + request.downloadHandler.text);
               OnCompleted?.Invoke(true);
           }
           else
           {
               Debug.LogError("데이터 추가 실패: " + request.error);
               OnCompleted?.Invoke(false);
           }
       }
   }
   #endregion
   
   #region DeleteData
   public virtual IEnumerator DeleteData(Action<bool> OnCompleted, int identifier)
   {
       yield return StartCoroutine(DeleteDataCoroutine(OnCompleted, identifier, null));
   }

   protected virtual IEnumerator DeleteDataCoroutine(Action<bool> OnCompleted,  int identifier,string requestUrl)
   {
       using (UnityWebRequest request = UnityWebRequest.Delete(requestUrl))
       {
           yield return request.SendWebRequest();

           if (request.result == UnityWebRequest.Result.Success)
           {
               Debug.Log("데이터 삭제 성공");
               OnCompleted?.Invoke(true);
           }
           else
           {
               Debug.LogError($"데이터 삭제 실패: {request.responseCode} - {request.error}");
               OnCompleted?.Invoke(false);
           }
       }
   }
   #endregion
   
   #region GetAllData
   public virtual IEnumerator GetAllData(Action<bool> OnCompleted, Action<T[]> OnCompletedDatas)
   {
       yield return StartCoroutine(GetAllDataCoroutine(OnCompleted, OnCompletedDatas, null));
   }

   protected virtual IEnumerator GetAllDataCoroutine(Action<bool> OnCompleted, Action<T[]> OnCompletedDatas, string requestUrl)
   {
       if (string.IsNullOrEmpty(requestUrl))
       {
           Debug.LogError("GetAllData 요청 URL이 null이거나 비어있습니다.");
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
               T[] result = JsonConvert.DeserializeObject<T[]>(json);
               Debug.Log($"데이터 전체 조회 성공: {result.Length}개");
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
   #endregion
}
