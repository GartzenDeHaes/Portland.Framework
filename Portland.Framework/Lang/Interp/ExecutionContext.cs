using System;
using System.Collections.Generic;
using System.Diagnostics;

using Portland.AI;
using Portland.Basic;
using Portland.Collections;

namespace Portland.Interp
{
	public sealed class ExecutionContext : IDisposable, ICommandRunner
	{
		private readonly Vector<StackFrame> _ctxStk = new Vector<StackFrame>();

		public Action<string> OnPrint;
		public Action<LogMessageSeverity, string> OnLog;

		public object UserData;

		public ref StackFrame Context
		{
			get { return ref _ctxStk.LastElementRef(); }
		}

		public bool HasError
		{
			get; private set;
		}

		public string LastError;

		// TODO: convert built-ins to static or a table
		private readonly Dictionary<SubSig, IFunction> _globalFuncs;

		// Application specific commands. (built-ins are handled by the parser).
		private readonly ICommandRunner _cmds;

		// TODO: subs need to be persistant, change CodeDocument object or copy from it.
		private Dictionary<SubSig, IFunction> _progSubs;

		public ExecutionContext(Dictionary<SubSig, IFunction> globalFuncs, ICommandRunner cmds, object userData)
		: this(cmds)
		{
			_globalFuncs = globalFuncs;
			UserData = userData;
		}

		public ExecutionContext(ICommandRunner cmds)
		{
			_cmds = cmds;
			_ctxStk.Add(StackFrame.Create());
			_globalFuncs = new Dictionary<SubSig, IFunction>();
		}

		public ExecutionContext()
		: this(null)
		{
			_cmds = this;
		}


		public void SetError(string message)
		{
			LastError = message;
			HasError = true;
			OnLog?.Invoke(LogMessageSeverity.Error, message);
		}

		public void ClearError()
		{
			HasError = false;
		}

		public void ReadyRun(Dictionary<SubSig, IFunction> subs)
		{
			ClearError();
			_progSubs = subs;
		}

		public void EndRun()
		{
			_progSubs = null;
		}

		public IFunction GetSub(string name, int argCount)
		{
			if (_globalFuncs.TryGetValue(new SubSig { Name = name, ArgCount = argCount }, out var fn))
			{
				return fn;
			}

			//if (_progSubs == null)
			//{
			//	return null;
			//}

			return _progSubs[new SubSig { Name = name, ArgCount = argCount }];
		}

		public void PushContex()
		{
			_ctxStk.Add(StackFrame.Create());
			HasError = false;
		}

		public Variant PopContext()
		{
			return _ctxStk.Pop().Data;
		}

		public Variant FindVariable(string name)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].TryGetProp(name, out var value))
				{
					return value;
				}
			}

			return default(Variant);
		}

		public bool TryFindVariable(string name, out Variant value)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].TryGetProp(name, out value))
				{
					return true;
				}
			}

			value = default(Variant);
			return false;
		}

		public void ClearVariable(string name)
		{
			for (int x = _ctxStk.Count - 1; x >= 0; x--)
			{
				if (_ctxStk[x].HasProp(name))
				{
					_ctxStk[x].ClearProp(name);
					return;
				}
			}

			Context.ClearProp(name);
		}

		public void SetVariable(string name, in Variant value)
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

		public void SetVariableArray(string name, string index, in Variant value)
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


		public void SetReturnValue(float val)
		{
			_ctxStk.LastElementRef().Data.Set(val);
		}

		public void SetReturnValue(int val)
		{
			_ctxStk.LastElementRef().Data.Set(val);
		}

		public void SetReturnValue(string val)
		{
			_ctxStk.LastElementRef().Data.Set(val);
		}

		public void SetReturnValue(in Variant val)
		{
			_ctxStk.LastElementRef().Data.Set(val);
		}

		public Variant GetReturnValue()
		{
			return _ctxStk.LastElementRef().Data;
		}
		public void RunCommand(string name, Variant args)
		{
			_cmds.ICommandRunner_Exec(this, name, args);
		}

		public void WriteLine(string str)
		{
			OnPrint?.Invoke(str);
		}

		public void Dispose()
		{
			_ctxStk.Clear();
			OnPrint = null;
			OnLog = null;
			//_functs.Clear();
			_progSubs = null;
		}

		public void ICommandRunner_Exec(ExecutionContext ctx, string name, Variant args)
		{
			throw new NotImplementedException();
		}
	}
}
