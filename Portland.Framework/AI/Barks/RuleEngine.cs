using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.CheckedEvents;
using Portland.Collections;
using Portland.Mathmatics;

namespace Portland.AI.Barks
{
	public sealed class RuleEngine
	{
		readonly Rule[] _rules;
		readonly World _world;
		readonly TextTable _strings;
		readonly IRandom _random;

		//RingBuffer<BarkCommand> _runcmds = new RingBuffer<BarkCommand>(16);
		List<BarkCommand> _cmdsDelaying = new List<BarkCommand>();

		public Command<TextTableToken, string> DoSay = new Command<TextTableToken, string>();
		//public Notify<TextTableToken> OnConceptChanged = new Notify<TextTableToken>();
		public Notify<TextTableToken> OnEventRaised = new Notify<TextTableToken>();

		public void Update()
		{
			BarkCommand cmd;

			for (int i = 0; i < _cmdsDelaying.Count; i++)
			{
				cmd = _cmdsDelaying[i];

				if (cmd.DelayRemainingToRun < _world.Clock.Time)
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
				// Arg1 is text key and Arg2 is the default text
				DoSay.Send(cmd.Arg1, cmd.Arg2);
				
				//OnConceptChanged.Send(cmd.Arg1);

				if (cmd.Duration > 0f)
				{
					_cmdsDelaying.Add(new BarkCommand() { CommandName = BarkCommand.CommandConcept, Arg1 = cmd.Arg1, Rule = cmd.Rule });
				}
				else
				{
					// arg1 is the object
					TryMatch(new ThematicEvent { Action = ThematicEvent.ActionSay, DirectObject = cmd.Arg1, Agent = cmd.Rule.ObserverName });
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
					SetVars(_world.Actors[cmd.ActorName].Facts, cmd);
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
				TryMatch(new ThematicEvent { Action = ThematicEvent.ActionSay, DirectObject = cmd.Arg1, Agent = cmd.Rule.ObserverName });
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
					AddToVar(_world.Actors[cmd.ActorName].Facts, cmd);
				}
			}
			else
			{
				throw new Exception($"Unknown command {cmd.CommandName}");
			}
		}

		void SetVars(Dictionary<TextTableToken, Variant8> facts, BarkCommand cmd)
		{
			if (!facts.ContainsKey(cmd.Arg1))
			{
				facts.Add(cmd.Arg1, cmd.Arg2);
			}
			else
			{
				facts[cmd.Arg1] = cmd.Arg2;
			}
		}

		void AddToVar(Dictionary<TextTableToken, Variant8> facts, BarkCommand cmd)
		{
			if (!facts.ContainsKey(cmd.Arg1))
			{
				facts.Add(cmd.Arg1, cmd.Arg2);
			}
			else
			{
				facts[cmd.Arg1] = facts[cmd.Arg1] + cmd.Arg2;
			}
		}

		public int RuleCount { get { return _rules.Length; } }

		public int DelayingCount { get { return _cmdsDelaying.Count; } }

		public RuleEngine(World world, TextTable strings, IRandom rand, string ruleText)
		{
			_world = world;
			_strings = strings;
			_random = rand;

			BarkSerializer parser = new BarkSerializer(strings);
			var rulelist = parser.Deserialize(ruleText);
			_rules = rulelist.OrderByDescending(r => r.Priority()).ToArray();
		}

		void Execute(Rule rule)
		{
			rule.HasRun = true;
			BarkCommand cmd;

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
			Rule rule = null;
			bool docont = false;

			for (int x = 0; x < _rules.Length; x++)
			{
				rule = _rules[x];
				if (rule.HasRun || rule.Action != happened.Action || rule.ObjectName != happened.DirectObject)
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

				if (rule.ActorName.Index != 0 && rule.ActorName != happened.Agent)
				{
					continue;
				}

				// TODO: Instrument

				docont = false;
				for (int i = 0; i < rule.ActorFlags.Count; i++)
				{
					var flagf = rule.ActorFlags[i];
					var actor = _world.Actors[flagf.ActorName];
					var flagName = _strings.GetString(flagf.FlagName);
					if (actor.Flags.Bits.IsSet(AgentStateFlags.BitNameToNum(flagName)) != flagf.Not)
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
					if (! filter.IsMatch(_world.Facts))
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
					var actor = _world.Actors[filter.ActorName];

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
					if (_random.NextFloat(1.0f) > rule.Probability)
					{
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
