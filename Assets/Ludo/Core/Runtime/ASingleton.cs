using UnityEngine;

namespace Ludo.Core
{
    public abstract class ASingleton<T> : CachedMonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                }
                return _instance;
            }
        }
    }
}