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
using Portland.RPG;
using Portland.RPG.Dialogue;
using Portland.Text;
using Portland.Threading;
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
		Dictionary<String, CharacterSheet> _charsById = new();
		Vector<Agent> _agents = new();

		public IMessageBus<SimpleMessage> Events;

		public IFactionManager Factions = new FactionManager();

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

		public World(IClock clock, IRandom rnd, IMessageBus<SimpleMessage> eventBus)
		{
			Clock = clock;
			//Strings = strings;
			Events = eventBus;

			PropertyMan = new PropertyManager();
			UtilitySystem = new UtilityFactory(clock, PropertyMan);
			BarkEngine = new BarkRuleEngine(this, rnd);
			Items = new ItemFactory();
			_globalBasFuncs = LoadBasFunctions();
			CharacterManager = new CharacterManager(_globalBasFuncs, GlobalFacts, UtilitySystem, PropertyMan, Items);

			DialogueMan = new DialogueManager(Flags, GlobalFacts, _charsById, Events);
		}

		public World(DateTime epoc, float minutesPerDay = 1440f)
		: this(new Clock(epoc, minutesPerDay), MathHelper.Rnd, new EventBus())
		{
			_globalBasFuncs = LoadBasFunctions();
		}

		public World(DateTime epoc, IMessageBus<SimpleMessage> bus, float minutesPerDay = 1440f)
		: this(new Clock(epoc, minutesPerDay), MathHelper.Rnd, bus)
		{
			_globalBasFuncs = LoadBasFunctions();
		}

		public CharacterSheet CreateCharacter
		(
			in String charId,
			string charUniqueName,
			string longName
		)
		{
			return CreateCharacter(charId, charUniqueName, longName, String.Empty, String.Empty, String.Empty);
		}

		public CharacterSheet CreateCharacter
		(
			in String charId,
			string charUniqueName,
			string longName,
			in String raceEffectGrp,
			in String classEffectGrp,
			in String faction
		)
		{
			var charDef = CharacterManager.GetCharacterDefinition(charId);
			string utilitySetId = charDef.UtilitySetId;

			//var agent = CreateAgent(utilitySetId, agentId, charUniqueName, longName);

			var character = CharacterManager.CreateCharacter(charUniqueName, charId, raceEffectGrp, classEffectGrp, faction);

			_charsById.Add(charUniqueName, character);
			character.Agent.RuntimeIndex = _agents.Count;
			_agents.Add(character.Agent);

			return character;
		}

		public bool TryGetCharacter(in String agentId, out CharacterSheet actor)
		{
			return _charsById.TryGetValue(agentId, out actor);
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

		public void DestroyCharacter(in String charUniqueId)
		{
			if (_charsById.TryGetValue(charUniqueId, out var character))
			{
				_charsById.Remove(charUniqueId);
				_agents.RemoveAt(character.Agent.RuntimeIndex);

				UtilitySystem.DestroyInstance(character.Agent.UtilitySet.Name);
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

			if (TryGetCharacter(actorName, out var actor))
			{
				if (actor.Facts.TryGetValue(healthPropertyName, out var prop))
				{
					actor.Agent.Alerts += () =>
					{
						//actor.Flags.IsDead = prop.Value < 0.01f;
						Flags.Bits.SetTest(bitNum, !actor.Agent.Flags.IsDead);
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
			.Add("STAT", 1, (ExecutionContext ctx) =>
			{
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.TryGetStat(name.ToString(), out float value))
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
			.Add("STAT", 2, (ExecutionContext ctx) =>
			{
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (!chr.TrySetStat(name.ToString(), ctx.Context["b"].ToFloat()))
				{
					ctx.SetError($"{"STAT"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.SetReturnValue(ctx.Context["b"]);
				}
			})
			// Returns the maximum range for a stat
			// float: STATMAX("HP")
			.Add("STATMAX", 1, (ExecutionContext ctx) =>
			{
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.TryGetMaximum(name.ToString(), out float value))
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
			.Add("STATMAX", 2, (ExecutionContext ctx) =>
			{
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (!chr.TrySetMaximum(name.ToString(), ctx.Context["b"].ToFloat()))
				{
					ctx.SetError($"{"STATMAX"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.SetReturnValue(0f);
				}
			})
			// Returns the probability set for a stat in dice notation (3d8)
			// string: STATDICE("HP")
			.Add("STATDICE", 1, (ExecutionContext ctx) =>
			{
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.TryGetProbability(name.ToString(), out var value))
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
			.Add("STATROLL", 1, (ExecutionContext ctx) =>
			{
				var chr = (CharacterSheet)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.TryGetProbability(name.ToString(), out var value))
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
			.Add("ROLLDICE", 1, (ExecutionContext ctx) =>
			{
				string dicetxt = ctx.Context["a"].ToString();
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
			.Add("SELECTED", 0, (ctx) =>
			{
				ctx.SetReturnValue(((CharacterSheet)ctx.UserData).InventoryWindow.SelectedSlot);
			})
			// Sum all of the named properties in a window area grid, fe DEFENCE in the equipment armor grid to calcuate AC
			// INVENTORY("SUM", "WINDOW AREA NAME", "PROPERTY NAME")
			.Add("INVENTORY", 3, (ExecutionContext ctx) =>
			{
				var chr = (CharacterSheet)ctx.UserData;
				string op = ctx.Context["a"].ToString();
				var window = ctx.Context["b"];
				string propName = ctx.Context["c"].ToString();

				if (op.Equals("GET"))
				{
					if (window.IsWholeNumber())
					{
						// Get property for slot
						// INVENTORY('SUM', SlotNum, 'Property Name');
						if (chr.InventoryWindow.TryGetProperty(window.ToInt(), propName, out var value8))
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
						chr.InventoryWindow.TrySumItemProp(window.ToString(), propName, out float amt);
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
				in String playerUtilityClass
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

				_world.UtilitySystem.CreateAgentType("player", playerUtilityClass)
					.AddObjectiveIdle();

				_world.UtilitySystem.CreateAgent(playerUtilityClass, playerCharId);

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
			return Parse(xml, new EventBus());
		}

		public static World Parse(string xml, IMessageBus<SimpleMessage> bus)
		{
			XmlLex lex = new XmlLex(xml);
			lex.SkipComments = true;

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
					else if (lex.Lexum.IsEqualTo("minutes_per_day"))
					{
						minutesPerDay = Single.Parse(lex.MatchProperty("minutes_per_day"));
					}
					else
					{
						throw new Exception($"Unexpected property {lex.Lexum} on line {lex.LineNum}");
					}
				}

				lex.MatchTagClose();
			}

			World world = new World(dt, bus, minutesPerDay);

			while (lex.Token == XmlLex.XmlLexToken.TAG)
			{
				if (lex.Lexum.IsEqualTo("properties"))
				{
					world.PropertyMan.ParsePropertyDefinitions(lex);
				}
				else if (lex.Lexum.IsEqualTo("property_sets"))
				{
					world.PropertyMan.ParseDefinitionSets(lex);
				}
				else if (lex.Lexum.IsEqualTo("locations"))
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
				else if (lex.Lexum.IsEqualTo("utility"))
				{
					world.UtilitySystem.ParseLoad(lex);
				}
				else if (lex.Lexum.IsEqualTo("items"))
				{
					world.Items.Parse(lex);
				}
				else if (lex.Lexum.IsEqualTo("character_types"))
				{
					world.CharacterManager.Parse(lex);
				}
				else if (lex.Lexum.IsEqualTo("effects"))
				{
					world.CharacterManager.Parse(lex);
				}
				else if (lex.Lexum.IsEqualTo("characters"))
				{
					ParseCharacters(world, lex);
				}
				else if (lex.Lexum.IsEqualTo("factions"))
				{
					ParseFactions(world, lex);
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

		static void ParseFactions(World world, XmlLex lex)
		{
			lex.MatchTag("factions");

			while (lex.Lexum.IsEqualTo("faction"))
			{
				lex.MatchTagStart("faction");

				string id = String.Empty;
				string desc = String.Empty;
				string alignment = "NN";
				string grouping = "";

				while (lex.Token == XmlLex.XmlLexToken.STRING)
				{
					if (lex.Lexum.IsEqualTo("faction_id"))
					{
						id = lex.MatchProperty("faction_id");
					}
					if (lex.Lexum.IsEqualTo("desc"))
					{
						desc = lex.MatchProperty("desc");
					}
					if (lex.Lexum.IsEqualTo("alignment"))
					{
						desc = lex.MatchProperty("alignment");
					}
					if (lex.Lexum.IsEqualTo("grouping"))
					{
						desc = lex.MatchProperty("grouping");
					}
				}

				world.Factions.DefineFaction(id, desc, alignment, grouping);

				if (lex.Token == XmlLex.XmlLexToken.TAG_END)
				{
					lex.MatchTagEnd();
				}
				else
				{
					lex.MatchTagClose();

					while (lex.Lexum.IsEqualTo("relation"))
					{
						lex.MatchTagStart("relation");
						var otherFactionId = lex.MatchProperty("faction_id");
						var relation = Single.Parse(lex.MatchProperty("relation"));
						lex.MatchTagEnd();

						world.Factions.SetFactionRelation(id, otherFactionId, relation);
					}

					lex.MatchTagClose("faction");
				}
			}

			lex.MatchTagClose("factions");
		}

		static void ParseCharacters(World world, XmlLex lex)
		{
			if (! lex.Lexum.IsEqualTo("characters"))
			{
				return;
			}

			lex.MatchTag("characters");

			while (lex.Lexum.IsEqualTo("character"))
			{
				lex.MatchTagStart("character");

				string charTypeId = String.Empty;
				string raceGrp = String.Empty;
				string clsGrp = String.Empty;
				string faction = String.Empty;
				string uniqueName = String.Empty;
				string longName = String.Empty;

				while (lex.Token == XmlLex.XmlLexToken.STRING)
				{
					if (lex.Lexum.IsEqualTo("chartype_id"))
					{
						charTypeId = lex.MatchProperty("chartype_id");
					}
					else if (lex.Lexum.IsEqualTo("race"))
					{
						raceGrp = lex.MatchProperty("race");
					}
					else if (lex.Lexum.IsEqualTo("class"))
					{
						clsGrp = lex.MatchProperty("class");
					}
					else if (lex.Lexum.IsEqualTo("faction"))
					{
						faction = lex.MatchProperty("faction");
					}
					else if (lex.Lexum.IsEqualTo("unique_name"))
					{
						uniqueName = lex.MatchProperty("unique_name");
					}
					else if (lex.Lexum.IsEqualTo("long_name"))
					{
						longName = lex.MatchProperty("long_name");
					}
					else
					{
						throw new Exception($"Unexpected property {lex.Lexum} on line {lex.LineNum}");
					}
				}

				if (uniqueName == String.Empty)
				{
					uniqueName = charTypeId;
				}
				if (longName == String.Empty)
				{
					longName = uniqueName;
				}

				var agent = world.CreateCharacter(charTypeId, uniqueName, longName, raceGrp, clsGrp, faction);

				if (lex.Token == XmlLex.XmlLexToken.TAG_END)
				{
					lex.MatchTagEnd();
				}
				else
				{
					lex.MatchTagClose();

					while (lex.Token != XmlLex.XmlLexToken.CLOSE && !lex.IsEOF)
					{
						if (lex.Lexum.IsEqualTo("override"))
						{
							lex.MatchTagStart("override");

							while (lex.Token == XmlLex.XmlLexToken.STRING)
							{
								string propId = lex.Lexum.ToString();
								lex.Match(XmlLex.XmlLexToken.STRING);
								lex.Match(XmlLex.XmlLexToken.EQUAL);
								string value = lex.Lexum.ToString();
								lex.Next();

								agent.Facts.Set(propId, Variant8.Parse(value));
							}

							lex.MatchTagEnd();
						}
						else if (lex.Lexum.IsEqualTo("item"))
						{
							lex.MatchTagStart("item");

							string itemId = String.Empty;
							string windowSection = String.Empty;
							int count = 1;

							while (lex.Token != XmlLex.XmlLexToken.TAG_END && !lex.IsEOF)
							{
								if (lex.Lexum.IsEqualTo("item_id"))
								{
									itemId = lex.MatchProperty("item_id");
								}
								else if (lex.Lexum.IsEqualTo("count"))
								{
									count = Int32.Parse(lex.MatchProperty("count"));
								}
								else if (lex.Lexum.IsEqualTo("window_section"))
								{
									windowSection = lex.MatchProperty("window_section");
								}
								else
								{
									throw new Exception($"Unknown property {lex.Lexum} on line {lex.LineNum}");
								}
							}

							var item = world.Items.CreateItem(0, itemId, count);
							agent.InventoryWindow.TryMergSectionItem(windowSection, item);

							lex.MatchTagEnd();
						}
						else
						{
							throw new Exception($"Unexpected property {lex.Lexum} on line {lex.LineNum}");
						}
					}

					lex.MatchTagClose("character");
				}
			}

			lex.MatchTagClose("characters");
		}
	}
}
