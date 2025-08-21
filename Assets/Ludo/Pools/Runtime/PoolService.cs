using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ludo.Pools.Runtime
{
    public sealed class PoolService : IPoolService, IDisposable
    {
        // Queues keyed by PREFAB
        private readonly Dictionary<Component, Queue<Component>> _pools = new();
        // Reverse lookup: INSTANCE -> PREFAB
        private readonly Dictionary<Component, Component> _instanceToPrefab = new();
        
        private readonly Transform _poolRoot;
        
        public PoolService(bool hideInHierarchy = true)
        {
            _poolRoot = new GameObject("Pool Root").transform;
            if (hideInHierarchy) _poolRoot.gameObject.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(_poolRoot);
        }

        public T Get<T>(T prefab) where T : Component
        {
            if (prefab == null) return null;

            if (!_pools.TryGetValue(prefab, out var q))
                _pools[prefab] = q = new Queue<Component>();

            if (q.Count > 0)
            {
                var inst = (T)q.Dequeue();
                inst.gameObject.SetActive(true);
                return inst;
            }

            var created = Object.Instantiate(prefab, _poolRoot, true);
            _instanceToPrefab[created] = prefab;
            return created;
        }

        public void Release<T>(T instance) where T : Component
        {
            if (instance == null) return;

            if (!_instanceToPrefab.TryGetValue(instance, out var prefab))
            {
                // First time we see this instance: assume its "original" is itself to avoid losing it.
                // Better: require registration or attach a small PoolTag with the prefab ref.
                _instanceToPrefab[instance] = prefab = instance;
                if (!_pools.ContainsKey(prefab))
                    _pools[prefab] = new Queue<Component>();
            }

            instance.transform.SetParent(_poolRoot, false);
            instance.gameObject.SetActive(false);
            _pools[prefab].Enqueue(instance);
        }

        public void Warm<T>(T prefab, int count) where T : Component
        {
            for (int i = 0; i < count; i++)
            {
                var inst = Object.Instantiate(prefab);
                _instanceToPrefab[inst] = prefab;
                inst.gameObject.SetActive(false);
                if (!_pools.TryGetValue(prefab, out var q))
                    _pools[prefab] = q = new Queue<Component>();
                q.Enqueue(inst);
            }
        }

        public void Clear()
        {
            foreach (var q in _pools.Values)
                while (q.Count > 0) Object.Destroy(q.Dequeue().gameObject);
            _pools.Clear();
            _instanceToPrefab.Clear();
        }

        public void Dispose()
        {
            Clear();
            Object.Destroy(_poolRoot.gameObject);
        }
    }
}