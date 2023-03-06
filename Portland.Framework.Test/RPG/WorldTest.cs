using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.AI;
using Portland.RPG.Dialogue;

namespace Portland.RPG
{
	internal class WorldTest
	{
		[Test]
		public void ParseEmptyTest()
		{
			string xml = @"<world></world>";
			var world = World.Parse(xml);
		}

		[Test]
		public void ParseEmptyLocationsTest()
		{
			string xml = @"<world><locations></locations></world>";
			var world = World.Parse(xml);
		}

		[Test]
		public void ParseEmptyutilityTest()
		{
			string xml = @"<world><utilities></utilities></world>";
			var world = World.Parse(xml);
		}

		[Test]
		public void ParseEmptyWithDateTest()
		{
			string xml = @"<world date='1/1/2000 11:00 AM'></world>";
			var world = World.Parse(xml);
			Assert.That(world.Clock.Now, Is.EqualTo(DateTime.Parse("1/1/2000 11:00 AM")));
		}

		[Test]
		public void ParseEmptyWithMinutesPerDayTest()
		{
			string xml = @"<world minutesPerDay=24></world>";
			var world = World.Parse(xml);
			Assert.That(world.Clock.SecondsPerHour, Is.AtLeast(60f).And.AtMost(61f));
		}

		[Test]
		public void ParseEmptyWithDateAndMinutesTest()
		{
			string xml = @"<world date='1/1/2000 11:00 AM' minutesPerDay=24></world>";
			var world = World.Parse(xml);
			Assert.That(world.Clock.Now, Is.EqualTo(DateTime.Parse("1/1/2000 11:00 AM")));
			Assert.That(world.Clock.SecondsPerHour, Is.AtLeast(60f).And.AtMost(61f));
		}

		[Test]
		public void ParseLocationTest()
		{
			string xml = @"<world><locations><location name='Vannah Rooftop'/></locations></world>";
			var world = World.Parse(xml);
			Assert.That(world.Locations.Count, Is.EqualTo(1));
			Assert.That(world.Locations[0], Is.EqualTo("Vannah Rooftop"));
		}

		[Test]
		public void ParseDialogueEmpty()
		{
			string xml = @"<world>
<dialogues>
</dialogues>
</world>
";
			var world = World.Parse(xml);
		}

		[Test]
		public void ParseDialogueBase()
		{
			string xml = @"<world>
<dialogues>
Title: Start
---
CharacterId: This is a line of test dialogue.
===
</dialogues>
</world>
";
			var world = World.Parse(xml);
			Assert.That(world.DialogueMan.NodeCount, Is.EqualTo(1));
			Assert.Null(world.DialogueMan.Current);

			//world.DialogueMan.StartDialog("Start");
			//Assert.NotNull(world.DialogueMan.Current);

			//Assert.That(world.DialogueMan.Current, Is.TypeOf<SayNode>());
			//Assert.That(((SayNode)world.DialogueMan.Current).CurrentText, Is.EqualTo("This is a line of test dialogue."));
		}
	}
}
