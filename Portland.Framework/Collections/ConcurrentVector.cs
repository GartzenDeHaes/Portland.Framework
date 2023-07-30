using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Portland.Mathmatics;

namespace Portland.Collections
{
	[Serializable]
	public sealed class ConcurrentVectorSlim<T> : IIndexed<T>, IEnumerable<T>, IList<T>
	{
		readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

		T[] m_data;
		int m_used;

		public bool IsReadOnly { get { return false; } }

		public int Count
		{
			get { return Volatile.Read(ref m_used); }
		}

		public ConcurrentVectorSlim()
		{
			m_data = new T[10];
		}

		public ConcurrentVectorSlim(int size)
		{
			m_data = new T[size];
		}

		public ConcurrentVectorSlim(IEnumerable<T> values)
		{
			m_data = values.ToArray();
			m_used = m_data.Length;
		}

		public T Pop()
		{
			_lock.Wait();
			try
			{
				if (m_used > 0)
				{
					return m_data[--m_used];
				}
				else
				{
					return default(T);
				}
			}
			finally { _lock.Release(); }
		}

		public bool TryTake(out T value)
		{
			_lock.Wait();
			try
			{
				if (m_used > 0)
				{
					value = m_data[--m_used];
					return true;
				}
				else
				{
					value = default(T);
					return false;
				}
			}
			finally { _lock.Release(); }
		}

		/// <summary>
		/// Bottom of stack, first element in list
		/// </summary>
		public T Head()
		{
			_lock.Wait();
			try
			{
				return m_data[0];
			}
			finally { _lock.Release(); }
		}

		/// <summary>
		/// Top of stack, last element in list
		/// </summary>
		public T Tail()
		{
			_lock.Wait();
			try
			{
				return m_data[m_used - 1];
			}
			finally { _lock.Release(); }
		}

		public int Capacity()
		{
			return m_data.Length;
		}

		public int AddElement(in T o)
		{
			_lock.Wait();
			try
			{
				Extend();
				m_data[m_used] = o;
				return m_used++;
			}
			finally { _lock.Release(); }
		}

		public void Add(T o)
		{
			_lock.Wait();
			try
			{
				Extend();
				m_data[m_used++] = o;
			}
			finally { _lock.Release(); }
		}

		public int AddAndGetIndex(in T o)
		{
			_lock.Wait();
			try
			{
				Extend();
				m_data[m_used++] = o;
				return m_used - 1;
			}
			finally { _lock.Release(); }
		}

		public void Add(T[] ta)
		{
			for (int x = 0; x < ta.Length; x++)
			{
				Add(ta[x]);
			}
		}

		public void AddRangeWithPad(T[] a, int count)
		{
			for (int x = 0; x < count; x++)
			{
				if (x < a.Length)
				{
					Add(a[x]);
				}
				else
				{
					Add(default(T));
				}
			}
		}

		void Extend()
		{
			if (m_used >= m_data.Length)
			{
				//int space = m_data.Length;
				//while (m_used >= space)
				//{
				//	//space <<= 1;
				//	space += space / 3;
				//}
				T[] array2 = new T[m_data.Length + m_data.Length / 3 + 1];
				int i;
				for (i = 0; (i < m_used); i++)
				{
					array2[i] = m_data[i];
				}
				m_data = array2;
			}
		}

		public void SetSize(int s)
		{
			_lock.Wait();

			if (m_data.Length < s)
			{
				m_data = new T[s];
			}
			m_used = s;

			_lock.Release();
		}

		public void Clear()
		{
			Volatile.Write(ref m_used, 0);
		}

		public T ElementAt(int at)
		{
			_lock.Wait();
			try
			{
				return m_data[at];
			}
			finally { _lock.Release(); }
		}

		public ref T ElementAtRef(int at)
		{
			return ref m_data[at];
		}

		public T this[int idx]
		{
			get { return ElementAt(idx); }
			set { SetElementAt(idx, value); }
		}

		public void SetElementAt(int at, in T o)
		{
			m_data[at] = o;
		}

		public T FirstElement()
		{
			if (Volatile.Read(ref m_used) > 0)
			{
				return m_data[0];
			}
			else
			{
				return default(T);
			}
		}

		public T LastElement()
		{
			if (m_used > 0)
			{
				return m_data[m_used - 1];
			}
			else
			{
				return default(T);
			}
		}

		public ref T LastElementRef()
		{
			int end = Volatile.Read(ref m_used);
			if (end > 0)
			{
				return ref m_data[end - 1];
			}
			else
			{
				throw new Exception("Vector empty");
			}
		}

		public bool Remove(T o)
		{
			int i;
			for (i = 0; (i < m_used); i++)
			{
				if (m_data[i].Equals(o))
				{
					RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		public void RemoveAt(int at)
		{
			int i;
			for (i = at; (i < (m_used - 1)); i++)
			{
				m_data[i] = m_data[i + 1];
			}
			m_used--;
		}

		public void RemoveWhen(Func<T, bool> test)
		{
			int i = 0;
			while (i < m_used)
			{
				if (test(m_data[i]))
				{
					RemoveAt(i);
					continue;
				}
				i++;
			}
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public int IndexOf(T o)
		{
			int i;
			for (i = 0; (i < m_used); i++)
			{
				if (m_data[i].Equals(o))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int at, T o)
		{
			int i;
			Extend();
			for (i = m_used; (i > at); i--)
			{
				m_data[i] = m_data[i - 1];
			}
			m_data[at] = o;
			m_used++;
		}

		public void Copy(IList<T> v)
		{
			Clear();
			for (int i = 0; i < v.Count; i++)
			{
				AddElement(v.ElementAt(i));
			}
		}

		public void InsertBefore(in T oNew, in T oExisting)
		{
			int at = IndexOf(oExisting);
			if (at < 0)
			{
				throw new Exception("insertBefore can't find before");
				//addElement(oNew);
				//return;
			}
			Insert(at, oNew);
		}

		public bool IsEmpty()
		{
			return Volatile.Read(ref m_used) == 0;
		}

		public T[] ToArray()
		{
			T[] ta = new T[Count];
			for (int x = 0; x < ta.Length; x++)
			{
				ta[x] = ElementAt(x);
			}
			return ta;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			int len = Volatile.Read(ref m_used);
			for (int x = 0; x < len; x++)
			{
				array[x + arrayIndex] = ElementAt(x);
			}
		}

		public T[] Data()
		{
			return m_data;
		}

		public T RandomElement()
		{
			return ElementAt(MathHelper.RandomRange(0, Count - 1));
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new VectorEnumerator(m_data, m_used);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new VectorEnumerator(m_data, m_used);
		}

		class VectorEnumerator : IEnumerator<T>
		{
			T[] m_data;
			readonly int m_len;
			int m_pos;
			public T Current { get { return m_pos >= m_len ? default(T) : m_data[m_pos]; } }
			object IEnumerator.Current { get { return m_pos >= m_len ? default(T) : m_data[m_pos]; } }

			public VectorEnumerator(T[] data, int len)
			{
				m_data = data;
				m_len = len;
				m_pos = -1;
			}

			public bool MoveNext()
			{
				return ++m_pos < m_len;
			}

			public void Dispose()
			{
				m_data = null;
			}

			public void Reset()
			{
				m_pos = -1;
			}
		}
	}

	[Serializable]
	public sealed class ConcurrentVectorSpin<T> : IIndexed<T>, IEnumerable<T>, IList<T>
	{
		SpinLock _lock = new SpinLock();

		T[] m_data;
		int m_used;

		public bool IsReadOnly { get { return false; } }

		public int Count
		{
			get { return Volatile.Read(ref m_used); }
		}

		public ConcurrentVectorSpin()
		{
			m_data = new T[10];
		}

		public ConcurrentVectorSpin(int size)
		{
			m_data = new T[size];
		}

		public ConcurrentVectorSpin(IEnumerable<T> values)
		{
			m_data = values.ToArray();
			m_used = m_data.Length;
		}

		public T Pop()
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				if (m_used > 0)
				{
					return m_data[--m_used];
				}
				else
				{
					return default(T);
				}
			}
			finally { if (gotlock) { _lock.Exit(false); } }
		}

		public bool TryTake(out T value)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				if (m_used > 0)
				{
					value = m_data[--m_used];
					return true;
				}
				else
				{
					value = default(T);
					return false;
				}
			}
			finally { if (gotlock) { _lock.Exit(false); } }
		}

		/// <summary>
		/// Bottom of stack, first element in list
		/// </summary>
		public T Head()
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				return m_data[0];
			}
			finally { if (gotlock) { _lock.Exit(false); } }
		}

		/// <summary>
		/// Top of stack, last element in list
		/// </summary>
		public T Tail()
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				return m_data[m_used - 1];
			}
			finally { if (gotlock) { _lock.Exit(false); } }
		}

		public int Capacity()
		{
			return m_data.Length;
		}

		public int AddElement(in T o)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				Extend();
				m_data[m_used] = o;
				return m_used++;
			}
			finally { if (gotlock) { _lock.Exit(false); } }
		}

		public void Add(T o)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				Extend();
				m_data[m_used++] = o;
			}
			finally { if (gotlock) { _lock.Exit(false); } }
		}

		public int AddAndGetIndex(in T o)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				Extend();
				m_data[m_used++] = o;
				return m_used - 1;
			}
			finally { if (gotlock) { _lock.Exit(false); } }
		}

		public void Add(T[] ta)
		{
			for (int x = 0; x < ta.Length; x++)
			{
				Add(ta[x]);
			}
		}

		public void AddRangeWithPad(T[] a, int count)
		{
			for (int x = 0; x < count; x++)
			{
				if (x < a.Length)
				{
					Add(a[x]);
				}
				else
				{
					Add(default(T));
				}
			}
		}

		void Extend()
		{
			if (m_used >= m_data.Length)
			{
				//int space = m_data.Length;
				//while (m_used >= space)
				//{
				//	//space <<= 1;
				//	space += space / 3;
				//}
				T[] array2 = new T[m_data.Length + m_data.Length / 3 + 1];
				int i;
				for (i = 0; (i < m_used); i++)
				{
					array2[i] = m_data[i];
				}
				m_data = array2;
			}
		}

		public void SetSize(int s)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);

			if (m_data.Length < s)
			{
				m_data = new T[s];
			}
			m_used = s;

			if (gotlock) { _lock.Exit(); }
		}

		public void Clear()
		{
			Volatile.Write(ref m_used, 0);
		}

		public T ElementAt(int at)
		{
			bool gotlock = false;
			_lock.Enter(ref gotlock);
			try
			{
				return m_data[at];
			}
			finally { if (gotlock) { _lock.Exit(); } }
		}

		public ref T ElementAtRef(int at)
		{
			return ref m_data[at];
		}

		public T this[int idx]
		{
			get { return ElementAt(idx); }
			set { SetElementAt(idx, value); }
		}

		public void SetElementAt(int at, in T o)
		{
			m_data[at] = o;
		}

		public T FirstElement()
		{
			if (Volatile.Read(ref m_used) > 0)
			{
				return m_data[0];
			}
			else
			{
				return default(T);
			}
		}

		public T LastElement()
		{
			if (m_used > 0)
			{
				return m_data[m_used - 1];
			}
			else
			{
				return default(T);
			}
		}

		public ref T LastElementRef()
		{
			int end = Volatile.Read(ref m_used);
			if (end > 0)
			{
				return ref m_data[end - 1];
			}
			else
			{
				throw new Exception("Vector empty");
			}
		}

		public bool Remove(T o)
		{
			int i;
			for (i = 0; (i < m_used); i++)
			{
				if (m_data[i].Equals(o))
				{
					RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		public void RemoveAt(int at)
		{
			int i;
			for (i = at; (i < (m_used - 1)); i++)
			{
				m_data[i] = m_data[i + 1];
			}
			m_used--;
		}

		public void RemoveWhen(Func<T, bool> test)
		{
			int i = 0;
			while (i < m_used)
			{
				if (test(m_data[i]))
				{
					RemoveAt(i);
					continue;
				}
				i++;
			}
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public int IndexOf(T o)
		{
			int i;
			for (i = 0; (i < m_used); i++)
			{
				if (m_data[i].Equals(o))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int at, T o)
		{
			int i;
			Extend();
			for (i = m_used; (i > at); i--)
			{
				m_data[i] = m_data[i - 1];
			}
			m_data[at] = o;
			m_used++;
		}

		public void Copy(IList<T> v)
		{
			Clear();
			for (int i = 0; i < v.Count; i++)
			{
				AddElement(v.ElementAt(i));
			}
		}

		public void InsertBefore(in T oNew, in T oExisting)
		{
			int at = IndexOf(oExisting);
			if (at < 0)
			{
				throw new Exception("insertBefore can't find before");
				//addElement(oNew);
				//return;
			}
			Insert(at, oNew);
		}

		public bool IsEmpty()
		{
			return Volatile.Read(ref m_used) == 0;
		}

		public T[] ToArray()
		{
			T[] ta = new T[Count];
			for (int x = 0; x < ta.Length; x++)
			{
				ta[x] = ElementAt(x);
			}
			return ta;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			int len = Volatile.Read(ref m_used);
			for (int x = 0; x < len; x++)
			{
				array[x + arrayIndex] = ElementAt(x);
			}
		}

		public T[] Data()
		{
			return m_data;
		}

		public T RandomElement()
		{
			return ElementAt(MathHelper.RandomRange(0, Count - 1));
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new VectorEnumerator(m_data, m_used);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new VectorEnumerator(m_data, m_used);
		}

		class VectorEnumerator : IEnumerator<T>
		{
			T[] m_data;
			readonly int m_len;
			int m_pos;
			public T Current { get { return m_pos >= m_len ? default(T) : m_data[m_pos]; } }
			object IEnumerator.Current { get { return m_pos >= m_len ? default(T) : m_data[m_pos]; } }

			public VectorEnumerator(T[] data, int len)
			{
				m_data = data;
				m_len = len;
				m_pos = -1;
			}

			public bool MoveNext()
			{
				return ++m_pos < m_len;
			}

			public void Dispose()
			{
				m_data = null;
			}

			public void Reset()
			{
				m_pos = -1;
			}
		}
	}
}
