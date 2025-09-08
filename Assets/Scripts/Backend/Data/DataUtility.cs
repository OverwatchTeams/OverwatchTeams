using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class DataUtility
{
    public static IEnumerator GetData<T>(Action<Response<T>> OnCompleted, string requestUrl)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
        {
            yield return www.SendWebRequest();
            string output = www.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<Response<T>>(output);
            Debug.Log($"Response: {response.code} - {response.message}");
            OnCompleted?.Invoke(response);
        }
    }

    public static IEnumerator AddData<T, R>(Action<Response<R>> OnCompleted, string requestUrl, T data)
    {
        string input = JsonConvert.SerializeObject(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(input);

        using (UnityWebRequest www = new UnityWebRequest(requestUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            string output = www.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<Response<R>>(output);
            Debug.Log($"Response: {response.code} - {response.message}");
            OnCompleted?.Invoke(response);
        }
    }

    public static IEnumerator DeleteData(Action<Response<string>> OnCompleted, string requestUrl)
    {

        using (UnityWebRequest www = UnityWebRequest.Delete(requestUrl))
        {
            yield return www.SendWebRequest();
            string output = www.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<Response<string>>(output);
            Debug.Log($"Response: {response.code} - {response.message}");
            OnCompleted?.Invoke(response);
        }
    }
    
    public static IEnumerator UpdateData(Action<Response<bool>> OnCompleted, string requestUrl)
    {
        using (UnityWebRequest www = new UnityWebRequest(requestUrl, "POST"))
        {
            // DownloadHandler 추가
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();
            string output = www.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<Response<bool>>(output);
            Debug.Log($"Response: {response.code} - {response.message}");
            OnCompleted?.Invoke(response);
        }
    }
}