using System;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Negate : UnaryOperator
	{
		private static Negate _single = new Negate();

		public static Negate GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			if (left != null)
			{
				Debug.Assert(right == null);
				return left.Execute(ctx).MathNeg();
			}

			return right.Execute(ctx).MathNeg();
		}
	}
}
