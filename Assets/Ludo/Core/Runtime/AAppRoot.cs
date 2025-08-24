using Ludo.Core.Boot;
using UnityEngine;

namespace Ludo.Core
{
    /// <summary>
    /// Abstract base class for application root components that manage application lifecycle and boot steps.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AAppRoot : MonoBehaviour
    {
        [Header("Boot Steps")]
        /// <summary>
        /// Collection of boot steps executed during startup.
        /// </summary>
        [SerializeField] private BootStep[] bootSteps;
        
        protected static AAppRoot instance;
        
        /// <summary>
        /// Global access to the application root instance.
        /// </summary>
        public static AAppRoot Instance => instance;

        /// <summary>
        /// Ensures a single persistent instance and registers services.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this);
            RegisterServices();
            System.Array.Sort(bootSteps, (a, b) => a.Order.CompareTo(b.Order));
        }

        /// <summary>
        /// Initializes services and executes boot steps after Awake.
        /// </summary>
        protected virtual void Start()
        {
            InitializeServices();
            RunBootSteps();
        }

        /// <summary>
        /// Cleans up services when the root is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance != this) return;
            TeardownServices();
            instance = null;
        }

        /// <summary>
        /// Registers core services with the service locator.
        /// </summary>
        protected abstract void RegisterServices();

        /// <summary>
        /// Initializes registered services.
        /// </summary>
        protected abstract void InitializeServices();

        /// <summary>
        /// Shuts down services in reverse order of initialization.
        /// </summary>
        protected abstract void TeardownServices();

        /// <summary>
        /// Runs configured boot steps in ascending order.
        /// </summary>
        private void RunBootSteps()
        {
            if (bootSteps == null || bootSteps.Length == 0) return;
            
            foreach (var step in bootSteps)
            {
                try
                {
                    step?.Boot();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}