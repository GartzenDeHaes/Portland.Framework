using System;
using System.Collections.Generic;
using System.Text;

using Portland.Mathmatics;

namespace Portland.PGC
{
	public class Blend : NoiseGen
	{
		INoise _lhs;
		INoise _rhs;
		INoise _blend;

		public Blend(INoise left, INoise right, INoise blendFactor)
		{
			_lhs = left;
			_rhs = right;
			_blend = blendFactor;
		}

		public override double Value2D(double x, double y)
		{
			var a = _lhs.Value2D(x, y);
			var b = _rhs.Value2D(x, y);
			var c = _blend.Value2D(x, y);
			return MathHelper.LerpUnclamped(a, b, c);
		}

		public override double Value3D(double x, double y, double z)
		{
			var a = _lhs.Value3D(x, y, z);
			var b = _rhs.Value3D(x, y, z);
			var c = _blend.Value3D(x, y, z);
			return MathHelper.LerpUnclamped(a, b, c);
		}
	}
}
