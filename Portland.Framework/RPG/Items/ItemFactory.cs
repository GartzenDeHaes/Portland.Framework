using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Types;

namespace Portland.RPG
{
	public class ItemFactory
	{
		Dictionary<String, ItemPropertyDefinition> _propertyDefinitions = new Dictionary<String, ItemPropertyDefinition>();
		Dictionary<String, ItemDefinition> _itemDefinitions = new Dictionary<String, ItemDefinition>();
		//StatFactory _stats;
		List<String> _categories = new List<String>();

		public bool HasItemDefined(in String itemId)
		{
			return _itemDefinitions.ContainsKey(itemId);
		}

		public ItemStack CreateItem(int collectionIndex, in String itemId, int stackCount = 1)
		{
			if (_itemDefinitions.TryGetValue(itemId, out var def))
			{
				return new ItemStack(collectionIndex, def, stackCount);
			}

			throw new Exception($"Unknown item Id {itemId}");
		}

		public bool HasProperty(in String propId)
		{
			return _propertyDefinitions.ContainsKey(propId);
		}

		public void DefineProperty(in String propId, ItemPropertyType type, string displayName, bool isInstancedPerItem)
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
			if (!_categories.Contains(category))
			{
				_categories.Add(category);
			}
		}

		public bool HasCategory(string category)
		{
			return _categories.Contains(category);
		}

		public ItemPropertyDefinition GetItemPropertyDefinition(in String propId)
		{
			return _propertyDefinitions[propId];
		}

		public void SetPropertyLocalizedDisplayName(in String attributeId, ResourceDescription localizationKey)
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

		public ItemDefinitionBuilder DefineItem(string itemCategory, in String itemId)
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
