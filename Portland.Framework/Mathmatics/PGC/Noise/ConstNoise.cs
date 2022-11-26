using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Portland.PGC
{
	public class ConstNoise : NoiseGen
	{
		double _value;

		public ConstNoise(double value)
		{
			Debug.Assert(value >= 0f);
			Debug.Assert(value <= 1f);

			_value = value;
		}

		public override double Value2D(double x, double y)
		{
			return _value;
		}

		public override double Value3D(double x, double y, double z)
		{
			return _value;
		}
	}
}
