using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.AI.Semantics
{
	public class EventMatchRuleSet
	{
		private List<EventMatchRule> _rules = new List<EventMatchRule>();

		public EventMatchRuleSet()
		{
		}

		public ObserverMatchRuleSet GetRulesForObserver(string name)
		{
			ObserverMatchRuleSet agent = new ObserverMatchRuleSet(name);
			EventMatchRule rule;

			for (int x = 0; x < _rules.Count; x++)
			{
				rule = _rules[x];
				if (StringHelper.Like(rule.Observer, name))
				{
					agent.Add(rule);
				}
			}

			return agent;
		}

		public void LoadCsv(string csvtxt)
		{
			CsvParser csv = new CsvParser(csvtxt);

			while (!csv.IsEOF)
			{
				var rule = new EventMatchRule();
				if (rule.Parse(csv))
				{
					rule.Init();

					_rules.Add(rule);
				}
				
				csv.NextRow();
			}

			_rules.Sort((r1, r2) => { return r2.SpecificityScore - r1.SpecificityScore; });
		}
	}
}
