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
		public readonly PropertyDefinition Definition;

		public float Normalized
		{
			get
			{
				Debug.Assert(Max == Definition.Maximum);
				return (Amt.Value - Definition.Minimum) / (Max - Definition.Minimum);
			}
		}

		public PropertyValue(PropertyDefinition propertyDef)
		{
			Definition = propertyDef;
			Max = propertyDef.Maximum;
			Amt = new ObservableValue<Variant8>();
			Amt.Set(Definition.DefaultValueForInitialization());
		}

		public void AddToValue(float val)
		{
			Set(Amt.Value + val);
		}

		public void Set(float val)
		{
			val = val > Max ? Max : val;
			val = val < Definition.Minimum ? Definition.Minimum : val;
			Amt.Set(val);
		}

		public void Update(float timeDelta)
		{
			if (Definition.ChangePerSec != 0f)
			{
				Set(Amt.Value + timeDelta * Definition.ChangePerSec);
			}
		}
	}
}
