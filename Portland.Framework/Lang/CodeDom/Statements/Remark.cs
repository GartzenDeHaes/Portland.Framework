using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;
//using Maximum.Threading;

namespace Portland.CodeDom.Statements
{
	public sealed class Remark : Statement
	{
		private string _comment;

		public Remark(int lineNum, string comment)
		: base(lineNum)
		{
			_comment = comment;
		}

		public override bool Execute(ExecutionContext ctx)
		{
			ctx.OnLog?.Invoke(LogMessageSeverity.Info, _comment);
			return true;
		}
	}
}
