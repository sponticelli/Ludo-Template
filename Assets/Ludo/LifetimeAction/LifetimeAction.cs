using UnityEngine;

namespace Ludo.LifetimeAction
{
    public class LifetimeAction : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Events.UnityEvent onAwake;
        [SerializeField] private UnityEngine.Events.UnityEvent onEnable;
        [SerializeField] private UnityEngine.Events.UnityEvent onStart;
        [SerializeField] private UnityEngine.Events.UnityEvent onUpdate;
        [SerializeField] private UnityEngine.Events.UnityEvent onLateUpdate;
        [SerializeField] private UnityEngine.Events.UnityEvent onFixedUpdate;
        [SerializeField] private UnityEngine.Events.UnityEvent onDisable;
        [SerializeField] private UnityEngine.Events.UnityEvent onDestroy;
        
        public event System.Action OnAwake;
        public event System.Action OnEnabled;
        public event System.Action OnStart;
        public event System.Action OnUpdate;
        public event System.Action OnLateUpdate;
        public event System.Action OnFixedUpdate;
        public event System.Action OnDisabled;
        public event System.Action OnDestroied;
        
        
        private void Awake()
        {
            onAwake?.Invoke();
            OnAwake?.Invoke();
        }
        
        private void OnEnable()
        {
            onEnable?.Invoke();
            OnEnabled?.Invoke();
        }
        
        private void Start()
        {
            onStart?.Invoke();
            OnStart?.Invoke();
        }
        
        private void Update()
        {
            onUpdate?.Invoke();
            OnUpdate?.Invoke();
        }
        
        private void LateUpdate()
        {
            onLateUpdate?.Invoke();
            OnLateUpdate?.Invoke();
        }
        
        private void FixedUpdate()
        {
            onFixedUpdate?.Invoke();
            OnFixedUpdate?.Invoke();
        }
        
        private void OnDisable()
        {
            onDisable?.Invoke();
            OnDisabled?.Invoke();
        }
        
        private void OnDestroy()
        {
            onDestroy?.Invoke();
            OnDestroied?.Invoke();
        }
    }
}