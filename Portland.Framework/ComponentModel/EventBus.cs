using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Text;

namespace Portland.ComponentModel
{
	public struct EventMessage
	{
		public TwoPartName8 EventName;
		public String8 ArgS;
		public Variant8 ArgV;
	}

	/// <summary>
	/// EventBus is a polled, non-threaded message bus
	/// </summary>
	public class EventBus
	{
		public struct Subscription
		{
			public TwoPartName8 EventName;
			public Action<EventMessage> Callback;
			public String8 SubscriberKey;
		}

		Dictionary<TwoPartName8, Vector<Subscription>> _subscriptions = new Dictionary<TwoPartName8, Vector<Subscription>>();
		Vector<EventMessage> _pending = new Vector<EventMessage>();

		public void Poll()
		{
			Vector<Subscription> vec;

			for (int x = 0; x < _pending.Count; x++)
			{
				var msg = _pending[x];
				vec = _subscriptions[msg.EventName];

				for (int i = 0; i < vec.Count; i++)
				{
					vec[i].Callback(msg);
				}
			}

			_pending.Clear();
		}

		public void Publish(in TwoPartName8 eventName, in String8 argStr, in Variant8 argVar)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new EventMessage { EventName = eventName, ArgS = argStr, ArgV = argVar });
		}

		public void Publish(in TwoPartName8 eventName, in String8 argStr)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new EventMessage { EventName = eventName, ArgS = argStr });
		}

		public void Publish(in TwoPartName8 eventName, in Variant8 argVar)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new EventMessage { EventName = eventName, ArgV = argVar });
		}

		public void Publish(in TwoPartName8 eventName)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new EventMessage { EventName = eventName });
		}

		public void Subscribe(String8 subscriberKey, TwoPartName8 eventName, Action<EventMessage> handler)
		{
			Vector<Subscription> vec;
			if (! _subscriptions.TryGetValue(eventName, out vec))
			{
				vec = new Vector<Subscription>();
				_subscriptions.Add(eventName, vec);
			}

			vec.Add(new Subscription { EventName = eventName, Callback = handler, SubscriberKey = subscriberKey });
		}

		public void UnSubscribe(String8 subscriberKey)
		{
			var rmfn = (Subscription s) => { return s.SubscriberKey == subscriberKey; };

			foreach(var vec in _subscriptions.Values)
			{
				vec.RemoveWhen(rmfn);	
			}
		}
	}
}
