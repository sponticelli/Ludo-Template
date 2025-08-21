using UnityEngine;

namespace Ludo.Core.Structures
{
    public static class AsyncOperationExtensions
    {
        public static AwaitableAsyncOp AsAwaitable(this AsyncOperation op) => new AwaitableAsyncOp(op);
    }
}