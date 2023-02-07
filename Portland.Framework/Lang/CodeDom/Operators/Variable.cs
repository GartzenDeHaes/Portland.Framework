using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Variable : UnaryOperator
	{
		private string _name;

		public Variable(string name)
		{
			_name = name;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			Debug.Assert(left == null && right == null);

			return ctx.FindVariable(_name);
		}
	}
}
