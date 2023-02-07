using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class Print : Statement
	{
		public ArgumentList Args
		{
			get; private set;
		}

		public Print(int lineNum)
		: base(lineNum)
		{
			Args = new ArgumentList();
		}

		public override bool Execute(ExecutionContext ctx)
		{
			StringBuilder buf = new StringBuilder();
			for (int x = 0; x < Args.Count; x++)
			{
				if (x > 0)
				{
					buf.Append(' ');
				}
				buf.Append((string)Args.Execute(x, ctx));
			}

			ctx.WriteLine(buf.ToString());

			return true;
		}
	}
}
