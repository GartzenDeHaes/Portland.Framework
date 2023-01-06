using System;
using System.Collections.Generic;
using System.Linq;

namespace Portland.CheckedEvents
{
	public sealed class ObservableValue<T> //: Marshallable
	{
		private Func<T, bool> _validator;
		private Action<T> _listeners;

		private T _value;

		public T Value
		{
			get { return _value; }
			set
			{
				Set(value);
			}
		}

		public ObservableValue()
		{
		}

		public ObservableValue(T value)
		{
			_value = value;
		}

		public void Set(T value, bool marshall = false)
		{
			if (value != null && value.Equals(_value))
			{
				return;
			}
			bool wasSuccessful = (_validator == null || _validator(value));
			if (wasSuccessful)
			{
				_value = value;
				//if (marshall || RequiresMarshalling())
				//{
				//	SendToMarshaller(() => { _listeners?.Invoke(value); });
				//}
				//else
				//{
					_listeners?.Invoke(value);
				//}
			}
		}

		/// <summary>
		/// Registers a method that will validate changes
		/// </summary>
		public void SetValidator(Func<T, bool> validator)
		{
			_validator = validator;
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
	}
}
