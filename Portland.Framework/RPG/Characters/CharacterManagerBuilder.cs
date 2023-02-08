using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.RPG
{
	public static class CharacterManagerBuilder
	{
		private static void AddDndStat(PropertyManager props, AsciiId4 id, string name, List<AsciiId4> proplist)
		{
			proplist.Add(id);

			props.DefineProperty(id, name, "STAT")
				.SetMinimum(1)
				.SetMaximum(20)
				.SetDefault(8)
				.SetRandomizeDefault(true)
				.SetProbability("3d6");
		}

		private static void AddDndResistance(PropertyManager props, AsciiId4 id, string name, List<AsciiId4> proplist)
		{
			proplist.Add(id);

			props.DefineProperty(id, name, "RES")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(1)
				.SetProbability("1d20");
		}

		private static void AddDndSkill(PropertyManager props, AsciiId4 id, string name, List<AsciiId4> proplist)
		{
			proplist.Add(id);

			props.DefineProperty(id, name, "SKILL")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(1)
				.SetProbability("1d20");
		}

		public static CharacterManager CreateDnD_Defaults()
		{
			// Stats

			PropertyManager props = new PropertyManager();

			List<AsciiId4> proplist = new List<AsciiId4>();

			AddDndStat(props, "STR", "Strength", proplist);
			AddDndStat(props, "INT", "Intellegence", proplist);
			AddDndStat(props, "WIS", "Wisdom", proplist);
			AddDndStat(props, "CON", "Constitution", proplist);
			AddDndStat(props, "DEX", "Dexterity", proplist);
			AddDndStat(props, "CHR", "Charisma", proplist);

			proplist.Add("AC");
			props.DefineProperty("AC", "Armor Class", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(0);

			// Hit probability: 1d20 > THAC0 - LV - SWORD/BOW/ETC - STR + TARGET_AC + TARGET_DEX
			proplist.Add("THAC0");
			props.DefineProperty("THAC0", "Attack", "DSTATS")
				.SetMinimum(1)
				.SetMaximum(20)
				.SetProbability("1d20")
				.SetDefault(18);

			proplist.Add("HP");
			props.DefineProperty("HP", "Hit Points", "DSTATS")
				.SetupDepletionType()
				.SetMaximum(6)
				.SetDefault(1)
				.SetProbability("1d6");

			proplist.Add("LV");
			props.DefineProperty("LV", "Level", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(99)
				.SetDefault(1);

			proplist.Add("XP");
			props.DefineProperty("XP", "Experience Points", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(1000)
				.SetDefault(0);

			proplist.Add("ENCB");
			props.DefineProperty("ENCB", "Encumbrance", "DSTATS")
				.SetMinimum(0)
				.SetMaximum(20)
				.SetDefault(0);

			AddDndResistance(props, "RFIR", "Fire Resistance", proplist);
			AddDndResistance(props, "RPOI", "Poison Resistance", proplist);
			AddDndResistance(props, "RSHK", "Shock Resistance", proplist);

			AddDndSkill(props, "BOW", "Weapon Bow", proplist);
			AddDndSkill(props, "LOCK", "Lock Pick", proplist);
			AddDndSkill(props, "SNEK", "Sneek", proplist);
			AddDndSkill(props, "SWORD", "Weapon Sword", proplist);
			AddDndSkill(props, "TALK", "Speech", proplist);

			props.DefinePropertySet("CHAR", proplist.ToArray());

			// Race Effects

			CharacterManager mgr = new CharacterManager(Variant8.StrTab, props);

			// no race effects
			mgr.DefineEffectGroup("HUMAN", Array.Empty<String8>(), Array.Empty<PropertyRequirement>());

			mgr.DefineEffect("STR-1", "STR", -1, EffectValueType.CurrentDelta);
			mgr.DefineEffect("INT+1", "INT", 1, EffectValueType.CurrentDelta);
			mgr.DefineEffect("DEX+1", "DEX", 1, EffectValueType.CurrentDelta);
			mgr.DefineEffect("CON-1", "CON", -1, EffectValueType.CurrentDelta);
			mgr.DefineEffect("BOW+1", "BOW", 1, EffectValueType.CurrentDelta);
			mgr.DefineEffect("TALK+1", "TALK", 1, EffectValueType.CurrentDelta);
			mgr.DefineEffectGroup("ELF", new String8[] { "STR-1", "INT+1", "DEX+1", "CON-1", "TALK+1", "BOW+1" }, Array.Empty<PropertyRequirement>());

			mgr.DefineEffect("STR 12", "STR", 12, EffectValueType.CurrentAbs);
			mgr.DefineEffect("CHR 5", "CHR", 5, EffectValueType.CurrentAbs);
			mgr.DefineEffect("HP 1d8", "HP", "1d8", EffectValueType.Probability);
			mgr.DefineEffect("HP 8", "HP", 8, EffectValueType.CurrentAbs);
			mgr.DefineEffect("AC 4", "AC", 4, EffectValueType.CurrentAbs);
			mgr.DefineEffect("THAC0-15", "THAC0", -10, EffectValueType.CurrentDelta);
			mgr.DefineEffectGroup("ORC", new String8[] { "STR 12", "CHR 5", "HP 1d8", "HP 8", "AC 4", "CON-1", "THAC0-15" }, Array.Empty<PropertyRequirement>());

			// Class Effects

			mgr.DefineEffect("SWORD+1", "SWORD", 1, EffectValueType.CurrentDelta);
			mgr.DefineEffect("XPMAX+F", "XP", 750, EffectValueType.MaxDelta);
			mgr.DefineEffect("THAC0-2", "THAC0", -2, EffectValueType.CurrentDelta);
			mgr.DefineEffectGroup("FIGHTER", new String8[] { "SWORD+1", "XPMAX+F", "THAC0-2" }, Array.Empty<PropertyRequirement>());

			mgr.DefineEffect("XPMAX+A", "XP", 1250, EffectValueType.MaxDelta);
			mgr.DefineEffect("THAC0-1", "THAC0", -1, EffectValueType.CurrentDelta);
			mgr.DefineEffectGroup("ARCHER", new String8[] { "BOW+1", "XPMAX+A", "THAC0-1" }, Array.Empty<PropertyRequirement>());

			// Derived Effect Calculations

			// Character Template


			// Inventory

			return mgr;
		}
	}
}
