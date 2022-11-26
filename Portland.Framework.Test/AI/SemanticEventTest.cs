using System;

using Portland.AI.Semantics;
using Portland.Text;

using NUnit.Framework;

namespace Portland.AI
{
	public class SemanticEventTest
	{
		public const string TestCSV = "#\n" +
"# TAG					|	FLAGS		| OBSERVER	|	AGENT		|	FRAME	|	GOAL	|	ACT	|	DIRECT OBJECT		|	WITH	|	LOCATION	| DIALOG		| COND_JS	| RESULT_JS		\n" +
"ellis:hello			,				, ellis		,				,			,			, say		, hello					, ellis	, roof		, \"SAY, Hey, I know where there's a gunshop where we can pickup some guns.\",\n" +
"nick:ellis:hello		,				, nick		, 				,			,			, say		, ellis:hello			,			,				, \"SAY, I guess being a hillbilly is finally starting to payoff.\",\n" +
"rochelle:hello		,				, rochelle	,				,			,			, hello	,							,			, roof		, \"SAY, I'm in the news and the news said to take a weapon.\",\n" +
"rochelle:take:axe	,				, rochelle	, rochelle	,			,			, take	, axe						,			,				, \"SAY, Axe me a question, I dare you.\",\n" +
"ellis:rochelle:axe	,				, ellis		, rochelle	,			,			, say		, rochelle:take:axe	,			,				, \"SAY, Nope, not me.\",\n" +
"rochelle:axe:zombie	,				, rochelle	, rochelle	,			,			, hit		, zombie					, axe		,				, \"SAY, I warned you.\",\n" +
"coach:reload			, repeat		, coach		, coach		,			,			, reload	,							,			,				, \"SAY, Reloading.\",\n";

		[Test]
		public void CsvParseTest()
		{
			CsvParser csv = new CsvParser(TestCSV);
			var rule1 = new EventMatchRule();
			Assert.IsTrue(rule1.Parse(csv));
			Assert.IsTrue(csv.NextRow());
			var rule2 = new EventMatchRule();
			Assert.IsTrue(rule2.Parse(csv));
			Assert.IsTrue(csv.NextRow());
			var rule3 = new EventMatchRule();
			Assert.IsTrue(rule3.Parse(csv));
			Assert.IsTrue(csv.NextRow());
			var rule4 = new EventMatchRule();
			Assert.IsTrue(rule4.Parse(csv));
			Assert.IsTrue(csv.NextRow());
			var rule5 = new EventMatchRule();
			Assert.IsTrue(rule5.Parse(csv));
			Assert.IsTrue(csv.NextRow());
			var rule6 = new EventMatchRule();
			Assert.IsTrue(rule6.Parse(csv));
			Assert.IsTrue(csv.NextRow());
			var rule7 = new EventMatchRule();
			Assert.IsTrue(rule7.Parse(csv));
			Assert.IsFalse(csv.NextRow());

			Assert.IsTrue(csv.IsEOF);

			rule1.Init();
			rule2.Init();
			rule3.Init();
			rule4.Init();
			rule5.Init();
			rule6.Init();
			rule7.Init();

			Assert.AreEqual(3, rule1.SpecificityScore);
			Assert.AreEqual(1, rule2.SpecificityScore);
			Assert.AreEqual(1, rule3.SpecificityScore);
			Assert.AreEqual(2, rule4.SpecificityScore);
			Assert.AreEqual(2, rule5.SpecificityScore);
			Assert.AreEqual(3, rule6.SpecificityScore);
			Assert.AreEqual(1, rule7.SpecificityScore);

			WorldStateFlags wflags = new WorldStateFlags();
			AgentStateFlags aflags = new AgentStateFlags();
			Assert.IsTrue(rule1.Eligible(wflags, aflags));
			Assert.IsTrue(rule2.Eligible(wflags, aflags));
			Assert.IsTrue(rule3.Eligible(wflags, aflags));
			Assert.IsTrue(rule4.Eligible(wflags, aflags));
			Assert.IsTrue(rule5.Eligible(wflags, aflags));
			Assert.IsTrue(rule6.Eligible(wflags, aflags));
			Assert.IsTrue(rule7.Eligible(wflags, aflags));

			wflags.Daylight = true;
			aflags.Bits.RawBits = 0xF1a2234; // junk
			Assert.IsTrue(rule1.Eligible(wflags, aflags));
			Assert.IsTrue(rule2.Eligible(wflags, aflags));
			Assert.IsTrue(rule3.Eligible(wflags, aflags));
			Assert.IsTrue(rule4.Eligible(wflags, aflags));
			Assert.IsTrue(rule5.Eligible(wflags, aflags));
			Assert.IsTrue(rule6.Eligible(wflags, aflags));
			Assert.IsTrue(rule7.Eligible(wflags, aflags));

			rule1.WorldStateNope.SetByName("DAYLIGHT", true);
			Assert.IsFalse(rule1.Eligible(wflags, aflags));
		}
	}
}
