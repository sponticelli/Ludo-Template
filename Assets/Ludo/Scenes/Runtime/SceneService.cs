using Ludo.Core.Structures;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludo.Scenes
{
    public class SceneService : ISceneService
    {
        public AwaitableAsyncOp Load(string name)
        {
           var op = SceneManager.LoadSceneAsync(name).AsAwaitable();
           op.GetAwaiter().OnCompleted(() =>
           {
               var scn = SceneManager.GetSceneByName(name);
               if (scn.IsValid())
               {
                   SceneManager.SetActiveScene(scn);
               }
               else
               {
                   Debug.LogError($"Scene {name} not found");
               }
           });
           return op; 
        }

        public AwaitableAsyncOp LoadAdditive(string name)
        {
            var op =  SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive).AsAwaitable();
            op.GetAwaiter().OnCompleted(() =>
            {
                var scn = SceneManager.GetSceneByName(name);
                if (scn.IsValid())
                {
                    SceneManager.SetActiveScene(scn);
                }
                else
                {
                    Debug.LogError($"Scene {name} not found");
                }
            });
            return op; 
        }

        public AwaitableAsyncOp Unload(string name) => SceneManager.UnloadSceneAsync(name).AsAwaitable();
    }
}