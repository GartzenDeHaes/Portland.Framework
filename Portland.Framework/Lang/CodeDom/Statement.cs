using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom
{
	public abstract class Statement
	{
		public int LineNumber
		{
			get; private set;
		}

		public Statement(int lineNo)
		{
			LineNumber = lineNo;
		}

		public abstract bool Execute(ExecutionContext ctx);
	}
}
