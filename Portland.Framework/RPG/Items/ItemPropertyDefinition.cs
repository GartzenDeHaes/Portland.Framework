using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.RPG
{
	public enum ItemPropertyType
	{
		Flag,
		Bool,
		Int,
		IntRange,
		RandomInt,
		Float,
		FloatRange,
		RandomFloat,
		String,
		Sound,
		DiceRoll
	}

	[Serializable]
	public class ItemPropertyDefinition
	{
		public String8 PropertyId;
		public ItemPropertyType PropertyType;
		public string DisplayName;
		public ResourceDescription LocalizedDisplayName;
		public bool IsInstancedPerItem;
	}
}
