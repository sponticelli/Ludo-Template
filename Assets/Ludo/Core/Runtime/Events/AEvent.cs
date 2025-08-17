using System;

namespace Ludo.Core.Events
{
    /// <summary>
    ///     Event with no parameters.
    /// </summary>
    public class AEvent : ABaseEvent
    {
        public void AddListener(Action action)
        {
            AddListener((object)action);
        }

        public void RemoveListener(Action action)
        {
            RemoveListener((object)action);
        }

        public void Invoke()
        {
            base.Invoke();
        }

        protected override void HandleInvoke(object action, params object[] p)
        {
            ((Action)action)();
        }
    }

    /// <summary>
    ///     Event with a single parameter of type <typeparamref name="T" />.
    /// </summary>
    public abstract class AEvent<T> : ABaseEvent
    {
        public void AddListener(Action<T> action)
        {
            AddListener((object)action);
        }

        public void RemoveListener(Action<T> action)
        {
            RemoveListener((object)action);
        }

        public void Invoke(T t)
        {
            Invoke((object)t);
        }

        protected override void HandleInvoke(object action, params object[] p)
        {
            if (action.GetType() == typeof(Action<T>))
            {
                ((Action<T>)action)((T)p[0]);
            }
            else
            {
                ((Action)action)();
            }
        }
    }

    /// <summary>
    ///     Event with two parameters of types <typeparamref name="T1" /> and <typeparamref name="T2" />.
    /// </summary>
    public abstract class AEvent<T1, T2> : ABaseEvent
    {
        public void AddListener(Action<T1, T2> action)
        {
            AddListener((object)action);
        }

        public void RemoveListener(Action<T1, T2> action)
        {
            RemoveListener((object)action);
        }

        public void Invoke(T1 t1, T2 t2)
        {
            Invoke((object)t1, (object)t2);
        }

        protected override void HandleInvoke(object action, params object[] p)
        {
            if (action.GetType() == typeof(Action<T1, T2>))
            {
                ((Action<T1, T2>)action)((T1)p[0], (T2)p[1]);
            }
            else if (action.GetType() == typeof(Action<T1>))
            {
                ((Action<T1>)action)((T1)p[0]);
            }
            else
            {
                ((Action)action)();
            }
        }
    }

    /// <summary>
    ///     Event with three parameters of types <typeparamref name="T1" />, <typeparamref name="T2" />
    ///     and <typeparamref name="T3" />.
    /// </summary>
    public abstract class AEvent<T1, T2, T3> : ABaseEvent
    {
        public void AddListener(Action<T1, T2, T3> action)
        {
            AddListener((object)action);
        }

        public void RemoveListener(Action<T1, T2, T3> action)
        {
            RemoveListener((object)action);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3)
        {
            Invoke((object)t1, (object)t2, (object)t3);
        }

        protected override void HandleInvoke(object action, params object[] p)
        {
            if (action.GetType() == typeof(Action<T1, T2, T3>))
            {
                ((Action<T1, T2, T3>)action)((T1)p[0], (T2)p[1], (T3)p[2]);
            }
            else if (action.GetType() == typeof(Action<T1, T2>))
            {
                ((Action<T1, T2>)action)((T1)p[0], (T2)p[1]);
            }
            else if (action.GetType() == typeof(Action<T1>))
            {
                ((Action<T1>)action)((T1)p[0]);
            }
            else
            {
                ((Action)action)();
            }
        }
    }
}
