using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Interp
{
	public interface ICommandRunner
	{
		void Run(ExecutionContext ctx, string name, Variant args);
	}
}
