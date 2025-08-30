using System;

namespace Ludo.Events.Signals
{
    /// <summary>
    /// Provides a convenient interface for binding and unbinding signal listeners.
    /// Manages signal-action bindings and automatically handles listener registration/deregistration.
    /// Supports signals with 0-3 parameters and various action signatures.
    /// </summary>
    public class SignalBinder : BaseSignalBinder
    {
        /// <summary>
        /// Binds an action to a parameterless signal.
        /// </summary>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The action to execute when the signal is invoked</param>
        public void Bind(Signal e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds an action from a parameterless signal.
        /// </summary>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The action to remove</param>
        public void Unbind(Signal e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a parameterless action to a single-parameter signal.
        /// The action will be called regardless of the signal's parameter value.
        /// </summary>
        /// <typeparam name="T">Type of the signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The parameterless action to execute</param>
        public void Bind<T>(Signal<T> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a typed action to a single-parameter signal.
        /// </summary>
        /// <typeparam name="T">Type of the signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The typed action to execute with the signal parameter</param>
        public void Bind<T>(Signal<T> e, Action<T> action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a parameterless action from a single-parameter signal.
        /// </summary>
        /// <typeparam name="T">Type of the signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The parameterless action to remove</param>
        public void Unbind<T>(Signal<T> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a typed action from a single-parameter signal.
        /// </summary>
        /// <typeparam name="T">Type of the signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The typed action to remove</param>
        public void Unbind<T>(Signal<T> e, Action<T> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a parameterless action to a two-parameter signal.
        /// The action will be called regardless of the signal's parameter values.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The parameterless action to execute</param>
        public void Bind<T1, T2>(Signal<T1, T2> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a single-parameter action to a two-parameter signal.
        /// The action will receive only the first parameter.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The single-parameter action to execute</param>
        public void Bind<T1, T2>(Signal<T1, T2> e, Action<T1> action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a two-parameter action to a two-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The two-parameter action to execute</param>
        public void Bind<T1, T2>(Signal<T1, T2> e, Action<T1, T2> action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a parameterless action from a two-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The parameterless action to remove</param>
        public void Unbind<T1, T2>(Signal<T1, T2> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a single-parameter action from a two-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The single-parameter action to remove</param>
        public void Unbind<T1, T2>(Signal<T1, T2> e, Action<T1> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a two-parameter action from a two-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The two-parameter action to remove</param>
        public void Unbind<T1, T2>(Signal<T1, T2> e, Action<T1, T2> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a parameterless action to a three-parameter signal.
        /// The action will be called regardless of the signal's parameter values.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The parameterless action to execute</param>
        public void Bind<T1, T2, T3>(Signal<T1, T2, T3> e, Action action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a single-parameter action to a three-parameter signal.
        /// The action will receive only the first parameter.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The single-parameter action to execute</param>
        public void Bind<T1, T2, T3>(Signal<T1, T2, T3> e, Action<T1> action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a two-parameter action to a three-parameter signal.
        /// The action will receive the first two parameters.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The two-parameter action to execute</param>
        public void Bind<T1, T2, T3>(Signal<T1, T2, T3> e, Action<T1, T2> action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Binds a three-parameter action to a three-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to bind to</param>
        /// <param name="action">The three-parameter action to execute</param>
        public void Bind<T1, T2, T3>(Signal<T1, T2, T3> e, Action<T1, T2, T3> action)
        {
            HandleBind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a parameterless action from a three-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The parameterless action to remove</param>
        public void Unbind<T1, T2, T3>(Signal<T1, T2, T3> e, Action action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a single-parameter action from a three-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The single-parameter action to remove</param>
        public void Unbind<T1, T2, T3>(Signal<T1, T2, T3> e, Action<T1> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a two-parameter action from a three-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The two-parameter action to remove</param>
        public void Unbind<T1, T2, T3>(Signal<T1, T2, T3> e, Action<T1, T2> action)
        {
            HandleUnbind(new Binding(e, action));
        }

        /// <summary>
        /// Unbinds a three-parameter action from a three-parameter signal.
        /// </summary>
        /// <typeparam name="T1">Type of the first signal parameter</typeparam>
        /// <typeparam name="T2">Type of the second signal parameter</typeparam>
        /// <typeparam name="T3">Type of the third signal parameter</typeparam>
        /// <param name="e">The signal to unbind from</param>
        /// <param name="action">The three-parameter action to remove</param>
        public void Unbind<T1, T2, T3>(Signal<T1, T2, T3> e, Action<T1, T2, T3> action)
        {
            HandleUnbind(new Binding(e, action));
        }
    }
}