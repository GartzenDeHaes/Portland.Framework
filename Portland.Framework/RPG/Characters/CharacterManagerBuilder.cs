using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Types;

using static Portland.RPG.CharacterDefinition;

namespace Portland.RPG
{
	public static class CharacterManagerBuilder
	{
		private static void AddDndStat
		(
			PropertyManager props,
			String id, 
			string name, 
			bool randomize,
			List<String> proplist
		)
		{
			proplist.Add(id);

			props.DefineProperty(id, name, "STAT")
				.SetMinimum(1)
				.SetMaximum(20)
				.SetDefault(8)
				.SetRandomizeDefault(randomize)
				.SetProbability("3d6");
		}

		private static void AddDndResistance(PropertyManager props, String id, string name, List<String> proplist)
		{
			proplist.Add(id);

			props.DefineProperty(id, name, "RES")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(1)
				.SetProbability("1d20");
		}

		private static void AddDndSkill(PropertyManager props, String id, string name, List<String> proplist)
		{
			proplist.Add(id);

			props.DefineProperty(id, name, "SKILL")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(1)
				.SetProbability("1d20");
		}

		/// <summary>
		/// This is an example of how to setup a full system, useful for unit testing
		/// </summary>
		public static CharacterManager CreateDnDTest(ItemFactory items, bool randomizeStats = true)
		{
			StringBuilder statsRecalc = new StringBuilder();
			//StringBuilder statsRecalcOnEquip = new StringBuilder();

			statsRecalc.AppendLine("REM -== RECALC DERIVED STATS ==-");
			statsRecalc.AppendLine();
			//statsRecalcOnEquip.AppendLine("REM -== RECALC DERIVED STATS DUE TO EQUIPMENT ==-");
			//statsRecalcOnEquip.AppendLine();

			// Stats

			PropertyManager props = new PropertyManager();

			List<String> proplist = new List<String>();

			AddDndStat(props, "STR", "Strength", randomizeStats, proplist);
			AddDndStat(props, "INT", "Intellegence", randomizeStats, proplist);
			AddDndStat(props, "WIS", "Wisdom", randomizeStats, proplist);
			AddDndStat(props, "CON", "Constitution", randomizeStats, proplist);
			AddDndStat(props, "DEX", "Dexterity", randomizeStats, proplist);
			AddDndStat(props, "CHR", "Charisma", randomizeStats, proplist);

			proplist.Add("AC");
			props.DefineProperty("AC", "Armor Class", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(0);
			// AC = DEFENSE of armor and shild plus a dexterity bonus
			statsRecalc.AppendLine("CALL STAT('AC', INVENTORY('SUM', 'ARMOR', 'DEFENSE') + INVENTORY('SUM', 'SHIELD', 'DEFENSE') + SQR(STAT('DEX') / 2.0))");

			// Hit probability: 1d20 > THAC0 - LV - SWORD/BOW/ETC - STR + TARGET_AC + TARGET_DEX
			proplist.Add("THAC0");
			props.DefineProperty("THAC0", "Attack", "DSTATS")
				.SetMinimum(1)
				.SetMaximum(20)
				.SetProbability("1d20")
				.SetDefault(20);
			// To hit AC 0: 20 - LV - (skill for equipped weapon) - STR
			statsRecalc.AppendLine("CALL STAT('THAC0', STATMAX('THAC0') - STAT('ATTACK') - SQR(STAT('LV')) - SQR(STAT('STR')) - STAT(INVENTORY('GET', SELECTED(), 'TYPE')))");

			proplist.Add("ATTACK");
			props.DefineProperty("ATTACK", "THAC0 Modifier", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(8)
				.SetDefault(0);

			proplist.Add("HP");
			props.DefineProperty("HP", "Hit Points", "DSTATS")
				.SetupDepletionType()
				.SetMaximum(6)
				.SetDefault(1)
				.SetProbability("1d6");
			statsRecalc.AppendLine("CALL STATMAX('HP', STAT('LV') * SQR(STAT('CON')))");

			proplist.Add("LV");
			props.DefineProperty("LV", "Level", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(99)
				.SetDefault(1);

			proplist.Add("XPMOD");
			props.DefineProperty("XPMOD", "XP Max Class Modifier", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(10)
				.SetDefault(1);

			proplist.Add("XP");
			props.DefineProperty("XP", "Experience Points", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(1000)
				.SetDefault(0);
			statsRecalc.AppendLine("CALL STATMAX('XP', STAT('LV') * STAT('XPMOD') * (1000 - STAT('INT')*STAT('INT')))");

			proplist.Add("ENCB");
			props.DefineProperty("ENCB", "Encumbrance", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(0);
			statsRecalc.AppendLine("CALL STATMAX('ENCB', STAT('STR') + SQR(STAT('CON')))");
			statsRecalc.AppendLine("CALL STAT('ENCB', INVENTORY('SUM', '*', 'WEIGHT'))");

			// Resistances

			AddDndResistance(props, "RFIR", "Fire Resistance", proplist);
			AddDndResistance(props, "RPOI", "Poison Resistance", proplist);
			AddDndResistance(props, "RSHK", "Shock Resistance", proplist);

			// Skills

			AddDndSkill(props, "BOW", "Weapon Bow", proplist);
			AddDndSkill(props, "LOCK", "Lock Pick", proplist);
			AddDndSkill(props, "SNEEK", "Sneek", proplist);
			AddDndSkill(props, "SWORD", "Weapon Sword", proplist);

			props.DefinePropertySet("CHAR", proplist.ToArray());

			// Race Effects

			CharacterManager mgr = new CharacterManager(Variant8.StrTab, props, items);

			// no race effects
			mgr.DefineEffectGroup("HUMAN", Array.Empty<String>());

			mgr.DefineStatEffect_Delta("STR-1", "STR", -1);
			mgr.DefineStatEffect_Delta("INT+1", "INT", 1);
			mgr.DefineStatEffect_Delta("DEX+1", "DEX", 1);
			mgr.DefineStatEffect_Delta("CON-1", "CON", -1);
			mgr.DefineStatEffect_Delta("BOW+1", "BOW", 1);
			mgr.DefineEffectGroup("ELF", new String[] { "STR-1", "INT+1", "DEX+1", "CON-1", "BOW+1" });

			// Class Effects

			mgr.DefineStatEffect_Delta("SWORD+1", "SWORD", 1);
			mgr.DefineStatEffect_Set("XPMOD+F", "XPMOD", 1.1f);
			mgr.DefineStatEffect_Delta("ATK+2", "ATTACK", 2);
			mgr.DefineEffectGroup("FIGHTER", new String[] { "SWORD+1", "XPMOD+F", "ATK+2" });

			mgr.DefineStatEffect_Set("XPMOD+A", "XPMOD", 1.2f);
			mgr.DefineStatEffect_Delta("ATK+1", "ATTACK", 1);
			mgr.DefineEffectGroup("ARCHER", new String[] { "BOW+1", "XPMOD+A", "ATK+1" });

			mgr.DefineStatEffect_Set("STR 12", "STR", 12);
			mgr.DefineStatEffect_Set("CHR 5", "CHR", 5);
			mgr.DefineStatEffect_Set("CON 12", "CON", 12);
			mgr.DefineStatEffect_Set("AC 4", "AC", 4);
			mgr.DefineEffect_RangeMax("HPMAX 8", "HP", 8);
			mgr.DefineStatEffect_Set("HP 8", "HP", 8);
			mgr.DefineStatEffect_Set("THAC0 5", "THAC0", 5);
			mgr.DefineEffectGroup("ORC", new String[] { "STR 12", "CHR 5", "CON 12", "HPMAX 8", "AC 4", "HP 8", "THAC0 5" });

			// Items

			items.DefineCategory("Armor");
			items.DefineCategory("Consumable");
			items.DefineCategory("Melee");
			items.DefineCategory("Useable");
			items.DefineCategory("Resource");
			items.DefineCategory("Ranged");
			items.DefineCategory("Shield");
			items.DefineCategory("Throwable");
			items.DefineCategory("Tool");

			if (!items.HasProperty("TYPE"))
			{
				// Weapon skill type: BOW, SWORD
				items.DefineProperty("TYPE", ItemPropertyType.String, "Weapon Skill Type", false);
			}
			if (! items.HasProperty("DEFENSE"))
			{
				// AC modifer
				items.DefineProperty("DEFENSE", ItemPropertyType.Int, "Defense Modifer", false);
			}
			if (!items.HasProperty("DAMAGE"))
			{
				// damage on hit, fe 1d8
				items.DefineProperty("DAMAGE", ItemPropertyType.DiceRoll, "Weapon Damage Roll", false);
			}
			if (!items.HasProperty("WEIGHT"))
			{
				// item weight in encumbrance units (20 max, so 1 == about 10 lbs)
				items.DefineProperty("WEIGHT", ItemPropertyType.Float, "Weight", false);
			}

			if (!items.HasItemDefined("ORKSKIN"))
			{
				items.DefineItem("Armor", "ORKSKIN")
					.DisplayName("Orc Hide")
					.MaxStackCapacity(1)
					.AddProperty("WEIGHT", 1)
					.AddProperty("DEFENSE", 4)
					.Build();
			}
			if (! items.HasItemDefined("CLUB6"))
			{
				items.DefineItem("Melee", "CLUB6")
					.DisplayName("Wooden Club")
					.MaxStackCapacity(1)
					.AddProperty("WEIGHT", 2)
					.AddProperty("TYPE", "BLUNT")
					.AddProperty("DAMAGE", "1d6")
					.Build();
			}

			// Character Templates

			mgr.CreateCharacterDefinition("PLAYER")
				.PropertyGroupId("CHAR")
				.AutoCountInventory(true)
				.SetOnChangeScriptBas(statsRecalc.ToString())
				.AddInventorySection("HOTBAR", 0, false, 4, 1)
				.AddInventorySection("MAIN", 1, false, 9, 1)
				.AddInventorySection("ARMOR", 2, false, 1, 1)
				.AddInventorySection("SHIELD", 3, false, 1, 1)
				.SetSelectedSlot(0)
				.PrepareProgram()
				;

			mgr.CreateCharacterDefinition("MONSTER")
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

			return mgr;
		}
	}
}
