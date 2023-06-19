using System;

namespace Portland.PGC
{
	public sealed class FuncPostNoise2D : NoiseGen
	{
		INoise _noise;
		Func<double, double> _fn;

		public FuncPostNoise2D(INoise noise, Func<double, double> fn)
		{
			_noise = noise;
			_fn = fn;
		}

		public override double Value2D(double x, double y)
		{

			return _fn(_noise.Value2D(x, y));
		}

		public override double Value3D(double x, double y, double z)
		{
			throw new NotImplementedException();
		}
	}

	public sealed class FuncPostNoise3D : NoiseGen
	{
		INoise _noise;
		Func<double, double> _fn;

		public FuncPostNoise3D(INoise noise, Func<double, double> fn)
		{
			_noise = noise;
			_fn = fn;
		}

		public override double Value2D(double x, double y)
		{
			throw new NotImplementedException();
		}

		public override double Value3D(double x, double y, double z)
		{
			return _fn(_noise.Value3D(x, y, z));
		}
	}
}