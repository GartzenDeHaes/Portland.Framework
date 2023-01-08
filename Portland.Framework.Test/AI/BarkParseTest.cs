using System;

using NUnit.Framework;

using Portland.Text;

namespace Portland.AI.Barks
{
	[TestFixture]
	internal class BarkParseTest
	{
		[Test]
		public void ParseEmptyString_Returns_A_Rule()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(String.Empty);

			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(0));
		}

		[Test]
		public void Parse_When_Do_Period()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE one WHEN DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
		}

		//[Test]
		//public void Parse_Location_Star_Only()
		//{
		//	var parser = new BarkSerializer();
		//	var rules = parser.Deserialize("WHEN LOCATION is * DO .");
		//	Assert.That(rules, Is.Not.Null);
		//	Assert.That(rules.Count, Is.EqualTo(1));
		//	Assert.That(rules[0].Location.Index, Is.Not.EqualTo(0));
		//	Assert.That(parser.Strings.GetString(rules[0].Location), Is.EqualTo("*"));
		//}

		[Test]
		public void Parse_Verb_Object_Only()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE see:barrel WHEN ACTION IS SEE, OBJECT IS barrel DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].Action.ToString(), Is.EqualTo("SEE"));
			Assert.That(parser.Strings.GetString(rules[0].ObjectName), Is.EqualTo("barrel"));
		}

		//[Test]
		//public void Parse_Actor_Is_Only()
		//{
		//	var parser = new BarkSerializer();
		//	var rules = parser.Deserialize("WHEN ACTOR IS COACH DO .");
		//	Assert.That(rules, Is.Not.Null);
		//	Assert.That(rules.Count, Is.EqualTo(1));
		//	Assert.That(rules[0].ActorName.Index, Is.Not.EqualTo(0));
		//	Assert.That(parser.Strings.GetString(rules[0].ActorName), Is.EqualTo("COACH"));
		//}

		//[Test]
		//public void Parse_Actor_Is_Star_Only()
		//{
		//	var parser = new BarkSerializer();
		//	var rules = parser.Deserialize("WHEN ACTOR IS * DO .");
		//	Assert.That(rules, Is.Not.Null);
		//	Assert.That(rules.Count, Is.EqualTo(1));
		//	Assert.That(rules[0].ActorName.Index, Is.Not.EqualTo(0));
		//	Assert.That(parser.Strings.GetString(rules[0].ActorName), Is.EqualTo("*"));
		//}

		[Test]
		public void Parse_Flag_Only()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE see:barrel_a WHEN FLAG IS DAYLIGHT DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].WorldFlagsSet.Daylight, Is.True);
			Assert.That(rules[0].WorldFlagsSet.Bits.NumberOfBitsSet(), Is.EqualTo(1));
			Assert.That(rules[0].WorldFlagsClear.Bits.NumberOfBitsSet(), Is.Zero);
		}

		[Test]
		public void Parse_Not_Flag_ParseExcpetion()
		{
			var parser = new BarkSerializer();
			Assert.Catch<ParseException>(() => parser.Deserialize("WHEN !FLAG IS DAYLIGHT DO ."));
		}

		[Test]
		public void Parse_Not_Flag_Only()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN FLAG IS !DAYLIGHT DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].WorldFlagsSet.Daylight, Is.False);
			Assert.That(rules[0].WorldFlagsSet.Bits.NumberOfBitsSet(), Is.Zero);

			Assert.That(rules[0].WorldFlagsClear.Daylight, Is.True);
			Assert.That(rules[0].WorldFlagsClear.Bits.NumberOfBitsSet(), Is.EqualTo(1));
		}

		[Test]
		public void Parse_Char_Dot_Flag()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN FLAG IS COACH.ALLY_NEAR DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].ActorFlags.Count, Is.EqualTo(1));
			
			var filter = rules[0].ActorFlags[0];
			Assert.That(parser.Strings.GetString(filter.ActorName), Is.EqualTo("COACH"));
			Assert.That(parser.Strings.GetString(filter.FlagName), Is.EqualTo("ALLY_NEAR"));
			Assert.False(filter.Not);
		}

		[Test]
		public void Parse_Char_Dot_Not_Flag()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN FLAG IS !COACH.ALLY_NEAR DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].ActorFlags.Count, Is.EqualTo(1));

			var filter = rules[0].ActorFlags[0];
			Assert.That(parser.Strings.GetString(filter.ActorName), Is.EqualTo("COACH"));
			Assert.That(parser.Strings.GetString(filter.FlagName), Is.EqualTo("ALLY_NEAR"));
			Assert.True(filter.Not);
		}

		[Test]
		public void Parse_Flags_Are()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN FLAGS ARE COACH.ALLY_NEAR DAYLIGHT DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].ActorFlags.Count, Is.EqualTo(1));

			var filter = rules[0].ActorFlags[0];
			Assert.That(parser.Strings.GetString(filter.ActorName), Is.EqualTo("COACH"));
			Assert.That(parser.Strings.GetString(filter.FlagName), Is.EqualTo("ALLY_NEAR"));
			Assert.False(filter.Not);

			Assert.That(rules[0].WorldFlagsSet.Daylight, Is.True);
			Assert.That(rules[0].WorldFlagsSet.Bits.NumberOfBitsSet(), Is.EqualTo(1));
			Assert.That(rules[0].WorldFlagsClear.Bits.NumberOfBitsSet(), Is.Zero);
		}

		[Test]
		public void Parse_Flags_Alias()
		{
			const string ruleText = @"ALIAS FLAG DAYLIGHT AS DLT.
ALIAS FLAG ALLY_NEAR AS HAS_FRIENDS.
RULE test WHEN FLAGS ARE COACH.HAS_FRIENDS DLT DO .";

			var parser = new BarkSerializer();
			var rules = parser.Deserialize(ruleText);
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].ActorFlags.Count, Is.EqualTo(1));

			var filter = rules[0].ActorFlags[0];
			Assert.That(parser.Strings.GetString(filter.ActorName), Is.EqualTo("COACH"));
			Assert.That(parser.Strings.GetString(filter.FlagName), Is.EqualTo("ALLY_NEAR"));
			Assert.False(filter.Not);

			Assert.That(rules[0].WorldFlagsSet.Daylight, Is.True);
			Assert.That(rules[0].WorldFlagsSet.Bits.NumberOfBitsSet(), Is.EqualTo(1));
			Assert.That(rules[0].WorldFlagsClear.Bits.NumberOfBitsSet(), Is.Zero);
		}

		[Test]
		public void Parse_Barrels_Is_Unset()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN TEST barrels IS UNSET DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].WorldFilters.Count, Is.EqualTo(1));

			var expr = rules[0].WorldFilters[0];
			Assert.That(parser.Strings.GetString(expr.FactName), Is.EqualTo("barrels"));
			Assert.That(expr.Op, Is.EqualTo(ComparisionOp.NotExists));
		}

		[Test]
		public void Parse_Barrels_EqEq_Int()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN TEST barrels == 1 DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].WorldFilters.Count, Is.EqualTo(1));

			var expr = rules[0].WorldFilters[0];
			Assert.That(parser.Strings.GetString(expr.FactName), Is.EqualTo("barrels"));
			Assert.That(expr.Op, Is.EqualTo(ComparisionOp.Equals));
			Assert.That(expr.Value.ToInt(), Is.EqualTo(1));
		}

		[Test]
		public void Parse_Barrels_NotEq_Int()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN TEST barrels != 1 DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].WorldFilters.Count, Is.EqualTo(1));

			var expr = rules[0].WorldFilters[0];
			Assert.That(parser.Strings.GetString(expr.FactName), Is.EqualTo("barrels"));
			Assert.That(expr.Op, Is.EqualTo(ComparisionOp.NotEquals));
			Assert.That(expr.Value.ToInt(), Is.EqualTo(1));
		}

		[Test]
		public void Parse_Barrels_Lt_Float()
		{
			var parser = new BarkSerializer();
			var rules = parser.Deserialize("RULE test WHEN TEST barrels < 1.0 DO .");
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));
			Assert.That(rules[0].WorldFilters.Count, Is.EqualTo(1));

			var expr = rules[0].WorldFilters[0];
			Assert.That(parser.Strings.GetString(expr.FactName), Is.EqualTo("barrels"));
			Assert.That(expr.Op, Is.EqualTo(ComparisionOp.LessThan));
			Assert.That(expr.Value.ToFloat(), Is.EqualTo(1.0f));
		}

		[Test]
		public void Parse_When_OneOfEachCase()
		{
			string rule_text = @"RULE testing WHEN
	ACTION IS SEE,
	OBJECT IS hello,
	FLAGS ARE DAYLIGHT,
	TEST helicopter IS UNSET,
	CHANCE 50%
DO .
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);
			Assert.That(rules, Is.Not.Null);
			Assert.That(rules.Count, Is.EqualTo(1));

			Assert.That(rules[0].Probability, Is.EqualTo(0.5f));
			Assert.That(rules[0].Action.ToString(), Is.EqualTo("SEE"));
			Assert.That(parser.Strings.GetString(rules[0].ObjectName), Is.EqualTo("hello"));

			Assert.That(rules[0].WorldFlagsSet.Daylight, Is.True);
			Assert.That(rules[0].WorldFlagsSet.Bits.NumberOfBitsSet(), Is.EqualTo(1));
			Assert.That(rules[0].WorldFlagsClear.Bits.NumberOfBitsSet(), Is.Zero);

			var expr = rules[0].WorldFilters[0];
			Assert.That(parser.Strings.GetString(expr.FactName), Is.EqualTo("helicopter"));
			Assert.That(expr.Op, Is.EqualTo(ComparisionOp.NotExists));

		}

		[Test]
		public void Parse_Do_Say()
		{
			string rule_text = @"RULE testing WHEN
	ACTION IS hello,
	FLAGS ARE DAYLIGHT,
	TEST helicopter IS UNSET
DO FRANCIS SAYS francis_hates_hotels ""I hate hotels"".
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSay, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_hotels"));

			Assert.NotNull(cmd.DefaultTexts);
			Assert.NotZero(cmd.DefaultTexts.Count);

			var arg2 = cmd.DefaultTexts[0];
			Assert.That(arg2, Is.EqualTo("I hate hotels"));
		}

		[Test]
		public void Parse_Do_Say_Duration()
		{
			string rule_text = @"RULE testing WHEN
	ACTION IS hello,
	FLAGS ARE DAYLIGHT,
	TEST helicopter IS UNSET
DO FRANCIS SAYS francis_hates_hotels ""I hate hotels"" DURATION 10.0.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSay, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_hotels"));

			var arg2 = cmd.DefaultTexts[0];
			Assert.That(arg2, Is.EqualTo("I hate hotels"));
			Assert.That(cmd.Duration, Is.EqualTo(10f));
		}

		[Test]
		public void Parse_Do_Say_Duration_Delay()
		{
			string rule_text = @"RULE testing WHEN
	ACTION IS hello,
	FLAGS ARE DAYLIGHT,
	TEST helicopter IS UNSET
DO FRANCIS SAYS francis_hates_hotels ""I hate hotels"" DURATION 10.0 DELAY 30.0.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSay, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_hotels"));

			var arg2 = cmd.DefaultTexts[0];
			Assert.That(arg2, Is.EqualTo("I hate hotels"));
			Assert.That(cmd.Duration, Is.EqualTo(10f));
			Assert.That(cmd.DelayTime, Is.EqualTo(30f));
		}

		[Test]
		public void Parse_Do_Say_Delay()
		{
			string rule_text = @"RULE testing WHEN
	ACTION IS hello,
	FLAGS ARE DAYLIGHT,
	TEST helicopter IS UNSET
DO FRANCIS SAYS francis_hates_hotels ""I hate hotels"" DELAY 30.0.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSay, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_hotels"));

			var arg2 = cmd.DefaultTexts[0];
			Assert.That(arg2, Is.EqualTo("I hate hotels"));
			Assert.That(cmd.Duration, Is.EqualTo(5f));
			Assert.That(cmd.DelayTime, Is.EqualTo(30f));
		}

		[Test]
		public void Parse_Do_Say_Wait()
		{
			string rule_text = @"RULE testing WHEN
	ACTION IS hello,
	FLAGS ARE DAYLIGHT,
	TEST helicopter IS UNSET
DO FRANCIS SAYS francis_hates_hotels ""I hate hotels"" DELAY 22.0 .
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSay, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_hotels"));

			var arg2 = cmd.DefaultTexts[0];
			Assert.That(arg2, Is.EqualTo("I hate hotels"));
			Assert.That(cmd.DelayTime, Is.EqualTo(22f));
		}

		[Test]
		public void Parse_Do_Set()
		{
			string rule_text = @"
RULE testing WHEN
	ACTION IS hello,
DO 
	SET francis_hates_things TO 1
.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSetVar, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_things"));
			var arg2 = cmd.Arg2.ToString();
			Assert.That(arg2, Is.EqualTo("1"));
		}

		[Test]
		public void Parse_Do_Add()
		{
			string rule_text = @"
RULE testing WHEN
	ACTION IS hello
DO 
	ADD 1 TO francis_hates_things
.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameAdd, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_things"));
			var arg2 = cmd.Arg2.ToString();
			Assert.That(arg2, Is.EqualTo("1"));
		}

		[Test]
		public void Parse_Do_Set_Actor()
		{
			string rule_text = @"
RULE testing WHEN
	ACTION IS hello,
DO 
	SET intro TO ""false"", 
	SET FRANCES.hates TO 1
.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSetVar, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("intro"));
			var arg2 = cmd.Arg2.ToString();
			Assert.That(arg2, Is.EqualTo("false"));

			cmd = rules[0].Response[1];
			Assert.AreEqual(BarkCommand.CommandNameSetVar, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.ActorName), Is.EqualTo("FRANCES"));
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("hates"));
			arg2 = cmd.Arg2.ToString();
			Assert.That(arg2, Is.EqualTo("1"));
		}

		[Test]
		public void Parse_Do_Wait()
		{
			string rule_text = @"
RULE testing WHEN
	ACTION IS hello
DO 
	RESET DELAY 8.2
.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameResetRule, cmd.CommandName);
			Assert.That(cmd.DelayTime, Is.EqualTo(8.2f));
		}

		[Test]
		public void Parse_Do_Raise()
		{
			string rule_text = @"
RULE testing WHEN
	ACTION IS hello
DO 
	RAISE coach:i_saw_a_barrel
.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameRaise, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("coach:i_saw_a_barrel"));
		}

		[Test]
		public void Parse_Do_Reset()
		{
			string rule_text = @"
RULE testing WHEN
	ACTION IS hello
DO 
	RESET DELAY 20.2
.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameResetRule, cmd.CommandName);
			Assert.That(cmd.DelayTime.ToString(), Is.EqualTo("20.2"));
		}

		[Test]
		public void Parse_WhenDo()
		{
			string rule_text = @"RULE testing WHEN
	ACTION IS hello,
	FLAGS ARE DAYLIGHT,
	TEST helicopter IS UNSET
DO 
	FRANCIS SAYS francis_hates_hotels ""I hate hotels"",
	SET francis_hates TO 1,
	RESET DELAY 300
.
";
			var parser = new BarkSerializer();
			var rules = parser.Deserialize(rule_text);

			var cmd = rules[0].Response[0];
			Assert.AreEqual(BarkCommand.CommandNameSay, cmd.CommandName);
			Assert.That(parser.Strings.GetString(cmd.Arg1), Is.EqualTo("francis_hates_hotels"));
			var arg2 = cmd.DefaultTexts[0];
			Assert.That(arg2, Is.EqualTo("I hate hotels"));
		}
	}
}
