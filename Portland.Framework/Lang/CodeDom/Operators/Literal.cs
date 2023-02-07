using System;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Literal : UnaryOperator
	{
		private Variant _value;

		public Literal(Variant val)
		{
			_value = val;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			Debug.Assert(left == null && right == null);
			return _value;
		}
	}
}
