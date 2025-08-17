using System;

namespace Ludo.Core.Events
{
    public class EventBinder : ABaseEventBinder
    {
        public void Bind(Event e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind(Event e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Bind<T>(Event<T> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T>(Event<T> e, Action<T> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind<T>(Event<T> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T>(Event<T> e, Action<T> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Bind<T1, T2>(Event<T1, T2> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2>(Event<T1, T2> e, Action<T1> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2>(Event<T1, T2> e, Action<T1, T2> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind<T1, T2>(Event<T1, T2> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2>(Event<T1, T2> e, Action<T1> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2>(Event<T1, T2> e, Action<T1, T2> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(Event<T1, T2, T3> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(Event<T1, T2, T3> e, Action<T1> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(Event<T1, T2, T3> e, Action<T1, T2> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(Event<T1, T2, T3> e, Action<T1, T2, T3> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(Event<T1, T2, T3> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(Event<T1, T2, T3> e, Action<T1> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(Event<T1, T2, T3> e, Action<T1, T2> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(Event<T1, T2, T3> e, Action<T1, T2, T3> action)
        {
            HandleUnbind(new Binding(e, action));
        }
    }
}