using System;
using UnityEngine;

namespace Ludo.Core
{
    public abstract class CachedMonoBehaviour : MonoBehaviour
    {
        [NonSerialized]
        private Transform _transform;

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