using System;
using System.Collections.Generic;

namespace Ludo.Core.Events
{
    public abstract class ABaseEvent
    {
        private readonly HashSet<object> _actions = new HashSet<object>();

        public void AddListener(object action)
        {
            _actions.Add(action);
        }

        public void RemoveListener(object action)
        {
            _actions.Remove(action);
        }

        public void ClearListeners()
        {
            _actions.Clear();
        }

        protected void Invoke(params object[] p)
        {
            foreach (object item in new List<object>(_actions))
            {
                HandleInvoke(item, p);
            }
        }

        protected abstract void HandleInvoke(object action, params object[] p);
    }
}