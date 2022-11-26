using System;
using System.Collections.Generic;
using System.Text;

using Portland.Text;

namespace Portland.Threading
{
	public class MessageBusNull : IMessageBus
	{
		public void AddMessageObserver(Action<Subscription, SimpleMessage> observer)
		{
		}

		public void DefineMessage(TwoPartName8 name)
		{
		}

		public void Publish(TwoPartName8 msgName)
		{
		}

		public void Publish(TwoPartName8 msgName, Variant16 arg)
		{
		}

		public void Publish(TwoPartName8 msgName, object data)
		{
		}

		public void Publish(TwoPartName8 msgName, Variant16 arg, object data)
		{
		}

		public void RemoveSubscriber(string subrUniqueKey, TwoPartName8 msgName)
		{
		}

		public void RemoveSubscriber(string subrUniqueKey)
		{
		}

		public void SetMessageMarshaller(Action<Subscription, SimpleMessage> runner)
		{
		}

		public void Shutdown()
		{
		}

		public void Start()
		{
		}

		public void StopAcceptingNewMessageDefinitions()
		{
		}

		public void Subscribe(string subrUniqueKey, TwoPartName8 msgName, Action<SimpleMessage> action, MessageExecContext ctx = MessageExecContext.BACKGROUND)
		{
		}
	}
}
