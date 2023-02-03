using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.Text;

namespace Portland.RPG
{
	[TestFixture]
	public class StatManasgerTest
	{
		[Test]
		public void BasicTest()
		{
			StatFactory mgr = new StatFactory();

			mgr.DefineStat("Strength", "STR", 1, 20);
			mgr.DefineStat("Intellegence", "INT", 1, 20);

			mgr.DefineStatSet("HUMN", new AsciiId4[] { "INT", "STR" });
			
			Assert.True(mgr.TryGetStatIndex("HUMN", "INT", out int index));
			Assert.That(index, Is.EqualTo(0));

			Assert.True(mgr.TryGetStatIndex("HUMN", "STR", out index));
			Assert.That(index, Is.EqualTo(1));

			var set = mgr.CreateStats("HUMN");

			Assert.That(set.Stats.Count, Is.EqualTo(2));
			Assert.That(set.Stats[0].StatId.ToString(), Is.EqualTo("INT"));
			Assert.That(set.Stats[1].StatId.ToString(), Is.EqualTo("STR"));

			Assert.That(set.Stats[0].Value, Is.AtMost(10));
			Assert.That(set.Stats[1].Value, Is.AtMost(10));
			Assert.That(set.Stats[0].Value, Is.AtLeast(9));
			Assert.That(set.Stats[1].Value, Is.AtLeast(9));
		}
	}
}
