using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Ludo.Pools
{
    public sealed class PoolService : MonoBehaviour, IPoolService
    {

        // ---- Internal wrappers so we can store generic pools in non-generic maps ----
        private interface IPoolWrapper
        {
            void Warmup(int count);
            UnityEngine.Object Rent(Vector3 pos, Quaternion rot, Transform parent);
            void Return(UnityEngine.Object obj);
            void Clear();
            bool Owns(UnityEngine.Object obj);
        }

        private sealed class GameObjectPoolWrapper : IPoolWrapper
        {
            private readonly GameObject _prefab;
            private readonly Transform _root; // where pooled instances live when inactive
            private readonly IObjectPool<GameObject> _pool;
            private readonly HashSet<int> _owned = new();

            public GameObjectPoolWrapper(GameObject prefab, Transform poolsRoot, int defaultCapacity = 10,
                int maxSize = 200)
            {
                _prefab = prefab;
                _root = new GameObject($"[Pool] {prefab.name}").transform;
                _root.SetParent(poolsRoot, false);

                _pool = new ObjectPool<GameObject>(
                    createFunc: () =>
                    {
                        var go = UnityEngine.Object.Instantiate(_prefab, _root);
                        _owned.Add(go.GetInstanceID());
                        go.SetActive(false);
                        return go;
                    },
                    actionOnGet: go =>
                    {
                        if (go == null) return;
                        // Reparent is done by caller after Rent to keep control
                        go.SetActive(true);
                        foreach (var p in go.GetComponentsInChildren<IPoolable>(true))
                        {
                            p.OnBeforeRent();
                        }
                    },
                    actionOnRelease: go =>
                    {
                        if (go == null) return;
                        foreach (var p in go.GetComponentsInChildren<IPoolable>(true))
                        {
                            p.OnBeforeReturn();
                            p.ResetState();
                        }

                        go.SetActive(false);
                        go.transform.SetParent(_root, false);
                    },
                    actionOnDestroy: go =>
                    {
                        if (go != null) _owned.Remove(go.GetInstanceID());
                        if (go != null) UnityEngine.Object.Destroy(go);
                    },
                    collectionCheck: false,
                    defaultCapacity: defaultCapacity,
                    maxSize: maxSize
                );
            }

            public void Warmup(int count)
            {
                var temp = new List<GameObject>(count);
                for (int i = 0; i < count; i++) temp.Add(_pool.Get());
                foreach (var t in temp) _pool.Release(t);
            }

            public UnityEngine.Object Rent(Vector3 pos, Quaternion rot, Transform parent)
            {
                var go = _pool.Get();
                go.transform.SetPositionAndRotation(pos, rot);
                if (parent != null) go.transform.SetParent(parent, true);
                return go;
            }

            public void Return(UnityEngine.Object obj)
            {
                var go = obj as GameObject;
                if (go == null) return;
                // Don’t release if already destroyed or not ours
                if (!_owned.Contains(go.GetInstanceID()))
                {
                    UnityEngine.Object.Destroy(go);
                    return;
                }

                _pool.Release(go);
            }

            public void Clear() => _pool.Clear();

            public bool Owns(UnityEngine.Object obj) =>
                obj is GameObject go && _owned.Contains(go.GetInstanceID());
        }

        private sealed class ComponentPoolWrapper<T> : IPoolWrapper where T : Component
        {
            private readonly T _prefab;
            private readonly Transform _root;
            private readonly IObjectPool<T> _pool;
            private readonly HashSet<int> _owned = new();

            public ComponentPoolWrapper(T prefab, Transform poolsRoot, int defaultCapacity = 10, int maxSize = 200)
            {
                _prefab = prefab;
                _root = new GameObject($"[Pool] {prefab.GetType().Name}:{prefab.name}").transform;
                _root.SetParent(poolsRoot, false);

                _pool = new ObjectPool<T>(
                    createFunc: () =>
                    {
                        var inst = UnityEngine.Object.Instantiate(_prefab, _root);
                        _owned.Add(inst.gameObject.GetInstanceID());
                        inst.gameObject.SetActive(false);
                        return inst;
                    },
                    actionOnGet: c =>
                    {
                        if (c == null) return;
                        c.gameObject.SetActive(true);
                        foreach (var p in c.GetComponentsInChildren<IPoolable>(true))
                        {
                            p.OnBeforeRent();
                        }
                    },
                    actionOnRelease: c =>
                    {
                        if (c == null) return;
                        foreach (var p in c.GetComponentsInChildren<IPoolable>(true))
                        {
                            p.OnBeforeReturn();
                            p.ResetState();
                        }

                        c.gameObject.SetActive(false);
                        c.transform.SetParent(_root, false);
                    },
                    actionOnDestroy: c =>
                    {
                        if (c != null) _owned.Remove(c.gameObject.GetInstanceID());
                        if (c != null) UnityEngine.Object.Destroy(c.gameObject);
                    },
                    collectionCheck: false,
                    defaultCapacity: defaultCapacity,
                    maxSize: maxSize
                );
            }

            public void Warmup(int count)
            {
                var temp = new List<T>(count);
                for (int i = 0; i < count; i++) temp.Add(_pool.Get());
                foreach (var t in temp) _pool.Release(t);
            }

            public UnityEngine.Object Rent(Vector3 pos, Quaternion rot, Transform parent)
            {
                var c = _pool.Get();
                var tr = c.transform;
                tr.SetPositionAndRotation(pos, rot);
                if (parent != null) tr.SetParent(parent, true);
                return c;
            }

            public void Return(UnityEngine.Object obj)
            {
                if (obj is T c)
                {
                    if (!_owned.Contains(c.gameObject.GetInstanceID()))
                    {
                        UnityEngine.Object.Destroy(c.gameObject);
                        return;
                    }

                    _pool.Release(c);
                }
                else if (obj is GameObject go)
                {
                    var comp = go.GetComponent<T>();
                    if (comp == null || !_owned.Contains(go.GetInstanceID()))
                    {
                        UnityEngine.Object.Destroy(go);
                        return;
                    }

                    _pool.Release(comp);
                }
            }

            public void Clear() => _pool.Clear();

            public bool Owns(UnityEngine.Object obj)
            {
                if (obj is T c) return _owned.Contains(c.gameObject.GetInstanceID());
                if (obj is GameObject go) return _owned.Contains(go.GetInstanceID());
                return false;
            }
        }

        // ---- Storage ----
        [SerializeField] private int defaultCapacity = 16;
        [SerializeField] private int maxSizePerPool = 512;

        private Transform _poolsRoot;
        private readonly Dictionary<int, IPoolWrapper> _goPools = new(); // key: prefab.GetInstanceID()
        private readonly Dictionary<int, IPoolWrapper> _compPools = new(); // key: prefab.GetInstanceID()

        private Transform PoolsRoot
        {
            get
            {
                if (_poolsRoot == null)
                {
                    var g = new GameObject("[PoolService]");
                    g.transform.SetParent(transform, false);
                    _poolsRoot = g.transform;
                }

                return _poolsRoot;
            }
        }

        // ---- Public API: GameObject ----
        public GameObject Spawn(GameObject prefab, Vector3 pos = default, Quaternion rot = default,
            Transform parent = null)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            var key = prefab.GetInstanceID();
            if (!_goPools.TryGetValue(key, out var wrapper))
            {
                wrapper = new GameObjectPoolWrapper(prefab, PoolsRoot, defaultCapacity, maxSizePerPool);
                _goPools.Add(key, wrapper);
            }

            return (GameObject)wrapper.Rent(pos, rot == default ? Quaternion.identity : rot, parent);
        }

        public void Despawn(GameObject instance, float delaySeconds = 0f)
        {
            if (instance == null) return;
            var wrapper = FindOwnerWrapper(instance);
            if (wrapper == null)
            {
                Destroy(instance);
                return;
            }

            if (delaySeconds > 0f)
            {
                StartCoroutine(DespawnAfterDelay(wrapper, instance, delaySeconds));
            }
            else wrapper.Return(instance);
        }

        public void Warmup(GameObject prefab, int count) =>
            GetOrCreateGoWrapper(prefab).Warmup(count);

        // ---- Public API: Component ----
        public T Spawn<T>(T prefab, Vector3 pos = default, Quaternion rot = default, Transform parent = null)
            where T : Component
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            var wrapper = GetOrCreateCompWrapper(prefab);
            var obj = wrapper.Rent(pos, rot == default ? Quaternion.identity : rot, parent);
            return obj is T c ? c : ((GameObject)obj).GetComponent<T>();
        }

        public void Despawn(Component instance, float delaySeconds = 0f)
        {
            if (instance == null) return;
            var wrapper = FindOwnerWrapper(instance);
            if (wrapper == null)
            {
                Destroy(instance.gameObject);
                return;
            }

            if (delaySeconds > 0f)
            {
                StartCoroutine(DespawnAfterDelay(wrapper, instance, delaySeconds));
            }
            else wrapper.Return(instance);
        }

        public void Warmup<T>(T prefab, int count) where T : Component =>
            GetOrCreateCompWrapper(prefab).Warmup(count);

        public void ReleaseAllPools()
        {
            foreach (var p in _goPools.Values) p.Clear();
            foreach (var p in _compPools.Values) p.Clear();
            _goPools.Clear();
            _compPools.Clear();
            if (_poolsRoot != null) Destroy(_poolsRoot.gameObject);
            _poolsRoot = null;
        }

        // ---- Helpers ----
        private IPoolWrapper GetOrCreateGoWrapper(GameObject prefab)
        {
            var key = prefab.GetInstanceID();
            if (_goPools.TryGetValue(key, out var w)) return w;
            w = new GameObjectPoolWrapper(prefab, PoolsRoot, defaultCapacity, maxSizePerPool);
            _goPools.Add(key, w);
            return w;
        }

        private IPoolWrapper GetOrCreateCompWrapper<T>(T prefab) where T : Component
        {
            var key = prefab.GetInstanceID();
            if (_compPools.TryGetValue(key, out var w)) return w;
            w = new ComponentPoolWrapper<T>(prefab, PoolsRoot, defaultCapacity, maxSizePerPool);
            _compPools.Add(key, w);
            return w;
        }

        private IPoolWrapper FindOwnerWrapper(UnityEngine.Object obj)
        {
            foreach (var w in _goPools.Values)
                if (w.Owns(obj))
                    return w;
            foreach (var w in _compPools.Values)
                if (w.Owns(obj))
                    return w;
            return null;
        }

        private IEnumerator DespawnAfterDelay(IPoolWrapper w, UnityEngine.Object obj, float t)
        {
            yield return new WaitForSeconds(t);
            if (obj != null) w.Return(obj);
        }
    }
}