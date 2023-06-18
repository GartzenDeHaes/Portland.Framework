using System;

namespace Portland.PGC
{
	public sealed class NormalizeInput : NoiseGen
	{
		double _min, _max;
		INoise _input;

		public override double Value2D(double x, double y)
		{
			var val = _input.Value2D(x, y);
			return (val - _min) / (_max - _min);
		}

		public override double Value3D(double x, double y, double z)
		{
			var val = _input.Value3D(x, y, z);
			return (val - _min) / (_max - _min);
		}

		public NormalizeInput(double min, double max, INoise input)
		{
			_min = min;
			_max = max;
			_input = input;
		}
	}

	public sealed class NormalizeInputAuto : NoiseGen
	{
		double _min, _max;
		INoise _input;

		public override double Value2D(double x, double y)
		{
			var val = _input.Value2D(x, y);
			if (val > _max)
			{
				_max = val;
			}
			else if (val < _min)
			{
				_min = val;
			}
			return (val - _min) / (_max - _min);
		}

		public override double Value3D(double x, double y, double z)
		{
			var val = _input.Value3D(x, y, z);
			return (val - _min) / (_max - _min);
		}

		public NormalizeInputAuto(double min, double max, INoise input)
		{
			_min = min;
			_max = max;
			_input = input;
		}
	}
}
