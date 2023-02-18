using System;

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
		public String PropertyId;
		public ItemPropertyType PropertyType;
		public string DisplayName;
		public ResourceDescription LocalizedDisplayName;
		public bool IsInstancedPerItem;
	}
}
