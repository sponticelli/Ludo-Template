using UnityEngine;
using UnityEngine.Events;

namespace Ludo.Core
{
    [DefaultExecutionOrder(ServiceExecutionOrder.Installer)]
    public abstract class ServiceInstaller : MonoBehaviour
    {
        [Header("Unity Events")]
        [SerializeField] private UnityEvent onInitializeComplete;
        
        protected virtual void Awake() => Install();

        protected virtual void Start()
        {
            Initialize();
            onInitializeComplete?.Invoke();
        } 
        
        protected virtual void OnDestroy() => Uninstall();
        
        public abstract void Initialize();
        protected abstract void Install();
        protected abstract void Uninstall();
    }
}