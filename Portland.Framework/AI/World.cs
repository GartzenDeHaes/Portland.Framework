using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.Barks;
using Portland.AI.Utility;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Mathmatics;

namespace Portland.AI
{
    public sealed class World
	{
		public readonly IClock Clock;
		public WorldStateFlags Flags;
		public readonly UtilityFactory UtilitySystem;
		public BarkRuleEngine BarkEngine;
		public Dictionary<StringTableToken, IObservableValue<Variant8>> Facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();
		Dictionary<StringTableToken, Agent> _actors = new Dictionary<StringTableToken, Agent>();
		public readonly StringTable Strings;

		List<Action<Agent>> _alertsToAddToActor = new List<Action<Agent>>();
		StringTableToken _actorUtilityObjectiveFactName;

		public void Update(float deltaTimeInSeconds)
		{
			Clock.Update(deltaTimeInSeconds);
			Flags.Daylight = !Clock.IsNightTime;
			UtilitySystem.TickAgents();
			BarkEngine.Update();
		}

		public World(IClock clock, StringTable strings, IRandom rnd)
		{
			Clock = clock;
			Strings = strings;
			UtilitySystem = new UtilityFactory(clock);
			BarkEngine = new BarkRuleEngine(this, strings, rnd);
		}

		public void CreateActor(string className, string actorName)
		{
			var actorTok = Strings.Get(actorName);

			var agent = new Agent { 
				Class = Strings.Get(className), 
				Name = actorTok,
				UtilitySet = UtilitySystem.CreateAgentInstance(className, actorName)
			};

			// add the utility considerations as agent facts
			foreach(var oprop in agent.UtilitySet.Properties.Values)
			{
				agent.Facts.Add(Strings.Get(oprop.Definition.PropertyId), oprop.Amt);
			}

			_actors.Add(actorTok, agent);

			// Ensure all global utility consideration are set as global facts
			for (var cons = UtilitySystem.GetGlobalConsiderationNameEnumerator(); cons.MoveNext();)
			{
				if (! Facts.ContainsKey(Strings.Get(cons.Current)))
				{
					Facts.Add(Strings.Get(cons.Current), UtilitySystem.GetGlobalProperty(cons.Current).Amt);
				}
			}

			// add any defined consideration to alert flags
			for (int x = 0; x < _alertsToAddToActor.Count; x++)
			{
				_alertsToAddToActor[x].Invoke(agent);
			}

			if (_actorUtilityObjectiveFactName.Index != 0)
			{
				agent.Facts.Add(_actorUtilityObjectiveFactName, agent.UtilitySet.CurrentObjective);
			}
		}

		public bool TryGetActor(StringTableToken name, out Agent actor)
		{
			return _actors.TryGetValue(name, out actor);
		}

		public bool TryGetActor(string name, out Agent actor)
		{
			return _actors.TryGetValue(Strings.Get(name), out actor);
		}

		/// <summary>
		/// Agent alerts for properties setup by UtilityFactory.LivingBuilder.AddAllObjectives
		/// </summary>
		public void DefineStandardUtilityAlerts()
		{
			DefineActorFactAlert_WhenOverValue("hunger", "ALERT_HUNGER", 80f);
			DefineActorFactAlert_WhenOverValue("thirst", "ALERT_THIRST", 80f);
			DefineActorFactAlert_WhenUnderValue("health", "ALERT_HEALTH", 20f);
			DefineActorFactAlert_WhenUnderValue("stamina", "ALERT_STAMINA", 20f);
			DefineActorFactAlert_WhenOverValue("sleepy", "ALERT_SLEEP", 80f);
		}

		public void DefineNameForActorUtilityObjectiveFact(string factNameToUseIs)
		{
			_actorUtilityObjectiveFactName = Strings.Get(factNameToUseIs);
		}

		public void DefineActorFactAlert_WhenUnderValue(string factPropName, string flagName, Variant8 threshold)
		{
			var factTok = Strings.Get(factPropName);
			var flagBit = AgentStateFlags.BitNameToNum(flagName);

			Action<Agent> addTo = (Agent a) =>
			{
				if (a.Facts.TryGetValue(factTok, out var obValue))
				{
					obValue.AddValidator((v) => {
						a.Flags.Bits.SetTest(flagBit, v < threshold);
						return true;
					});
				}
			};

			_alertsToAddToActor.Add(addTo);
		}

		public void DefineActorFactAlert_WhenOverValue(string factPropName, string flagName, Variant8 threshold)
		{
			var factTok = Strings.Get(factPropName);
			var flagBit = AgentStateFlags.BitNameToNum(flagName);

			Action<Agent> addTo = (Agent a) =>
			{
				if (a.Facts.TryGetValue(factTok, out var obValue))
				{
					obValue.AddValidator((v) => {
						a.Flags.Bits.SetTest(flagBit, v > threshold);
						return true;
					});
				}
			};

			_alertsToAddToActor.Add(addTo);
		}

		public void DefineActorAsCharacter0X(string actorName, int characterNum_1to4)
		{
			Debug.Assert(characterNum_1to4 > 0 && characterNum_1to4 < 5);
			string bitKey = $"CHARACTER_0{characterNum_1to4}_ALIVE";
			int bitNum = WorldStateFlags.BitNameToNum(bitKey);

			var actor = _actors[Strings.Get(actorName)];
			Flags.Bits.SetTest(bitNum, !actor.Flags.IsDead);

			if (actor.Facts.TryGetValue(Strings.Get("health"), out var prop))
			{
				prop.AddValidator((health) => {
					Flags.Bits.SetTest(bitNum, health > .95f);
					return true;
				});
			}
		}
	}
}
