using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.RPG
{
	[TestFixture]
	internal class InventoryTest
	{
		[Test]
		public void BaseTest()
		{
			ItemCollection inv = new ItemCollection("TEST", 8);
			Assert.That(inv.Count, Is.EqualTo(8));

		}
	}
}
