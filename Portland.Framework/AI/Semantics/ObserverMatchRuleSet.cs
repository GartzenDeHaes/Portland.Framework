using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.AI.Semantics
{
    public class ObserverMatchRuleSet
	{
		public string Observer;
		public List<EventMatchRule> Rules = new List<EventMatchRule>();

		public ObserverMatchRuleSet(string name)
		{
			Observer = name;
		}

		public void Add(EventMatchRule rule)
		{
			Rules.Add(rule);
		}

		public void Add(ObserverMatchRuleSet rules)
		{
			Rules.AddRange(rules.Rules);
		}

		public EventMatchRule Match(WorldState wstate, Agent agent, SemanticEvent agentDid)
		{
			EventMatchRule rule;
			EventMatchRule ret = null;
			int score = 0;

			for (int x = 0; x < Rules.Count; x++)
			{
				rule = Rules[x];
				if (rule.Eligible(wstate.Flags, agent.State.Flags))
				{
					if (agentDid.Act == rule.Act)
					{
						var rscore = rule.MatchScore(agentDid);
						if (rscore > score || score == 0)
						{
							ret = rule;
							score = rscore;
						}
					}
				}
			}
			return ret;
		}
	}
}
