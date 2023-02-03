using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public enum ItemRequirementType
	{
		Any = 0,
		ItemId,
		Category,
		HasProperty,
		PropertyValueEquals
	}

	[Serializable]
	public class ItemRequirement
	{
		public readonly ItemRequirementType RequirementType;
		public readonly string NameRequirement;
		public readonly Variant8 ValueRequirement;

		public bool MeetsRequirement(ItemStack item)
		{
			bool ret = false;

			switch (RequirementType)
			{
				case ItemRequirementType.Any:
					ret = true;
					break;
				case ItemRequirementType.ItemId:
					ret = item.Definition.ItemId == NameRequirement;
					break;
				case ItemRequirementType.Category:
					ret = item.Definition.Category.Equals(NameRequirement);
					break;
				case ItemRequirementType.HasProperty:
					ret = item.HasProperty(NameRequirement);
					break;
				case ItemRequirementType.PropertyValueEquals:
					ret = item.IsPropertyEquals(NameRequirement, ValueRequirement);
					break;
			}

			return ret;
		}

		public ItemRequirement()
		{
		}

		public ItemRequirement(ItemRequirementType requirementType, string nameRequirement, Variant8 valueRequirement)
		{
			RequirementType = requirementType;
			NameRequirement = nameRequirement;
			ValueRequirement = valueRequirement;
		}

		public ItemRequirement(ItemRequirementType requirementType, string nameRequirement)
		{
			RequirementType = requirementType;
			NameRequirement = nameRequirement;
		}
	}
}
