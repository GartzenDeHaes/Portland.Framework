using System;

using Portland.Text;

namespace Portland.Threading
{
	public interface IMessageBus
	{
		void Start();
		void Shutdown();

		void Subscribe(string subrUniqueKey, TwoPartName8 msgName, Action<SimpleMessage> action, MessageExecContext ctx = MessageExecContext.BACKGROUND);
		void RemoveSubscriber(string subrUniqueKey, TwoPartName8 msgName);
		void RemoveSubscriber(string subrUniqueKey);

		void Publish(TwoPartName8 msgName);
		void Publish(TwoPartName8 msgName, Variant16 arg);
		void Publish(TwoPartName8 msgName, object data);
		void Publish(TwoPartName8 msgName, Variant16 arg, object data);
		
		void StopAcceptingNewMessageDefinitions();
		void DefineMessage(TwoPartName8 name);

		void SetMessageMarshaller(Action<Subscription, SimpleMessage> runner);
		void AddMessageObserver(Action<Subscription, SimpleMessage> observer);
	}
}
