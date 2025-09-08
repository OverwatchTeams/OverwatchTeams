using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResponseHandlers
{
    public static Action<Response<T>> Create<T>(Action<T> onSuccess, Action<string> onError)
    {
        var handler = new ResponseHandler<T>(onSuccess, onError);
        return handler.HandleResponse;
    }
}

