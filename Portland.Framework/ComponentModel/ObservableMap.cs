using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Portland.CodeDom.Operators;
using Portland.Collections;

namespace Portland.ComponentModel
{
	public sealed class ObservableMap<TKEY, TVALUE> : IObservableMap<TKEY, TVALUE>
	{
		struct Subscriber
		{
			public string ListenerKey;
			public Action<TKEY, TVALUE> OnChange;
		}

		Vector<Subscriber> _listenersAllChanges = new Vector<Subscriber>(4);
		Dictionary<TKEY, List<Subscriber>> _listeners = new Dictionary<TKEY, List<Subscriber>>();
		Dictionary<TKEY, TVALUE> _map = new Dictionary<TKEY, TVALUE>();

		public int Count { get { return _map.Count; } }

		public TVALUE this[TKEY key]
		{
			get { return Get(key); }
			set { Set(key, value); }
		}

		public IEnumerable<TKEY> Keys { get { return _map.Keys; } }

		public TVALUE Get(in TKEY key)
		{
			return _map[key];
		}

		public bool TryGet(in TKEY key, out TVALUE value)
		{
			return _map.TryGetValue(key, out value);
		}

		public void Set(in TKEY key, in TVALUE value)
		{
			TrySet(key, value);
		}

		public bool TrySet(in TKEY key, in TVALUE value)
		{
			if (_map.TryGetValue(key, out var current))
			{
				if (!current.Equals(value))
				{
					_map[key] = value;

					Raise(key, value);

					return true;
				}
			}
			return false;
		}

		void Raise(in TKEY key, in TVALUE value)
		{
			for (int i = 0; i < _listenersAllChanges.Count; i++)
			{
				_listenersAllChanges[i].OnChange(key, value);
			}

			if (_listeners.TryGetValue(key, out var listeners))
			{
				for (int i = 0; i < listeners.Count; i++)
				{
					listeners[i].OnChange(key, value);
				}
			}
		}

		public bool Contains(in TKEY key)
		{
			return _map.ContainsKey(key);
		}

		public void AddListener(in TKEY valueKey, string listenerKey, Action<TKEY, TVALUE> onChange)
		{
			List<Subscriber> listeners;
			if (! _listeners.TryGetValue(valueKey, out listeners))
			{
				listeners = new List<Subscriber>();
				_listeners.Add(valueKey, listeners);
			}

			listeners.Add(new Subscriber { ListenerKey = listenerKey, OnChange = onChange });
		}

		public void AddListener(string listenerKey, Action<TKEY, TVALUE> onAnyChange)
		{
			_listenersAllChanges.Add(new Subscriber { ListenerKey = listenerKey, OnChange = onAnyChange });
		}

		public void RemoveListener(string listenerKey)
		{
			int i = 0;
			while (i < _listenersAllChanges.Count)
			{
				if (_listenersAllChanges[i].ListenerKey.Equals(listenerKey))
				{
					_listenersAllChanges.RemoveAt(i);
					continue;
				}
				i++;
			}

			foreach (var lst in _listeners.Values)
			{
				RemoveSubscriber(lst, listenerKey);
			}
		}

		void RemoveSubscriber(List<Subscriber> subscribers, string listenerKey)
		{
			int i = 0;
			while (i < subscribers.Count)
			{
				if (subscribers[i].ListenerKey.Equals(listenerKey))
				{
					subscribers.RemoveAt(i);
					continue;
				}
				i++;
			}
		}

		public void RemoveListener(string listenerKey, in TKEY key)
		{
			if (_listeners.TryGetValue(key, out var listeners))
			{
				RemoveSubscriber(listeners, listenerKey);
			}
		}
	}
}
