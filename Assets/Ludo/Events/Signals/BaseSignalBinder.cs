using System;
using System.Collections.Generic;

namespace Ludo.Events.Signals
{
    /// <summary>
    /// Abstract base class for signal binding management.
    /// Provides functionality to track and manage signal-action bindings,
    /// ensuring proper cleanup and preventing duplicate bindings.
    /// </summary>
    public abstract class BaseSignalBinder
    {
        /// <summary>
        /// Represents a binding between a signal and an action.
        /// Implements equality comparison to prevent duplicate bindings.
        /// </summary>
        protected struct Binding : IEquatable<Binding>
        {
            /// <summary>
            /// The signal that this binding is associated with.
            /// </summary>
            public readonly BaseSignal Signal;

            /// <summary>
            /// The action that will be invoked when the signal is triggered.
            /// </summary>
            public readonly object Action;

            /// <summary>
            /// Initializes a new binding between a signal and an action.
            /// </summary>
            /// <param name="e">The signal to bind to</param>
            /// <param name="action">The action to bind</param>
            public Binding(BaseSignal e, object action)
            {
                Signal = e;
                Action = action;
            }

            /// <summary>
            /// Determines whether this binding is equal to another binding.
            /// Two bindings are equal if they have the same signal and action.
            /// </summary>
            /// <param name="other">The other binding to compare with</param>
            /// <returns>True if the bindings are equal, false otherwise</returns>
            public bool Equals(Binding other)
            {
                return Equals(Signal, other.Signal) && Equals(Action, other.Action);
            }

            /// <summary>
            /// Determines whether this binding is equal to the specified object.
            /// </summary>
            /// <param name="obj">The object to compare with</param>
            /// <returns>True if the object is a Binding and is equal to this binding, false otherwise</returns>
            public override bool Equals(object obj)
            {
                return obj is Binding other && Equals(other);
            }

            /// <summary>
            /// Returns a hash code for this binding based on the signal and action.
            /// </summary>
            /// <returns>A hash code for the current binding</returns>
            public override int GetHashCode()
            {
                return HashCode.Combine(Signal, Action);
            }
        }

        /// <summary>
        /// Collection of all active bindings managed by this binder.
        /// </summary>
        protected readonly HashSet<Binding> Bindings = new HashSet<Binding>();

        /// <summary>
        /// Handles the binding of a signal-action pair.
        /// Prevents duplicate bindings and null signals.
        /// </summary>
        /// <param name="binding">The binding to add</param>
        protected void HandleBind(Binding binding)
        {
            if (binding.Signal == null || Bindings.Contains(binding)) return;

            Bindings.Add(binding);
            binding.Signal.AddListener(binding.Action);
        }

        /// <summary>
        /// Handles the unbinding of a signal-action pair.
        /// Removes the binding from tracking and the listener from the signal.
        /// </summary>
        /// <param name="binding">The binding to remove</param>
        protected void HandleUnbind(Binding binding)
        {
            if (!Bindings.Remove(binding)) return;
            binding.Signal.RemoveListener(binding.Action);
        }

        /// <summary>
        /// Unbinds all currently tracked bindings.
        /// Removes all listeners from their respective signals and clears the binding collection.
        /// This method should be called when the binder is no longer needed to prevent memory leaks.
        /// </summary>
        public void Unbind()
        {
            foreach (Binding binding in Bindings)
            {
                binding.Signal.RemoveListener(binding.Action);
            }
            Bindings.Clear();
        }
    }
}