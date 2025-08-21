using Ludo.Core.Structures;
using UnityEngine;

namespace Ludo.Scenes
{
    public interface ISceneService
    {
        AwaitableAsyncOp Load(string name);
        AwaitableAsyncOp LoadAdditive(string name);
        AwaitableAsyncOp Unload(string name);
    }
}