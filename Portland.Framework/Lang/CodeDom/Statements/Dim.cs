using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class Dim : Statement
	{
		public string VarName
		{
			get;
		}

		public Expression Indexer
		{
			get; set;
		}

		public Dim(int lineNum, string varName)
		: base(lineNum)
		{
			VarName = varName;
		}

		public override bool Execute(ExecutionContext ctx)
		{
			if (Indexer == null)
			{
				ctx.ClearVariable(VarName);
			}
			else
			{
				// array
				ctx.ClearVariableArray(VarName, Indexer.Execute(ctx));
			}

			return true;
		}
	}
}
