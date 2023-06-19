using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portland.PGC
{
	public sealed class InvertNoise : NoiseGen
	{
		INoise Noise;

		public InvertNoise(INoise Noise)
		{
			this.Noise = Noise;
		}

		public override double Value2D(double X, double Y)
		{
			return 1.0 - Noise.Value2D(X, Y);
		}

		public override double Value3D(double X, double Y, double Z)
		{
			return 1.0 - Noise.Value3D(X, Y, Z);
		}
	}
}