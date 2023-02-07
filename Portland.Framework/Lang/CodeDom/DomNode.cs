using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom
{
	public abstract class DomNode
	{
		private static DomNode _doNothing = new Operators.CollectTerms();

		public static DomNode GetDoNothing()
		{
			return _doNothing;
		}

		public abstract bool IsBinary
		{
			get;
		}

		public abstract Variant Execute(ExecutionContext ctx, Expression left, Expression right);
	}
}
