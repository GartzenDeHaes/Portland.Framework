using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portland.PGC
{
	public sealed class AbsNoise : NoiseGen
	{
		INoise Noise;

		public AbsNoise(INoise Noise)
		{
			this.Noise = Noise;
		}

		public override double Value2D(double X, double Y)
		{
			return Math.Abs(Noise.Value2D(X, Y));
		}

		public override double Value3D(double X, double Y, double Z)
		{
			return Math.Abs(Noise.Value3D(X, Y, Z));
		}
	}
}