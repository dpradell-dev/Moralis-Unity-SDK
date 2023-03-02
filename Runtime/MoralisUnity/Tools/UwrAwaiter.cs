using System;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MoralisUnity.Tools
{
    public struct UwrAwaiter : INotifyCompletion
    {
        private UnityWebRequestAsyncOperation asyncOp;
        private Action continuation;

        public UwrAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            this.asyncOp = asyncOp;
            continuation = null;
        }

        public bool IsCompleted { get { return asyncOp.isDone; } }

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
            asyncOp.completed += OnRequestCompleted;
        }

        private void OnRequestCompleted(AsyncOperation obj)
        {
            continuation?.Invoke();
        }
    }

    public static class ExtensionMethods
    {
        public static UwrAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UwrAwaiter(asyncOp);
        }
    }   
}