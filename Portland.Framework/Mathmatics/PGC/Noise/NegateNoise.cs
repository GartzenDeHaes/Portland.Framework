using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portland.PGC
{
	public sealed class NegateNoise : NoiseGen
	{
		INoise Noise;

		public NegateNoise(INoise Noise)
		{
			this.Noise = Noise;
		}

		public override double Value2D(double X, double Y)
		{
			return -Noise.Value2D(X, Y);
		}

		public override double Value3D(double X, double Y, double Z)
		{
			return -Noise.Value3D(X, Y, Z);
		}
	}
}