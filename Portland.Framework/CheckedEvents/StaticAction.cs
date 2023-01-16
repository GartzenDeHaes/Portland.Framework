using System;

namespace Portland.CheckedEvents
{
	public static class StaticAction<T>
	{
		private static Action<T> _listeners;

		public static void Raise(T message)
		{
			_listeners?.Invoke(message);
		}

		public static void Add(Action<T> action)
		{
			_listeners += action;
		}

		public static void Remove(Action<T> action) 
		{
			_listeners -= action;
		}

		public static void Clear()
		{
			_listeners = null;
		}
	}
}
