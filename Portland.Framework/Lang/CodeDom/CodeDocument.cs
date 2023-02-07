using System;
using System.Collections.Generic;

using Portland.CodeDom.Statements;
using Portland.Collections;
using Portland.Interp;

namespace Portland.CodeDom
{
	public class CodeDocument : IDisposable
	{
		private readonly Vector<Statement> _statements = new Vector<Statement>();
		
		private readonly Dictionary<string, IFunction> _userSubs = new Dictionary<string, IFunction>();

		public bool HasSyntaxError
		{
			get; protected set;
		}

		public string ErrorText
		{
			get; protected set;
		}

		public void Execute(ExecutionContext ctx)
		{
			if (HasSyntaxError)
			{
				throw new Exception("Called program with error of: " + ErrorText);
			}

			ctx.ReadyRun(_userSubs);

			int count = _statements.Count;
			for (int x = 0; x < count; x++)
			{
				var stmt = _statements[x];

				stmt.Execute(ctx);
			}

			ctx.EndRun();
		}

		public CodeDocument()
		{
		}

		public void Clear()
		{
			_statements.Clear();
		}

		public void AddStatement(Statement stmt)
		{
			_statements.Add(stmt);
		}

		public void AddSub(DefFn fn)
		{
			if (_userSubs.ContainsKey(fn.Name))
			{
				throw new Exception("Duplicate sub name " + fn.Name);
			}
			_userSubs.Add(fn.Name, fn);
		}

		public virtual void Dispose()
		{
			_statements.Clear();
			_userSubs.Clear();
		}
	}
}
