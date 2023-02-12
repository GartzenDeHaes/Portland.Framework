using System;
using System.Diagnostics;

using Portland.ComponentModel;

namespace Portland.AI.Utility
{
	/// <summary>
	/// cref="ConciderationPropertyDef" Associated with a cref="Consideration" in an cref="Objective".
	/// </summary>
	public sealed class PropertyValue
	{
		public readonly ObservableValue<Variant8> Amt;
		public float Max;
		public readonly ConsiderationPropertyDef PropertyDef;

		public float Normalized
		{
			get
			{
				Debug.Assert(Max == PropertyDef.Max);
				return (Amt.Value - PropertyDef.Min) / (Max - PropertyDef.Min);
			}
		}

		public PropertyValue(ConsiderationPropertyDef propertyDef)
		{
			PropertyDef = propertyDef;
			Max = propertyDef.Max;
			Amt = new ObservableValue<Variant8>();
			Amt.Set(PropertyDef.DefaultValueForInitialization());
		}

		public void AddToValue(float val)
		{
			Set(Amt.Value + val);
		}

		public void Set(float val)
		{
			val = val > Max ? Max : val;
			val = val < PropertyDef.Min ? PropertyDef.Min : val;
			Amt.Set(val);
		}

		public void Update(float timeDelta)
		{
			if (PropertyDef.ChangePerSec != 0f)
			{
				Set(Amt.Value + timeDelta * PropertyDef.ChangePerSec);
			}
		}
	}
}
