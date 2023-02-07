using System;
using System.Collections.Generic;
using System.Diagnostics;

using Portland.Basic;
using Portland.Collections;

namespace Portland.Interp
{
	public sealed class ExecutionContext
	{
		private readonly Vector<Variant> _ctxStk = new Vector<Variant>();

		public Action<string> OnPrint;
		public Action<LogMessageSeverity, string> OnLog;

		public Variant Context
		{
			get { return _ctxStk.LastElement(); }
		}

		public bool HasError
		{
			get; private set;
		}

		// TODO: convert built-ins to static or a table
		private readonly Dictionary<string, IFunction> _functs = new Dictionary<string, IFunction>();

		// Application specific commands. (built-ins are handled by the parser).
		private readonly ICommandRunner _cmds;

		// TODO: subs need to be persistant, change CodeDocument object or copy from it.
		private Dictionary<string, IFunction> _progSubs;

		public ExecutionContext
		(
			ICommandRunner cmds
		)
		{
			_cmds = cmds;
			_ctxStk.Add(String.Empty);
		}

		public BasicNativeFunctionBuilder GetFunctionBuilder()
		{
			return new BasicNativeFunctionBuilder { InternalAdd = (name, fn) => _functs.Add(name, fn) };
		}

		public void SetError(string message)
		{
			HasError = true;
			OnLog?.Invoke(LogMessageSeverity.Error, message);
		}

		public void ClearError()
		{
			HasError = false;
		}

		public void ReadyRun(Dictionary<string, IFunction> subs)
		{
			_progSubs = subs;
		}

		public void EndRun()
		{
			_progSubs = null;
		}

		public IFunction GetSub(string name)
		{
			if (_functs.TryGetValue(name, out var fn))
			{
				return fn;
			}

			if (_progSubs == null)
			{
				return null;
			}

			return _progSubs[name];
		}

		public void PushContex()
		{
			_ctxStk.Add(new Variant());
			HasError = false;
		}

		public Variant PopContext()
		{
			return _ctxStk.Pop();
		}

		public Variant FindVariable(string name)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].HasProp(name))
				{
					return _ctxStk[x][name];
				}
			}

			return new Variant();
		}

		public void ClearVariable(string name)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].HasProp(name))
				{
					_ctxStk[x][name].Clear();
					return;
				}
			}

			Context.ClearProp(name);
		}

		public void SetVariable(string name, Variant value)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].HasProp(name))
				{
					_ctxStk[x].SetProp(name, value);
					return;
				}
			}

			Context.SetProp(name, value);
		}

		public void ClearVariableArray(string name, int size)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].HasProp(name))
				{
					_ctxStk[x].ClearPropArray(name, size);
					return;
				}
			}

			Context.ClearPropArray(name, size);
		}

		public void SetVariableArray(string name, string index, Variant value)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].HasProp(name))
				{
					_ctxStk[x].SetPropArray(name, index, value);
					return;
				}
			}

			Context.SetPropArray(name, index, value);
		}

		public void RunCommand(string name, Variant args)
		{
			_cmds.Run(this, name, args);
		}

		public void WriteLine(string str)
		{
			OnPrint?.Invoke(str);
		}
	}
}
