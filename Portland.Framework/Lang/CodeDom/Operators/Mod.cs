using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Mod : BinaryOperator
	{
		private static Mod _single = new Mod();

		public static Mod GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) % right.Execute(ctx);
		}
	}
}
