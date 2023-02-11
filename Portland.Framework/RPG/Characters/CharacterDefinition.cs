﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Basic;
using Portland.Collections;
using Portland.Interp;
using Portland.Mathmatics;

using static Portland.RPG.CharacterDefinition;

namespace Portland.RPG
{
	public sealed class CharacterDefinition
	{
		sealed class InventorySection
		{
			public String8 SectionName;
			public int SectionTypeId;
			public int Start;
			public int Width;
			public int Height;
			public bool IsReadOnly;
			public Vector<ItemRequirement> Requirements = new Vector<ItemRequirement>(3);
		}

		public struct ItemPropertyDefault
		{
			public String8 PropId;
			public Variant8 Default;
		}

		public sealed class DefaultItemSpec
		{
			public String8 ItemId;
			public int Count;
			public String8 WindowSectionName;
			public int WindowSectionIndex;
			public ItemPropertyDefault[] Properties;
		}

		public String8 CharId;
		public String8 PropertyGroupId;

		public string OnStatChangeBas = String.Empty;
		public BasicProgram OnStatChangeRun = new BasicProgram();
		//public string OnEquipChangeBas;

		public Vector<EffectGroup> EffectGroups = new Vector<EffectGroup>(4);

		public int TotalInventorySlots;
		public int DefaultSelectedInventorySlot;
		Vector<InventorySection> _inventorySections = new Vector<InventorySection>(6);

		public Dictionary<String8, List<DefaultItemSpec>> DefaultItems = new Dictionary<String8, List<DefaultItemSpec>>();

		public void AddInventorySection
		(
			in String8 name, 
			int sectionTypeId,
			bool isReadOnly, 
			int start, 
			int width, 
			int height, 
			ItemRequirement[] requirements
		)
		{
			_inventorySections.Add(new InventorySection { SectionName = name, SectionTypeId = sectionTypeId, Start = start, Width = width, Height = height, IsReadOnly = isReadOnly });
		}

		public InventoryWindow CreateInventoryWindow(ItemCollection inv)
		{
			List<InventoryWindowGrid> grids = new List<InventoryWindowGrid>();

			for (int i = 0; i < _inventorySections.Count; i++)
			{
				var s = _inventorySections[i];

				grids.Add(new InventoryWindowGrid(s.SectionTypeId, s.Start, inv, s.Start, s.SectionName, s.Width, s.Height, s.IsReadOnly, s.Requirements.ToArray()));
			}

			return new InventoryWindow(inv.CollectionName, 0, 0, grids.ToArray());
		}
	}

	public class CharacterDefinitionBuilder
	{
		CharacterDefinition Character;
		int _invNext;
		bool _autoCountInventorySlots;
		Dictionary<String8, EffectGroup> _effectGroups;

		public CharacterDefinitionBuilder(CharacterDefinition chr, Dictionary<String8, EffectGroup> effectGroups)
		{
			Character = chr;
			_effectGroups = effectGroups;
		}

		public CharacterDefinitionBuilder PropertyGroupId(in String8 id)
		{
			Character.PropertyGroupId = id;
			return this;
		}

		public CharacterDefinitionBuilder AutoCountInventory(bool autoCount)
		{
			_autoCountInventorySlots = autoCount;
			return this;
		}

		public CharacterDefinitionBuilder AddInventorySection
		(
			in String8 name, 
			int sectionTypeId,
			bool isReadOnly, 
			int width, 
			int height
		)
		{
			Character.AddInventorySection(name, sectionTypeId, isReadOnly, _invNext, width, height, Array.Empty<ItemRequirement>());
			_invNext += width * height;
			if (_autoCountInventorySlots)
			{
				Character.TotalInventorySlots = _invNext;
			}
			return this;
		}

		public CharacterDefinitionBuilder SetSelectedSlot(int slot)
		{
			Character.DefaultSelectedInventorySlot = slot;
			return this;
		}

		public CharacterDefinitionBuilder SetOnChangeScriptBas(string bas)
		{
			Character.OnStatChangeBas = bas;
			return this;
		}

		public CharacterDefinitionBuilder AddEffectGroup(in String8 groupId)
		{
			Character.EffectGroups.Add(_effectGroups[groupId]);
			return this;
		}

		public CharacterDefinitionBuilder AddDefaultItem(in String8 raceOrClass, DefaultItemSpec item)
		{
			if (! Character.DefaultItems.TryGetValue(raceOrClass, out var lst))
			{
				lst = new List<DefaultItemSpec>();
				Character.DefaultItems.Add(raceOrClass, lst);
			}

			lst.Add(item);

			return this;
		}

		public void PrepareProgram()
		{
			var prog = Character.OnStatChangeRun;

			prog.GetFunctionBuilder()
			.AddAllBuiltin()
			// Get the current value of a stat
			// float: STAT("HP")
			.Add("STAT", 1, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetValue(name.ToString(), out float value))
				{
					ctx.Context.Set(value);
				}
				else
				{
					ctx.SetError($"{"STAT"}('{name}'): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			})
			// Set the current value of a stat
			// STAT("STR", STAT("STR") + 1)
			.Add("STAT", 2, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
				var name = ctx.Context["a"];
				if (!chr.Stats.TrySetValue(name.ToString(), ctx.Context["b"]))
				{
					ctx.SetError($"{"STAT"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.Context.Set(ctx.Context["b"]);
				}
			})
			// Returns the maximum range for a stat
			// float: STATMAX("HP")
			.Add("STATMAX", 1, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetMaximum(name.ToString(), out float value))
				{
					ctx.Context.Set(value);
				}
				else
				{
					ctx.SetError($"{"STATMAX"}('{name}'): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			})
			// Set the maximum value for a stat, use for XP, HP
			// STATMAX("HP", STATMAX("HP") + STATROLL("HP"))
			.Add("STATMAX", 2, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
				var name = ctx.Context["a"];
				if (!chr.Stats.TrySetMaximum(name.ToString(), ctx.Context["b"]))
				{
					ctx.SetError($"{"STATMAX"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			})
			// Returns the probability set for a stat in dice notation (3d8)
			// string: STATDICE("HP")
			.Add("STATDICE", 1, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetProbability(name.ToString(), out var value))
				{
					ctx.Context.Set(value.ToString());
				}
				else
				{
					ctx.SetError($"{"STATDICE"}('{name}'): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			})
			// Set the probability for a stat (can be used to adjust HP levelup amount, fe)
			// float: STATDICE("HP", "1d4")
			.Add("STATDICE", 2, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
				var name = ctx.Context["a"];
				string dicetxt = ctx.Context["b"];

				if (!DiceTerm.TryParse(dicetxt, out var dice))
				{
					ctx.SetError($"{"STATDICE"}('{dicetxt}') INVALID DICE TERM");
					ctx.Context.Set(0f);
				}
				else if (!chr.Stats.TrySetProbability(name.ToString(), dice))
				{
					ctx.SetError($"{"STATDICE"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			})
			// Dice roll for the probability set for a stat
			// float: STATROLL("HP")
			.Add("STATROLL", 1, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
				var name = ctx.Context["a"];
				if (chr.Stats.TryGetProbability(name.ToString(), out var value))
				{
					ctx.Context.Set(value.Roll(MathHelper.Rnd));
				}
				else
				{
					ctx.SetError($"{"STATROLL"}('{name}'): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			})
			// Random number based on dice roll
			// float: ROLLDICE("1d4+2")
			.Add("ROLLDICE", 1, (ExecutionContext ctx) => {
				string dicetxt = ctx.Context["a"];
				if (DiceTerm.TryParse(dicetxt, out var dice))
				{
					ctx.Context.Set(dice.Roll(MathHelper.Rnd));
				}
				else
				{
					ctx.SetError($"{"ROLLDICE"}('{dicetxt}'): INVALID DICE TERM");
					ctx.Context.Set(0f);
				}
			})
			.Add("SELECTED", 0, (ctx) => {
				ctx.Context.Set(((Character)ctx.UserData).InventoryWindow.SelectedSlot);
			})
			// Sum all of the named properties in a window area grid, fe DEFENCE in the equipment armor grid to calcuate AC
			// INVENTORY("SUM", "WINDOW AREA NAME", "PROPERTY NAME")
			.Add("INVENTORY", 3, (ExecutionContext ctx) => {
				var chr = (Character)ctx.UserData;
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
								ctx.Context.Set((int)value8);
							}
							else if (value8.TypeIs == VariantType.Float)
							{
								ctx.Context.Set((int)value8);
							}
							else
							{
								ctx.Context.Set((string)value8);
							}
						}
						else
						{
							ctx.Context.Set(new Variant());
						}
					}
				}
				else if (op.Equals("SUM"))
				{
					if (window.Length == 0 || (window.Equals("*")))
					{
						// Sum property for entire inventory, WEIGHT fe
						chr.InventoryWindow.TrySumItemProp(propName, out float amt);
						ctx.Context.Set(amt);
					}
					else
					{
						// Sum property for window
						chr.InventoryWindow.TrySumItemProp(window, propName, out float amt);
						ctx.Context.Set(amt);
					}
				}
				else
				{
					ctx.SetError($"{"INVENTORY"}('{op}', '{window}', '{propName}'): INVALID OPERATION '{op}'");
					ctx.Context.Set(0f);
				}
			})
			;

			prog.Parse(Character.OnStatChangeBas);
		}
	}
}
