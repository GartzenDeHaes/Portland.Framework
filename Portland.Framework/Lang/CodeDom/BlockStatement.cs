using System;
using System.Collections.Generic;
using System.Text;

using Portland.Collections;
using Portland.Interp;

namespace Portland.CodeDom
{
	public sealed class BlockStatement : Statement
	{
		private Vector<Statement> _statements = new Vector<Statement>();

		public BlockStatement(int lineNum)
		: base(lineNum)
		{
		}

		public void Add(Statement stmt)
		{
			_statements.Add(stmt);
		}

		public override bool Execute(ExecutionContext ctx)
		{
			int count = _statements.Count;
			for (int x = 0; x < count; x++)
			{
				if (! _statements[x].Execute(ctx))
				{
					return false;
				}
			}
			return true;
		}
	}
}
