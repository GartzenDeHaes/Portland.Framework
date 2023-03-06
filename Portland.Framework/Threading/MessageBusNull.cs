using System;
using System.Collections.Generic;
using System.Text;

using Portland.Text;

namespace Portland.Threading
{
	public sealed class MessageBusNull : IMessageBus<SimpleMessage>
	{
		public void AddMessageObserver(Action<Subscription<SimpleMessage>, SimpleMessage> observer)
		{
		}

		public void DefineMessage(in String10 name)
		{
		}

		public void Publish(in SimpleMessage msg)
		{
		}

		public void Publish(in String10 msgName)
		{
		}

		public void Publish(in String10 msgName, in Variant16 arg)
		{
		}

		public void Publish(in String10 msgName, object data)
		{
		}

		public void Publish(in String10 msgName, in Variant16 arg, object data)
		{
		}

		public void UnSubscribe(string subrUniqueKey, in String10 msgName)
		{
		}

		public void RemoveSubscriber(string subrUniqueKey)
		{
		}

		public void SetMessageMarshaller(Action<Subscription<SimpleMessage>, SimpleMessage> runner)
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

		public void Subscribe(string subrUniqueKey, in String10 msgName, Action<SimpleMessage> action, MessageExecContext ctx = MessageExecContext.BACKGROUND)
		{
		}
	}
}
