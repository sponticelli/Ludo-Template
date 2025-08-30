using UnityEngine;

namespace Ludo.Pools
{
    public interface IPoolService
    {
        // GameObject
        GameObject Spawn(GameObject prefab, Vector3 pos = default, Quaternion rot = default, Transform parent = null);
        void Despawn(GameObject instance, float delaySeconds = 0f);
        void Warmup(GameObject prefab, int count);

        // Component
        T Spawn<T>(T prefab, Vector3 pos = default, Quaternion rot = default, Transform parent = null)
            where T : Component;

        void Despawn(Component instance, float delaySeconds = 0f);
        void Warmup<T>(T prefab, int count) where T : Component;

        void ReleaseAllPools(); // optional: clears caches (use at shutdown)
    }
}