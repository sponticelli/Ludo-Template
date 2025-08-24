using System;

namespace Ludo.Core.Signals
{
	/// <summary>
	/// A parameterless signal that can invoke registered Action delegates.
	/// This signal type is used for events that don't need to pass any data to listeners.
	/// </summary>
	public class Signal : BaseSignal
	{
		/// <summary>
		/// Adds a typed Action listener to this signal.
		/// </summary>
		/// <param name="action">The Action to add as a listener</param>
		public void AddListener(Action action)
		{
			AddListener((object)action);
		}

		/// <summary>
		/// Removes a typed Action listener from this signal.
		/// </summary>
		/// <param name="action">The Action to remove from listeners</param>
		public void RemoveListener(Action action)
		{
			RemoveListener((object)action);
		}

		/// <summary>
		/// Invokes all registered listeners without any parameters.
		/// </summary>
		public void Invoke()
		{
			base.Invoke();
		}

		/// <summary>
		/// Handles the invocation of a specific action.
		/// Casts the action to Action and invokes it without parameters.
		/// </summary>
		/// <param name="action">The action to invoke</param>
		/// <param name="p">Parameters (unused for parameterless signals)</param>
		protected override void HandleInvoke(object action, params object[] p)
		{
			((Action)action)();
		}
	}
	/// <summary>
	/// A single-parameter signal that can invoke registered Action&lt;T&gt; and Action delegates.
	/// This signal type is used for events that need to pass one piece of typed data to listeners.
	/// Supports both typed listeners (Action&lt;T&gt;) and parameterless listeners (Action).
	/// </summary>
	/// <typeparam name="T">The type of the parameter passed to listeners</typeparam>
	public class Signal<T> : BaseSignal
	{
		/// <summary>
		/// Adds a typed Action&lt;T&gt; listener to this signal.
		/// </summary>
		/// <param name="action">The Action&lt;T&gt; to add as a listener</param>
		public void AddListener(Action<T> action)
		{
			AddListener((object)action);
		}

		/// <summary>
		/// Removes a typed Action&lt;T&gt; listener from this signal.
		/// </summary>
		/// <param name="action">The Action&lt;T&gt; to remove from listeners</param>
		public void RemoveListener(Action<T> action)
		{
			RemoveListener((object)action);
		}

		/// <summary>
		/// Invokes all registered listeners with the specified parameter.
		/// </summary>
		/// <param name="t">The parameter to pass to listeners</param>
		public void Invoke(T t)
		{
			Invoke((object)t);
		}

		/// <summary>
		/// Handles the invocation of a specific action with type checking.
		/// Supports both Action&lt;T&gt; (receives the parameter) and Action (parameterless) listeners.
		/// </summary>
		/// <param name="action">The action to invoke</param>
		/// <param name="p">Parameters passed from the signal invocation</param>
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
	/// A two-parameter signal that can invoke registered Action&lt;T1, T2&gt;, Action&lt;T1&gt;, and Action delegates.
	/// This signal type is used for events that need to pass two pieces of typed data to listeners.
	/// Supports listeners with varying parameter counts: full (T1, T2), partial (T1), or none.
	/// </summary>
	/// <typeparam name="T1">The type of the first parameter passed to listeners</typeparam>
	/// <typeparam name="T2">The type of the second parameter passed to listeners</typeparam>
	public class Signal<T1, T2> : BaseSignal
	{
		/// <summary>
		/// Adds a typed Action&lt;T1, T2&gt; listener to this signal.
		/// </summary>
		/// <param name="action">The Action&lt;T1, T2&gt; to add as a listener</param>
		public void AddListener(Action<T1, T2> action)
		{
			AddListener((object)action);
		}

		/// <summary>
		/// Removes a typed Action&lt;T1, T2&gt; listener from this signal.
		/// </summary>
		/// <param name="action">The Action&lt;T1, T2&gt; to remove from listeners</param>
		public void RemoveListener(Action<T1, T2> action)
		{
			RemoveListener((object)action);
		}

		/// <summary>
		/// Invokes all registered listeners with the specified parameters.
		/// </summary>
		/// <param name="t1">The first parameter to pass to listeners</param>
		/// <param name="t2">The second parameter to pass to listeners</param>
		public void Invoke(T1 t1, T2 t2)
		{
			Invoke((object)t1, (object)t2);
		}

		/// <summary>
		/// Handles the invocation of a specific action with type checking.
		/// Supports Action&lt;T1, T2&gt; (receives both parameters), Action&lt;T1&gt; (receives first parameter),
		/// and Action (parameterless) listeners.
		/// </summary>
		/// <param name="action">The action to invoke</param>
		/// <param name="p">Parameters passed from the signal invocation</param>
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
	/// A three-parameter signal that can invoke registered Action&lt;T1, T2, T3&gt;, Action&lt;T1, T2&gt;, Action&lt;T1&gt;, and Action delegates.
	/// This signal type is used for events that need to pass three pieces of typed data to listeners.
	/// Supports listeners with varying parameter counts: full (T1, T2, T3), partial (T1, T2), single (T1), or none.
	/// </summary>
	/// <typeparam name="T1">The type of the first parameter passed to listeners</typeparam>
	/// <typeparam name="T2">The type of the second parameter passed to listeners</typeparam>
	/// <typeparam name="T3">The type of the third parameter passed to listeners</typeparam>
	public class Signal<T1, T2, T3> : BaseSignal
	{
		/// <summary>
		/// Adds a typed Action&lt;T1, T2, T3&gt; listener to this signal.
		/// </summary>
		/// <param name="action">The Action&lt;T1, T2, T3&gt; to add as a listener</param>
		public void AddListener(Action<T1, T2, T3> action)
		{
			AddListener((object)action);
		}

		/// <summary>
		/// Removes a typed Action&lt;T1, T2, T3&gt; listener from this signal.
		/// </summary>
		/// <param name="action">The Action&lt;T1, T2, T3&gt; to remove from listeners</param>
		public void RemoveListener(Action<T1, T2, T3> action)
		{
			RemoveListener((object)action);
		}

		/// <summary>
		/// Invokes all registered listeners with the specified parameters.
		/// </summary>
		/// <param name="t1">The first parameter to pass to listeners</param>
		/// <param name="t2">The second parameter to pass to listeners</param>
		/// <param name="t3">The third parameter to pass to listeners</param>
		public void Invoke(T1 t1, T2 t2, T3 t3)
		{
			Invoke((object)t1, (object)t2, (object)t3);
		}

		/// <summary>
		/// Handles the invocation of a specific action with type checking.
		/// Supports Action&lt;T1, T2, T3&gt; (receives all parameters), Action&lt;T1, T2&gt; (receives first two parameters),
		/// Action&lt;T1&gt; (receives first parameter), and Action (parameterless) listeners.
		/// </summary>
		/// <param name="action">The action to invoke</param>
		/// <param name="p">Parameters passed from the signal invocation</param>
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
