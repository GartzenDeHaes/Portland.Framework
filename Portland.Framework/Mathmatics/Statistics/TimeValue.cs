using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portland.Mathmatics
{
	public class TimeValue
	{
		public TimeValue(Date dt, double val)
		{
			Date = dt;
			Value = val;
		}

		public Date Date
		{
			get;
			private set;
		}

		public double Value
		{
			get;
			private set;
		}
	}
}
