using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Portland.Collections
{
	/// <summary>
	/// Base class for StringTable
	/// </summary>
	public class ValueTable<T>
	{
		private ConcurrentDictionary<int, int> _hashDict = new ConcurrentDictionary<int, int>();
		private List<T> _strs = new List<T>();

		/// <summary>
		/// Lookup item.
		/// </summary>
		/// <param name="key">Key returned by Add()</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected T Get(int key)
		{
			return _strs[key];
		}

		/// <summary>
		/// Add the item to the table.
		/// </summary>
		/// <param name="str">Item to add</param>
		/// <param name="hashCode">Item hash code provided by the using class</param>
		/// <returns>Item index to use with Get()</returns>
		protected int Add(T str, int hashCode)
		{
			if (_hashDict.TryGetValue(hashCode, out int idx))
			{
				if (str.Equals(_strs[idx]))
				{
					return idx;
				}

				_strs.Add(str);
				return _strs.Count - 1;
			}

			idx = _strs.Count;

			_strs.Add(str);
			_hashDict.TryAdd(hashCode, idx);

			return idx;
		}

		/// <summary>
		/// Returns the index for the hash, or -1.
		/// </summary>
		protected int IndexOf(int hashCode)
		{
			if (_hashDict.TryGetValue(hashCode, out int idx))
			{
				return idx;
			}

			return -1;
		}
	}
}
