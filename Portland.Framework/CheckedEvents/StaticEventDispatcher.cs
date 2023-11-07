using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.CheckedEvents
{
	public static class StaticEventDispatcher<MSG>
	{
		struct Listener
		{
			public Action<MSG> Callback;
			public String10 Key;
		}

		static Vector<Listener> _listeners = new(2);

		public static void Raise(in MSG msg)
		{
#if DEBUG
			if (_listeners.Count == 0)
			{
#if UNITY_5_3_OR_NEWER
				UnityEngine.Debug.Log($"{callerKey} raised event {msg} with no listeners.");
#else
				Debug.WriteLine($"Raised event {msg} with no listeners.");
#endif
			}
#endif
			for (int i = 0; i < _listeners.Count; i++)
			{
				_listeners[i].Callback.Invoke(msg);
			}
		}

		public static void ListenStart(in String10 key, Action<MSG> callback)
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
