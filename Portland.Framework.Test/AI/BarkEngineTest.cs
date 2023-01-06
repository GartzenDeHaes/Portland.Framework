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
		class RndMax : IRandom
		{
			public int Next()
			{
				return Int32.MaxValue;
			}

			public int Next(int maxExclusive)
			{
				return maxExclusive - 1;
			}

			public bool NextBool()
			{
				return true;
			}

			public byte NextByte()
			{
				return 255;
			}

			public byte[] NextBytes(int count)
			{
				throw new NotImplementedException();
			}

			public double NextDouble()
			{
				return Double.MaxValue;
			}

			public double NextDouble(double maxInclusive)
			{
				return maxInclusive;
			}

			public double NextDouble(double minInclusive, double maxInclusive)
			{
				return maxInclusive;
			}

			public float NextFloat()
			{
				return Single.MaxValue;
			}

			public float NextFloat(float maxInclusive)
			{
				return maxInclusive;
			}

			public uint NextUInt()
			{
				return UInt32.MaxValue;
			}

			public uint NextUInt(uint maxExclusive)
			{
				return maxExclusive - 1;
			}

			public float Range(float minInclusive, float maxInclusive)
			{
				return maxInclusive;
			}

			public int Range(int minInclusive, int maxExclusive)
			{
				return maxExclusive - 1;
			}

			public uint Range(uint minInclusive, uint maxExclusive)
			{
				return maxExclusive - 1;
			}
		}

		[Test]
		public void WhenConceptIsBarrel_DoSayBarrel()
		{
			const string DemoRuleText_BasicRule = @"
WHEN ACTION IS SEE, OBJECT IS barrel 
DO SAY thats_a_barrel ""COACH: That's a barrel"".";

			World world = new World();
			world.Clock = new Clock(DateTime.Now, 1440);
			TextTable strings = new TextTable();
			RuleEngine eng = new RuleEngine(world, strings, new RndMax(), DemoRuleText_BasicRule);

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

		const string BarkScript = @"
ALIAS FLAG CHARACTER_01_ALIVE AS BILL_ALIVE.
ALIAS FLAG CHARACTER_02_ALIVE AS FRANCIS_ALIVE.
ALIAS FLAG CHARACTER_03_ALIVE AS LOUIS_ALIVE.
ALIAS FLAG CHARACTER_01_ALIVE AS ZOEY_ALIVE.

WHEN 
	OBJECT IS scene_01:start,
	OBSERVER IS LOUIS
DO
	SAY the_apc_is_dead ""Louis: Guys, I think the APC is toast."" DURATION 3
.
WHEN
	ACTION IS SAY,
	OBJECT IS the_apc_is_dead,
	AGENT IS LOUIS,
	OBSERVER IS BILL,
	FLAG BILL_ALIVE
DO 
	SAY cp_quest_statement ""Bill: Get it together people. The CEDA command post is on the top floor, maybe we can find out where they went."" DURATION 5
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
	OBSERVER IS ZOEY,
	FLAG ZOEY_ALIVE,
	ZOEY.ceda_trailers_seen == 1
DO
	SAY couldnt_hold_out ""Zoey: Guess they couldn't hold out."" DURATION 3
.
WHEN
	ACTION IS SEE,
	OBJECT IS hotel_lobby,
	OBSERVER IS FRANCIS,
	FLAG FRANCIS_ALIVE
DO
	SAY i_hate_hotels ""Francis: I hate hotels"" DURATION 3
.
WHEN
	ACTION IS SAY,
	OBJECT IS i_hate_hotels,
	OBSERVER IS ZOEY,
	FLAGS ARE ZOEY_ALIVE FRANCIS_ALIVE
DO
	SAY what_dont_you_hate ""Zoey: What don't you hate?"" DURATION 3
.
WHEN
	ACTION IS SAY,
	OBJECT IS what_dont_you_hate,
	OBSERVER IS FRANCIS,
	FLAGS ARE ZOEY_ALIVE FRANCIS_ALIVE
DO
	SAY there_is_one_thing ""Francis: Well, there is one thing."" DURATION 3
.

";

		[Test]
		public void I_Hate_Hotels_Test()
		{
			World world = new World();
			world.Clock = new Clock(DateTime.Now, 1440);
			TextTable strings = new TextTable();
			RuleEngine eng = new RuleEngine(world, strings, new RndMax(), BarkScript);

			world.Flags.Daylight = true;
			world.Flags.IsCharacter01Alive = true;
			world.Flags.IsCharacter02Alive = true;
			world.Flags.IsCharacter03Alive = true;
			world.Flags.IsCharacter04Alive = true;

			world.Actors.Add(strings.Get("BILL"), new Actor
			{
				Class = strings.Get("human"),
				Name = strings.Get("BILL")
			});
			world.Actors.Add(strings.Get("FRANCIS"), new Actor
			{
				Class = strings.Get("human"),
				Name = strings.Get("FRANCIS")
			});
			world.Actors.Add(strings.Get("LOUIS"), new Actor
			{
				Class = strings.Get("human"),
				Name = strings.Get("LOUIS")
			});
			world.Actors.Add(strings.Get("ZOEY"), new Actor { 
				Class = strings.Get("human"), 
				Name = strings.Get("ZOEY")
			});

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
			Assert.That(world.Actors[strings.Get("ZOEY")].Facts[strings.Get("ceda_trailers_seen")].ToInt(), Is.EqualTo(1));

			world.Clock.Update(5f);
			eng.Update();
			Assert.That(eng.DelayingCount, Is.Zero);

			Assert.True(eng.TryMatch(sentence));
			Assert.That(eng.DelayingCount, Is.EqualTo(1));
			Assert.That(world.Actors[strings.Get("ZOEY")].Facts[strings.Get("ceda_trailers_seen")].ToInt(), Is.EqualTo(1));
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
	}
}
