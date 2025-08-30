using Ludo.Core;
using UnityEngine;

namespace Ludo.Pools
{
    /// <summary>
    /// Base class for pooled MonoBehaviours
    /// </summary>
    public abstract class PooledBehaviour : MonoBehaviour, IPoolable
    {
        public virtual void OnBeforeRent()
        {
        }

        public virtual void OnBeforeReturn()
        {
        }

        public virtual void ResetState()
        {
        }

        // Call this to return yourself to the pool (if you don’t have a reference handy)
        protected void ReturnSelf()
        {
            var poolService = ServiceLocator.Get<IPoolService>();
            poolService?.Despawn(this);
        } 
    }
}