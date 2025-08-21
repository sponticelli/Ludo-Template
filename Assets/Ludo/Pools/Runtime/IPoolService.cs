using UnityEngine;

namespace Ludo.Pools.Runtime
{
    public interface IPoolService
    {
        T Get<T>(T prefab) where T : Component;
        void Release<T>(T instance) where T : Component;
        void Warm<T>(T prefab, int count) where T : Component; // optional
        void Clear(); // optional
    }
}