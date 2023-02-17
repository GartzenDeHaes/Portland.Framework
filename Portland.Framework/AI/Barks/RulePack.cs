using System;
using System.Data;
using System.Linq;

using Portland.Collections;

namespace Portland.AI.Barks
{
	public class RulePack
	{
		public BarkRule[] Rules;

		public RulePack()
		{
			Rules = Array.Empty<BarkRule>();
		}

		public void DisableRule(string actorId, AsciiId4 action, string directObject)
		{
			BarkRule rule = null;

			for (int x = 0; x < Rules.Length; x++)
			{
				rule = Rules[x];
				if (!rule.HasRun && rule.ActorName == actorId && rule.Action == action && rule.ObjectName == directObject)
				{
					rule.HasRun = true;
				}
			}
		}

		public void Parse(string ruleText)
		{
			BarkSerializer parser = new BarkSerializer();
			var rulelist = parser.Deserialize(ruleText);
			rulelist.AddRange(Rules);

			Rules = rulelist.OrderByDescending(r => r.Priority).ToArray();
		}

		public BarkRule.RuleWhenBuilder CreateRule(string ruleKey)
		{
			var rule = new BarkRule { RuleKey = ruleKey };
			var rules = Rules.ToList();
			rules.Add(rule);
			Rules = rules.ToArray();

			return new BarkRule.RuleWhenBuilder() { Rule = rule };
		}

		public void CreationOfRulesComplete()
		{
			Rules = Rules.OrderByDescending(r => r.Priority).ToArray();
		}
	}
}
