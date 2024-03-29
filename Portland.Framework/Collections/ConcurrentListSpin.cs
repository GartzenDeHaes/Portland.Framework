﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Portland.Collections
{
	public sealed class ConcurrentListSpin<T> : /*IList<T>, IIndexed<T>,*/ IEnumerable<T>, IDisposable
	{
		SpinLock _lock = new SpinLock();
		readonly ListNode<T> _head = new ListNode<T>();
		int _count;

		public ConcurrentListSpin()
		{
		}

		public int Count
		{
			get
			{
				bool gotlock = false;
				_lock.Enter(ref gotlock);

				int count = _count;

				if (gotlock) _lock.Exit(false);

				return count;
			}
		}

		//public bool IsReadOnly
		//{
		//	get
		//	{
		//		return false;
		//	}
		//}

		//public T this[int index]
		//{
		//	get
		//	{
		//		bool gotlock = false;
		//		_lock.Enter(ref gotlock);

		//		T result;
		//		try
		//		{
		//			result = _list[index];
		//		}
		//		finally
		//		{
		//			if (gotlock) _lock.Exit();
		//		}

		//		return result;
		//	}
		//	set
		//	{
		//		bool gotlock = false;
		//		_lock.Enter(ref gotlock);

		//		try
		//		{
		//			_list[index] = value;
		//		}
		//		finally
		//		{
		//			if (gotlock) _lock.Exit();
		//		}
		//	}
		//}

		public void Add(T item)
		{
			var node = new ListNode<T>() { Item = item };

			bool gotlock = false;
			_lock.Enter(ref gotlock);

			node.Next = _head.Next;
			_head.Next = node;
			_count++;

			if (gotlock) _lock.Exit(false);
		}

		//public int IndexOf(T item)
		//{
		//	bool gotlock = false;
		//	_lock.Enter(ref gotlock);

		//	int result;
		//	try
		//	{
		//		result = _list.IndexOf(item);
		//	}
		//	finally
		//	{
		//		if (gotlock) _lock.Exit();
		//	}

		//	return result;
		//}

		//public void Insert(int index, T item)
		//{
		//	bool gotlock = false;
		//	_lock.Enter(ref gotlock);

		//	try
		//	{
		//		_list.Insert(index, item);
		//	}
		//	finally
		//	{
		//		if (gotlock) _lock.Exit();
		//	}
		//}

		//public void RemoveAt(int index)
		//{
		//	bool gotlock = false;
		//	_lock.Enter(ref gotlock);

		//	try
		//	{
		//		_list.RemoveAt(index);
		//	}
		//	finally
		//	{
		//		if (gotlock) _lock.Exit();
		//	}
		//}

		public void Clear()
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);

			_head.Next = null;
			_count = 0;

			if (gotlock) _lock.Exit();
		}

		public bool Contains(T item, Func<T, T, bool> compare)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);

			bool found = false;

			var node = _head.Next;
			while (node != null)
			{
				if (compare(item, node.Item))
				{
					found = true;
					break;
				}
				node = node.Next;
			}

			if (gotlock) _lock.Exit(false);

			return found;
		}

		//public void CopyTo(T[] array, int arrayIndex)
		//{
		//	bool gotlock = false;
		//	_lock.Enter(ref gotlock);

		//	try
		//	{
		//		_list.CopyTo(array, arrayIndex);
		//	}
		//	finally
		//	{
		//		if (gotlock) _lock.Exit();
		//	}
		//}

		public bool Remove(T item, Func<T, T, bool> compare)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);

			bool found = false;

			var prev = _head;
			var node = _head.Next;
			while (node != null)
			{
				if (compare(node.Item, item))
				{
					found = true;
					_count--;
					prev.Next = node.Next;
					break;
				}

				prev = node;
				node = node.Next;
			}

			if (gotlock) _lock.Exit(false);

			return found;
		}

		public bool TryTake(out T item)
		{
			bool found = true;

			bool gotlock = false;
			_lock.Enter(ref gotlock);

			var node = _head.Next;
			if (node == null)
			{
				found = false;
				item = default;
			}
			else
			{
				_head.Next = node.Next;
				item = node.Item;
				_count--;
			}

			if (gotlock) _lock.Exit(false);

			return found;
		}

		public IEnumerator<T> GetEnumerator()
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);

			try
			{
				var node = _head.Next;
				while (node != null)
				{
					yield return node.Item;
					node = node.Next;
				}
			}
			finally
			{
				if (gotlock) _lock.Exit(false);
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void ForEach(Action<T> act)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);

			try
			{
				var node = _head.Next;
				while (node != null)
				{
					act(node.Item);
					node = node.Next;
				}
			}
			finally
			{
				if (gotlock) _lock.Exit(false);
			}
		}

		public void Dispose()
		{
			_head.Next = null;
			_count = 0;
		}
	}
}
