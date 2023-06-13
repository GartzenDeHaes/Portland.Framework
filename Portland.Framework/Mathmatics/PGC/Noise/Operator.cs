using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portland.PGC
{
	public enum NoiseOperation
	{
		Add,
		Subtract,
		Multiply,
		Power
	}

	public sealed class Operator : NoiseGen
	{
		public INoise PrimaryNoise { get; set; }
		public INoise SecondaryNoise { get; set; }
		public NoiseOperation Modifier { get; set; }

		public Operator(INoise primaryNoise, INoise secondaryNoise, NoiseOperation modifier = NoiseOperation.Add)
		{
			this.PrimaryNoise = primaryNoise;
			this.SecondaryNoise = secondaryNoise;
			this.Modifier = modifier;
		}

		public override double Value2D(double x, double y)
		{
			switch (Modifier)
			{
				case NoiseOperation.Add:
					return PrimaryNoise.Value2D(x, y) + SecondaryNoise.Value2D(x, y);
				case NoiseOperation.Multiply:
					return PrimaryNoise.Value2D(x, y) * SecondaryNoise.Value2D(x, y);
				case NoiseOperation.Power:
					return Math.Pow(PrimaryNoise.Value2D(x, y), SecondaryNoise.Value2D(x, y));
				case NoiseOperation.Subtract:
					return PrimaryNoise.Value2D(x, y) - SecondaryNoise.Value2D(x, y);
				default:
					//This is unreachable.
					return PrimaryNoise.Value2D(x, y) + SecondaryNoise.Value2D(x, y);
			}
		}

		public override double Value3D(double x, double y, double z)
		{
			switch (Modifier)
			{
				case NoiseOperation.Add:
					return PrimaryNoise.Value3D(x, y, z) + SecondaryNoise.Value3D(x, y, z);
				case NoiseOperation.Multiply:
					return PrimaryNoise.Value3D(x, y, z) * SecondaryNoise.Value3D(x, y, z);
				case NoiseOperation.Power:
					return Math.Pow(PrimaryNoise.Value3D(x, y, z), SecondaryNoise.Value3D(x, y, z));
				case NoiseOperation.Subtract:
					return PrimaryNoise.Value3D(x, y, z) - SecondaryNoise.Value3D(x, y, z);
				default:
					//This is unreachable.
					return PrimaryNoise.Value3D(x, y, z) + SecondaryNoise.Value3D(x, y, z);
			}
		}
	}
}
