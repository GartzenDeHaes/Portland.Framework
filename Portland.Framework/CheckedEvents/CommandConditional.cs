using System;

namespace Portland.CheckedEvents
{
	public sealed class CommandConditional //: Marshallable
	{
		private Func<bool> _tryer;
		private Action _listeners;

		/// <summary>
		/// Registers a method that will try to execute this action.
		/// </summary>
		public void SetTryer(Func<bool> tryer)
		{
			_tryer = tryer;
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddListener(Action listener)
		{
			_listeners += listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveListener(Action listener)
		{
			_listeners -= listener;
		}

		public bool Try()
		{
			bool wasSuccessful = (_tryer == null || _tryer());
			if (wasSuccessful)
			{
				//if (RequiresMarshalling())
				//{
				//	SendToMarshaller(() => { _listeners?.Invoke(); });
				//}
				//else
				//{
					_listeners?.Invoke();
				//}
				return true;
			}

			return false;
		}
	}

	public class CommandConditional<T> //: Marshallable
	{
		private Func<T, bool> _tryer;
		private Action<T> _listeners;

		/// <summary>
		/// Registers a method that will try to execute this action.
		/// </summary>
		public void SetTryer(Func<T, bool> tryer)
		{
			_tryer = tryer;
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddListener(Action<T> listener)
		{
			_listeners += listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveListener(Action<T> listener)
		{
			_listeners -= listener;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Try(T arg)
		{
			bool wasSuccessful = (_tryer == null || _tryer(arg));
			if (wasSuccessful)
			{
				//if (RequiresMarshalling())
				//{
				//	SendToMarshaller(() => { _listeners?.Invoke(arg); });
				//}
				//else
				//{
					_listeners?.Invoke(arg);
				//}
				return true;
			}

			return false;
		}
	}
}
