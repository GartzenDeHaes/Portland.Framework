using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Interp
{
	public interface ICommandRunner
	{
		void ICommandRunner_Exec(ExecutionContext ctx, string name, Variant args);
	}
}
