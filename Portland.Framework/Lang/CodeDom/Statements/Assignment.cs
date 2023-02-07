using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class Assignment : Statement
	{
		public string VarName
		{
			get;
		}

		public Expression Value
		{
			get; set;
		}

		public Expression Indexer
		{
			get; set;
		}

		public Assignment(int lineNum, string varName)
		: base(lineNum)
		{
			VarName = varName;
		}

		public override bool Execute(ExecutionContext ctx)
		{
			if (Indexer == null)
			{
				ctx.SetVariable(VarName, Value.Execute(ctx));
			}
			else
			{
				// array
				ctx.SetVariableArray(VarName, Indexer.Execute(ctx), Value.Execute(ctx));
			}
			return true;
		}
	}
}
