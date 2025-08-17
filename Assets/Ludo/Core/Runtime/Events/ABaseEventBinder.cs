using System;
using System.Collections.Generic;

namespace Ludo.Core.Events
{
    public abstract class ABaseEventBinder
    {
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

        protected readonly HashSet<Binding> Bindings = new();

        protected void HandleBind(Binding binding)
        {
            if (binding.Event != null && !Bindings.Contains(binding))
            {
                Bindings.Add(binding);
                binding.Event.AddListener(binding.Action);
            }
        }

        protected void HandleUnbind(Binding binding)
        {
            if (Bindings.Remove(binding))
            {
                binding.Event.RemoveListener(binding.Action);
            }
        }

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