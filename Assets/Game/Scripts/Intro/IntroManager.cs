using Ludo.Core;
using Ludo.Scenes;
using UnityEngine;

namespace Game.Intro
{
    public class IntroManager : MonoBehaviour
    {
        
        private void Start()
        {
          var sceneService = ServiceLocator.Get<ISceneService>();
          sceneService.Load("MainMenu");   
        }
    }
}