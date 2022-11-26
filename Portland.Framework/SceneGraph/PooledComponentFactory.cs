using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Portland.SceneGraph
{
	public class PooledComponentFactory<T> : IComponentFactory where T : IComponent, new()
	{
		SpinLock _lock;
		Stack<IComponent> _stack;
		int _numAlloc;

		public Type ManagedComponent { get; }

		public virtual IComponent GetComponent()
		{
			if (_stack.Count < 2)
			{
				Fill();
			}

			bool taken = false;
			_lock.Enter(ref taken);
			var ret = _stack.Pop();
			if (taken)
			{
				_lock.Exit();
			}
			return ret;
		}

		public virtual void ReleaseComponent(IComponent component)
		{
			Debug.Assert(component != null);

			bool taken = false;
			_lock.Enter(ref taken);
			_stack.Push(component);
			if (taken)
			{
				_lock.Exit();
			}
		}

		public PooledComponentFactory(int numAlloc = 8)
		{
			_lock = new SpinLock();
			_stack = new Stack<IComponent>();
			_numAlloc = numAlloc;
			ManagedComponent = typeof(T);

			Fill();
		}

		void Fill()
		{
			bool taken = false;
			_lock.Enter(ref taken);

			for (int x = 0; x < _numAlloc; x++)
			{
				_stack.Push(new T());
			}

			if (taken)
			{
				_lock.Exit();
			}
		}
	}
}
