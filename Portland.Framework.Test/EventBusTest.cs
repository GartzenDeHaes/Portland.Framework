using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.Text;

namespace Portland.ComponentModel
{
	[TestFixture]
	internal class EventBusTest
	{
		[Test]
		public void BaseTest()
		{
			EventBus bus = new EventBus();

			bool eventCalled = false;

			bus.Subscribe("TEST", new String10("A.B"), (e) => { eventCalled = true; });
			bus.Publish(new String10("A.B"));
			Assert.IsFalse(eventCalled);
			bus.Poll();
			Assert.IsTrue(eventCalled);

			eventCalled = false;
			bus.RemoveSubscriber("TEST");
			bus.Publish(new String10("A.B"));
			bus.Poll();
			Assert.IsFalse(eventCalled);
		}
	}
}
