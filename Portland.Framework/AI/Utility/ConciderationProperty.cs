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
		private ConsiderationPropertyDef _propertyDef;
		public ObservableValue<Variant8> Amt;

		public float Normalized
		{
			get
			{
				return (Amt.Value - _propertyDef.Min) / (_propertyDef.Max - _propertyDef.Min);
			}
		}

		public ConciderationProperty(ConsiderationPropertyDef propertyDef)
		{
			_propertyDef = propertyDef;
			Amt = new ObservableValue<Variant8>();

			if (_propertyDef.StartRand)
			{
				Amt.Set(MathHelper.RandomRange(_propertyDef.Min, _propertyDef.Max));
			}
			else
			{
				Amt.Set(_propertyDef.Start);
			}
		}

		public void AddToValue(float val)
		{
			Set(Amt.Value + val);
		}

		public void Set(float val)
		{
			val = val > _propertyDef.Max ? _propertyDef.Max : val;
			val = val < _propertyDef.Min ? _propertyDef.Min : val;
			Amt.Set(val);
		}

		public void Update(float timeDelta)
		{
			if (_propertyDef.ChangePerSec != 0f)
			{
				Set(Amt.Value + timeDelta * _propertyDef.ChangePerSec);
			}
		}
	}
}
