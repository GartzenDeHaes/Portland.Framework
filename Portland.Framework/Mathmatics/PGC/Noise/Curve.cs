using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Portland.Mathmatics;

namespace Portland.PGC
{
	public class Curve : NoiseGen
	{
		INoise _input;
		FuncTabled _func;

		public Curve(INoise input, FuncTabled func)
		{
			//Debug.Assert(func.DomainMin == 0f || func.DomainMin == -1f);
			//Debug.Assert(func.DomainMax == 1f);

			_input = input;
			_func = func;
		}

		public override double Value2D(double x, double y)
		{
			return _func[_input.Value2D(x, y)];
		}

		public override double Value3D(double x, double y, double z)
		{
			return _func[_input.Value3D(x, y, z)];
		}
	}
}
