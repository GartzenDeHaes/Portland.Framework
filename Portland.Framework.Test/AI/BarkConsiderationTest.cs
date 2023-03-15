using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Portland.Collections;

namespace Portland.AI.Barks
{
    [TestFixture]
	public class BarkConsiderationTest
	{
		[Test]
		public void FullSetupBaseTest()
		{
			//StringTable strings = new StringTable();
			World world = new World(new Clock(DateTime.Now, 1440), new RandMax());

			world.UtilitySystem.CreateObjectiveSetBuilder("living")
				.AddTestObjectives();
			world.UtilitySystem.CreateAgentType("living", "human")
				.AddCommonObjectives();
			world.UtilitySystem.CreateAgent("human", "human");

			//world.DefineNameForActorUtilityObjectiveFact("objective");
			world.DefineTestUtilityAlerts();

			RulePack rulePack = new RulePack();

			rulePack.CreateRule("testing")
				.WhenActionIs("SEE")
				.WhenConceptIs("barrel")
				.Do()
				.Say("COACH", "say:thats_a_barrel", 3f, "COACH: That's a barrel.");

			world.BarkEngine.SetRules(rulePack);

			world.CharacterManager.CreateCharacterDefinition("human");
			world.CreateAgent("human", "COACH");

			string saidText = String.Empty;
			world.BarkEngine.OnSay.Listeners += (cmd, rule) => { saidText = cmd.DefaultTexts.RandomElement(); };

			var tevent = new ThematicEvent { Action = ThematicEvent.ActionSee, Concept = "barrel" };
			var found = world.BarkEngine.TryMatch(tevent);

			Assert.IsTrue(found);
			Assert.That(world.BarkEngine.DelayingCount, Is.EqualTo(1));
			Assert.That(saidText, Is.EqualTo("COACH: That's a barrel."));

			world.Clock.Update(5f);
			world.BarkEngine.Update();
			Assert.That(world.BarkEngine.DelayingCount, Is.EqualTo(0));
		}

		[Test]
		public void HealthAlertTest()
		{
			//StringTable strings = new StringTable();
			World world = new World(new Clock(new DateTime(2001, 01, 03, 9, 0, 0), 1440), new RandMin());

			world.CharacterManager.CreateCharacterDefinition("human")
				.UtilitySetId("human");

			world.UtilitySystem.CreateObjectiveSetBuilder("living")
				.AddTestObjectives();
			world.UtilitySystem.CreateAgentType("living", "human")
				.AddCommonObjectives();
			world.UtilitySystem.CreateAgent("human", "human");

			//world.DefineNameForActorUtilityObjectiveFact("objective");
			
			world.DefineTestUtilityAlerts();

			string saidConcept = String.Empty;
			world.BarkEngine.OnSay.Listeners += (cmd, rule) => { saidConcept = rule.RuleKey; };

			RulePack rulePack = new RulePack();

			rulePack.CreateRule("coach:hello")
				.WhenConceptIs("hello")
				.WhenProbabilityCheckIs(0.2f, true)
				.Do()
				.Say("COACH", "coach:hello", 3, "Hey! Come back! Come back! Ahh, he ain't comin' back.");

			rulePack.CreateRule("coach:say:dying")
				.WhenActionIs("IDLE")
				//.WhenWorldFlagMustBeSetIs("CHARACTER_01_ALIVE")
				.WhenProbabilityCheckIs(0.2f, true)
				.WhenActorFlagMustBeSetIs("COACH", "ALERT_HEALTH")
				.Do()
				.Say("COACH", "SAY", 3, "I can walk this off.");

			rulePack.CreateRule("coach:say:hungry")
				.WhenActionIs("IDLE")
				//.WhenWorldFlagMustBeSetIs("CHARACTER_01_ALIVE")
				.WhenProbabilityCheckIs(0.2f, true)
				.WhenActorFlagMustBeSetIs("COACH", "ALERT_HUNGER")
				.Do()
				.Say("COACH", "SAY", 3, "I find a Burger Tank in this place, I'm gonna be a one-man cheeseburger apocalypse.");

			world.BarkEngine.SetRules(rulePack);

			world.CreateAgent("human", "COACH");
			world.DefineActorAsCharacter0X("COACH", 1, "health");

			Agent coach;
			Assert.True(world.TryGetAgent("COACH", out coach));

			//Assert.That(world.Flags.IsCharacter01Alive, Is.True);
			Assert.That((string)coach.Facts.Get("objective").Value, Is.EqualTo(String.Empty));
			Assert.That((float)coach.Facts.Get("const30%").Value, Is.EqualTo(0.3f));
			Assert.That((float)coach.Facts.Get("health").Value, Is.EqualTo(100f));
			Assert.That((float)coach.Facts.Get("hunger").Value, Is.EqualTo(0f));
			Assert.That((float)coach.Facts.Get("thirst").Value, Is.EqualTo(0f));
			Assert.That((float)coach.Facts.Get("stamina").Value, Is.EqualTo(100f));
			Assert.That((float)coach.Facts.Get("sleepy").Value, Is.EqualTo(0f));

			// should set the objective
			world.Update(0f);
			//Assert.That(world.Flags.IsCharacter01Alive, Is.True);
			Assert.That((string)coach.Facts.Get("objective").Value, Is.EqualTo("idle"));
			Assert.That((float)coach.Facts.Get("health").Value, Is.EqualTo(100f));
			Assert.That((float)coach.Facts.Get("hunger").Value, Is.EqualTo(0f));
			Assert.That((float)coach.Facts.Get("thirst").Value, Is.EqualTo(0f));
			Assert.That((float)coach.Facts.Get("stamina").Value, Is.EqualTo(100f));
			Assert.That((float)coach.Facts.Get("sleepy").Value, Is.EqualTo(0f));

			Assert.False(world.BarkEngine.TryMatch(new ThematicEvent { Action = "IDLE" }));
			Assert.True(world.BarkEngine.TryMatch(new ThematicEvent { Concept = "hello" }));
			Assert.That(saidConcept, Is.EqualTo("coach:hello"));

			// one day
			world.Update(1440f*60f);
			//Assert.That(world.Flags.IsCharacter01Alive, Is.True);
			Assert.That((string)coach.Facts.Get("objective").Value, Is.EqualTo("sleep"));
			Assert.That((float)coach.Facts.Get("health").Value, Is.EqualTo(100f));
			Assert.That(coach.Flags.AlertHealth, Is.False);
			Assert.That((float)coach.Facts.Get("hunger").Value, Is.EqualTo(100f));
			Assert.That(coach.Flags.AlertHunger, Is.True);
			Assert.That((float)coach.Facts.Get("thirst").Value, Is.EqualTo(100f));
			Assert.That(coach.Flags.AlertThrist, Is.True);
			Assert.That((float)coach.Facts.Get("stamina").Value, Is.EqualTo(100f));
			Assert.That((float)coach.Facts.Get("sleepy").Value, Is.EqualTo(100f));
			Assert.That(coach.Flags.AlertSleep, Is.True);

			Assert.True(world.BarkEngine.TryMatch(new ThematicEvent { Action = "IDLE" }));
			Assert.That(saidConcept, Is.EqualTo("coach:say:hungry"));

			world.Update(60f);
			Assert.False(world.BarkEngine.TryMatch(new ThematicEvent { Action = "IDLE" }));

			coach.Facts.Get("health").Set(10f);
			world.Update(60f);
			Assert.True(world.BarkEngine.TryMatch(new ThematicEvent { Action = "IDLE" }));
			Assert.That(saidConcept, Is.EqualTo("coach:say:dying"));
		}
	}
}
