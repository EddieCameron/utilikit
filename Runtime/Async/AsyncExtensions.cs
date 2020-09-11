using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class AsyncExtensions {
    public static TaskAwaiter GetAwaiter( this TimeSpan timeSpan ) {
        return Task.Delay( timeSpan ).GetAwaiter();
    }

    public static TaskAwaiter<AsyncOperation> GetAwaiter( this AsyncOperation asyncOperation ) {
        var tcs = new TaskCompletionSource<AsyncOperation>();
        if ( asyncOperation.isDone ) {
            tcs.TrySetResult( asyncOperation );
        }
        else {
            asyncOperation.completed += _ => {
                tcs.TrySetResult( asyncOperation );
            };
        }
        return tcs.Task.GetAwaiter();
    }
}
