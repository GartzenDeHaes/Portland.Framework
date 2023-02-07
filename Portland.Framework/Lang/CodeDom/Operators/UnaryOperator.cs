using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.CodeDom.Operators
{
	public abstract class UnaryOperator : DomNode
	{
		public override bool IsBinary
		{
			get { return false; }
		}
	}
}
