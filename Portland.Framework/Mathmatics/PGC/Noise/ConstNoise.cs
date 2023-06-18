using System;

namespace Portland.PGC
{
	public sealed class ConstNoise : NoiseGen
	{
		double _value;

		public ConstNoise(double value)
		{
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
