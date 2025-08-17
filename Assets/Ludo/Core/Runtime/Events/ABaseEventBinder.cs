using System;
using System.Collections.Generic;

namespace Ludo.Core.Events
{
    /// <summary>
    ///     Base class that tracks bindings between events and delegates so they
    ///     can easily be removed as a group.
    /// </summary>
    public abstract class ABaseEventBinder
    {
        /// <summary>
        ///     Represents a binding between an event and a delegate.
        /// </summary>
        protected struct Binding : IEquatable<Binding>
        {
            public ABaseEvent Event;

            public object Action;

            public Binding(ABaseEvent e, object action)
            {
                Event = e;
                Action = action;
            }

            public bool Equals(Binding other)
            {
                return Equals(Event, other.Event) && Equals(Action, other.Action);
            }

            public override bool Equals(object obj)
            {
                return obj is Binding other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Event, Action);
            }
        }

        /// <summary>
        ///     Set of active bindings managed by this binder.
        /// </summary>
        protected readonly HashSet<Binding> Bindings = new();

        /// <summary>
        ///     Registers a binding and subscribes to the event if it is not already bound.
        /// </summary>
        protected void HandleBind(Binding binding)
        {
            if (binding.Event != null && !Bindings.Contains(binding))
            {
                Bindings.Add(binding);
                binding.Event.AddListener(binding.Action);
            }
        }

        /// <summary>
        ///     Removes a binding and unsubscribes from the event.
        /// </summary>
        protected void HandleUnbind(Binding binding)
        {
            if (Bindings.Remove(binding))
            {
                binding.Event.RemoveListener(binding.Action);
            }
        }

        /// <summary>
        ///     Unbinds all registered bindings.
        /// </summary>
        public void Unbind()
        {
            foreach (Binding binding in Bindings)
            {
                binding.Event.RemoveListener(binding.Action);
            }
            Bindings.Clear();
        }
    }
}
