﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.AI.Utility;

namespace Portland.AI.Semantics
{
	public class SemanticAgent
    {
        public SemanticTag AgentTag;
        public string Faction;
        public AgentState State = new AgentState();
        private ObserverMatchRuleSet _barkRules;
        public UtilitySet Utility;
        public string Frame;
        public string Goal;
        public string UtilityObjective;

        public SemanticAgent()
        {
            //Utility.CurrentObjective.AddListener(OnUtilityObjectiveChanged);
        }

        public SemanticAgent(string cls, string name)
        {
            AgentTag = new SemanticTag(cls, name);
            _barkRules = new ObserverMatchRuleSet(name);
        }

        public void AddBarkRules(ObserverMatchRuleSet rules)
        {
            _barkRules.Rules.AddRange(rules.Rules);
        }

        private void OnUtilityObjectiveChanged(string objective)
        {
            State.UtilityObjective = objective;
        }

        public bool SelectRule(WorldState wstate, SemanticEvent evnt, out EventMatchRule rule)
        {
            rule = _barkRules.Match(wstate, this, evnt);
            return rule != null;
        }
    }
}
