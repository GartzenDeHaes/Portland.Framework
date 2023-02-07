using System;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class CollectTerms : UnaryOperator
	{
		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			if (left != null)
			{
				Variant lhs = left.Execute(ctx);

				Debug.Assert(right == null);

				return lhs;
			}
			if (right != null)
			{
				return right.Execute(ctx);
			}

			return Variant.GetError();
		}
	}
}
