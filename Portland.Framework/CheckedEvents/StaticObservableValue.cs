using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.CheckedEvents
{
	public static class StaticObservableValue<VALUE>
	{
		struct Listener
		{
			public Action<VALUE, VALUE> Callback;
			public String10 Key;
		}

		static VALUE _value;
		public static VALUE Value { get { return _value; } }

		static Vector<Listener> _listeners = new(2);

		public static void Set(in VALUE value)
		{
			if (_value.Equals(value))
			{
				return;
			}

#if DEBUG
			if (_listeners.Count == 0)
			{
#if UNITY_5_3_OR_NEWER
				UnityEngine.Debug.Log($"Set value {value} with no listeners.");
#else
				Debug.WriteLine($"Set value {value} with no listeners.");
#endif
			}
#endif
			for (int i = 0; i < _listeners.Count; i++)
			{
				_listeners[i].Callback.Invoke(value, _value);
			}

			_value = value;
		}

		public static void ListenStart(in String10 key, Action<VALUE, VALUE> callback)
		{
			for (int i = 0; i < _listeners.Count; i++)
			{
				if (_listeners[i].Key == key)
				{
					throw new Exception($"Duplicate listener key '{key}'");
				}
			}
			_listeners.Add(new Listener { Key = key, Callback = callback });
		}

		public static void ListenStop(in String10 key)
		{
			for (int i = 0; i < _listeners.Count; i++)
			{
				if (_listeners[i].Key == key)
				{
					_listeners.RemoveAt(i);
					return;
				}
			}
		}
	}
}
