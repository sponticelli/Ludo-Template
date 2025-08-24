using System.Collections.Generic;

namespace Ludo.Core.Signals
{
	/// <summary>
	/// Abstract base class for all signal types in the signal system.
	/// Provides core functionality for managing listeners and invoking actions.
	/// Signals are type-safe event dispatchers that allow decoupled communication between components.
	/// </summary>
	public abstract class BaseSignal
	{
		/// <summary>
		/// Collection of registered action listeners for this signal.
		/// </summary>
		private readonly HashSet<object> _actions = new HashSet<object>();

		/// <summary>
		/// Adds a listener action to this signal.
		/// The action will be invoked when the signal is triggered.
		/// </summary>
		/// <param name="action">The action to add as a listener</param>
		public void AddListener(object action)
		{
			_actions.Add(action);
		}

		/// <summary>
		/// Removes a listener action from this signal.
		/// The action will no longer be invoked when the signal is triggered.
		/// </summary>
		/// <param name="action">The action to remove from listeners</param>
		public void RemoveListener(object action)
		{
			_actions.Remove(action);
		}

		/// <summary>
		/// Removes all listener actions from this signal.
		/// After calling this method, no actions will be invoked when the signal is triggered.
		/// </summary>
		public void ClearListeners()
		{
			_actions.Clear();
		}

		/// <summary>
		/// Invokes all registered listeners with the provided parameters.
		/// Creates a copy of the listeners collection to avoid modification during iteration.
		/// </summary>
		/// <param name="p">Parameters to pass to the listener actions</param>
		protected void Invoke(params object[] p)
		{
			foreach (object item in new List<object>(_actions))
			{
				HandleInvoke(item, p);
			}
		}

		/// <summary>
		/// Abstract method that derived classes must implement to handle action invocation.
		/// This method defines how the specific signal type casts and invokes the action with parameters.
		/// </summary>
		/// <param name="action">The action to invoke</param>
		/// <param name="p">Parameters to pass to the action</param>
		protected abstract void HandleInvoke(object action, params object[] p);
	}
}
