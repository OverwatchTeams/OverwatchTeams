using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseHandler<T>
{
    private Dictionary<long, Action<Response<T>>> responseActions = new Dictionary<long, Action<Response<T>>>();
    
    private Action<T> _onSuccess;
    private Action<string> _onError;

    public ResponseHandler(Action<T> onSuccess, Action<string> onError)
    {
        _onSuccess = onSuccess;
        _onError = onError;
    }
    
    public void HandleResponse(Response<T> response)
    {
        if (response.isSuccess == true)
        {
            _onSuccess?.Invoke(response.DATA);
        }
        else
        {
            _onError.Invoke($"[{response.statusCode}-{response.code}] {response.message}]");
        }
    }

}

[Serializable]
public class Response<T>
{
    public bool isSuccess;
    public int statusCode;
    public string message;
    public string code;
    public string timestamp;
    public T DATA;
}
