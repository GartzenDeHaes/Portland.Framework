using System;
using System.Collections.Generic;
using System.Diagnostics;

using Portland.Collections;
using Portland.Text;
using Portland.Threading;

namespace Portland.ComponentModel
{
	//public struct EventMessage
	//{
	//	public String10 EventName;
	//	public String ArgS;
	//	public Variant8 ArgV;
	//}

	/// <summary>
	/// EventBus is a polled, non-threaded message bus
	/// </summary>
	public sealed class EventBus : IMessageBus<SimpleMessage>
	{
		//public struct Subscription
		//{
		//	public String10 EventName;
		//	public Action<EventMessage> Callback;
		//	public string SubscriberKey;
		//}

		Dictionary<String10, Vector<Subscription<SimpleMessage>>> _subscriptions = new Dictionary<String10, Vector<Subscription<SimpleMessage>>>();
		Vector<SimpleMessage> _pending = new Vector<SimpleMessage>(5);
		bool _acceptNewMessageTypes = true;

		public void Poll()
		{
			Vector<Subscription<SimpleMessage>> vec;

			for (int x = 0; x < _pending.Count; x++)
			{
				var msg = _pending[x];
				vec = _subscriptions[msg.MsgName];

				for (int i = 0; i < vec.Count; i++)
				{
					vec[i].Callback(msg);
				}
			}

			_pending.Clear();
		}

		public void Publish(in String10 eventName, in Variant16 arg, in object data)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new SimpleMessage { MsgName = eventName, Arg = arg, Data = data });
		}

		public void Publish(in String10 eventName, in Variant16 arg)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new SimpleMessage { MsgName = eventName, Arg = arg });
		}

		public void Publish(in String10 eventName, in object data)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new SimpleMessage { MsgName = eventName, Data = data });
		}

		public void Publish(in String10 eventName)
		{
			Debug.Assert(_subscriptions.ContainsKey(eventName));

			_pending.Add(new SimpleMessage { MsgName = eventName });
		}

		public void Publish(in SimpleMessage msg)
		{
			Debug.Assert(_subscriptions.ContainsKey(msg.MsgName));

			_pending.Add(msg);
		}

		public void Subscribe(string subscriberKey, in String10 eventName, Action<SimpleMessage> handler, MessageExecContext notUsed = MessageExecContext.UI_UPDATE)
		{
			Vector<Subscription<SimpleMessage>> vec;
			if (!_subscriptions.TryGetValue(eventName, out vec))
			{
				if (_acceptNewMessageTypes)
				{
					vec = new Vector<Subscription<SimpleMessage>>();
					_subscriptions.Add(eventName, vec);
				}
				else
				{
					throw new Exception($"No longer accepting new message types {subscriberKey} > {eventName}");
				}
			}

			vec.Add(new Subscription<SimpleMessage> { MessageName = eventName, Callback = handler, SubscriberUniqueKey = subscriberKey });
		}

		public void RemoveSubscriber(string subscriberKey)
		{
			Func<Subscription<SimpleMessage>, bool> rmfn = (s) => { return s.SubscriberUniqueKey == subscriberKey; };

			foreach(var vec in _subscriptions.Values)
			{
				vec.RemoveWhen(rmfn);	
			}
		}

		public void UnSubscribe(string subscriberKey, in String10 eventName)
		{
			var msgName = eventName;
			Func<Subscription<SimpleMessage>, bool> rmfn = (s) => { return s.SubscriberUniqueKey == subscriberKey && s.MessageName == msgName; };

			foreach (var vec in _subscriptions.Values)
			{
				vec.RemoveWhen(rmfn);
			}
		}

		public void Start()
		{
		}

		public void Shutdown()
		{
		}

		public void StopAcceptingNewMessageDefinitions()
		{
			_acceptNewMessageTypes = false;
		}

		public void DefineMessage(in String10 eventName)
		{
			if (!_subscriptions.TryGetValue(eventName, out var vec))
			{
				if (_acceptNewMessageTypes)
				{
					vec = new Vector<Subscription<SimpleMessage>>();
					_subscriptions.Add(eventName, vec);
				}
				else
				{
					throw new Exception($"No longer accepting new message types {eventName}");
				}
			}
		}
	}
}
