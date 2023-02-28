using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.Barks;
using Portland.AI.NLP;
using Portland.AI.Utility;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Framework.AI;
using Portland.Mathmatics;
using Portland.RPG;
using Portland.Types;

namespace Portland.AI
{
	public sealed class World
	{
		public readonly IClock Clock;
		public readonly IPropertyManager PropertyMan;

		public WorldStateFlags Flags;
		public readonly IBlackboard<String> GlobalFacts = new Blackboard<String>();

		public readonly IUtilityFactory UtilitySystem;
		public readonly BarkRuleEngine BarkEngine;

		public readonly ICharacterManager CharacterManager;
		public readonly ItemFactory Items;

		//public Dictionary<StringTableToken, IObservableValue<Variant8>> Facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();
		//Dictionary<string, Agent> _actors = new Dictionary<string, Agent>();
		Vector<Agent> _actors = new Vector<Agent>();
		//public readonly StringTable Strings;

		//List<Action<Agent>> _alertsToAddToActor = new List<Action<Agent>>();
		//string _actorUtilityObjectiveFactName;

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

		public World(IClock clock, IRandom rnd)
		{
			Clock = clock;
			//Strings = strings;
			
			PropertyMan = new PropertyManager();
			UtilitySystem = new UtilityFactory(clock, PropertyMan);
			BarkEngine = new BarkRuleEngine(this, rnd);
			Items = new ItemFactory();
			CharacterManager = new CharacterManager(PropertyMan, Items);
		}

		public World(DateTime epoc, float minutesPerDay = 1440f)
		: this(new Clock(epoc, minutesPerDay), MathHelper.Rnd)
		{
		}

		public Agent CreateAgent(string className, string actorName)
		{
			return CreateAgent(className, actorName, String.Empty, String.Empty, String.Empty);
		}

		public Agent CreateAgent
		(
			string className, 
			string actorName, 
			in string raceEffectGrp, 
			in string classEffectGrp, 
			in string faction
		)
		{
			var agent = new Agent
			(
				UtilitySystem,
				CharacterManager,
				className,
				actorName,
				raceEffectGrp, 
				classEffectGrp, 
				faction
			);

			//// add the utility considerations as agent facts
			//foreach(var oprop in agent.UtilitySet.Properties.Values)
			//{
			//	agent.Facts.Add(oprop.Definition.PropertyId, oprop);
			//}

			_actors.Add(agent);

			//if (!String.IsNullOrWhiteSpace(_actorUtilityObjectiveFactName))
			//{
			//	agent.Facts.Add(_actorUtilityObjectiveFactName, agent.UtilitySet.CurrentObjective);
			//}

			return agent;
		}

		public bool TryGetActor(in String name, out Agent actor)
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

		public void DefineAlertHungerFlag(in String propertId, float threshold = 80f)
		{
			DefineActorFactAlert_WhenOverValue(propertId, "ALERT_HUNGER", threshold);
		}

		public void DefineAlertThirstFlag(in String propertId, float threshold = 80f)
		{
			DefineActorFactAlert_WhenOverValue(propertId, "ALERT_THIRST", threshold);
		}

		public void DefineAlertHealthFlag(in String propertId, float threshold = 20f)
		{
			DefineActorFactAlert_WhenUnderValue(propertId, "ALERT_HEALTH", threshold);
		}

		public void DefineAlertDeadFlag(in String propertId, float threshold = 0.1f)
		{
			DefineActorFactAlert_WhenUnderValue(propertId, "IS_DEAD", threshold);
		}

		public void DefineAlertStaminaFlag(in String propertId, float threshold = 10f)
		{
			DefineActorFactAlert_WhenUnderValue(propertId, "ALERT_STAMINA", threshold);
		}

		public void DefineAlertSleepinessFlag(in String propertId, float threshold = 80f)
		{
			DefineActorFactAlert_WhenOverValue(propertId, "ALERT_SLEEP", threshold);
		}

		/// <summary>
		/// Agent alerts for properties setup by UtilityFactory.LivingBuilder.AddAllObjectives
		/// </summary>
		public void DefineTestUtilityAlerts()
		{
			DefineAlertHungerFlag("hunger");
			DefineAlertThirstFlag("thirst");
			DefineAlertHealthFlag("health");
			DefineAlertDeadFlag("health");
			DefineAlertStaminaFlag("stamina");
			DefineAlertSleepinessFlag("sleepy");
		}

		//public void DefineNameForActorUtilityObjectiveFact(string factNameToUseIs)
		//{
		//	_actorUtilityObjectiveFactName = factNameToUseIs;
		//}

		public void DefineActorFactAlert_WhenUnderValue(in String factPropName, string flagName, Variant8 threshold)
		{
			if (UtilitySystem.HasPropertyDefinition(factPropName))
			{
				UtilitySystem.DefineAlertForPropertyDefinition(factPropName, PropertyDefinition.AlertType.Below, threshold, flagName);
			}
			else if (CharacterManager.HasStatDefined(factPropName))
			{
				CharacterManager.DefineAlertForStatDefinition(factPropName, PropertyDefinition.AlertType.Below, threshold, flagName);
			}
			else
			{
				throw new Exception($"Property not found {factPropName}");
			}
		}

		public void DefineActorFactAlert_WhenOverValue(in String factPropName, string flagName, Variant8 threshold)
		{
			if (UtilitySystem.HasPropertyDefinition(factPropName))
			{
				UtilitySystem.DefineAlertForPropertyDefinition(factPropName, PropertyDefinition.AlertType.Above, threshold, flagName);
			}
			else if (CharacterManager.HasStatDefined(factPropName))
			{
				CharacterManager.DefineAlertForStatDefinition(factPropName, PropertyDefinition.AlertType.Above, threshold, flagName);
			}
			else
			{
				throw new Exception($"Property not found {factPropName}");
			}
		}

		public void DefineActorAsCharacter0X(in String actorName, int characterNum_1to4, in String healthPropertyName)
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

		public WorldBuilder GetBuilder()
		{
			return new WorldBuilder(this);
		}

		public class WorldBuilder
		{
			World _world;

			public WorldBuilder(World world)
			{
				_world = world;
			}

			public ItemDefinitionBuilder DefineItemSimpleWeapon
			(
				string itemTypeId,
				string description,
				string weaponType,      // must match skill types in SetupSimplePlayer()
				string damgeDiceTerm,
				float weight
			)
			{
				return _world.Items.DefineItem("WEAPON", itemTypeId)
					.DisplayName(description)
					.AddProperty("TYPE", weaponType)
					.AddProperty("DAMAGE", damgeDiceTerm)
					.AddProperty("WEIGHT", weight);
			}

			public WorldBuilder SetupSimplePlayer
			(
				in String playerCharId, 
				in String playerRace
			)
			{
				//
				// Character
				// 

				_world.CharacterManager.GetBuilder().DefineSimpleCharacter(playerCharId, CharacterManagerBuilder.InventoryType.Test);
	
				// Define alerts that set flags in the agent for use by the bark system

				_world.DefineAlertHealthFlag("HP", 10);
				_world.DefineAlertDeadFlag("HP");


				// Item categories

				_world.Items.DefineCategory("Armor");
				_world.Items.DefineCategory("Consumable");
				_world.Items.DefineCategory("Melee");
				_world.Items.DefineCategory("Useable");
				_world.Items.DefineCategory("Resource");
				_world.Items.DefineCategory("Ranged");
				_world.Items.DefineCategory("Shield");
				_world.Items.DefineCategory("Throwable");
				_world.Items.DefineCategory("Tool");

				// Item properties used in the derived stat calculation above

				// Weapon skill type: BOW, SWORD
				_world.Items.DefineProperty("TYPE", ItemPropertyType.String, "Weapon Skill Type", false);
				// AC modifer
				_world.Items.DefineProperty("DEFENSE", ItemPropertyType.Int, "Defense Modifer", false);				
				// damage on hit, fe 1d8
				_world.Items.DefineProperty("DAMAGE", ItemPropertyType.DiceRoll, "Weapon Damage Roll", false);
				// Item weight
				_world.Items.DefineProperty("WEIGHT", ItemPropertyType.Float, "Weight", false);

				// Character template and inventory

				//
				// Utility
				// 

				//_world.DefineNameForActorUtilityObjectiveFact("objective");

				// Any character properties marked as unity need to have a matching property here with the same name

				// Setup a stub objective set that only has IDLE

				if (!_world.UtilitySystem.HasObjectiveSet("player"))
				{
					_world.UtilitySystem.CreateObjectiveSetBuilder("player")
						.AddObjectiveIdleAt30pct();
				}

				// Define an "agenct" that can be used to specialize an objective set

				_world.UtilitySystem.CreateAgentType("player", playerRace)
					.AddObjectiveIdle();

				_world.UtilitySystem.CreateAgent(playerRace, playerCharId);

				return this;

			}

			//public WorldBuilder SetupFallout3Player(in String playerCharId, in String playerRace)
			//{
			//	/// Define a 1 to 10 stat with a default of 4
			//	_world.CharacterManager.GetBuilder()
			//		.AddFalloutStat("STR", "Strength")
			//		.AddFalloutStat("PER", "Perception")
			//		.AddFalloutStat("END", "Endurance")
			//		.AddFalloutStat("CHR", "Chrisma")
			//		.AddFalloutStat("INT", "Intelligence")
			//		.AddFalloutStat("AGL", "Agility")
			//		.AddFalloutStat("LUC", "Luck")
			//		.AddFalloutDerivedStat("AP", "Action Points", 85, false)
			//		.AddFalloutDerivedStat("HP", "Hit Points", 90, false)
			//		.AddFalloutDerivedStat("CARY", "Carry Weight", 100, false)
			//		.AddFalloutDerivedStat("MELE", "Melee Damage", 2, false)
			//		.AddFalloutDerivedStat("NERV", "Companion Nerve", 2, false)
			//		.AddFalloutDerivedStat("WEAP", "Weapon Damage", 2, false)
			//		.AddFalloutDerivedStat("UNAR", "Unarmed Damage", 2, false)
			//		;



			//	//if (!_world.UtilitySystem.HasObjectiveSet("player"))
			//	//{
			//	//	_world.UtilitySystem.CreateObjectiveSetBuilder("player")
			//	//		.AddObjectiveIdleAt30pct();
			//	//}

			//	//_world.UtilitySystem.CreateAgentType("player", playerRace)
			//	//	.AddObjectiveIdle();

			//	//if (!_world.UtilitySystem.HasPropertyDefinition(healthPropertyId))
			//	//{
			//	//	_world.UtilitySystem.CreatePropertyDef_0to100_Descreasing(false, healthPropertyId);

			//	//	_world.UtilitySystem.CreateConsideration("idle", healthPropertyId);
			//	//}

			//	//_world.UtilitySystem.CreateAgent(race, race);

			//	//_world.DefineNameForActorUtilityObjectiveFact("objective");

			//	//_world.DefineAlertHungerFlag(healthPropertyId);


			//	return this;

			//}
		}
	}
}
