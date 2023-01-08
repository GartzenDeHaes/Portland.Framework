
using Portland.AI.NLP;
using Portland.AI.Semantics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.AI
{
    [TestFixture]
	public class EventMatchRuleTests
	{
		[Test]
		public void EligibleTest()
		{
			WorldStateFlags wflags = new WorldStateFlags();
			AgentStateFlags aflags = new AgentStateFlags();

			EventMatchRule rule = new EventMatchRule();

			Assert.IsTrue(rule.Eligible(wflags, aflags));

			wflags.Daylight = true;
			Assert.IsTrue(rule.Eligible(wflags, aflags));

			rule.WorldStateNope.Daylight = true;
			Assert.IsFalse(rule.Eligible(wflags, aflags));

			wflags.Daylight = false;
			rule.WorldStateNope.Daylight = false;
			rule.WorldStateRequired.Daylight = true;
			Assert.IsFalse(rule.Eligible(wflags, aflags));

			wflags.Daylight = true;
			Assert.IsTrue(rule.Eligible(wflags, aflags));

			rule.RemainingTimesToMatch = 0;
			Assert.IsFalse(rule.Eligible(wflags, aflags));
		}

		[Test]
		public void MatchScoreTest()
		{
			EventMatchRule rule = new EventMatchRule();
			SemanticEvent ev = new SemanticEvent();

			Assert.AreEqual(0, rule.MatchScore(ev));

			ev.Act = "see";
			Assert.AreEqual(-1, rule.MatchScore(ev));
			rule.Act = "see";
			Assert.AreEqual(1, rule.MatchScore(ev));
		}

		[Test]
		public void MatchRuleSetTest()
		{
			EventMatchRuleSet rset = new EventMatchRuleSet();
			rset.LoadCsv(SemanticEventTest.TestCSV);

			SemanticAgent ellis = new SemanticAgent("human", "ellis");
			var barkRules = rset.GetRulesForObserver(ellis.AgentTag.Name);
			Assert.AreEqual(2, barkRules.Rules.Count);
			ellis.AddBarkRules(barkRules);

			SemanticAgent rochelle = new SemanticAgent("human", "rochelle");
			barkRules = rset.GetRulesForObserver(rochelle.AgentTag.Name);
			Assert.AreEqual(3, barkRules.Rules.Count);
			rochelle.AddBarkRules(barkRules);

			SemanticAgent coach = new SemanticAgent("human", "coach");
			barkRules = rset.GetRulesForObserver(coach.AgentTag.Name);
			Assert.AreEqual(1, barkRules.Rules.Count);
			coach.AddBarkRules(barkRules);

			SemanticAgent nick = new SemanticAgent("human", "nick");
			barkRules = rset.GetRulesForObserver(nick.AgentTag.Name);
			Assert.AreEqual(1, barkRules.Rules.Count);
			nick.AddBarkRules(barkRules);

			TemplateMatcher inEvPrs = new TemplateMatcher();
			inEvPrs.LoadXml(TemplateMatchTests.MatchXml);

			WorldState wstate = new WorldState();

			inEvPrs.Match("Say hello to Ellis", out var evnt);
			Assert.AreEqual("say", evnt.Act);
			Assert.AreEqual("ellis", evnt.IoName);
			Assert.AreEqual("hello", evnt.DoName);

			Assert.IsTrue(ellis.SelectRule(wstate, evnt, out var mrule));
			Assert.AreEqual("ellis:hello", mrule.RuleTag);
			Assert.AreEqual("say", mrule.Act);
			Assert.IsTrue(mrule.ActionSpecLine.StartsWith("SAY, Hey, I know where"));
			Assert.AreEqual(1, mrule.RemainingTimesToMatch);

			Portland.Text.StringPart pt = new Text.StringPart("DEBUG");
			int h1 = pt.GetHashCode();
			pt = new Text.StringPart("STOP");
			int h2 = pt.GetHashCode();

		}
	}
}
