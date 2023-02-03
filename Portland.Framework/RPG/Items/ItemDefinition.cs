using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public struct ItemPropertySetting
	{
		public ItemProperty TemplateProperty;
		public ItemPropertyDefinition Definition;
	}

	public class ItemDefinition
	{
		public String8 ItemId;
		public string Category;
		public string Name;
		public string Description;
		public float Weight;
		public int MaxStackSize;

		public ItemPropertySetting[] Properties;

		public Effect[] StatEffects;
		public PropertyEffect[] PropertyEffects;

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
		public bool HasProperty(in String8 propId)
		{
			return TryFindProperty(propId, out var _);
		}

		bool TryFindProperty(in String8 propId, out int i)
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
		public Variant8 GetPropertyVariant(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentVariant();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public int GetPropertyInt(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentInt();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public float GetPropertyFloat(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentFloat();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public String8 GetPropertyString(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentString();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public bool GetPropertyBool(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].TemplateProperty.CurrentBool();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		/// <summary>Class properties for all items of this type</summary>
		public bool IsPropertyEquals(in String8 propId, in Variant8 value)
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
		List<Effect> _statEffs = new List<Effect>();
		List<PropertyEffect> _propEffs = new List<PropertyEffect>();

		public ItemDefinitionBuilder(ItemFactory factory, string category, in String8 itemId)
		{
			_factory = factory;
			_itemdef = new ItemDefinition();
			_itemdef.ItemId = itemId;
			_itemdef.Category = category;
			_itemdef.Name = itemId;
		}

		public ItemDefinitionBuilder Name(string name)
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

		public ItemDefinitionBuilder AddProperty(in String8 propId)
		{
			ItemPropertySetting template = new ItemPropertySetting { Definition = _factory.GetItemPropertyDefinition(propId) };
			template.TemplateProperty = new ItemProperty(template.Definition);
			_props.Add(template);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String8 propId, String8 defaultValue)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetDefault(defaultValue);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String8 propId, float defaultValue)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetDefault(defaultValue);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String8 propId, int defaultValue)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetDefault(defaultValue);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String8 propId, float min, float max, float current)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetRange(min, max);
			prop.TemplateProperty.Set(current);
			return this;
		}

		public ItemDefinitionBuilder AddProperty(in String8 propId, int min, int max, int current)
		{
			AddProperty(propId);
			var prop = FindProperty(propId);
			prop.TemplateProperty.SetRange(min, max);
			prop.TemplateProperty.Set(current);
			return this;
		}

		ItemPropertySetting FindProperty(in String8 propId)
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

		public ItemDefinitionBuilder SetPropertyRange(in String8 propId, float min, float max)
		{
			FindProperty(propId).TemplateProperty.SetRange(min, max);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyRange(in String8 propId, int min, int max)
		{
			FindProperty(propId).TemplateProperty.SetRange(min, max);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyDefault(in String8 propId, int value)
		{
			FindProperty(propId).TemplateProperty.SetDefault(value);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyDefault(in String8 propId, float value)
		{
			FindProperty(propId).TemplateProperty.SetDefault(value);

			return this;
		}

		public ItemDefinitionBuilder SetPropertyDefault(in String8 propId, in String8 value)
		{
			FindProperty(propId).TemplateProperty.SetDefault(value);

			return this;
		}

		public ItemDefinitionBuilder AddStatEffect(in Effect statEffect)
		{
			_statEffs.Add(statEffect);
			return this;
		}

		public ItemDefinitionBuilder AddPropertyEffect(in PropertyEffect propEffect)
		{
			_propEffs.Add(propEffect);
			return this;
		}

		public void Build()
		{
			//_itemdef.StatIds = _stats.ToArray();
			_itemdef.Properties = _props.ToArray();
			_itemdef.StatEffects = _statEffs.ToArray();
			_itemdef.PropertyEffects = _propEffs.ToArray();

			_factory.AddItemDefinition(_itemdef);

			_itemdef = null;
			_factory = null;

			//_stats.Clear();
			//_stats = null;
			_props.Clear();
			_props = null;
			_statEffs.Clear();
			_statEffs = null;
			_propEffs.Clear();
			_propEffs = null;
		}

		public static readonly ItemDefinition Empty = new ItemDefinition
		{
			ItemId = String.Empty,
			Category = String.Empty,
			Name = String.Empty,
			Description = String.Empty,
			Properties = new ItemPropertySetting[0],
			StatEffects = new Effect[0],
			PropertyEffects = new PropertyEffect[0],
			Recipe = new ItemRecipe()
		};
	}
}
