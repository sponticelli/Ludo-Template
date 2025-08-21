using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ludo.Core.Structures
{
    public readonly struct AwaitableAsyncOp
    {
        readonly AsyncOperation _op;
        public AwaitableAsyncOp(AsyncOperation op) => _op = op;
        public Awaiter GetAwaiter() => new Awaiter(_op);

        public readonly struct Awaiter : INotifyCompletion
        {
            readonly AsyncOperation _op;
            public Awaiter(AsyncOperation op) => _op = op;
            public bool IsCompleted => _op.isDone;
            public void OnCompleted(Action continuation) => _op.completed += _ => continuation();
            public void GetResult() { }
        }
    }
}