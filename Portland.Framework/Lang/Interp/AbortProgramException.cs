using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Interp
{
	public class AbortProgramException : Exception
	{
		public AbortProgramException(string reason)
		: base(reason)
		{
		}
	}
}
