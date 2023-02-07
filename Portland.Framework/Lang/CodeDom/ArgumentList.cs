using System;
using System.Collections.Generic;
using System.Text;

using Portland.Collections;
using Portland.Interp;

namespace Portland.CodeDom
{
	public sealed class ArgumentList
	{
		private Vector<Expression> _args = new Vector<Expression>(4);

		public int Count
		{
			get { return _args.Count; }
		}

		public Variant Execute(int idx, ExecutionContext ctx)
		{
			return _args[idx].Execute(ctx);
		}

		public void AddArgument(Expression expr)
		{
			_args.Add(expr);
		}
	}
}
