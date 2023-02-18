using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Types;

namespace Portland.RPG
{
	public struct ItemPropertySetting
	{
		public ItemProperty TemplateProperty;
		public ItemPropertyDefinition Definition;
	}

	public class ItemDefinition
	{
		public String ItemId;
		public String Category;
		public string Name;
		public string Description;
		public float Weight;
		public int MaxStackSize;

		public ItemPropertySetting[] Properties;

		public Effect[] Effects;

		public ItemRecipe Recipe;
		public ResourceDescription LocalizedName;
		public ResourceDescription LocalizedDescription;
		public ResourceDescription Icon;
		public ResourceDescription WorldDropObject;

		/// <summary></summary>
		public ItemProperty[] CreateProperties()
		{
			List<ItemProperty> props = new List<ItemProperty>();

			for (int i = 0; i < Properties.Length; i++)
			{
				if (Properties[i].Definition.IsInstancedPerItem)
				{
					props.Add(Properties[i].TemplateProperty.CloneAsTemplate());
				}
			}

			return props.ToArray();
		}

		/// <summary>Class properties for all items of this type</summary>
		public bool HasProperty(in String propId)
		{
			return TryFindProperty(propId, out var _);
		}

		bool TryFindProperty(in String propId, out int i)
		{
			for (i = 0; i < Properties.Length; i++)
			{
				if (Properties[i].Definition.PropertyId == propId)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>Class properties for all items of this type</summary>
		public bool TryGetProperty(in String propId, out Variant8 value)
		{
			if (TryFindProperty(propId, out int i))
			{
				value = Properties[i].TemplateProperty.CurrentVariant();
				return true;
			}

			value = default(Variant8);
			return false;
		}

		/// <summary>Class properties for all items of this type</summary>
		public Variant8 GetPropertyVariant(in String propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentVariant();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public int GetPropertyInt(in String propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentInt();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public float GetPropertyFloat(in String propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentFloat();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public String8 GetPropertyString(in String propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentString();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public bool GetPropertyBool(in String propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentBool();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public bool IsPropertyEquals(in String propId, in Variant8 value)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentVariant() == value;
			}

			throw new Exception($"Property '{propId}' not found");
		}
	}

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
