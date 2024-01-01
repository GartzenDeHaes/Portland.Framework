using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Xor : BinaryOperator
	{
		private static Xor _single = new Xor();

		public static Xor GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return new Variant(left.Execute(ctx).ToInt() ^ right.Execute(ctx).ToInt());
		}
	}
}
