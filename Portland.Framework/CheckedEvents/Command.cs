using System;

namespace Portland.CheckedEvents
{
	public class Command //: Marshallable
	{
		public Action Listeners;

		/// <summary>
		/// Called from FPMeleeEventHandler.On_Hit, called from Mecanim anim event
		/// </summary>
		public void Send()
		{
			//if (RequiresMarshalling())
			//{
			//	SendToMarshaller(() => { Listeners?.Invoke(); });
			//}
			//else
			//{
				Listeners?.Invoke();
			//}
		}

		//public void SendUnMarshalled()
		//{
		//	Listeners?.Invoke();
		//}

		public void Clear()
		{
			Listeners = null;
		}
	}

	public class Command<T> //: Marshallable
	{
		public Action<T> Listeners;

		/// <summary>
		/// Called from FPMeleeEventHandler.On_Hit, called from Mecanim anim event
		/// </summary>
		public void Send(T arg)
		{
			//if (RequiresMarshalling())
			//{
			//	SendToMarshaller(() => { Listeners?.Invoke(arg); });
			//}
			//else
			//{
				Listeners?.Invoke(arg);
			//}
		}

		//public void SendUnMarshalled(T arg)
		//{
		//	Listeners?.Invoke(arg);
		//}
	}

	public class Command<T1, T2> //: Marshallable
	{
		public Action<T1, T2> Listeners;

		/// <summary>
		/// Called from FPMeleeEventHandler.On_Hit, called from Mecanim anim event
		/// </summary>
		public void Send(T1 arg1, T2 arg2)
		{
			//if (RequiresMarshalling())
			//{
			//	SendToMarshaller(() => { Listeners?.Invoke(arg1, arg2); });
			//}
			//else
			//{
				Listeners?.Invoke(arg1, arg2);
			//}
		}

		//public void SendUnMarshalled(T1 arg1, T2 arg2)
		//{
		//	Listeners?.Invoke(arg1, arg2);
		//}
	}

	public class Command<T1, T2, T3> //: Marshallable
	{
		public Action<T1, T2, T3> Listeners;

		/// <summary>
		/// Called from FPMeleeEventHandler.On_Hit, called from Mecanim anim event
		/// </summary>
		public void Send(T1 arg1, T2 arg2, T3 arg3)
		{
			//if (RequiresMarshalling())
			//{
			//	SendToMarshaller(() => { Listeners?.Invoke(arg1, arg2, arg3); });
			//}
			//else
			//{
				Listeners?.Invoke(arg1, arg2, arg3);
			//}
		}

		public void SendUnMarshalled(T1 arg1, T2 arg2, T3 arg3)
		{
			Listeners?.Invoke(arg1, arg2, arg3);
		}
	}

	public class Command<T1, T2, T3, T4> //: Marshallable
	{
		public Action<T1, T2, T3, T4> Listeners;

		public void Send(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			//if (RequiresMarshalling())
			//{
			//	SendToMarshaller(() => { Listeners?.Invoke(arg1, arg2, arg3, arg4); });
			//}
			//else
			//{
				Listeners?.Invoke(arg1, arg2, arg3, arg4);
			//}
		}

		//public void SendUnMarshalled(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		//{
		//	Listeners?.Invoke(arg1, arg2, arg3, arg4);
		//}
	}

	public class Command<T1, T2, T3, T4, T5> //: Marshallable
	{
		public Action<T1, T2, T3, T4, T5> Listeners;

		public void Send(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			//if (RequiresMarshalling())
			//{
			//	SendToMarshaller(() => { Listeners?.Invoke(arg1, arg2, arg3, arg4, arg5); });
			//}
			//else
			//{
				Listeners?.Invoke(arg1, arg2, arg3, arg4, arg5);
			//}
		}

		//public void SendUnMarshalled(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		//{
		//	Listeners?.Invoke(arg1, arg2, arg3, arg4, arg5);
		//}
	}

	public class Command<T1, T2, T3, T4, T5, T6> //: Marshallable
	{
		public Action<T1, T2, T3, T4, T5, T6> Listeners;

		public void Send(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			//if (RequiresMarshalling())
			//{
			//	SendToMarshaller(() => { Listeners?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6); });
			//}
			//else
			//{
				Listeners?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
			//}
		}

		//public void SendUnMarshalled(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		//{
		//	Listeners?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
		//}
	}
}
