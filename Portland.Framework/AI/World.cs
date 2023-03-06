using System;
using System.Collections.Generic;
using System.Diagnostics;

using Portland.AI.Barks;
using Portland.AI.Utility;
using Portland.Basic;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Interp;
using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;
using Portland.RPG;
using Portland.RPG.Dialogue;
using Portland.Text;
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

		public readonly DialogueManager DialogueMan;

		//public Dictionary<StringTableToken, IObservableValue<Variant8>> Facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();
		Dictionary<String, Agent> _agentsById = new Dictionary<String, Agent>();
		readonly Vector<Agent> _agents = new Vector<Agent>();

		public EventBus Events = new EventBus();

		//List<Action<Agent>> _alertsToAddToActor = new List<Action<Agent>>();
		//string _actorUtilityObjectiveFactName;

		public List<string> Locations = new List<string>();

		private readonly Dictionary<SubSig, IFunction> _globalBasFuncs;

		public void Update(float deltaTimeInSeconds)
		{
			Clock.Update(deltaTimeInSeconds);
			Flags.Daylight = !Clock.IsNightTime;
			UtilitySystem.TickAgents();

			for (int i = 0; i < _agents.Count; i++)
			{
				_agents[i].Update(deltaTimeInSeconds);
			}

			BarkEngine.Update();
			
			DialogueMan.Update(deltaTimeInSeconds);

			Events.Poll();
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
			_globalBasFuncs = LoadBasFunctions();

			DialogueMan = new DialogueManager(Flags, GlobalFacts, _agentsById, Events); 
		}

		public World(DateTime epoc, float minutesPerDay = 1440f)
		: this(new Clock(epoc, minutesPerDay), MathHelper.Rnd)
		{
		}

		public Agent CreateAgent(in String className, in String agentId, string shortName, string longName)
		{
			return CreateAgent(className, agentId, shortName, longName, String.Empty, String.Empty, String.Empty);
		}

		public Agent CreateAgent(in String className, in String agentId)
		{
			return CreateAgent(className, agentId, agentId, agentId, String.Empty, String.Empty, String.Empty);
		}

		public Agent CreateAgent
		(
			in String className, 
			in String agentId, 
			string shortName,
			string longName,
			in String raceEffectGrp, 
			in String classEffectGrp, 
			in String faction
		)
		{
			var agent = new Agent
			(
				_globalBasFuncs,
				UtilitySystem,
				CharacterManager,
				GlobalFacts,
				className,
				agentId,
				shortName,
				longName,
				raceEffectGrp, 
				classEffectGrp, 
				faction
			);

			//// add the utility considerations as agent facts
			//foreach(var oprop in agent.UtilitySet.Properties.Values)
			//{
			//	agent.Facts.Add(oprop.Definition.PropertyId, oprop);
			//}

			agent.RuntimeIndex = _agents.AddAndGetIndex(agent);
			_agentsById.Add(agentId, agent);

			//if (!String.IsNullOrWhiteSpace(_actorUtilityObjectiveFactName))
			//{
			//	agent.Facts.Add(_actorUtilityObjectiveFactName, agent.UtilitySet.CurrentObjective);
			//}

			return agent;
		}

		public bool TryGetAgent(in String agentId, out Agent actor)
		{
			return _agentsById.TryGetValue(agentId, out actor);
			//for (int i = 0; i < _agents.Count; i++)
			//{
			//	actor = _agents[i];
			//	if (actor.AgentId == agentId)
			//	{
			//		return true;
			//	}
			//}
			//actor = null;
			//return false;
		}

		public void DestroyAgent(in String agentId)
		{
			if (_agentsById.TryGetValue(agentId, out var agent))
			{
				_agentsById.Remove(agentId);
				_agents.RemoveAt(agent.RuntimeIndex);
				
				UtilitySystem.DestroyInstance(agentId);
				//BarkEngine.RemoveRules(agentId);
				//DialogueMgr.RemoveRules(agentId);
			}
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

			if (TryGetAgent(actorName, out Agent actor))
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

		public static Dictionary<SubSig, IFunction> LoadBasFunctions()
		{
			Dictionary<SubSig, IFunction> funcs = new Dictionary<SubSig, IFunction>();
			LoadBasFunctions(funcs);
			return funcs;
		}

		public static void LoadBasFunctions(Dictionary<SubSig, IFunction> funcs)
		{
			new BasicNativeFunctionBuilder
			{
				InternalAdd = (name, argCount, fn) => funcs.Add(new SubSig { Name = name, ArgCount = argCount }, fn),
				HasFunction = (name, argCount) => funcs.ContainsKey(new SubSig { Name = name, ArgCount = argCount })
			}
			// ABS, ATAN, CINT, COS, CSTR, ERROR, EXP, HAS, LEN, LOG, NOW, RND, SGN, SIN, SQR, TAN
			.AddAllBuiltin()
			// Get the current value of a stat
			// float: STAT("HP")
			.Add("STAT", 1, (ExecutionContext ctx) => {
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetValue(name.ToString(), out float value))
				{
					ctx.SetReturnValue(value);
				}
				else
				{
					ctx.SetError($"{"STAT"}('{name}'): '{name}' NOT FOUND");
					ctx.SetReturnValue(0f);
				}
			})
			// Set the current value of a stat
			// STAT("STR", STAT("STR") + 1)
			.Add("STAT", 2, (ExecutionContext ctx) => {
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (!chr.Stats.TrySetValue(name.ToString(), ctx.Context["b"]))
				{
					ctx.SetError($"{"STAT"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.SetReturnValue(ctx.Context["b"]);
				}
			})
			// Returns the maximum range for a stat
			// float: STATMAX("HP")
			.Add("STATMAX", 1, (ExecutionContext ctx) => {
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetMaximum(name.ToString(), out float value))
				{
					ctx.SetReturnValue(value);
				}
				else
				{
					ctx.SetError($"{"STATMAX"}('{name}'): '{name}' NOT FOUND");
					ctx.SetReturnValue(0f);
				}
			})
			// Set the maximum value for a stat, use for XP, HP
			// STATMAX("HP", STATMAX("HP") + STATROLL("HP"))
			.Add("STATMAX", 2, (ExecutionContext ctx) => {
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (!chr.Stats.TrySetMaximum(name.ToString(), ctx.Context["b"]))
				{
					ctx.SetError($"{"STATMAX"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.SetReturnValue(0f);
				}
			})
			// Returns the probability set for a stat in dice notation (3d8)
			// string: STATDICE("HP")
			.Add("STATDICE", 1, (ExecutionContext ctx) => {
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetProbability(name.ToString(), out var value))
				{
					ctx.SetReturnValue(value.ToString());
				}
				else
				{
					ctx.SetError($"{"STATDICE"}('{name}'): '{name}' NOT FOUND");
					ctx.SetReturnValue(0f);
				}
			})
			//// Set the probability for a stat (can be used to adjust HP levelup amount, fe)
			//// float: STATDICE("HP", "1d4")
			//.Add("STATDICE", 2, (ExecutionContext ctx) => {
			//	var chr = (CharacterSheet)ctx.UserData;
			//	var name = ctx.Context["a"];
			//	string dicetxt = ctx.Context["b"];

			//	if (!DiceTerm.TryParse(dicetxt, out var dice))
			//	{
			//		ctx.SetError($"{"STATDICE"}('{dicetxt}') INVALID DICE TERM");
			//		ctx.Context.Set(0f);
			//	}
			//	else if (!chr.Stats.TrySetProbability(name.ToString(), dice))
			//	{
			//		ctx.SetError($"{"STATDICE"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
			//		ctx.Context.Set(0f);
			//	}
			//})
			// Dice roll for the probability set for a stat
			// float: STATROLL("HP")
			.Add("STATROLL", 1, (ExecutionContext ctx) => {
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetProbability(name.ToString(), out var value))
				{
					ctx.SetReturnValue(value.Roll(MathHelper.Rnd));
				}
				else
				{
					ctx.SetError($"{"STATROLL"}('{name}'): '{name}' NOT FOUND");
					ctx.SetReturnValue(0f);
				}
			})
			// Random number based on dice roll
			// float: ROLLDICE("1d4+2")
			.Add("ROLLDICE", 1, (ExecutionContext ctx) => {
				string dicetxt = ctx.Context["a"];
				if (DiceTerm.TryParse(dicetxt, out var dice))
				{
					ctx.SetReturnValue(dice.Roll(MathHelper.Rnd));
				}
				else
				{
					ctx.SetError($"{"ROLLDICE"}('{dicetxt}'): INVALID DICE TERM");
					ctx.SetReturnValue(0f);
				}
			})
			// Returns the selected inventory slot number
			// int: SELECTED()
			.Add("SELECTED", 0, (ctx) => {
				ctx.SetReturnValue(((CharacterSheet)ctx.UserData).InventoryWindow.SelectedSlot);
			})
			// Sum all of the named properties in a window area grid, fe DEFENCE in the equipment armor grid to calcuate AC
			// INVENTORY("SUM", "WINDOW AREA NAME", "PROPERTY NAME")
			.Add("INVENTORY", 3, (ExecutionContext ctx) => {
				var chr = (CharacterSheet)ctx.UserData;
				string op = ctx.Context["a"];
				var window = ctx.Context["b"];
				string propName = ctx.Context["c"];

				if (op.Equals("GET"))
				{
					if (window.IsWholeNumber())
					{
						// Get property for slot
						// INVENTORY('SUM', SlotNum, 'Property Name');
						if (chr.InventoryWindow.TryGetProperty(window, propName, out var value8))
						{
							if (value8.TypeIs == VariantType.Int)
							{
								ctx.SetReturnValue((int)value8);
							}
							else if (value8.TypeIs == VariantType.Float)
							{
								ctx.SetReturnValue((int)value8);
							}
							else
							{
								ctx.SetReturnValue((string)value8);
							}
						}
						else
						{
							ctx.SetReturnValue(new Variant());
						}
					}
					else
					{
						ctx.SetError($"{"INVENTORY"}('{op}', {window}, '{propName}'): INVALID WINDOW NUM");
						ctx.SetReturnValue(0f);
					}
				}
				else if (op.Equals("SUM"))
				{
					if (window.Length == 0 || (window.Equals("*")))
					{
						// Sum property for entire inventory, WEIGHT fe
						chr.InventoryWindow.TrySumItemProp(propName, out float amt);
						ctx.SetReturnValue(amt);
					}
					else
					{
						// Sum property for window
						chr.InventoryWindow.TrySumItemProp(window, propName, out float amt);
						ctx.SetReturnValue(amt);
					}
				}
				else
				{
					ctx.SetError($"{"INVENTORY"}('{op}', '{window}', '{propName}'): INVALID OPERATION '{op}'");
					ctx.SetReturnValue(0f);
				}
			})
			;
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

		public static World Parse(string xml)
		{
			XmlLex lex = new XmlLex(xml);

			DateTime dt = DateTime.Now;
			float minutesPerDay = 1440f;

			if (lex.Token == XmlLex.XmlLexToken.TAG)
			{
				lex.MatchTag("world");
			}
			else
			{
				lex.MatchTagStart("world");

				while (lex.Token == XmlLex.XmlLexToken.STRING)
				{
					if (lex.Lexum.IsEqualTo("date"))
					{
						string sdt = lex.MatchProperty("date");
						dt = DateTime.Parse(sdt);
					}
					else if (lex.Lexum.IsEqualTo("minutesPerDay"))
					{
						minutesPerDay = Single.Parse(lex.MatchProperty("minutesPerDay"));
					}
					else
					{
						throw new Exception($"Unexpected property {lex.Lexum} on line {lex.LineNum}");
					}
				}

				lex.MatchTagClose();
			}

			World world = new World(dt, minutesPerDay);

			while (lex.Token == XmlLex.XmlLexToken.TAG)
			{
				if (lex.Lexum.IsEqualTo("locations"))
				{
					lex.MatchTag("locations");
					ParseLocations(world, lex);
					lex.MatchTagClose("locations");
				}
				else if (lex.Lexum.IsEqualTo("dialogues"))
				{
					lex.MatchTag("dialogues");
					ParseDialogues(world, lex);
					lex.MatchTagClose("dialogues");
				}
				else if (lex.Lexum.IsEqualTo("utilities"))
				{
					lex.MatchTag("utilities");
					ParseUtility(world, lex);
					lex.MatchTagClose("utilities");
				}
				else
				{
					throw new Exception($"Unknown section {lex.Lexum} on line {lex.LineNum}");
				}
			}

			lex.MatchTagClose("world");

			return world;
		}

		static void ParseLocations(World world, XmlLex lex)
		{
			while (lex.Lexum.IsEqualTo("location"))
			{
				lex.MatchTagStart("location");

				if (lex.Lexum.IsEqualTo("name"))
				{
					world.Locations.Add(lex.MatchProperty("name"));
				}
				else
				{
					throw new Exception($"Unexpected property {lex.Lexum} on line {lex.LineNum}");
				}

				lex.MatchTagEnd();
			}
		}

		static void ParseDialogues(World world, XmlLex lex)
		{
			if (lex.Token != XmlLex.XmlLexToken.STRING)
			{
				return;
			}
			lex.NextText();
			string yarnish = lex.Lexum.ToString();
			lex.Match(XmlLex.XmlLexToken.TEXT);

			world.DialogueMan.Parse(yarnish);
		}

		static void ParseUtility(World world, XmlLex lex)
		{
			if (lex.Lexum.IsEqualTo("utility"))
			{
				world.UtilitySystem.ParseLoad(lex);
			}
		}
	}
}
