using System;
using System.Collections.Generic;

using Portland.Basic;
using Portland.Collections;
using Portland.Interp;
using Portland.Mathmatics;
using Portland.Types;

namespace Portland.RPG
{
	public sealed class DefaultItemSpec
	{
		public String ItemId;
		public int Count;
		public String WindowSectionName;
		//public int WindowSectionIndex;
		public ItemPropertyDefault[] Properties;
	}

	public struct ItemPropertyDefault
	{
		public String PropId;
		public Variant8 Default;
	}

	public sealed class CharacterDefinition
	{
		sealed class InventorySection
		{
			public String SectionName;
			public int SectionTypeId;
			public int Start;
			public int Width;
			public int Height;
			public bool IsReadOnly;
			public Vector<ItemRequirement> Requirements = new Vector<ItemRequirement>(3);
		}

		public String CharId;
		public String PropertyGroupId = String.Empty;
		public String UtilitySetId = String.Empty;

		public BasicProgram OnInventoryChangeRun = new BasicProgram();
		public BasicProgram OnLevelChangeRun = new BasicProgram();
		public BasicProgram OnEffectRun = new BasicProgram();

		public Vector<EffectGroup> EffectGroups = new Vector<EffectGroup>(4);

		public int TotalInventorySlots;
		public int DefaultSelectedInventorySlot;
		Vector<InventorySection> _inventorySections = new Vector<InventorySection>(6);

		public Dictionary<String, List<DefaultItemSpec>> DefaultItems = new Dictionary<String, List<DefaultItemSpec>>();

		public void AddInventorySection
		(
			in String name, 
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
		IPropertyManager _props;
		Dictionary<String, EffectGroup> _effectGroups;

		public CharacterDefinitionBuilder(CharacterDefinition chr, IPropertyManager props, Dictionary<String, EffectGroup> effectGroups)
		{
			Character = chr;
			_props = props;
			_effectGroups = effectGroups;
		}

		public CharacterDefinitionBuilder PropertyGroupId(in String id)
		{
			Character.PropertyGroupId = id;
			return this;
		}

		public CharacterDefinitionBuilder UtilitySetId(in String id)
		{
			Character.UtilitySetId = id;
			return this;
		}

		public CharacterDefinitionBuilder AutoCountInventory(bool autoCount)
		{
			_autoCountInventorySlots = autoCount;
			return this;
		}

		public CharacterDefinitionBuilder AddInventorySection
		(
			in String name, 
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

		//public CharacterDefinitionBuilder SetOnChangeScriptBas(string bas)
		//{
		//	Character.OnStatChangeBas = bas;
		//	return this;
		//}

		public CharacterDefinitionBuilder AddEffectGroup(in String groupId)
		{
			Character.EffectGroups.Add(_effectGroups[groupId]);
			return this;
		}

		public CharacterDefinitionBuilder AddDefaultItem(in String raceOrClass, DefaultItemSpec item)
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
			if (_props.TryGetDefinitionSet(Character.PropertyGroupId, out var grp))
			{
				Character.OnLevelChangeRun.Parse(grp.OnLevelScript);
				Character.OnInventoryChangeRun.Parse(grp.OnInventoryScript);
				Character.OnEffectRun.Parse(grp.OnEffectScript);
			}
			else
			{
				throw new Exception($"Unknown property set {Character.PropertyGroupId}");
			}
		}
	}
}
