using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Portland.Basic;
using Portland.CodeDom.Statements;
using Portland.Collections;
using Portland.Interp;

namespace Portland.CodeDom
{
	public class CodeDocument : IDisposable
	{
		private readonly Vector<Statement> _statements = new Vector<Statement>();
		
		private readonly Dictionary<SubSig, IFunction> _userSubs = new Dictionary<SubSig, IFunction>(/*StringComparer.InvariantCultureIgnoreCase*/);  //< 10% performance hit to ignore case

		//public bool HasSyntaxError
		//{
		//	get; protected set;
		//}

		public string ErrorText
		{
			get; protected set;
		}

		public void Execute(ExecutionContext ctx)
		{
			//if (HasSyntaxError)
			//{
			//	throw new Exception("Called program with error of: " + ErrorText);
			//}

			ctx.ReadyRun(_userSubs);

			int count = _statements.Count;
			for (int x = 0; x < count; x++)
			{
				_statements[x].Execute(ctx);
			}

			ctx.EndRun();
		}

		public BasicNativeFunctionBuilder GetFunctionBuilder()
		{
			return new BasicNativeFunctionBuilder { 
				InternalAdd = (name, argCount, fn) => _userSubs.Add(new SubSig { Name = String8.FromTruncate(name), ArgCount = argCount }, fn),
				HasFunction = (name, argCount) => _userSubs.ContainsKey(new SubSig { Name = String8.FromTruncate(name), ArgCount = argCount })
			};
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
			var ss = new SubSig { Name = String8.FromTruncate(fn.Name), ArgCount = fn.ArgCount };
			if (_userSubs.ContainsKey(ss))
			{
				throw new Exception("Duplicate sub name " + fn.Name);
			}
			_userSubs.Add(ss, fn);
		}

		public virtual void Dispose()
		{
			_statements.Clear();
			_userSubs.Clear();
		}
	}
}
