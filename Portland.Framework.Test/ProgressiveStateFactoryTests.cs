using Portland.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Collections
{
	[TestFixture]
	public class ProgressiveStateFactoryTests
	{
		[Test]
		public void ParseStateSetTest()
		{
			ProgressiveStateFactory fact = new ProgressiveStateFactory();
			fact.ParseStateSet("HERO_ARC:VILLAGE>MEET_WIZARD>\"Visit Lazy Town\">KILL_DRAGON");
			var states = fact.Create("HERO_ARC");

			Assert.IsFalse(states.IsStarted());
			Assert.IsFalse(states.IsSet(0));
			Assert.IsFalse(states.IsSet(fact.GetBitForStateName("HERO_ARC", "VILLAGE")));

			states.Set(fact.GetBitForStateName("HERO_ARC", "VILLAGE"));
			Assert.IsTrue(states.IsStarted());
			Assert.IsFalse(states.IsComplete());
			Assert.IsTrue(states.IsSet(0));
			Assert.IsTrue(states.IsSet(fact.GetBitForStateName("HERO_ARC", "VILLAGE")));

			Assert.IsFalse(states.IsSet(fact.GetBitForStateName("HERO_ARC", "Visit Lazy Town")));
			states.Set(fact.GetBitForStateName("HERO_ARC", "Visit Lazy Town"));
			Assert.IsTrue(states.IsSet(fact.GetBitForStateName("HERO_ARC", "Visit Lazy Town")));
			Assert.IsFalse(states.IsComplete());

			// Setting a state implies that all previous states are fulfilled.
			Assert.IsTrue(states.IsSet(fact.GetBitForStateName("HERO_ARC", "MEET_WIZARD")));

			fact.ParseStateSet("HELLO:ROOF>AXE>HIT_ZED_WITH_AXE\r\n");
			states = fact.Create("HELLO");
			states.Set(fact.GetBitForStateName("HELLO", "HIT_ZED_WITH_AXE"));
			Assert.IsTrue(states.IsSet(fact.GetBitForStateName("HELLO", "AXE")));
			Assert.IsTrue(states.IsComplete());
		}
	}
}