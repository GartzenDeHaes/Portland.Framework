using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.Mathmatics;

namespace Portland.AI.Barks
{
	[TestFixture]
	public class BarkEngineTest
	{
		[Test]
		public void WhenConceptIsBarrel_DoSayBarrel()
		{
			const string DemoRuleText_BasicRule = @"
WHEN ACTION IS SEE, OBJECT IS barrel 
DO COACH SAYS thats_a_barrel ""COACH: That's a barrel"".";

			TextTable strings = new TextTable();
			World world = new World(new Clock(DateTime.Now, 1440), strings);
			RuleEngine eng = new RuleEngine(world, strings, new RndMin(), DemoRuleText_BasicRule);

			string saidText = String.Empty;
			eng.DoSay.Listeners += (textid, defaultText) => { saidText = defaultText; };
			//TextTableToken textId = new TextTableToken();
			//eng.OnConceptChanged.Listeners += (textid) => { textId = textid; };

			var sentence = new ThematicEvent { Action = ThematicEvent.ActionSee, DirectObject = strings.Get("barrel") };
			var found = eng.TryMatch(sentence);
			Assert.IsTrue(found);
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			Assert.That(saidText, Is.EqualTo("COACH: That's a barrel"));
			
			//Assert.That(textId.Index, Is.Not.Zero);

			world.Clock.Update(5f);
			eng.Update();
			//Assert.That(textId.Index, Is.Not.Zero);

			Assert.That(eng.DelayingCount, Is.EqualTo(0));
		}

		[Test]
		public void TwoBotsAndBarrels()
		{
			const string barkScript = @"
WHEN 
	ACTION IS SEE,
	OBJECT IS barrel,
	TEST barrels IS UNSET
DO
	XBOT SAYS saw_barrel_01 ""XBOT: That's a barrel."" DURATION 3,
	SET barrels TO 1
.
WHEN 
	ACTION IS SEE,
	OBJECT IS barrel,
	TEST barrels == 1
DO
	XBOT SAYS saw_barrel_02 ""XBOT: Another barrel."" DURATION 3,
	ADD 1 TO barrels
.
WHEN 
	ACTION IS SEE,
	OBJECT IS barrel,
	TEST barrels == 2
DO
	XBOT SAYS saw_barrel_03 ""XBOT: This is the third barrel."" DURATION 3,
	ADD 1 TO barrels
.
";
			TextTable strings = new TextTable();
			World world = new World(new Clock(DateTime.Now, 1440), strings);
			RuleEngine eng = new RuleEngine(world, strings, new RndMax(), barkScript);

			world.CreateActor("bot", "XBOT");
			world.CreateActor("bot", "YBOT");

			TextTableToken lastDoSayId = default(TextTableToken);
			string lastDoSayText = String.Empty;

			eng.DoSay.Listeners += (id, defaulText) => { lastDoSayId = id; lastDoSayText = defaulText; };

			var sentence = new ThematicEvent
			{
				DirectObject = strings.Get("random_text")
			};
			Assert.False(eng.TryMatch(sentence));

			sentence = new ThematicEvent
			{
				Action = ThematicEvent.ActionSee,
				DirectObject = strings.Get("barrel"),
				Agent = strings.Get("XBOT")
			};
			Assert.True(eng.TryMatch(sentence));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("saw_barrel_01"));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			world.Clock.Update(5f);
			eng.Update();

			Assert.True(eng.TryMatch(sentence));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("saw_barrel_02"));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			world.Clock.Update(5f);
			eng.Update();

			Assert.True(eng.TryMatch(sentence));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("saw_barrel_03"));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			world.Clock.Update(5f);
			eng.Update();
		}

		[Test]
		public void I_Hate_Hotels_Test()
		{
			const string barkScript = @"
ALIAS FLAG CHARACTER_01_ALIVE AS BILL_ALIVE.
ALIAS FLAG CHARACTER_02_ALIVE AS FRANCIS_ALIVE.
ALIAS FLAG CHARACTER_03_ALIVE AS LOUIS_ALIVE.
ALIAS FLAG CHARACTER_01_ALIVE AS ZOEY_ALIVE.

WHEN 
	OBJECT IS scene_01:start,
DO
	LOUIS SAYS the_apc_is_dead ""Louis: Guys, I think the APC is toast."" DURATION 3
.
WHEN
	ACTION IS SAY,
	OBJECT IS the_apc_is_dead,
	FLAG BILL_ALIVE
DO 
	BILL SAYS cp_quest_statement ""Bill: Get it together people. The CEDA command post is on the top floor, maybe we can find out where they went."" DURATION 5
.
WHEN
	ACTION IS SEE,
	OBJECT IS ceda_trailer,
	OBSERVER IS ZOEY
DO
	ADD 1 TO ZOEY.ceda_trailers_seen,
	RESET DELAY 2
.
WHEN
	ACTION IS SEE,
	OBJECT IS ceda_trailer,
	FLAG ZOEY_ALIVE,
	TEST ZOEY.ceda_trailers_seen == 1
DO
	ZOEY SAYS couldnt_hold_out ""Zoey: Guess they couldn't hold out."" DURATION 3
.
WHEN
	ACTION IS SEE,
	OBJECT IS hotel_lobby,
	FLAG FRANCIS_ALIVE
DO
	FRANCIS SAYS i_hate_hotels ""Francis: I hate hotels"" DURATION 3
.
WHEN
	ACTION IS SAY,
	OBJECT IS i_hate_hotels,
	FLAGS ARE ZOEY_ALIVE FRANCIS_ALIVE
DO
	ZOEY SAYS what_dont_you_hate ""Zoey: What don't you hate?"" DURATION 3
.
WHEN
	ACTION IS SAY,
	OBJECT IS what_dont_you_hate,
	FLAGS ARE ZOEY_ALIVE FRANCIS_ALIVE
DO
	FRANCIS SAYS there_is_one_thing ""Francis: Well, there is one thing."" DURATION 3
.

";
			TextTable strings = new TextTable();
			World world = new World(new Clock(DateTime.Now, 1440), strings);
			RuleEngine eng = new RuleEngine(world, strings, new RndMax(), barkScript);

			world.Flags.Daylight = true;
			world.Flags.IsCharacter01Alive = true;
			world.Flags.IsCharacter02Alive = true;
			world.Flags.IsCharacter03Alive = true;
			world.Flags.IsCharacter04Alive = true;

			world.CreateActor("human", "BILL");
			world.CreateActor("human", "FRANCIS");
			world.CreateActor("human", "LOUIS");
			world.CreateActor("human", "ZOEY");

			TextTableToken lastDoSayId = default(TextTableToken);
			string lastDoSayText = String.Empty;
			//TextTableToken lastConceptEvent = default(TextTableToken);

			eng.DoSay.Listeners += (id, defaulText) => { lastDoSayId = id; lastDoSayText = defaulText; };
			//eng.OnConceptChanged.Listeners += (id) => { lastConceptEvent = id; };

			/* EXT. CEDA BARRICADE - SUNSET.

			East of the CEDA processing point at the hotel VANNAH, an APC
			crashes through the cement barricades and stops, wrecked.

			The rear ramp of the APC drops open.

			ENTER the survivors.

								LOUIS
					Guys, I think the APC is toast.

								BILL
					Get it together people. The CEDA command post is on the 
					top floor.  Maybe we can find out where they went.
			*/
			var sentence = new ThematicEvent { 
				DirectObject = strings.Get("scene_01:start") 
			};
			Assert.True(eng.TryMatch(sentence));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("the_apc_is_dead"));
			Assert.That(lastDoSayText, Is.EqualTo("Louis: Guys, I think the APC is toast."));    

			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			//Assert.That(strings.GetString(lastConceptEvent), Is.EqualTo("the_apc_is_dead"));

			world.Clock.Update(5f);
			eng.Update();
			//Assert.That(strings.GetString(lastConceptEvent), Is.EqualTo("cp_quest_statement"));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("cp_quest_statement"));
			Assert.True(lastDoSayText.StartsWith("Bill: Get it together"));

			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			world.Clock.Update(5f);
			eng.Update();
			Assert.That(eng.DelayingCount, Is.Zero);

			/* EXT. CEDA INTAKE - SUNSET
			 
							ZOEY
					Looks like they couldn't hold out.
			 */
			sentence = new ThematicEvent {
				Action = ThematicEvent.ActionSee,
				DirectObject = strings.Get("ceda_trailer")
			};
			Assert.True(eng.TryMatch(sentence));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			Assert.That(world.GetActor("ZOEY").Facts[strings.Get("ceda_trailers_seen")].ToInt(), Is.EqualTo(1));

			world.Clock.Update(5f);
			eng.Update();
			Assert.That(eng.DelayingCount, Is.Zero);

			Assert.True(eng.TryMatch(sentence));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			Assert.That(world.GetActor("ZOEY").Facts[strings.Get("ceda_trailers_seen")].ToInt(), Is.EqualTo(1));
			//Assert.That(strings.GetString(lastConceptEvent), Is.EqualTo("couldnt_hold_out"));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("couldnt_hold_out"));
			Assert.True(lastDoSayText.StartsWith("Zoey: Guess they couldn't hold out."));

			world.Clock.Update(5f);
			eng.Update();
			Assert.That(eng.DelayingCount, Is.Zero);

			/* EXT. VANNAH LOBBY - SUNSET
			
							FRANCIS
					I hate hotels.

							ZOEY
					Is there anything you don't hate?

							FRANCIS
					Well, there is one thing.
			*/
			sentence = new ThematicEvent
			{
				Action = ThematicEvent.ActionSee,
				DirectObject = strings.Get("hotel_lobby")
			};
			Assert.True(eng.TryMatch(sentence));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("i_hate_hotels"));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			world.Clock.Update(5f);
			eng.Update();
			Assert.That(eng.DelayingCount, Is.EqualTo(1));

			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("what_dont_you_hate"));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			world.Clock.Update(5f);
			eng.Update();
			Assert.That(eng.DelayingCount, Is.EqualTo(1));

			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("there_is_one_thing"));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			world.Clock.Update(5f);
			eng.Update();
			Assert.That(eng.DelayingCount, Is.Zero);
		}

		[Test]
		public void NickDyingTest()
		{
			const string barkScript = @"
ALIAS FLAG CHARACTER_01_ALIVE AS COACH_ALIVE.
ALIAS FLAG CHARACTER_02_ALIVE AS ELLIS_ALIVE.
ALIAS FLAG CHARACTER_03_ALIVE AS NICK_ALIVE.
ALIAS FLAG CHARACTER_01_ALIVE AS ROCHELLE_ALIVE.

WHEN 
	ACTION IS IDLE,
	FLAGS ARE NICK.ALERT_HEALTH !IS_CHARACTER_SPEAKING,
	CHANCE 10% NORETRY
DO
	NICK SAYS ally_dying ""NICK: Not dead yet, but not exactly healthy.""
		OR ""NICK: I really screwed the pouch back there.""
		OR ""NICK: I gotta take better care of myself.""
		OR ""NICK: If I go, you guys are gonna miss me.""
		OR ""NICK: I'm not dying in the middle of nowhere.""
		DURATION 3
.
WHEN 
	ACTION IS SAY,
	AGENT IS NICK,
	OBJECT IS ally_dying,
	FLAGS ARE COACH_ALIVE NICK_ALIVE,
	CHANCE 5%
DO
	COACH SAYS coach_coaches_nick_dying ""COACH: Hey Nick, at least you dressed for a funeral."" DURATION 3,
.
WHEN 
	ACTION IS SAY,
	AGENT IS ELLIS,
	OBJECT IS ally_dying,
	FLAGS aRE COACH_ALIVE ELLIS_ALIVE,
	CHANCE 5%
DO
	COACH SAYS coach_coaches_ally_dying ""COACH: Come on Ellis, you got it in ya."" 
		OR ""COACH: Come on yougin' If I can do it, you can do it.""
		DURATION 3
.
WHEN 
	ACTION IS SAY,
	OBJECT IS ally_dying,
	FLAGS ARE COACH_ALIVE ELLIS_ALIVE NICK_ALIVE ROCHELLE_ALIVE,
	CHANCE 5%
DO
	COACH SAYS coach_coaches_ally_dying ""COACH: Come on now, put it behind you, you good, you good."" 
		OR ""COACH: Come on now, put it all out there.""
		OR ""COACH: That's it, stay focused.""
		OR ""COACH: Keep it up, com on, keep it up. Kkep itup. You're gonna make it.""
		OR ""COACH: They put a hurtin on ya but ain't no thing.""
		DURATION 3
.
";
			TextTable strings = new TextTable();
			World world = new World(new Clock(DateTime.Now, 1440), strings);
			RuleEngine eng = new RuleEngine(world, strings, new RndMin(), barkScript);

			world.Flags.IsCharacter01Alive = true;
			world.Flags.IsCharacter02Alive = true;
			world.Flags.IsCharacter03Alive = true;
			world.Flags.IsCharacter04Alive = true;

			world.CreateActor("human", "COACH");
			world.CreateActor("human", "ELLIS");
			world.CreateActor("human", "NICK");
			world.CreateActor("human", "ROCHELLE");

			TextTableToken lastDoSayId = default(TextTableToken);
			string lastDoSayText = String.Empty;

			eng.DoSay.Listeners += (id, defaulText) => { lastDoSayId = id; lastDoSayText = defaulText; };

			var sentence = new ThematicEvent
			{
				Action = ThematicEvent.ActionIdle
			};
			Assert.False(eng.TryMatch(sentence));

			world.GetActor("NICK").Flags.AlertHealth = true;

			Assert.True(eng.TryMatch(sentence));
			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("ally_dying"));

			world.Clock.Update(5f);
			eng.Update();

			Assert.That(strings.GetString(lastDoSayId), Is.EqualTo("coach_coaches_nick_dying"));
		}
	}
}
