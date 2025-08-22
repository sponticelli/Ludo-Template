#nullable enable
using UnityEngine;

namespace Ludo.Scenes.Flow
{
    /// <summary>
    /// Base class for scene flow states.
    /// States encapsulate a single deterministic mode of the user experience.
    /// </summary>
    /// <typeparam name="TEvent">Type of events driving transitions.</typeparam>
    public abstract class FlowState<TEvent> where TEvent : struct
    {
        /// <summary>
        /// Reference to the owning scene controller.
        /// </summary>
        protected readonly SceneFlowController<TEvent> Controller;

        protected FlowState(SceneFlowController<TEvent> controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Called when the state becomes active.
        /// </summary>
        public virtual Awaitable Enter() => default!;

        /// <summary>
        /// Called every frame while this state is active.
        /// </summary>
        public virtual void Tick() { }

        /// <summary>
        /// Called when the state is about to be deactivated.
        /// </summary>
        public virtual Awaitable Exit() => default!;

        /// <summary>
        /// Attempts to handle an incoming event. Returns the next state if a transition is required,
        /// otherwise <c>null</c> to remain in the current state.
        /// </summary>
        public virtual FlowState<TEvent>? Handle(TEvent evt) => null;

        /// <summary>
        /// Creates a nested state machine to model modal or sub flows.
        /// Derived states are responsible for managing the returned machine's lifecycle.
        /// </summary>
        protected StateMachine<TNestedEvent> CreateNestedStateMachine<TNestedEvent>() where TNestedEvent : struct
        {
            return new StateMachine<TNestedEvent>();
        }
    }
}

