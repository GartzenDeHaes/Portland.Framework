using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Interp
{
	public interface IFunction
	{
		int ArgCount
		{
			get;
		}

		string GetArgName(int idx);

		bool Execute(ExecutionContext ctx);
	}
}
