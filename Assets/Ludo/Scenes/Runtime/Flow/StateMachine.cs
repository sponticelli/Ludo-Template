#nullable enable
using UnityEngine;

namespace Ludo.Scenes.Flow
{
    /// <summary>
    /// Generic finite state machine that operates on <see cref="FlowState{TEvent}"/> states.
    /// </summary>
    /// <typeparam name="TEvent">Type of events driving transitions.</typeparam>
    public class StateMachine<TEvent> where TEvent : struct
    {
        FlowState<TEvent>? _current;
        private Awaitable? _transition; // ensures sequential transitions

        /// <summary>
        /// The currently active state.
        /// </summary>
        public FlowState<TEvent>? Current => _current;

        /// <summary>
        /// Enters the provided state as the first state of the machine.
        /// </summary>
        public async Awaitable SetInitialState(FlowState<TEvent> state)
        {
            _current = state;
            await _current.Enter();
        }

        /// <summary>
        /// Sends an event to the current state and performs transitions if requested.
        /// </summary>
        public Awaitable Dispatch(TEvent evt)
        {
            var pending = _transition;
            return _transition = Run();

            async Awaitable Run()
            {
                if (pending != null)
                    await pending; // wait for any transition already in progress
                if (_current == null)
                    return;

                var next = _current.Handle(evt);
                if (next != null && next != _current)
                {
                    await _current.Exit();
                    _current = next;
                    await _current.Enter();
                }
            }
        }

        /// <summary>
        /// Updates the active state. Should be called once per frame.
        /// </summary>
        public void Tick() => _current?.Tick();

    }
}

