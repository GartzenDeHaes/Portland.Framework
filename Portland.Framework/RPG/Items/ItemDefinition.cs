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
}
