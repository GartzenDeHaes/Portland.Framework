using System;
using System.Collections.Generic;
using System.Text;

using Portland.Types;

namespace Portland.RPG
{
	public class CharacterManagerBuilder
	{
		CharacterManager _manager;
		IPropertyManager _props;
		ItemFactory _items;

		public CharacterManagerBuilder(CharacterManager manager, IPropertyManager propMan, ItemFactory items)
		{
			_manager = manager;
			_props = propMan;
			_items = items;
		}

		public enum InventoryType
		{
			Test,
			UltimateSurvival,
			Minecraft
		}

		public CharacterManagerBuilder DefineSimpleCharacter(string playerCharId, InventoryType invtype)
		{
			AddFalloutStat("STR", "Strength");
			AddFalloutStat("CON", "Constitution");
			AddFalloutStat("CHR", "Chrisma");
			AddFalloutStat("INT", "Intelligence");
			AddFalloutDerivedStat("HP", "Hit Points", 10, false);
			AddProperty("DERIVED", "LV", "Level", 0, 1, 99, false);
			AddProperty("DERIVED", "XP", "Experience Points", 0, 1, 1000, false);
			AddProperty("SYSTEM", "XPMOD", "XP Max Class Modifier", 0, 1, 10, false);
			AddProperty("DERIVED", "ATTACK", "Attack damaage modifier", 0, 1, 10, false);
			AddProperty("DERIVED", "DEFENSE", "Defense damaage modifier", 0, 1, 10, false);
			AddProperty("DERIVED", "CARRY", "Carry Weight", 0, 50, 200, false);
			AddProperty("SKILL", "EDGED", "Edged weapon skill", 0, 1, 10, false);
			AddProperty("SKILL", "BLUNT", "Blunt weapon skill", 0, 1, 10, false);
			AddProperty("SKILL", "PROJ", "Projectile weapon skill", 0, 1, 10, false);

			StringBuilder statsRecalc = new StringBuilder();
			statsRecalc.AppendLine("REM -== RECALC DERIVED STATS ==-");
			// XP = LV * XPMOD * (1000 - INT^2)
			statsRecalc.AppendLine("CALL STATMAX('XP', STAT('LV') * STAT('XPMOD') * (1000 - STAT('INT')*STAT('INT')))");
			// DEFENSE = DEFENSE of armor and shild plus a dexterity bonus
			statsRecalc.AppendLine("CALL STAT('DEFENSE', INVENTORY('SUM', 'ARMOR', 'DEFENSE') + INVENTORY('SUM', 'SHIELD', 'DEFENSE') + SQR(STAT('CON') / 2.0))");
			// ATTACK: 20 - LV - (skill for equipped weapon) - STR
			statsRecalc.AppendLine("CALL STAT('ATTACK', SQR(STAT('LV') + STAT(INVENTORY('GET', SELECTED(), 'TYPE'))))");
			// HP = LV * SQRT(CON)
			statsRecalc.AppendLine("CALL STATMAX('HP', STAT('LV') * SQR(STAT('CON')))");
			// CARRY = STR + SQRT(CON)
			statsRecalc.AppendLine("CALL STATMAX('CARRY', STAT('STR') + SQR(STAT('CON')))");
			// CARRY current value
			statsRecalc.AppendLine("CALL STAT('CARRY', INVENTORY('SUM', '*', 'WEIGHT'))");

			// Associate stats with player

			_props.DefinePropertySet(playerCharId, new string[] {
					"STR", "CON", "CHR", "INT",
					"HP", "LV", "XP", "XPMOD", "ATTACK", "DEFENSE", "CARRY",
					"EDGED", "BLUNT", "PROJ"
					},
				statsRecalc.ToString()
			);

			DefineCharacter(playerCharId, InventoryType.Test);

			return this;
		}

		void DefineCharacter(string playerCharId, InventoryType invType)
		{
			if (invType == InventoryType.Test)
			{
				_manager.CreateCharacterDefinition(playerCharId)
					.PropertyGroupId(playerCharId)
					.AutoCountInventory(true)
					//.SetOnChangeScriptBas(statUpdateBas)
					.AddInventorySection("HOTBAR", 0, false, 4, 1)
					.AddInventorySection("MAIN", 1, false, 9, 1)
					.AddInventorySection("ARMOR", 2, false, 1, 1)
					.AddInventorySection("SHIELD", 3, false, 1, 1)
					.SetSelectedSlot(0)
					.PrepareProgram()
					;
			}
			else if (invType == InventoryType.UltimateSurvival)
			{
				_manager.CreateCharacterDefinition(playerCharId)
					.PropertyGroupId(playerCharId)
					.AutoCountInventory(true)
					//.SetOnChangeScriptBas(statUpdateBas)
					.AddInventorySection("HOTBAR", 0, false, 6, 1)
					.AddInventorySection("MAIN", 1, false, 6, 4)
					.AddInventorySection("HEAD", 2, false, 1, 1)
					.AddInventorySection("SHIRT", 3, false, 1, 1)
					.AddInventorySection("PANTS", 4, false, 1, 1)
					.AddInventorySection("SHOES", 5, false, 1, 1)
					.SetSelectedSlot(0)
					.PrepareProgram()
					;
			}
			else if (invType == InventoryType.Minecraft)
			{
				_manager.CreateCharacterDefinition(playerCharId)
					.PropertyGroupId(playerCharId)
					.AutoCountInventory(true)
					//.SetOnChangeScriptBas(statUpdateBas)
					.AddInventorySection("HOTBAR", 0, false, 9, 1)
					.AddInventorySection("MAIN", 1, false, 9, 3)
					.AddInventorySection("HEAD", 2, false, 1, 1)
					.AddInventorySection("SHIRT", 3, false, 1, 1)
					.AddInventorySection("PANTS", 4, false, 1, 1)
					.AddInventorySection("SHOES", 5, false, 1, 1)
					.AddInventorySection("SHIELD", 6, false, 1, 1)
					.AddInventorySection("CRAFT", 7, false, 2, 2)
					.AddInventorySection("OUTPUT", 8, false, 1, 1)
					.SetSelectedSlot(0)
					.PrepareProgram()
					;
			}
		}

		public CharacterManagerBuilder AddFalloutStat
		(
			in String id,
			string name
		)
		{
			_props.DefineProperty(id, name, "STAT", false)
				.Minimum(1)
				.Maximum(10)
				.SetDefault(5)
				.RandomizeDefault(false)
				.Probability("1d10");

			return this;
		}

		public CharacterManagerBuilder AddFalloutDerivedStat
		(
			in String id, 
			string name,
			float maximum,
			bool isUtility
		)
		{
			_props.DefineProperty(id, name, "DERIVED", false)
				.Minimum(0)
				.Maximum(maximum)
				.SetDefault(maximum)
				.IsUtilitySystemProperty(isUtility);
			return this;
		}

		public CharacterManagerBuilder AddProperty
		(
			in String category,
			in String id,
			string name,
			float minimum,
			float start,
			float maximum,
			bool isUtility
		)
		{
			_props.DefineProperty(id, name, category, false)
				.Minimum(minimum)
				.Maximum(maximum)
				.SetDefault(start)
				.IsUtilitySystemProperty(isUtility);
			return this;
		}

		public CharacterManagerBuilder AddDnDStats(bool randomizeStats)
		{
			AddDndStat("STR", "Strength", randomizeStats);
			AddDndStat("INT", "Intellegence", randomizeStats);
			AddDndStat("WIS", "Wisdom", randomizeStats);
			AddDndStat("CON", "Constitution", randomizeStats);
			AddDndStat("DEX", "Dexterity", randomizeStats);
			AddDndStat("CHR", "Charisma", randomizeStats);

			return this;
		}

		public CharacterManagerBuilder AddDndStat
		(
			String id, 
			string name, 
			bool randomize
		)
		{
			_props.DefineProperty(id, name, "STAT", false)
				.Minimum(1)
				.Maximum(20)
				.SetDefault(8)
				.RandomizeDefault(randomize)
				.Probability("3d6");

			return this;
		}

		public CharacterManagerBuilder AddDndResistance(String id, string name)
		{
			_props.DefineProperty(id, name, "RES", false)
				.Minimum(0)
				.Maximum(20)
				.SetDefault(1)
				.Probability("1d20");

			return this;
		}

		public CharacterManagerBuilder AddDndSkill(String id, string name)
		{
			_props.DefineProperty(id, name, "SKILL", false)
				.Minimum(0)
				.Maximum(20)
				.SetDefault(1)
				.Probability("1d20");

			return this;
		}

		/// <summary>
		/// This is an example of how to setup a full system, useful for unit testing
		/// </summary>
		public void SetupDnDTest(bool randomizeStats = true)
		{
			StringBuilder statsRecalc = new StringBuilder();
			//StringBuilder statsRecalcOnEquip = new StringBuilder();

			statsRecalc.AppendLine("REM -== RECALC DERIVED STATS ==-");
			statsRecalc.AppendLine();
			//statsRecalcOnEquip.AppendLine("REM -== RECALC DERIVED STATS DUE TO EQUIPMENT ==-");
			//statsRecalcOnEquip.AppendLine();

			// Stats

			List<String> proplist = new List<String>();

			AddDnDStats(randomizeStats);

			proplist.Add("STR");
			proplist.Add("INT");
			proplist.Add("WIS");
			proplist.Add("CON");
			proplist.Add("DEX");
			proplist.Add("CHR");

			proplist.Add("AC");
			_props.DefineProperty("AC", "Armor Class", "DERIVED", false)
				.Minimum(0)
				.Maximum(20)
				.SetDefault(0);
			// AC = DEFENSE of armor and shild plus a dexterity bonus
			statsRecalc.AppendLine("CALL STAT('AC', INVENTORY('SUM', 'ARMOR', 'DEFENSE') + INVENTORY('SUM', 'SHIELD', 'DEFENSE') + SQR(STAT('DEX') / 2.0))");

			// Hit probability: 1d20 > THAC0 - LV - SWORD/BOW/ETC - STR + TARGET_AC + TARGET_DEX
			proplist.Add("THAC0");
			_props.DefineProperty("THAC0", "Attack", "DERIVED")
				.Minimum(1)
				.Maximum(20)
				.Probability("1d20")
				.SetDefault(20);
			// To hit AC 0: 20 - LV - (skill for equipped weapon) - STR
			statsRecalc.AppendLine("CALL STAT('THAC0', STATMAX('THAC0') - STAT('ATTACK') - SQR(STAT('LV')) - SQR(STAT('STR')) - STAT(INVENTORY('GET', SELECTED(), 'TYPE')))");

			proplist.Add("ATTACK");
			_props.DefineProperty("ATTACK", "THAC0 Modifier", "DERIVED")
				.Minimum(0)
				.Maximum(8)
				.SetDefault(0);

			proplist.Add("HP");
			_props.DefineProperty("HP", "Hit Points", "DERIVED")
				.SetupDepletionType()
				.Maximum(6)
				.SetDefault(1)
				.Probability("1d6");
			statsRecalc.AppendLine("CALL STATMAX('HP', STAT('LV') * SQR(STAT('CON')))");

			proplist.Add("LV");
			_props.DefineProperty("LV", "Level", "DERIVED")
				.Minimum(0)
				.Maximum(99)
				.SetDefault(1);

			proplist.Add("XPMOD");
			_props.DefineProperty("XPMOD", "XP Max Class Modifier", "DERIVED")
				.Minimum(0)
				.Maximum(10)
				.SetDefault(1);

			proplist.Add("XP");
			_props.DefineProperty("XP", "Experience Points", "DERIVED")
				.Minimum(0)
				.Maximum(1000)
				.SetDefault(0);
			statsRecalc.AppendLine("CALL STATMAX('XP', STAT('LV') * STAT('XPMOD') * (1000 - STAT('INT')*STAT('INT')))");

			proplist.Add("ENCB");
			_props.DefineProperty("ENCB", "Encumbrance", "DERIVED")
				.Minimum(0)
				.Maximum(20)
				.SetDefault(0);
			statsRecalc.AppendLine("CALL STATMAX('ENCB', STAT('STR') + SQR(STAT('CON')))");
			statsRecalc.AppendLine("CALL STAT('ENCB', INVENTORY('SUM', '*', 'WEIGHT'))");

			// Resistances
				
			AddDndResistance("RFIR", "Fire Resistance");
			proplist.Add("RFIR");
			AddDndResistance("RPOI", "Poison Resistance");
			proplist.Add("RPOI");
			AddDndResistance("RSHK", "Shock Resistance");
			proplist.Add("RSHK");

			// Skills

			AddDndSkill("BOW", "Weapon Bow");
			proplist.Add("BOW");
			AddDndSkill("LOCK", "Lock Pick");
			proplist.Add("LOCK");
			AddDndSkill("SNEEK", "Sneek");
			proplist.Add("SNEEK");
			AddDndSkill("SWORD", "Weapon Sword");
			proplist.Add("SWORD");

			_props.DefinePropertySet("CHAR", proplist.ToArray(), statsRecalc.ToString());

			// Race Effects

			// no race effects
			_manager.DefineEffectGroup("HUMAN", Array.Empty<String>());

			_manager.DefineStatEffect_Delta("STR-1", "STR", -1);
			_manager.DefineStatEffect_Delta("INT+1", "INT", 1);
			_manager.DefineStatEffect_Delta("DEX+1", "DEX", 1);
			_manager.DefineStatEffect_Delta("CON-1", "CON", -1);
			_manager.DefineStatEffect_Delta("BOW+1", "BOW", 1);
			_manager.DefineEffectGroup("ELF", new String[] { "STR-1", "INT+1", "DEX+1", "CON-1", "BOW+1" });

			// Class Effects

			_manager.DefineStatEffect_Delta("SWORD+1", "SWORD", 1);
			_manager.DefineStatEffect_Set("XPMOD+F", "XPMOD", 1.1f);
			_manager.DefineStatEffect_Delta("ATK+2", "ATTACK", 2);
			_manager.DefineEffectGroup("FIGHTER", new String[] { "SWORD+1", "XPMOD+F", "ATK+2" });

			_manager.DefineStatEffect_Set("XPMOD+A", "XPMOD", 1.2f);
			_manager.DefineStatEffect_Delta("ATK+1", "ATTACK", 1);
			_manager.DefineEffectGroup("ARCHER", new String[] { "BOW+1", "XPMOD+A", "ATK+1" });

			_manager.DefineStatEffect_Set("STR 12", "STR", 12);
			_manager.DefineStatEffect_Set("CHR 5", "CHR", 5);
			_manager.DefineStatEffect_Set("CON 12", "CON", 12);
			_manager.DefineStatEffect_Set("AC 4", "AC", 4);
			_manager.DefineEffect_RangeMax("HPMAX 8", "HP", 8);
			_manager.DefineStatEffect_Set("HP 8", "HP", 8);
			_manager.DefineStatEffect_Set("THAC0 5", "THAC0", 5);
			_manager.DefineEffectGroup("ORC", new String[] { "STR 12", "CHR 5", "CON 12", "HPMAX 8", "AC 4", "HP 8", "THAC0 5" });

			// Items

			_items.DefineCategory("Armor");
			_items.DefineCategory("Consumable");
			_items.DefineCategory("Melee");
			_items.DefineCategory("Useable");
			_items.DefineCategory("Resource");
			_items.DefineCategory("Ranged");
			_items.DefineCategory("Shield");
			_items.DefineCategory("Throwable");
			_items.DefineCategory("Tool");

			if (!_items.HasProperty("TYPE"))
			{
				// Weapon skill type: BOW, SWORD
				_items.DefineProperty("TYPE", ItemPropertyType.String, "Weapon Skill Type", false);
			}
			if (! _items.HasProperty("DEFENSE"))
			{
				// AC modifer
				_items.DefineProperty("DEFENSE", ItemPropertyType.Int, "Defense Modifer", false);
			}
			if (!_items.HasProperty("DAMAGE"))
			{
				// damage on hit, fe 1d8
				_items.DefineProperty("DAMAGE", ItemPropertyType.DiceRoll, "Weapon Damage Roll", false);
			}
			if (!_items.HasProperty("WEIGHT"))
			{
				// item weight in encumbrance units (20 max, so 1 == about 10 lbs)
				_items.DefineProperty("WEIGHT", ItemPropertyType.Float, "Weight", false);
			}

			if (!_items.HasItemDefined("ORKSKIN"))
			{
				_items.DefineItem("Armor", "ORKSKIN")
					.DisplayName("Orc Hide")
					.MaxStackCapacity(1)
					.AddProperty("WEIGHT", 1)
					.AddProperty("DEFENSE", 4)
					.Build();
			}
			if (! _items.HasItemDefined("CLUB6"))
			{
				_items.DefineItem("Melee", "CLUB6")
					.DisplayName("Wooden Club")
					.MaxStackCapacity(1)
					.AddProperty("WEIGHT", 2)
					.AddProperty("TYPE", "BLUNT")
					.AddProperty("DAMAGE", "1d6")
					.Build();
			}

			// Character Templates

			_manager.CreateCharacterDefinition("PLAYER")
				.PropertyGroupId("CHAR")
				.AutoCountInventory(true)
				//.SetOnChangeScriptBas(statsRecalc.ToString())
				.AddInventorySection("HOTBAR", 0, false, 4, 1)
				.AddInventorySection("MAIN", 1, false, 9, 1)
				.AddInventorySection("ARMOR", 2, false, 1, 1)
				.AddInventorySection("SHIELD", 3, false, 1, 1)
				.SetSelectedSlot(0)
				.PrepareProgram()
				;

			_manager.CreateCharacterDefinition("MONSTER")
				.PropertyGroupId("CHAR")
				.AutoCountInventory(true)
				.AddInventorySection("WEAPON", 0, false, 1, 1)
				.AddInventorySection("MAIN", 1, false, 3, 1)
				.AddInventorySection("ARMOR", 2, false, 1, 1)
				.AddInventorySection("SHIELD", 3, false, 1, 1)
				.SetSelectedSlot(0)
				.AddDefaultItem("ORC",
					new DefaultItemSpec {
						ItemId = "ORKSKIN",
						Count = 1,
						WindowSectionName = "ARMOR",
						WindowSectionIndex = 0,
						Properties = new ItemPropertyDefault[] {
							new ItemPropertyDefault { PropId = "WEIGHT", Default = 1 },
							new ItemPropertyDefault { PropId = "DEFENSE", Default = 4 },
					}
				})
				.AddDefaultItem("ORC",
					new DefaultItemSpec {
						ItemId = "CLUB6",
						Count = 1,
						WindowSectionName = "WEAPON",
						WindowSectionIndex = 0,
						Properties = new ItemPropertyDefault[] {
						new ItemPropertyDefault { PropId = "WEIGHT", Default = 2 },
						new ItemPropertyDefault { PropId = "TYPE", Default = "BLUNT" },
						new ItemPropertyDefault { PropId = "DAMAGE", Default = "1d6" },
					}
				})
				;
		}
	}
}
