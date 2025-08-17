using System;
using UnityEngine;

namespace Ludo.Core
{
    /// <summary>
    ///     Provides a cached reference to the <see cref="UnityEngine.Transform" />
    ///     component to avoid repeated property lookups.
    /// </summary>
    public abstract class CachedMonoBehaviour : MonoBehaviour
    {
        [NonSerialized]
        private Transform _transform;

        /// <summary>
        ///     Cached access to the object's <see cref="Transform" /> component.
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = base.transform;
                }
                return _transform;
            }
        }
    }
}
