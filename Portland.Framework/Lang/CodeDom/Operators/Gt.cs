using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Gt : BinaryOperator
	{
		private static Gt _single = new Gt();

		public static Gt GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return new Variant(left.Execute(ctx).CompareTo(right.Execute(ctx)) > 0);
		}
	}
}
