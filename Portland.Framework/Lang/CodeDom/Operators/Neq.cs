using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Neq : BinaryOperator
	{
		private static Neq _single = new Neq();

		public static Neq GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) != right.Execute(ctx);
		}
	}
}
