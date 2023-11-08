using System;

namespace Portland.Threading
{
	public interface IMessageBusTyped
	{
		void Clear();
		void Send<T>();
		void Send<T>(T message);
		void Subscribe<T>(Action action);
		void Subscribe<T>(Action<T> action);
		void Unsubscribe<T>(Action action);
		void Unsubscribe<T>(Action<T> action);
	}
}