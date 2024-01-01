using System;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Literal : UnaryOperator
	{
		private IVariant _value;

		public Literal(IVariant val)
		{
			_value = val;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			Debug.Assert(left == null && right == null);
			return _value;
		}
	}
}
