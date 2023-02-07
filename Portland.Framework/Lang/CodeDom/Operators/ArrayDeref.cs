using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class ArrayDeref : UnaryOperator
	{
		public string VariableName
		{
			get;
		}

		public Expression Indexer
		{
			get; set;
		}

		public ArrayDeref(string varname)
		{
			VariableName = varname;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return ctx.FindVariable(VariableName)[(string)Indexer.Execute(ctx)];
		}
	}
}
