using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.AI
{
	[TestFixture]
	internal class WorldTest
	{
		[Test]
		public void BasicConstructionTest()
		{
			World world = new World(DateTime.Parse("12/31/2000"));
			world.GetBuilder()
				.SetupSimplePlayer("player", "human");
	
			var chr = world.CreateCharacter("player", "player unique name", "Player");

			Assert.That(chr.Facts.Count, Is.AtLeast(16));
		}
	}
}
