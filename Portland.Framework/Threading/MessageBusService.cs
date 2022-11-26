using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Portland.Text;

namespace Portland.Threading
{
	public class MessageBusService : IMessageBus
	{
		private readonly ConcurrentDictionary<TwoPartName8, List<Subscription>> _subscriptions = new ConcurrentDictionary<TwoPartName8, List<Subscription>>();

		private readonly ConcurrentQueue<SimpleMessage> _messageQueue = new ConcurrentQueue<SimpleMessage>();

		private bool _lockNewMessages;
		private TwoPartName8 _shutdownMessage;

		protected Action<Subscription, SimpleMessage> _marshaller;

		private IThreadPool _threadPool;
		private ThreadService _thread;

		public MessageBusService(IThreadPool threadPool, TwoPartName8 shutdownMessage)
		{
			_shutdownMessage = shutdownMessage;
			_marshaller = RunCallback;

			_threadPool = threadPool;

			_thread = new ThreadService(nameof(MessageBusService), RunServiceOne);
			_thread.WaitTime = 1000;
		}

		public void StopAcceptingNewMessageDefinitions()
		{
			_lockNewMessages = true;
		}

		/// <summary>
		/// Called to dispatch message, should run Subscription.Callback(SimpleMessage)
		/// </summary>
		public void SetMessageMarshaller(Action<Subscription, SimpleMessage> runner)
		{
			_marshaller = runner;
		}

		/// <summary>
		/// Note, SetMessageMarshaller() removes observers
		/// </summary>
		public void AddMessageObserver(Action<Subscription, SimpleMessage> observer)
		{
			_marshaller += observer;
		}

		public void DefineMessage(TwoPartName8 msgName)
		{
			if (_lockNewMessages)
			{
				throw new Exception(msgName + " cannot add, locked");
			}
			if (!_subscriptions.ContainsKey(msgName))
			{
				_subscriptions.TryAdd(msgName, new List<Subscription>());
			}
		}

		public void Subscribe
		(
			string subscriberUniqueKeyForRemove,
			TwoPartName8 msgName,
			Action<SimpleMessage> action,
			MessageExecContext ctx = MessageExecContext.BACKGROUND
		)
		{
			if (!_subscriptions.ContainsKey(msgName))
			{
				if (_lockNewMessages)
				{
					throw new Exception(msgName + " cannot add, locked");
				}
				_subscriptions.TryAdd(msgName, new List<Subscription>());
			}

			_subscriptions[msgName].Add
			(
				new Subscription()
				{
					SubscriberUniqueKey = subscriberUniqueKeyForRemove,
					MessageName = msgName,
					ThreadContext = ctx,
					Callback = action
				}
			);
		}

		public void RemoveSubscriber(string subrUniqueKey)
		{
			foreach (var service in _subscriptions.Keys)
			{
				var vec = _subscriptions[service];

				for (int x = 0; x < vec.Count; x++)
				{
					if (x < vec.Count && vec[x].SubscriberUniqueKey.Equals(subrUniqueKey))
					{
						vec.RemoveAt(x);
						break;
					}
				}
			}
		}

		public void RemoveSubscriber(string subscriberUniqueKey, TwoPartName8 msgName)
		{
			if (!_subscriptions.ContainsKey(msgName))
			{
				return;
			}

			bool found = false;
			var vec = _subscriptions[msgName];

			for (int x = 0; x < vec.Count; x++)
			{
				if (x < vec.Count && vec[x].SubscriberUniqueKey.Equals(subscriberUniqueKey))
				{
					vec.RemoveAt(x);
					found = true;
					break;
				}
			}

			if (!found)
			{
				Debug.WriteLine("RemoveSubscriber: {0} for {1} not found", msgName, subscriberUniqueKey);
			}
		}

		public void Publish(TwoPartName8 messageName)
		{
			var msg = new SimpleMessage() { Name = messageName };
			Publish(msg);
		}

		public void Publish(TwoPartName8 messageName, Variant16 arg)
		{
			var msg = new SimpleMessage() { Name = messageName, Arg = arg };
			Publish(msg);
		}

		public void Publish(TwoPartName8 messageName, Variant16 arg, object data)
		{
			var msg = new SimpleMessage() { Name = messageName, Arg = arg, Data = data };
			Publish(msg);
		}

		public void Publish(TwoPartName8 messageName, object data)
		{
			var msg = new SimpleMessage() { Name = messageName, Data = data };
			Publish(msg);
		}

		private void Publish(SimpleMessage msg)
		{
			if (!_subscriptions.ContainsKey(msg.Name))
			{
#if UNITY_5_3_OR_NEWER
				UnityEngine.Debug.LogWarning("MessageDispatcher.Publish: No listener for " + msg.Name);
#else
				Debug.WriteLine("MessageDispatcher.Publish: No listener for " + msg.Name);
#endif
				return;
			}

			_messageQueue.Enqueue(msg);

			_thread.SignalRunState();
		}

		void RunServiceOne()
		{
			if (_messageQueue.Count == 0)
			{
				return;
			}

			SimpleMessage msg;

			while (_messageQueue.Count > 0)
			{
				if (!_messageQueue.TryDequeue(out msg))
				{
					continue;
				}

				var subs = _subscriptions[msg.Name];
				int sendCount = 0;

				for (int x = 0; x < subs.Count; x++)
				{
					var sub = subs.ElementAt(x);

					sendCount++;

					try
					{
						_marshaller.Invoke(sub, msg);
					}
					catch (Exception ex)
					{
#if UNITY_5_3_OR_NEWER
						UnityEngine.Debug.LogError(ex);
#else
						Debug.WriteLine(ex);
#endif
					}
				}

				if (sendCount == 0)
				{
#if UNITY_5_3_OR_NEWER
					UnityEngine.Debug.LogWarning("MessageBusService: No listeners for " + msg.Name);
#else
					Debug.WriteLine($"MessageBusService: No listeners for {msg.Name}");
#endif
				}
			}
		}

		private void RunCallback(Subscription sub, SimpleMessage msg)
		{
			_threadPool.QueueWorkItem(() => sub.Callback(msg));
		}

		public void Shutdown()
		{
			Publish(_shutdownMessage);

			_thread.Shutdown();
		}

		public void Start()
		{
			_thread.Start();
		}
	}
}
