using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portland.PGC
{
	public sealed class ClampNoiseConst : NoiseGen
	{
		public INoise Noise { get; set; }
		public double MinValue { get; set; }
		public double MaxValue { get; set; }

		public ClampNoiseConst(INoise noise, double min = 0.0, double max = 1.0)
		{
			Noise = noise;
			MinValue = min;
			MaxValue = max;
		}

		public override double Value2D(double x, double y)
		{
			var NoiseValue = Noise.Value2D(x, y);
			if (NoiseValue < MinValue)
				NoiseValue = MinValue;
			if (NoiseValue > MaxValue)
				NoiseValue = MaxValue;
			return NoiseValue;
		}

		public override double Value3D(double x, double y, double z)
		{
			var NoiseValue = Noise.Value3D(x, y, z);
			if (NoiseValue < MinValue)
				NoiseValue = MinValue;
			if (NoiseValue > MaxValue)
				NoiseValue = MaxValue;
			return NoiseValue;
		}
	}
}