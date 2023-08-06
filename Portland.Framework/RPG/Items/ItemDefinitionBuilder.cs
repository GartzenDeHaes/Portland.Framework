using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public class ItemPropertyBuilder
	{
		ItemPropertySetting _prop;

		public ItemPropertyBuilder(ItemPropertySetting prop)
		{
			_prop = prop;
		}

		public ItemPropertyBuilder DefaultValue(string value)
		{
			switch (_prop.TemplateProperty.PropertyType)
			{
				case ItemPropertyType.Bool:
					_prop.TemplateProperty.SetDefault(Boolean.Parse(value) ? 1 : 0);
					break;
				case ItemPropertyType.Int:
				case ItemPropertyType.IntRange:
					_prop.TemplateProperty.SetDefault(Int32.Parse(value));
					break;
				case ItemPropertyType.RandomInt:
					throw new Exception($"Invalid default for RandomInt '{value}'");
				case ItemPropertyType.Float:
				case ItemPropertyType.FloatRange:
				case ItemPropertyType.RandomFloat:
					_prop.TemplateProperty.SetDefault(Single.Parse(value));
					break;
				case ItemPropertyType.Flag:
				case ItemPropertyType.String:
				case ItemPropertyType.Sound:
				case ItemPropertyType.DiceRoll:
					_prop.TemplateProperty.SetDefault(value);
					break;
			}
			return this;
		}

		public ItemPropertyBuilder Range(string min, string max)
		{
			switch (_prop.TemplateProperty.PropertyType)
			{
				case ItemPropertyType.Flag:
				case ItemPropertyType.Bool:
				case ItemPropertyType.Int:
				case ItemPropertyType.String:
				case ItemPropertyType.Sound:
				case ItemPropertyType.DiceRoll:
					throw new Exception($"Range not allowed for {_prop.Definition.PropertyType}");
				case ItemPropertyType.IntRange:
				case ItemPropertyType.RandomInt:
					_prop.TemplateProperty.SetRange(Int32.Parse(min), Int32.Parse(max));
					break;
				case ItemPropertyType.Float:
					_prop.TemplateProperty.SetRange(Single.Parse(min), Single.Parse(max));
					break;
				case ItemPropertyType.FloatRange:
					_prop.TemplateProperty.SetRange(Single.Parse(min), Single.Parse(max));
					break;
				case ItemPropertyType.RandomFloat:
					_prop.TemplateProperty.SetRange(Single.Parse(min), Single.Parse(max));
					break;
			}
			return this;
		}

		public ItemPropertyBuilder Current(string value)
		{
			switch (_prop.TemplateProperty.PropertyType)
			{
				case ItemPropertyType.Bool:
					_prop.TemplateProperty.Set(Boolean.Parse(value));
					break;
				case ItemPropertyType.Int:
				case ItemPropertyType.IntRange:
					_prop.TemplateProperty.Set(Int32.Parse(value));
					break;
				case ItemPropertyType.RandomInt:
				case ItemPropertyType.RandomFloat:
				case ItemPropertyType.Flag:
				case ItemPropertyType.Sound:
				case ItemPropertyType.DiceRoll:
					throw new Exception($"Invalid current for {_prop.Definition.PropertyId}");
				case ItemPropertyType.Float:
				case ItemPropertyType.FloatRange:
					_prop.TemplateProperty.Set(Single.Parse(value));
					break;
				case ItemPropertyType.String:
					_prop.TemplateProperty.Set(String8.From(value));
					break;
			}
			return this;
		}

	}

	public class ItemDefinitionBuilder
	{
		ItemDefinition _itemdef;
		ItemFactory _factory;
		//List<AsciiId4> _stats = new List<AsciiId4>();
		List<ItemPropertySetting> _props = new List<ItemPropertySetting>();
		List<Effect> _effects = new List<Effect>();

		public ItemDefinitionBuilder(ItemFactory factory, string category, in String8 itemId)
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

		public ItemPropertyBuilder BuildProperty(in String propId)
		{
			ItemPropertySetting template = new ItemPropertySetting { Definition = _factory.GetItemPropertyDefinition(propId) };
			template.TemplateProperty = new ItemProperty(template.Definition);
			_props.Add(template);
			return new ItemPropertyBuilder(template);
		}

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
			ItemId = String8.Empty,
			Category = String.Empty,
			Name = String.Empty,
			Description = String.Empty,
			Properties = Array.Empty<ItemPropertySetting>(),
			Effects = Array.Empty<Effect>(),
			Recipe = new ItemRecipe()
		};
	}
}
