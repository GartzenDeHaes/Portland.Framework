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
using Portland.Framework.AI;
using Portland.Mathmatics;
using Portland.RPG;

namespace Portland.AI
{
    public sealed class World
	{
		public readonly IClock Clock;
		public WorldStateFlags Flags;
		public readonly IBlackboard<string> GlobalFacts = new Blackboard<string>();

		public readonly UtilityFactory UtilitySystem;
		public readonly BarkRuleEngine BarkEngine;

		public readonly PropertyManager PropertyMan;
		public readonly CharacterManager CharacterManager;
		public readonly ItemFactory ItemManager;

		//public Dictionary<StringTableToken, IObservableValue<Variant8>> Facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();
		//Dictionary<string, Agent> _actors = new Dictionary<string, Agent>();
		Vector<Agent> _actors = new Vector<Agent>();
		public readonly StringTable Strings;

		//List<Action<Agent>> _alertsToAddToActor = new List<Action<Agent>>();
		string _actorUtilityObjectiveFactName;

		public void Update(float deltaTimeInSeconds)
		{
			Clock.Update(deltaTimeInSeconds);
			Flags.Daylight = !Clock.IsNightTime;
			UtilitySystem.TickAgents();

			for (int i = 0; i < _actors.Count; i++)
			{
				_actors[i].Update(deltaTimeInSeconds);
			}

			BarkEngine.Update();
		}

		public World(IClock clock, StringTable strings, IRandom rnd)
		{
			Clock = clock;
			Strings = strings;
			UtilitySystem = new UtilityFactory(clock);
			BarkEngine = new BarkRuleEngine(this, rnd);

			PropertyMan = new PropertyManager();
			ItemManager = new ItemFactory();
			CharacterManager = new CharacterManager(Strings, PropertyMan, ItemManager);
		}

		public void CreateActor(string className, string actorName, in string raceEffectGrp, in string classEffectGrp, in string faction)
		{
			var agent = new Agent
			(
				className, 
				actorName,
				UtilitySystem.CreateAgentInstance(className, actorName),
				CharacterManager.CreateCharacter(className, raceEffectGrp, classEffectGrp, faction)
			);

			//// add the utility considerations as agent facts
			//foreach(var oprop in agent.UtilitySet.Properties.Values)
			//{
			//	agent.Facts.Add(oprop.Definition.PropertyId, oprop);
			//}

			_actors.Add(agent);

			// Ensure all global utility consideration are set as global facts
			for (var cons = UtilitySystem.GetGlobalConsiderationNameEnumerator(); cons.MoveNext();)
			{
				if (! GlobalFacts.ContainsKey(cons.Current))
				{
					GlobalFacts.Add(cons.Current, UtilitySystem.GetGlobalProperty(cons.Current));
				}
			}

			if (! String.IsNullOrWhiteSpace(_actorUtilityObjectiveFactName))
			{
				agent.Facts.Add(_actorUtilityObjectiveFactName, agent.UtilitySet.CurrentObjective);
			}
		}

		public bool TryGetActor(string name, out Agent actor)
		{
			//return _actors.TryGetValue(name, out actor);
			for (int i = 0; i < _actors.Count; i++)
			{
				actor = _actors[i];
				if (actor.Name == name)
				{
					return true;
				}
			}
			actor = null;
			return false;
		}

		/// <summary>
		/// Agent alerts for properties setup by UtilityFactory.LivingBuilder.AddAllObjectives
		/// </summary>
		public void DefineStandardUtilityAlerts()
		{
			DefineActorFactAlert_WhenOverValue("hunger", "ALERT_HUNGER", 80f);
			DefineActorFactAlert_WhenOverValue("thirst", "ALERT_THIRST", 80f);
			DefineActorFactAlert_WhenUnderValue("health", "ALERT_HEALTH", 20f);
			DefineActorFactAlert_WhenUnderValue("health", "IS_DEAD", 0.01f);
			DefineActorFactAlert_WhenUnderValue("stamina", "ALERT_STAMINA", 20f);
			DefineActorFactAlert_WhenOverValue("sleepy", "ALERT_SLEEP", 80f);
		}

		public void DefineNameForActorUtilityObjectiveFact(string factNameToUseIs)
		{
			_actorUtilityObjectiveFactName = factNameToUseIs;
		}

		public void DefineActorFactAlert_WhenUnderValue(in String8 factPropName, string flagName, Variant8 threshold)
		{
			UtilitySystem.DefineAlertForPropertyDefinition(factPropName, PropertyDefinition.AlertType.Below, threshold, flagName);
		}

		public void DefineActorFactAlert_WhenOverValue(string factPropName, string flagName, Variant8 threshold)
		{
			UtilitySystem.DefineAlertForPropertyDefinition(factPropName, PropertyDefinition.AlertType.Above, threshold, flagName);
		}

		public void DefineActorAsCharacter0X(string actorName, int characterNum_1to4, string healthPropertyName)
		{
			Debug.Assert(characterNum_1to4 > 0 && characterNum_1to4 < 5);

			string bitKey = $"USER_FLAG_0{characterNum_1to4}";
			int bitNum = WorldStateFlags.BitNameToNum(bitKey);

			if (TryGetActor(actorName, out Agent actor))
			{
				if (actor.Facts.TryGetValue(healthPropertyName, out var prop))
				{
					actor.Alerts += () =>
					{
						//actor.Flags.IsDead = prop.Value < 0.01f;
						Flags.Bits.SetTest(bitNum, !actor.Flags.IsDead);
					};
				}
			}
			else
			{
				throw new Exception($"Agent {actorName} not found");
			}
		}
	}
}
