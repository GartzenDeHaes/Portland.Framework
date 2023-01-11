using System;
using Portland.ComponentModel;
using Portland.Mathmatics;

namespace Portland.AI.Utility
{
	/// <summary>
	/// cref="ConciderationPropertyDef" Associated with a cref="Consideration" in an cref="Objective".
	/// </summary>
	public class ConciderationProperty
	{
		public readonly ConsiderationPropertyDef PropertyDef;
		public readonly ObservableValue<Variant8> Amt;

		public float Normalized
		{
			get
			{
				return (Amt.Value - PropertyDef.Min) / (PropertyDef.Max - PropertyDef.Min);
			}
		}

		public ConciderationProperty(ConsiderationPropertyDef propertyDef)
		{
			PropertyDef = propertyDef;
			Amt = new ObservableValue<Variant8>();

			if (PropertyDef.StartRand)
			{
				Amt.Set(MathHelper.RandomRange(PropertyDef.Min, PropertyDef.Max));
			}
			else
			{
				Amt.Set(PropertyDef.Start);
			}
		}

		public void AddToValue(float val)
		{
			Set(Amt.Value + val);
		}

		public void Set(float val)
		{
			val = val > PropertyDef.Max ? PropertyDef.Max : val;
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
