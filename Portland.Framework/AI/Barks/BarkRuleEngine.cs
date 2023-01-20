using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using NLog.LayoutRenderers.Wrappers;

using Portland.CheckedEvents;
using Portland.ComponentModel;
using Portland.Mathmatics;
using Portland.Text;

namespace Portland.AI.Barks
{
	public sealed class BarkRuleEngine
	{
		readonly World _world;
		readonly TextTable _strings;
		readonly IRandom _random;
		RulePack _rules;

		//RingBuffer<BarkCommand> _runcmds = new RingBuffer<BarkCommand>(16);
		List<BarkCommand> _cmdsDelaying = new List<BarkCommand>();

		public Command<BarkCommand, BarkRule> OnSay = new Command<BarkCommand, BarkRule>();
		public ObservableValue<TextTableToken> CurrentConcept = new ObservableValue<TextTableToken>();
		public Notify<TextTableToken> OnEventRaised = new Notify<TextTableToken>();

		public void SetRules(RulePack ruleSet)
		{
			_rules = ruleSet;
			_rules.CreationOfRulesComplete();
		}

		public void Update()
		{
			BarkCommand cmd;

			for (int i = 0; i < _cmdsDelaying.Count; i++)
			{
				cmd = _cmdsDelaying[i];

				if (cmd.DelayRemainingToRun < _world.Clock.RealTime)
				{
					_cmdsDelaying.Remove(cmd);

					RunOne(cmd);
				}
			}
		}

		void RunOne(in BarkCommand cmd)
		{
			if (cmd.CommandName == BarkCommand.CommandNameSay)
			{
				OnSay.Send(cmd, cmd.Rule);

				//OnConceptChanged.Send(cmd.Arg1);

				if (cmd.Duration > 0f)
				{
					_cmdsDelaying.Add(new BarkCommand() { CommandName = BarkCommand.CommandConcept, Arg1 = cmd.Arg1, Rule = cmd.Rule, DelayRemainingToRun = _world.Clock.RealTime + cmd.Duration + cmd.DelayTime });
				}
				else
				{
					// arg1 is the object
					TryMatch(new ThematicEvent { Action = ThematicEvent.ActionSay, Concept = cmd.Arg1, Actor = cmd.Rule.ObserverName });
				}
			}
			else if (cmd.CommandName == BarkCommand.CommandNameSetVar)
			{
				if (cmd.ActorName.Index == 0)
				{
					// world
					SetVars(_world.Facts, cmd);
				}
				else
				{
					if (_world.TryGetActor(cmd.ActorName, out var actor))
					{
						SetVars(actor.Facts, cmd);
					}
					else
					{
						throw new Exception($"Actor '{_strings.GetString(cmd.ActorName)}' not found.");
					}
				}
			}
			else if (cmd.CommandName == BarkCommand.CommandNameResetRule)
			{
				cmd.Rule.HasRun = false;
			}
			else if (cmd.CommandName == BarkCommand.CommandNameRaise)
			{
				OnEventRaised.Send(cmd.Arg1);
			}
			else if (cmd.CommandName == BarkCommand.CommandConcept)
			{
				TryMatch(new ThematicEvent { Action = ThematicEvent.ActionSay, Concept = cmd.Arg1, Actor = cmd.Rule.ObserverName });
			}
			else if (cmd.CommandName == BarkCommand.CommandNameDontSay)
			{
				_rules.DisableRule(cmd.ActorName, new AsciiId4(cmd.Arg2.ToInt()), cmd.Arg1);
			}
			else if (cmd.CommandName == BarkCommand.CommandNameAdd)
			{
				if (cmd.ActorName.Index == 0)
				{
					// world
					AddToVar(_world.Facts, cmd);
				}
				else
				{
					if (_world.TryGetActor(cmd.ActorName, out var actor))
					{
						AddToVar(actor.Facts, cmd);
					}
					else
					{
						throw new Exception($"Actor '{_strings.GetString(cmd.ActorName)}' not found.");
					}
				}
			}
			else
			{
				throw new Exception($"Unknown command {cmd.CommandName}");
			}
		}

		void SetVars(Dictionary<TextTableToken, ObservableValue<Variant8>> facts, BarkCommand cmd)
		{
			if (!facts.ContainsKey(cmd.Arg1))
			{
				facts.Add(cmd.Arg1, new ObservableValue<Variant8>(cmd.Arg2));
			}
			else
			{
				facts[cmd.Arg1].Value = cmd.Arg2;
			}
		}

		void AddToVar(Dictionary<TextTableToken, ObservableValue<Variant8>> facts, BarkCommand cmd)
		{
			if (!facts.ContainsKey(cmd.Arg1))
			{
				facts.Add(cmd.Arg1, new ObservableValue<Variant8>(cmd.Arg2));
			}
			else
			{
				facts[cmd.Arg1].Value = facts[cmd.Arg1].Value + cmd.Arg2;
			}
		}

		public int DelayingCount { get { return _cmdsDelaying.Count; } }

		public BarkRuleEngine(World world, TextTable strings, IRandom rand)
		{
			_world = world;
			_strings = strings;
			_random = rand;
		}

		void Execute(BarkRule rule)
		{
			rule.HasRun = true;
			BarkCommand cmd;

			CurrentConcept.Value = rule.ObjectName;

			for (int i = 0; i < rule.Response.Count; i++)
			{
				cmd = rule.Response[i];

				if (cmd.DelayTime > 0)
				{
					cmd.DelayRemainingToRun = _world.Clock.Time + cmd.DelayTime;
					_cmdsDelaying.Add(cmd);
				}
				else
				{
					RunOne(cmd);
				}
			}
		}

		public bool TryMatch(ThematicEvent happened)
		{
			BarkRule rule = null;
			bool docont = false;

			for (int x = 0; x < _rules.Rules.Length; x++)
			{
				rule = _rules.Rules[x];

				if (rule.HasRun || rule.Action != happened.Action || rule.ObjectName != happened.Concept)
				{
					continue;
				}

				if (!_world.Flags.Bits.IsAllSet(rule.WorldFlagsSet.Bits))
				{
					continue;
				}
				if (_world.Flags.Bits.IsAnySet(rule.WorldFlagsClear.Bits))
				{
					continue;
				}

				if (rule.ActorName.Index != 0 && rule.ActorName != happened.Actor)
				{
					continue;
				}

				// TODO: Instrument

				docont = false;
				for (int i = 0; i < rule.ActorFlags.Count; i++)
				{
					var flagf = rule.ActorFlags[i];

					Agent actor;
					if (!_world.TryGetActor(flagf.ActorName, out actor))
					{
						throw new Exception($"Actor '{_strings.GetString(flagf.ActorName)}' not found");
					}
					
					var flagName = _strings.GetString(flagf.FlagName);
					bool isSet = actor.Flags.Bits.IsSet(AgentStateFlags.BitNameToNum(flagName));
					if (!isSet != flagf.Not)
					{
						docont = true;
						break;
					}
				}
				if (docont)
				{
					continue;
				}

				for (int i = 0; i < rule.WorldFilters.Count; i++)
				{
					var filter = rule.WorldFilters[i];
					if (!filter.IsMatch(_world.Facts))
					{
						docont = true;
						break;
					}
				}
				if (docont)
				{
					continue;
				}

				for (int i = 0; i < rule.ActorFilters.Count; i++)
				{
					var filter = rule.ActorFilters[i];
					Agent actor;
					if (!_world.TryGetActor(filter.ActorName, out actor))
					{
						throw new Exception($"Actor '{_strings.GetString(filter.ActorName)}' not found");
					}

					if (!filter.IsMatch(actor.Facts))
					{
						docont = true;
						break;
					}
				}
				if (docont)
				{
					continue;
				}

				if (rule.Probability < 1.0f)
				{
					if (_random.NextFloat(0.99998f) > rule.Probability)
					{
						if (rule.NoRetryIfProbablityFails)
						{
							rule.HasRun = true;
						}
						continue;
					}
				}

				// all cases pass
				Execute(rule);

				return true;
			}

			return false;
		}
	}
}
