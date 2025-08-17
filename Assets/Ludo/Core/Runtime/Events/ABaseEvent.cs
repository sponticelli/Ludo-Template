using System;
using System.Collections.Generic;

namespace Ludo.Core.Events
{
    /// <summary>
    ///     Minimal event system storing listeners as generic objects. Specific
    ///     event implementations provide type-safe wrappers around this base class.
    /// </summary>
    public abstract class ABaseEvent
    {
        private readonly HashSet<object> _actions = new HashSet<object>();

        /// <summary>
        ///     Registers a listener with the event.
        /// </summary>
        public void AddListener(object action)
        {
            _actions.Add(action);
        }

        /// <summary>
        ///     Removes a previously registered listener from the event.
        /// </summary>
        public void RemoveListener(object action)
        {
            _actions.Remove(action);
        }

        /// <summary>
        ///     Removes all listeners from the event.
        /// </summary>
        public void ClearListeners()
        {
            _actions.Clear();
        }

        /// <summary>
        ///     Invokes the event for each listener, passing through the supplied parameters.
        /// </summary>
        protected void Invoke(params object[] p)
        {
            foreach (object item in new List<object>(_actions))
            {
                HandleInvoke(item, p);
            }
        }

        /// <summary>
        ///     Implementation specific invocation logic for individual listeners.
        /// </summary>
        protected abstract void HandleInvoke(object action, params object[] p);
    }
}
