using System.Collections.Generic;

namespace Ludo.SceneManagement
{
    public interface ISceneTransitionPlan
    {
        HashSet<string> ToLoad { get; }
        HashSet<string> ToUnload { get; }
        string ActiveScene { get; }
        bool UseOverlay { get; }
        bool CleanUnusedAssets { get; }
    }
}