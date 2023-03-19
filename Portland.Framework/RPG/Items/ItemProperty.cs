using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Portland.Mathmatics;
using Portland.Types;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

using Debug = System.Diagnostics.Debug;

namespace Portland.RPG
{
	[Serializable]
	public class ItemProperty
	{
		public String PropertyId;
		public ItemPropertyType PropertyType;
		public ItemPropertyValue Value;
		public ResourceDescription DisplayName;

		public ItemProperty()
		{
			PropertyId = String.Empty;
			PropertyType = ItemPropertyType.Flag;

			ResetToZero();
		}

		public ItemProperty(in String propertyId, ItemPropertyType propertyType)
		{
			PropertyId = propertyId;
			PropertyType = propertyType;
			Value = default(ItemPropertyValue);

			ResetToZero();
		}

		public ItemProperty(ItemPropertyDefinition def)
		{
			PropertyId = def.PropertyId;
			PropertyType = def.PropertyType;
			DisplayName = def.LocalizedDisplayName;

			ResetToZero();
		}

		public void ResetToZero()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Flag:
					break;
				case ItemPropertyType.Bool:
					Value.Boolean = false;
					break;
				case ItemPropertyType.Int:
					Value.Int.Current = 0;
					Value.Int.Default = 0;
					break;
				case ItemPropertyType.IntRange:
					Value.IntRange.Current = 0;
					Value.IntRange.Min = 0;
					Value.IntRange.Max = 0;
					break;
				case ItemPropertyType.RandomInt:
					Value.IntRnd.Min = 0;
					Value.IntRnd.Max = 100;
					break;
				case ItemPropertyType.Float:
					Value.Float.Current = 0;
					Value.Float.Default = 0;
					break;
				case ItemPropertyType.FloatRange:
					Value.FloatRange.Current = 0;
					Value.FloatRange.Min = 0;
					Value.FloatRange.Max = 0;
					break;
				case ItemPropertyType.RandomFloat:
					Value.FloatRnd.Min = 0f;
					Value.FloatRnd.Max = 1f;
					break;
				case ItemPropertyType.String:
					Value.Text = String8.Empty;
					break;
				case ItemPropertyType.Sound:
					Value.SoundResourceKey = PropertyId;
					break;
				case ItemPropertyType.DiceRoll:
					Value.Text = "1d6";
					break;
			}
		}

		public void SetCurrentToDefault()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Int:
					Value.Int.Current = Value.Int.Default;
					break;
				case ItemPropertyType.Float:
					Value.Float.Current = Value.Float.Default;
					break;
			}
		}

		public void Set(int intVal)
		{
			if (PropertyType == ItemPropertyType.Int)
			{
				Value.Int.Current = intVal;
			}
			else if (PropertyType == ItemPropertyType.IntRange)
			{
				Value.IntRange.Current = intVal;
			}
			else
			{
				throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be set to INT");
			}
		}

		public void SetDefault(int defVal)
		{
			if (PropertyType == ItemPropertyType.Int)
			{
				Value.Int.Default = defVal;
				Value.Int.Current = defVal;
			}
			else if (PropertyType == ItemPropertyType.Float)
			{
				SetDefault((float)defVal);
			}
			else
			{
				throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot default to INT");
			}
		}

		public void SetDefault(in String8 value)
		{
			if (PropertyType == ItemPropertyType.String || PropertyType == ItemPropertyType.DiceRoll)
			{
				Value.Text = value;
			}
			else
			{
				throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot default to STRING");
			}
		}

		public void Set(float val)
		{
			if (PropertyType == ItemPropertyType.Float)
			{
				Value.Float.Current = val;
			}
			else if (PropertyType == ItemPropertyType.FloatRange)
			{
				Value.FloatRange.Current = val;
			}
			else
			{
				throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be set to FLOAT");
			}
		}

		public void SetDefault(float val)
		{
			if (PropertyType == ItemPropertyType.Float)
			{
				Value.Float.Default = val;
				Value.Float.Current = val;
			}
			else
			{
				throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot default to FLOAT");
			}
		}

		public void Set(bool val)
		{
			Debug.Assert(PropertyType == ItemPropertyType.Bool);
			Value.Boolean = val;
		}

		public void Set(in String8 val)
		{
			Debug.Assert(PropertyType == ItemPropertyType.String || PropertyType == ItemPropertyType.DiceRoll);
			Value.Text = val;
		}

		public void Set(in Variant8 val)
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Bool:
					Value.Boolean = val.ToBool();
					break;
				case ItemPropertyType.Int:
					Value.Int.Current = val.ToInt();
					break;
				case ItemPropertyType.IntRange:
					Value.IntRange.Current = val.ToInt();
					break;
				case ItemPropertyType.Float:
					Value.Float.Current = val.ToFloat();
					break;
				case ItemPropertyType.FloatRange:
					Value.FloatRange.Current = val.ToFloat();
					break;
				case ItemPropertyType.DiceRoll:
				case ItemPropertyType.String:
					Value.Text = val.ToString();
					break;
			}
			throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be set to VARIANT");
		}

		public void SetRange(int min, int max)
		{
			if (PropertyType == ItemPropertyType.IntRange)
			{
				Value.IntRange.Min = min;
				Value.IntRange.Current = min;
				Value.IntRange.Max = max;
			}
			else
			{
				throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be set to INT RANGE");
			}
		}

		public void SetRange(float min, float max)
		{
			if (PropertyType == ItemPropertyType.FloatRange)
			{
				Value.FloatRange.Min = min;
				Value.FloatRange.Current = min;
				Value.FloatRange.Max = max;
			}
			else
			{
				throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be set to FLOAT RANGE");
			}
		}

		public Variant8 CurrentVariant()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Bool:
					return Value.Boolean;
				case ItemPropertyType.Int:
					return Value.Int.Current;
				case ItemPropertyType.IntRange:
					return Value.IntRange.Current;
				case ItemPropertyType.RandomInt:
					return Value.IntRnd.RandomValue;
				case ItemPropertyType.Float:
					return Value.Float.Current;
				case ItemPropertyType.FloatRange:
					return Value.FloatRange.Current;
				case ItemPropertyType.RandomFloat:
					return Value.FloatRnd.RandomValue;
				case ItemPropertyType.DiceRoll:
					return DiceTerm.Roll(Value.Text);
			}
			throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be converted to VARIANT");
		}

		public int CurrentInt()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Bool:
					return Value.Boolean ? 1 : 0;
				case ItemPropertyType.Int:
					return Value.Int.Current;
				case ItemPropertyType.IntRange:
					return Value.IntRange.Current;
				case ItemPropertyType.RandomInt:
					return Value.IntRnd.RandomValue;
				case ItemPropertyType.Float:
					return (int)Value.Float.Current;
				case ItemPropertyType.FloatRange:
					return (int)Value.FloatRange.Current;
				case ItemPropertyType.RandomFloat:
					return (int)Value.FloatRnd.RandomValue;
				case ItemPropertyType.DiceRoll:
					return DiceTerm.Roll(Value.Text);
			}
			throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be converted to INT");
		}

		public float CurrentFloat()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Bool:
					return Value.Boolean ? 1 : 0;
				case ItemPropertyType.Int:
					return Value.Int.Current;
				case ItemPropertyType.IntRange:
					return Value.IntRange.Current;
				case ItemPropertyType.RandomInt:
					return Value.IntRnd.RandomValue;
				case ItemPropertyType.Float:
					return Value.Float.Current;
				case ItemPropertyType.FloatRange:
					return Value.FloatRange.Current;
				case ItemPropertyType.RandomFloat:
					return Value.FloatRnd.RandomValue;
				case ItemPropertyType.DiceRoll:
					return DiceTerm.Roll(Value.Text);
			}
			throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be converted to INT");
		}

		public bool CurrentBool()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Bool:
					return Value.Boolean;
				case ItemPropertyType.Int:
					return Value.Int.Current != 0;
				case ItemPropertyType.IntRange:
					return Value.IntRange.Current != 0;
				case ItemPropertyType.Float:
					return Value.Float.Current > Single.Epsilon || Value.Float.Current < -Single.Epsilon;
				case ItemPropertyType.FloatRange:
					return Value.FloatRange.Current > Single.Epsilon || Value.FloatRange.Current < -Single.Epsilon;
			}
			throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be converted to BOOL");
		}

		public String8 CurrentString()
		{
			if (PropertyType == ItemPropertyType.String || PropertyType == ItemPropertyType.DiceRoll)
			{
				return Value.Text;
			}
			throw new Exception($"Item Property {PropertyId} is type {PropertyType} can cannot be converted to STRING");
		}

		public float CurrentRatio()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.IntRange:
					return Value.IntRange.Ratio;
				case ItemPropertyType.FloatRange:
					return Value.FloatRange.Ratio;
			}
			throw new Exception($"Item Property {PropertyId} is type {PropertyType} does not have a ratio");
		}

		public override string ToString()
		{
			switch (PropertyType)
			{
				case ItemPropertyType.Bool:
					return Value.Boolean.ToString();
				case ItemPropertyType.Int:
					return Value.Int.ToString();
				case ItemPropertyType.IntRange:
					return Value.IntRange.ToString();
				case ItemPropertyType.RandomInt:
					return Value.IntRnd.ToString();
				case ItemPropertyType.Float:
					return Value.Float.ToString();
				case ItemPropertyType.FloatRange:
					return Value.FloatRange.ToString();
				case ItemPropertyType.RandomFloat:
					return Value.FloatRnd.ToString();
				case ItemPropertyType.String:
				case ItemPropertyType.DiceRoll:
					return Value.Text.ToString();
				case ItemPropertyType.Sound:
					return Value.SoundResourceKey.ToString();
			}

			return PropertyId.ToString();
		}

		public ItemProperty Clone()
		{
			var ret = new ItemProperty(PropertyId, PropertyType);
			ret.Value = Value;
			ret.SetCurrentToDefault();
			return ret;
		}

		public bool Equals(int val)
		{
			return (PropertyType == ItemPropertyType.Int && Value.Int.Current == val)
				|| (PropertyType == ItemPropertyType.IntRange && Value.IntRange.Current == val)
				|| (PropertyType == ItemPropertyType.RandomInt && Value.IntRnd.RandomValue == val);
		}

		public bool Equals(float val)
		{
			return (PropertyType == ItemPropertyType.Float && Value.Float.Current == val)
				|| (PropertyType == ItemPropertyType.FloatRange && Value.FloatRange.Current == val);
		}

		public bool Equals(in String val)
		{
			return 
			(
				(PropertyType == ItemPropertyType.String || PropertyType == ItemPropertyType.DiceRoll) 
				&& Value.Text == val
			);
		}

		public bool Equals(bool val)
		{
			return (PropertyType == ItemPropertyType.Bool && Value.Boolean == val);
		}

		[Serializable]
		public struct IntValue
		{
			/// <summary>This is equal to Current / Default.</summary>
			public float Ratio { get { return (float)Current / Default; } }

			public int Current;

			public int Default;

			public override string ToString()
			{
				return Current.ToString();
			}
		}

		[Serializable]
		public struct IntRange
		{
			public int Current { get { return m_Current; } set { m_Current = MathHelper.Clamp(value, Min, Max); } }

			/// <summary>This is equal to Current / Max.</summary>
			public float Ratio { get { return (float)m_Current / Max; } }

			public int Min;

			public int Max;

#if UNITY_5_3_OR_NEWER
			[SerializeField]
#endif
			private int m_Current;

			public override string ToString()
			{
				return String.Format("{0} / {1}", Current, Max);
			}
		}

		[Serializable]
		public struct RandomInt
		{
			public int RandomValue { get { return MathHelper.Rnd.Range(Min, Max); } }

			public int Min;

			public int Max;

			public override string ToString()
			{
				return String.Format("{0} - {1}", Min, Max);
			}
		}

		[Serializable]
		public struct FloatValue
		{
			/// <summary>Current / Default.</summary>
			public float Ratio { get { return Current / Default; } }

			public float Current;

			public float Default;

			public override string ToString()
			{
				return Current.ToString();
			}
		}

		[Serializable]
		public struct FloatRange
		{
			public float Current { get { return _current; } set { _current = MathHelper.Clamp(value, Min, Max); } }

			/// <summary>Current / Max.</summary>
			public float Ratio { get { return _current / Max; } }

			public float Min;

			public float Max;

#if UNITY_5_3_OR_NEWER
			[SerializeField]
#endif
			private float _current;

			public override string ToString()
			{
				return String.Format("{0} / {1}", Current, Max);
			}
		}

		[Serializable]
		public struct RandomFloat
		{
			public float RandomValue { get { return MathHelper.Rnd.Range(Min, Max); } }

			public float Min;

			public float Max;

			public override string ToString()
			{
				return String.Format("{0} - {1}", Min, Max);
			}
		}

		[Serializable]
		[StructLayout(LayoutKind.Explicit)]
		public struct ItemPropertyValue
		{
			[FieldOffset(0)]
			public IntValue Int;
			[FieldOffset(0)]
			public IntRange IntRange;
			[FieldOffset(0)]
			public RandomInt IntRnd;
			[FieldOffset(0)]
			public FloatValue Float;
			[FieldOffset(0)]
			public FloatRange FloatRange;
			[FieldOffset(0)]
			public RandomFloat FloatRnd;
			[FieldOffset(0)]
			public bool Boolean;
			[FieldOffset(0)]
			public String8 Text;
			[FieldOffset(0)]
			public String8 SoundResourceKey;
		}
	}
}
