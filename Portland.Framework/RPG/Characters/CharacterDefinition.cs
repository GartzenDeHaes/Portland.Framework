using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Basic;
using Portland.Collections;

namespace Portland.RPG
{
	public class CharacterDefinition
	{
		class InventorySection
		{
			public String8 Name;
			public int Start;
			public int Length;
			public Vector<ItemRequirement> Requirements = new Vector<ItemRequirement>(3);
		}

		public String8 CharId;
		public String8 PropertyGroupId;

		public StringBuilder DerivedStatsRecalcBas = new StringBuilder();

		public int TotalInventorySlots;
		Vector<InventorySection> _inventorySections = new Vector<InventorySection>(8);

		public void AddInventorySection(in String8 name, int start, int length, ItemRequirement[] requirements)
		{
			_inventorySections.Add(new InventorySection { Name = name, Start = start, Length = length });
		}
	}

	public struct CharacterDefinitionBuilder
	{
		public CharacterDefinition Character;
		int _invNext;

		public CharacterDefinitionBuilder PropertyGroupId(in String8 id)
		{
			Character.PropertyGroupId = id;
			return this;
		}

		public CharacterDefinitionBuilder AddInventorySection(in String8 name, int length)
		{
			Character.AddInventorySection(name, _invNext, length, Array.Empty<ItemRequirement>());
			_invNext += length;
			return this;
		}
	}
}
