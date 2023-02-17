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
		//public readonly IObservableValue<Variant8> Amt;
		public Variant8 Value;
		public float Max;
		public readonly PropertyDefinition Definition;

		public float Normalized
		{
			get
			{
				//Debug.Assert(Max == Definition.Maximum);
				//return (Amt.Value - Definition.Minimum) / (Max - Definition.Minimum);
				return (Value - Definition.Minimum) / (Max - Definition.Minimum);
			}
		}

		public PropertyValue(PropertyDefinition propertyDef)
		{
			Definition = propertyDef;
			Max = propertyDef.Maximum;
			//Amt = new ObservableValue<Variant8>();
			if (Definition.TypeName == "string")
			{
				Value = String.Empty;
			}
			else
			{
				Value = Definition.DefaultValueForInitialization();
			}
		}

		public void AddToValue(in Variant8 val)
		{
			Set(Value + val);
		}

		public void Set(in Variant8 val)
		{
			if (! Value.IsNumeric)
			{
				Value = val;
			}
			else if (val > Max)
			{
				Value = Max;
			}
			else if (val < Definition.Minimum)
			{
				Value = Definition.Minimum;
			}
			else
			{
				Value = val;
			}
		}

		public void Update(float timeDelta)
		{
			if (Definition.ChangePerSec != 0f)
			{
				Set(Value + timeDelta * Definition.ChangePerSec);
			}
		}
	}
}
