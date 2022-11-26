using System;
using System.Collections.Generic;
using System.Text;

using Portland.Text;

namespace Portland.Threading
{
	public sealed class Subscription
	{
		public string SubscriberUniqueKey;
		public TwoPartName8 MessageName;
		public MessageExecContext ThreadContext;
		public Action<SimpleMessage> Callback;
	}
}
