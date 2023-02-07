using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class AbortProgram : Statement
	{
		public AbortProgram(int lineNum)
		: base(lineNum)
		{
		}

		public override bool Execute(ExecutionContext ctx)
		{
			throw new AbortProgramException($"ABORT ON LINE {LineNumber}");
		}
	}
}
