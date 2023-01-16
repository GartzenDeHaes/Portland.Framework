using System;
using System.Collections.Generic;

using Portland.Collections;
using Portland.Text;

namespace Portland.AI.Barks
{
	public sealed class BarkRule
	{
		public bool HasRun;

		public TextTableToken ActorName;
		public AsciiId4 Action;
		public TextTableToken ObjectName;
		public TextTableToken InstrumentName;

		public TextTableToken ObserverName;

		public WorldStateFlags WorldFlagsSet;
		public WorldStateFlags WorldFlagsClear;
		//public AgentStateFlags ActorFlagsSet;
		//public AgentStateFlags ActorFlagsClear;
		public List<FactFilter> WorldFilters = new List<FactFilter>();

		//public TextTableToken Location;
		public List<AgentStateFilter> ActorFlags = new List<AgentStateFilter>();
		public List<FactFilter> ActorFilters = new List<FactFilter>();

		/// <summary></summary>
		public float Probability = 1f;
		public bool NoRetryIfProbablityFails;

		public List<BarkCommand> Response = new List<BarkCommand>();

		public int Priority;

		public string RuleKey;

		public BarkRule()
		{
		}

		public void RecalcPriority()
		{
			Priority = WorldFlagsSet.Bits.NumberOfBitsSet() +
				WorldFlagsClear.Bits.NumberOfBitsSet() +
				//ActorFlagsSet.Bits.NumberOfBitsSet() +
				//ActorFlagsClear.Bits.NumberOfBitsSet() +
				(ActorName.Index > 0 ? 1 : 0) +
				(Action.Length > 0 ? 1 : 0) +
				(ObjectName.Index > 0 ? 1 : 0) +
				(InstrumentName.Index > 0 ? 1 : 0) +
				ActorFlags.Count +
				WorldFilters.Count +
				ActorFilters.Count +
				(Probability < 1f ? 1 : 0);

			//for (int i = 0; i < Response.Count; i++)
			//{
			//	Priority += Response[i].DefaultTexts?.Count ?? 0;
			//}
		}

		public struct RuleWhenBuilder
		{
			internal BarkRule Rule;
			internal TextTable Strings;

			/// <summary>
			/// The agent causing the event AGENT SAYS TEXT_KEY
			/// </summary>
			public RuleWhenBuilder WhenActorNameIs(string name)
			{
				Rule.ActorName = Strings.Get(name);
				return this;
			}

			/// <summary>
			/// The action or verb of the event (SEE, SAYS)
			/// </summary>
			public RuleWhenBuilder WhenActionIs(AsciiId4 verb)
			{
				Rule.Action = verb;
				return this;
			}

			/// <summary>
			/// The conept, object, or text key of the event (AGENT SAYS CONCEPT)
			/// </summary>
			public RuleWhenBuilder WhenConceptIs(string name)
			{
				Rule.ObjectName = Strings.Get(name);
				return this;
			}

			/// <summary>
			/// Indirect object (AGENT OPENED DOOR WITH KEY)
			/// </summary>
			public RuleWhenBuilder WhenIndirectObjectOrInstrumentIs(string name)
			{
				Rule.InstrumentName = Strings.Get(name);
				return this;
			}

			/// <summary>
			/// The responder of this rule, only required to be set when DO does not have a SAY command.
			/// </summary>
			public RuleWhenBuilder WhenObserverIs(string name)
			{
				Rule.ObserverName = Strings.Get(name);
				return this;
			}

			public RuleWhenBuilder WhenWorldFlagMustBeSetIs(string flagName)
			{
				Rule.WorldFlagsSet.SetByName(flagName, true);
				return this;
			}

			public RuleWhenBuilder WhenWorldFlagMustNotBeSetIs(string flagName)
			{
				Rule.WorldFlagsClear.SetByName(flagName, true);
				return this;
			}

			public RuleWhenBuilder WhenWorldFactCheckIs
			(
				string actorName,
				string factName,
				ComparisionOp op,
				Variant8 value
			)
			{
				Rule.WorldFilters.Add(new FactFilter { 
					ActorName = Strings.Get(actorName), 
					FactName = Strings.Get(factName), 
					Op = op, 
					Value = value
				});

				return this;
			}

			public RuleWhenBuilder WhenActorFlagMustBeSetIs(string actorName, string flagName)
			{
				Rule.ActorFlags.Add(new AgentStateFilter { ActorName = Strings.Get(actorName), Not = false, FlagName = Strings.Get(flagName) });
				return this;
			}

			public RuleWhenBuilder WhenActorFlagMustNotBeSetIs(string actorName, string flagName)
			{
				Rule.ActorFlags.Add(new AgentStateFilter { ActorName = Strings.Get(actorName), Not = true, FlagName = Strings.Get(flagName) });
				return this;
			}

			public RuleWhenBuilder WhenActorFactCheckIs(string actorName, string factName, ComparisionOp op, Variant8 value)
			{
				Rule.ActorFilters.Add(new FactFilter { ActorName = Strings.Get(actorName), FactName = Strings.Get(factName), Op = op, Value = value });
				return this;
			}

			/// <summary>
			/// If all other criteria match, do a random probability check.
			/// </summary>
			/// <param name="prob_0to1">0.0 to 1.0</param>
			/// <param name="disableRuleOnFail">Disable the rule if the probability check fails.</param>
			public RuleWhenBuilder WhenProbabilityCheckIs(float prob_0to1, bool disableRuleOnFail)
			{
				Rule.Probability = prob_0to1;
				Rule.NoRetryIfProbablityFails = disableRuleOnFail;
				return this;
			}

			/// <summary>
			/// This is a key that is passed in SAY command events.  Use for lookups into localization systems.
			/// </summary>
			public RuleWhenBuilder RuleKeyIs(string key)
			{
				Rule.RuleKey = key;
				return this;
			}

			public RuleDoBuilder Do()
			{
				return new RuleDoBuilder { Rule = Rule, Strings = Strings };
			}
		}

		public struct RuleDoBuilder
		{
			internal BarkRule Rule;
			internal TextTable Strings;

			public RuleDoBuilder Say(string speakerName, string conceptOrObject, float duration = 3f, params string[] defaultText)
			{
				var cmd = new BarkCommand {
					ActorName = Strings.Get(speakerName),
					Arg1 = Strings.Get(conceptOrObject),
					Rule = Rule,
					CommandName = BarkCommand.CommandNameSay,
					Duration = duration,
					DefaultTexts = new Vector<string>(3)
				};
				cmd.DefaultTexts.Add(defaultText);
				Rule.Response.Add(cmd);
				return this;
			}

			public RuleDoBuilder Set(string agentName, string factName, Variant8 value, float delayBeforeRunInSecs = 0f)
			{
				Rule.Response.Add(new BarkCommand { 
					CommandName = BarkCommand.CommandNameSetVar,
					Rule = Rule, 
					ActorName = Strings.Get(agentName), 
					Arg1 = Strings.Get(factName),
					Arg2 = value,
					DelayTime = delayBeforeRunInSecs
				});

				return this;
			}

			public RuleDoBuilder SetGlobal(string factName, Variant8 value, float delayBeforeRunInSecs = 0f)
			{
				Set(String.Empty, factName, value, delayBeforeRunInSecs);
				return this;
			}

			public RuleDoBuilder AddTo(string agentName, string factName, Variant8 value)
			{
				Rule.Response.Add(new BarkCommand { 
					CommandName = BarkCommand.CommandNameAdd,
					ActorName = Strings.Get(agentName),
					Arg1 = Strings.Get(factName),
					Arg2 = value,
					Rule = Rule
				});
				return this;
			}

			public RuleDoBuilder AddToGlobal(string factName, Variant8 value)
			{
				AddTo(String.Empty, factName, value);
				return this;
			}

			public RuleDoBuilder RaiseEvent(string conceptName, float delayBeforeRunInSecs = 0f)
			{
				Rule.Response.Add(new BarkCommand { CommandName = BarkCommand.CommandNameRaise, Arg1 = Strings.Get(conceptName), DelayTime = delayBeforeRunInSecs, Rule = Rule });
				return this;
			}

			/// <summary>
			/// Allow this rule to run again
			/// </summary>
			public RuleDoBuilder ReenableThisRule(float delayBeforeRunInSecs = 0f)
			{
				Rule.Response.Add(new BarkCommand { Rule = Rule, CommandName = BarkCommand.CommandNameResetRule, DelayTime = delayBeforeRunInSecs });
				return this;
			}

			public RuleDoBuilder DisableRules(string actorName, AsciiId4 verb, string concept)
			{
				// Arg2 supposed to be observer?

				Rule.Response.Add(new BarkCommand { 
					Rule = Rule,
					CommandName = BarkCommand.CommandNameDontSay,
					ActorName = Strings.Get(actorName),
					Arg2 = verb.ToInt()
				});
				return this;
			}
		}
	}
}
