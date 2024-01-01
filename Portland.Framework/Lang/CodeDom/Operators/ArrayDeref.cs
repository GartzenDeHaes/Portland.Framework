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

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			var theVar = ctx.FindVariable(VariableName);
			if (theVar.TryGetProp(Indexer.Execute(ctx), out var ret))
			{
				return ret;
			}
			return new Variant();
		}
	}
}
