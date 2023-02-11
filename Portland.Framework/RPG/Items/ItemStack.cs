using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	// Not serializable due to Definition
	//[Serializable]
	public sealed class ItemStack
	{
		int _stackCount;
		//public StatSet Stats;
		public readonly ItemProperty[] Properties;
		public readonly ItemDefinition Definition;
		public int CollectionIndex;

		public Action<String8, Variant8> OnPropertyChanged;
		public Action<int> OnStackCountChanged;

		public int StackCount { get { return _stackCount; } }

		public bool IsEmpty()
		{
			return _stackCount == 0;
		}

		public void ChangeStackCount(int delta)
		{
			_stackCount = Math.Clamp(_stackCount + delta, 0, Definition.MaxStackSize);

			// items with per-item properties should only have counts of zero or one
			Debug.Assert(Properties.Length == 0 || _stackCount == 0 || _stackCount == 1);

			OnStackCountChanged?.Invoke(_stackCount);
		}

		public void SetStackCount(int count)
		{
			_stackCount = Math.Clamp(count, 0, Definition.MaxStackSize);

			// items with per-item properties should only have counts of zero or one
			Debug.Assert(Properties.Length == 0 || _stackCount == 0 || _stackCount == 1);

			OnStackCountChanged?.Invoke(_stackCount);
		}

		bool TryFindProperty(in String8 propId, out int i)
		{
			for (i = 0; i < Properties.Length; i++)
			{
				if (Properties[i].PropertyId == propId)
				{
					return true;
				}
			}

			return false;
		}

		public void SetProperty(in String8 propId, bool value)
		{
			if (TryFindProperty(propId, out int i))
			{
				if (!Properties[i].Equals(value))
				{
					Properties[i].Set(value);
					OnPropertyChanged?.Invoke(propId, value);
				}
			}
			else
			{
				throw new Exception($"Property '{propId}' not found");
			}
		}

		public void SetProperty(in String8 propId, float value)
		{
			if (TryFindProperty(propId, out int i))
			{
				if (!Properties[i].Equals(value))
				{
					Properties[i].Set(value);
					OnPropertyChanged?.Invoke(propId, value);
				}
			}
			else
			{
				throw new Exception($"Property '{propId}' not found");
			}
		}

		public void SetProperty(in String8 propId, int value)
		{
			if (TryFindProperty(propId, out int i))
			{
				if (!Properties[i].Equals(value))
				{
					Properties[i].Set(value);
					OnPropertyChanged?.Invoke(propId, value);
				}
			}
			else
			{
				throw new Exception($"Property '{propId}' not found");
			}
		}

		public void SetProperty(in String8 propId, in String8 value)
		{
			if (TryFindProperty(propId, out int i))
			{
				if (!Properties[i].Equals(value))
				{
					Properties[i].Set(value);
					OnPropertyChanged?.Invoke(propId, value.ToString());
				}
			}
			else
			{
				throw new Exception($"Property '{propId}' not found");
			}
		}

		public void SetProperty(in String8 propId, in Variant8 value)
		{
			if (TryFindProperty(propId, out int i))
			{
				Properties[i].Set(value);
			}
			else
			{
				throw new Exception($"Property '{propId}' not found");
			}
		}

		public float GetPropertyRatio(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].CurrentRatio();
			}

			throw new Exception($"Property '{propId}' not found");
		}

		public bool TryGetProperty(in String8 propId, out Variant8 value)
		{
			if (TryFindProperty(propId, out int i))
			{
				value = Properties[i].CurrentVariant();
				return true;
			}

			return Definition.TryGetProperty(propId, out value);
		}

		public Variant8 GetPropertyVariant(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].CurrentVariant();
			}
			
			return Definition.GetPropertyVariant(propId);
		}

		public int GetPropertyInt(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].CurrentInt();
			}

			return Definition.GetPropertyInt(propId);
		}

		public float GetPropertyFloat(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].CurrentFloat();
			}

			return Definition.GetPropertyFloat(propId);
		}

		public String8 GetPropertyString(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].CurrentString();
			}

			return Definition.GetPropertyString(propId);
		}

		public bool GetPropertyBool(in String8 propId)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].CurrentBool();
			}

			return Definition.GetPropertyBool(propId);
		}

		public bool IsPropertyEquals(in String8 propId, in Variant8 value)
		{
			if (TryFindProperty(propId, out int i))
			{
				return Properties[i].CurrentVariant() == value;
			}

			return Definition.IsPropertyEquals(propId, value);
		}

		public bool HasProperty(in String8 propId)
		{
			return TryFindProperty(propId, out int _)
				|| Definition.HasProperty(propId);
		}

		public ItemStack Clone(int newCollectionIndex)
		{
			ItemProperty[] props = new ItemProperty[Properties.Length];
			for (int i = 0; i < Properties.Length; i++)
			{
				props[i] = Properties[i].CloneAsTemplate();
			}

			ItemStack item = new ItemStack (_stackCount, newCollectionIndex, Definition, props);

			return item;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(ItemStack item)
		{
			if (item.Definition == Definition && item.StackCount == StackCount)
			{
				for (int i = 0; i < Properties.Length; i++)
				{
					if (Properties[i].CurrentVariant != item.Properties[i].CurrentVariant)
					{
						return false;
					}
				}
				return true;
			}

			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is ItemStack item)
			{
				return Equals(item);
			}

			return false;
		}

		private ItemStack(int stackCount, int index, ItemDefinition def, ItemProperty[] properties)
		{
			_stackCount = stackCount;
			CollectionIndex = index;
			Definition = def;
			Properties = properties;
		}

		public ItemStack(int collectionIndex, ItemDefinition def, int stackCount = 1)
		{
			CollectionIndex = collectionIndex;
			_stackCount = stackCount;
			Definition = def;
			Properties = def.CreateProperties();

			Debug.Assert(Properties.Length == 0 || _stackCount < 2);
		}
	}
}
