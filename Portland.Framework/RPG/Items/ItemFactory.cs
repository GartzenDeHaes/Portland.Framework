using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public class ItemFactory
	{
		Dictionary<String8, ItemPropertyDefinition> _propertyDefinitions = new Dictionary<String8, ItemPropertyDefinition>();
		Dictionary<String8, ItemDefinition> _itemDefinitions = new Dictionary<String8, ItemDefinition>();
		//StatFactory _stats;
		List<string> _categories = new List<string>();

		public ItemStack CreateItem(int collectionIndex, in String8 itemId, int stackCount = 1)
		{
			if (_itemDefinitions.TryGetValue(itemId, out var def))
			{
				return new ItemStack(collectionIndex, def, stackCount);
			}

			throw new Exception($"Unknown item Id {itemId}");
		}

		public void DefineProperty(in String8 propId, ItemPropertyType type, string displayName, bool isInstancedPerItem)
		{
			if (_propertyDefinitions.TryGetValue(propId, out var def))
			{
				throw new Exception($"Property {propId} already exists as '{def.DisplayName}'");
			}

			def = new ItemPropertyDefinition { PropertyId = propId, PropertyType = type, DisplayName = displayName, IsInstancedPerItem = isInstancedPerItem };

			_propertyDefinitions.Add(propId, def);			
		}

		public void DefineCategory(string category)
		{
			_categories.Add(category);
		}

		public bool HasCategory(string category)
		{
			return _categories.Contains(category);
		}

		public ItemPropertyDefinition GetItemPropertyDefinition(in String8 propId)
		{
			return _propertyDefinitions[propId];
		}

		public void SetPropertyLocalizedDisplayName(in String8 attributeId, ResourceDescription localizationKey)
		{
			_propertyDefinitions[attributeId].LocalizedDisplayName = localizationKey;
		}

		public void AddItemDefinition(ItemDefinition def)
		{
			_itemDefinitions.Add(def.ItemId, def);
			//if (def.StatIds.Length > 0)
			//{
			//	_stats.DefineStatSet(def.ItemId, def.StatIds);
			//}
		}

		public ItemDefinitionBuilder DefineItem(string itemCategory, in String8 itemId)
		{
			if (!HasCategory(itemCategory))
			{
				DefineCategory(itemCategory);
			}

			if (_itemDefinitions.TryGetValue(itemId, out var existing))
			{
				throw new Exception($"Item {itemId} is already defined by {existing.Name}");
			}

			return new ItemDefinitionBuilder(this, itemCategory, itemId);
		}

		public ItemFactory()
		{
			//_stats = stats;
			//_itemDefinitions.Add("Empty", ItemDefinitionBuilder.Empty);
		}
	}
}
