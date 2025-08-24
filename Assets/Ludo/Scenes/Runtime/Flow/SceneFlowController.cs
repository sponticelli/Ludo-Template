using UnityEngine;

namespace Ludo.Scenes.Flow
{
    /// <summary>
    /// Base component for scene flow controllers.
    /// Each scene should implement a concrete controller to orchestrate its flow.
    /// </summary>
    /// <typeparam name="TEvent">Type of events driving state transitions.</typeparam>
    [DisallowMultipleComponent]
    public abstract class SceneFlowController<TEvent> : MonoBehaviour where TEvent : struct
    {
        private readonly StateMachine<TEvent> _stateMachine = new StateMachine<TEvent>();

        /// <summary>
        /// Access to the underlying state machine, allowing nested flows to dispatch events.
        /// </summary>
        protected StateMachine<TEvent> Machine => _stateMachine;

        /// <summary>
        /// Creates the initial state for this scene.
        /// </summary>
        protected abstract FlowState<TEvent> CreateInitialState();

        /// <summary>
        /// Dispatches an event into the scene flow.
        /// </summary>
        public Awaitable Dispatch(TEvent evt) => _stateMachine.Dispatch(evt);

        /// <summary>
        /// Unity callback. Initializes the state machine when the scene starts.
        /// </summary>
        protected async void Start()
        {
            Debug.Log("SceneFlowController Start");
            var initial = CreateInitialState();
            Debug.Log("SceneFlowController SetInitialState");
            await _stateMachine.SetInitialState(initial);
            Debug.Log("SceneFlowController Start done");
        }

        /// <summary>
        /// Unity callback executed every frame.
        /// </summary>
        protected virtual void Update()
        {
            _stateMachine.Tick();
        }
    }
}

