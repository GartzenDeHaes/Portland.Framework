using System;
using System.Collections.Generic;

namespace Portland.Threading
{
	public class MessageBusTyped
	{
		/// <summary>
		/// Holds every message which is send through this messagebus mapped to there actions                           
		/// </summary>
		readonly Dictionary<Type, List<Delegate>> _handler = new Dictionary<Type, List<Delegate>>();

		/// <summary>
		/// Constructor
		/// </summary>
		public MessageBusTyped()
		{
		}

		/// <summary>
		/// Sends an message
		/// </summary>
		/// <param name="message">The message-object to be send</param>
		public void Send<T>(T message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (_handler.TryGetValue(typeof(T), out var delegates))
			{
				CallHandler(message, delegates);
			}
			else
			{
#if UNITY_2017_1_OR_NEWER
				UnityEngine.Debug.LogWarning($"No receivers for {typeof(T)}");
#else
				System.Diagnostics.Debug.Write($"No receivers for {typeof(T)}");
#endif
			}
			//foreach (var handler in _handler.Keys.Where(handler => handler.GetTypeInfo().IsAssignableFrom(typeof(T))))
			//{
			//	CallHandler(message, _handler[handler]);
			//}
		}

		/// <summary>
		/// Sends an message
		/// </summary>
		/// <param name="message">The message-object to be send</param>
		public void Send<T>()
		{
			if (_handler.TryGetValue(typeof(T), out var delegates))
			{
				CallHandler(default(T), delegates);
			}
			else
			{
#if UNITY_2017_1_OR_NEWER
				UnityEngine.Debug.LogWarning($"No receivers for {typeof(T)}");
#else
				System.Diagnostics.Debug.Write($"No receivers for {typeof(T)}");
#endif
			}
		}

		/// <summary>
		/// Subscribes to the specific message and executes the registered action on receiving the message
		/// </summary>
		/// <param name="action">Action to be called, when the message is received</param>
		public void Subscribe<T>(Action action)
		{
			AddHandlerToMessageType(typeof(T), action);
		}

		/// <summary>
		/// Subscribes to the specific message and executes the registered action on receiving the message
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action">Action to be called, when the message is received</param>
		public void Subscribe<T>(Action<T> action)
		{
			AddHandlerToMessageType(typeof(T), action);
		}

		/// <summary>
		/// Revokes the subscription from the method to the message
		/// </summary>
		public void Unsubscribe<T>(Action action)
		{
			DeleteHandlerByMessage<T>(action);
		}

		/// <summary>
		/// Revokes the subscription from the method to the message
		/// </summary>
		public void Unsubscribe<T>(Action<T> action)
		{
			DeleteHandlerByMessage<T>(action);
		}

		/// <summary>
		/// Closes the messagebus
		/// </summary>
		public void Clear()
		{
			_handler.Clear();
		}

		static void CallHandler<T>(T message, List<Delegate> executingHandler)
		{
			Delegate actionHandler = null;

			for (int i = 0; i < executingHandler.Count; ++i)
			{
				actionHandler = executingHandler[i];
				if (actionHandler is Action<T> parametrizedAction)
				{
					parametrizedAction(message);
					continue;
				}

				 (actionHandler as Action).Invoke();
			}
		}

		void AddHandlerToMessageType(Type messageType, Delegate action)
		{
			MapActionToHandlerByType(messageType, action);
		}

		void MapActionToHandlerByType(Type messageType, Delegate action)
		{
			if (_handler.ContainsKey(messageType))
			{
				_handler[messageType].Add(action);
			}
			else
			{
				_handler[messageType] = new List<Delegate> { action };
			}
		}

		void DeleteHandlerByMessage<T>(Delegate handlerToRemove)
		{
			if (handlerToRemove == null)
			{
				throw new ArgumentNullException(nameof(handlerToRemove));
			}

			if (!_handler.ContainsKey(typeof(T)))
			{
				return;
			}

			var handler = _handler[typeof(T)];
			if (handler.Contains(handlerToRemove))
			{
				handler.Remove(handlerToRemove);
			}
		}
	}
}
