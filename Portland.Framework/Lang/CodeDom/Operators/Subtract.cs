using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Subtract : BinaryOperator
	{
		private static Subtract _single = new Subtract();

		public static Subtract GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) - right.Execute(ctx);
		}
	}
}
