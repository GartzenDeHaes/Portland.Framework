using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public class ItemDefinitionBuilder
	{
		ItemDefinition _itemdef;
		ItemFactory _factory;
		//List<AsciiId4> _stats = new List<AsciiId4>();
		List<ItemPropertySetting> _props = new List<ItemPropertySetting>();
		List<Effect> _effects = new List<Effect>();

		public ItemDefinitionBuilder(ItemFactory factory, string category, in String itemId)
		{
			_factory = factory;
			_itemdef = new ItemDefinition();
			_itemdef.ItemId = itemId;
			_itemdef.Category = category;
			_itemdef.Name = itemId;
		}

		/// <summary>
		/// Ensure an item category exists
		/// </summary>
		public ItemDefinitionBuilder DefineCategory(string category)
		{
			_factory.DefineCategory(category);
			return this;
		}

		/// <summary>
		/// Define a new property type. Does not add it to the item under construction (use AddProperty)
		/// </summary>
		/// <param name="itemPropertyId"></param>
		/// <param name="type">ItemPropertyType</param>
		/// <param name="desc">Description</param>
		/// <param name="instancePerItem">Does the value change per item instance?</param>
		public ItemDefinitionBuilder DefineItemProperty
		(
			in String itemPropertyId,
			ItemPropertyType type,
			string desc,
			bool instancePerItem
		)
		{
			if (!_factory.HasProperty(itemPropertyId))
			{
				_factory.DefineProperty(itemPropertyId, type, desc, instancePerItem);
			}

			return this;
		}

		public ItemDefinitionBuilder DisplayName(string name)
		{
			_itemdef.Name = name;
			return this;
		}

		public ItemDefinitionBuilder Description(string description)
		{
			_itemdef.Description = description;
			return this;
		}

		public ItemDefinitionBuilder Weight(float weight)
		{
			_itemdef.Weight = weight;
			return this;
		}

		public ItemDefinitionBuilder MaxStackCapacity(int maxStackCapacity)
		{
			_itemdef.MaxStackSize = maxStackCapacity;
			return this;
		}

		//public ItemDefinitionBuilder AddStat(in AsciiId4 statId)
		//{
		//	_stats.Add(statId);
		//	return this;
		//}

		public ItemDefinitionBuilder AddProperty(in String propId)
		{
			ItemPropertySetting template = new ItemPropertySetting { Definition = _factory.GetItemPropertyDefinition(propId) };
			template.TemplateProperty = new ItemProperty(template.Definition);
			_props.Add(template);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String propId, in String8 defaultValue)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetDefault(defaultValue);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String propId, float defaultValue)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetDefault(defaultValue);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String propId, int defaultValue)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetDefault(defaultValue);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String propId, float min, float max, float current)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetRange(min, max);
			prop.TemplateProperty.Set(current);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String propId, int min, int max, int current)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetRange(min, max);
			prop.TemplateProperty.Set(current);
			return this;
		}

		ItemPropertySetting FindProperty(in String propId)
		{
			for (int i = 0; i < _props.Count; i++)
			{
				if (_props[i].Definition.PropertyId == propId)
				{
					return _props[i];
				}
			}
			throw new Exception($"Property {propId} not found");
		}

		public ItemDefinitionBuilder SetPropertyRange(in String propId, float min, float max)
		{
			FindProperty(propId).TemplateProperty.SetRange(min, max);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyRange(in String propId, int min, int max)
		{
			FindProperty(propId).TemplateProperty.SetRange(min, max);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyDefault(in String propId, int value)
		{
			FindProperty(propId).TemplateProperty.SetDefault(value);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyDefault(in String propId, float value)
		{
			FindProperty(propId).TemplateProperty.SetDefault(value);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyDefault(in String propId, in String8 value)
		{
			FindProperty(propId).TemplateProperty.SetDefault(value);

			return this;
		}

		public ItemDefinitionBuilder AddEffect(in Effect effect)
		{
			_effects.Add(effect);
			return this;
		}

		public void Build()
		{
			//_itemdef.StatIds = _stats.ToArray();
			_itemdef.Properties = _props.ToArray();
			_itemdef.Effects = _effects.ToArray();

			_factory.AddItemDefinition(_itemdef);

			_itemdef = null;
			_factory = null;

			//_stats.Clear();
			//_stats = null;
			_props.Clear();
			_props = null;
			_effects.Clear();
			_effects = null;
		}

		public static readonly ItemDefinition Empty = new ItemDefinition
		{
			ItemId = String.Empty,
			Category = String.Empty,
			Name = String.Empty,
			Description = String.Empty,
			Properties = Array.Empty<ItemPropertySetting>(),
			Effects = Array.Empty<Effect>(),
			Recipe = new ItemRecipe()
		};
	}
}
