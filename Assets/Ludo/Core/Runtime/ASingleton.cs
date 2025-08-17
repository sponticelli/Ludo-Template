using UnityEngine;

namespace Ludo.Core
{
    /// <summary>
    ///     Generic base class for creating simple MonoBehaviour singletons. The
    ///     first instance of <typeparamref name="T" /> found in the scene is used as
    ///     the global instance.
    /// </summary>
    public abstract class ASingleton<T> : CachedMonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        /// <summary>
        ///     Gets the singleton instance, searching the scene on first access.
        /// </summary>
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
