using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Types;

namespace Portland.AI.Utility
{
	/// <summary>
	/// The definition of a unity function.
	/// TODO: class breaks transform
	/// </summary>
	public class Consideration
	{
		public enum TransformFunc
		{
			Normal,
			Inverse,
			Center,
			ClampLow,
			ClampHiLow,
			ClampCenter,
			ClampHi,
		}

		public String PropertyName = String.Empty;
		public float Weight = 1.0f;
		public TransformFunc DataTransFn;

		public Consideration()
		{
		}

		public Consideration(in String prop, float weight, string fnc)
		{
			PropertyName = prop;
			Weight = weight;

			ParseTransformFunc(fnc);
		}

		public float UtilityValue(float normalizedVal, float weight)
		{
			return UtilityValue(normalizedVal, weight, DataTransFn);
		}

		public float UtilityValue(float normalizedVal, float weight, TransformFunc fn)
		{
			float val;

			switch (DataTransFn)
			{
				case TransformFunc.Normal:
					val = normalizedVal;
					break;
				case TransformFunc.Inverse:
					val = (1f - normalizedVal);
					break;
				case TransformFunc.ClampLow:
					val = normalizedVal < 0.201f ? 1f : 0f;
					break;
				case TransformFunc.ClampHi:
					val = normalizedVal > 0.791f ? 1f : 0f;
					break;
				case TransformFunc.ClampHiLow:
					val = (normalizedVal > 0.7999f ? 1f : (normalizedVal < 0.201f ? 1f : 0f));
					break;
				case TransformFunc.ClampCenter:
					val = normalizedVal < 0.8001f && normalizedVal > 0.199999f ? 1f : 0f;
					break;
				default:
					// Center
					if (normalizedVal < 0.5f)
					{
						val = 0.5f - normalizedVal;
					}
					else
					{
						val = normalizedVal - 0.5f;
					}
					break;
			}

			return val * weight;
		}

		public Consideration Clone()
		{
			Consideration condi = new Consideration();
			condi.PropertyName = PropertyName;
			condi.Weight = Weight;
			condi.DataTransFn = DataTransFn;

			return condi;
		}

		public void ParseTransformFunc(string fnc)
		{
			if (fnc.Equals("normal"))
			{
				DataTransFn = TransformFunc.Normal;
			}
			else if (fnc.Equals("inverse"))
			{
				DataTransFn = TransformFunc.Inverse;
			}
			else if (fnc.Equals("center"))
			{
				DataTransFn = TransformFunc.Center;
			}
			else if (fnc.Equals("clamp_hi_low"))
			{
				DataTransFn = TransformFunc.ClampHiLow;
			}
			else if (fnc.Equals("clamp_low"))
			{
				DataTransFn = TransformFunc.ClampLow;
			}
			else
			{
				throw new Exception("Invalid transform func " + fnc);
			}
		}
	}
}
