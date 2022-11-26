using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Mathmatics;

#if UNITY_2018_1_OR_NEWER
using UnityEngine;
#endif

namespace Portland.AI.Semantics
{
	public class ValueSlotInstance
	{
		public ValueSlot Definition;
		private Variant8 _value = new Variant8();
		
		public float Normalized
		{
			get 
			{ 
				return Definition.UseMinMaxRange ? _value.ToFloat() / (Definition.Max - Definition.Min) : _value.ToFloat(); 
			}
		}

		public ValueSlotInstance()
		{
		}

		public ValueSlotInstance(ValueSlot def)
		{
			Definition = def;
			if (Definition.UseMinMaxRange && (Definition.DataType == VariantType.Float || Definition.DataType == VariantType.Int))
			{
				if (Definition.StartRandomize)
				{
					_value.Set(MathHelper.RandomRange(Definition.Min, Definition.Max));
				}
				else
				{
					_value.Set(Definition.Start);
				}

				if (Definition.DataType == VariantType.Int)
				{
					_value.Set(_value.ToInt());
				}
			}
			else
			{
				_value.SetTypeDefaultValue(Definition.DataType);
			}
		}

		public float AsFloat()
		{
			return _value.ToFloat();
		}

		public int AsInt()
		{
			return _value.ToInt();
		}

		public string AsString()
		{
			return _value.ToString();
		}

		public Vector3h AsVector3()
		{
			return _value.ToVector3d();
		}

		public Variant8 AsVariant()
		{
			return _value;
		}

		public void Set(float val)
		{
			_value.Set(val);
		}

		public void Set(int val)
		{
			_value.Set(val);
		}

		public void Set(string val)
		{
			_value.Set(val);
		}
		
		public void Set(Vector3h val)
		{
			_value.Set(val);
		}
	}
}
