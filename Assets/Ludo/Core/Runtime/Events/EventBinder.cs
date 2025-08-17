using System;

namespace Ludo.Core.Events
{
    /// <summary>
    ///     Convenience binder that exposes strongly typed helper methods for
    ///     binding and unbinding delegates to <see cref="AEvent" /> instances.
    /// </summary>
    public class EventBinder : ABaseEventBinder
    {
        public void Bind(AEvent e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind(AEvent e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Bind<T>(AEvent<T> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T>(AEvent<T> e, Action<T> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind<T>(AEvent<T> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T>(AEvent<T> e, Action<T> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Bind<T1, T2>(AEvent<T1, T2> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2>(AEvent<T1, T2> e, Action<T1> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2>(AEvent<T1, T2> e, Action<T1, T2> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind<T1, T2>(AEvent<T1, T2> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2>(AEvent<T1, T2> e, Action<T1> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2>(AEvent<T1, T2> e, Action<T1, T2> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action<T1> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action<T1, T2> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Bind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action<T1, T2, T3> action)
        {
            HandleBind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action<T1> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action<T1, T2> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        public void Unbind<T1, T2, T3>(AEvent<T1, T2, T3> e, Action<T1, T2, T3> action)
        {
            HandleUnbind(new Binding(e, action));
        }
    }
}
