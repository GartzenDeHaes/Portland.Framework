using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Portland.Text;

namespace Portland.Text
{
	/// <summary>
	/// A string table keyed with StringHelper.HashSimple32().
	/// </summary>
	public sealed class StringTable
	{
		private Dictionary<int, int> _hashDict = new Dictionary<int, int>();
		private List<string> _strs = new List<string>();

		/// <summary>
		/// Lookup item.
		/// </summary>
		/// <param name="key">Key returned by Add()</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		string Get(int key)
		{
			return _strs[key];
		}

		/// <summary>
		/// Add the item to the table.
		/// </summary>
		/// <param name="str">Item to add</param>
		/// <param name="hashCode">Item hash code provided by the using class</param>
		/// <returns>Item index to use with Get()</returns>
		int Add(string str, int hashCode)
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
		int IndexOf(int hashCode)
		{
			if (_hashDict.TryGetValue(hashCode, out int idx))
			{
				return idx;
			}

			return -1;
		}

		/// <summary>
		/// Get or create the index for the string
		/// </summary>
		public StringTabRef Get(string str)
		{
			int hash = StringHelper.HashMurmur32(str);
			int key = IndexOf(hash);

			if (key < 0)
			{
				key = Add(str, hash);
			}

			return new StringTabRef(key, str.Length);
		}

		/// <summary>
		/// Get or create the index for the string.  No alloc if the string is already in the table.
		/// </summary>
		public StringTabRef Get(StringBuilder buf)
		{
			int hash = StringHelper.HashSimple32(buf);
			int key = IndexOf(hash);

			if (key < 0)
			{
				key = Add(buf.ToString(), hash);
			}

			return new StringTabRef(key, buf.Length);
		}

		///// <summary>
		///// Get or create the index for the string
		///// </summary>
		//public StringTabRef Get(String3 str3)
		//{
		//	string str = str3;
		//	int hash = StringHelper.HashSimple32(str);
		//	int key = IndexOf(hash);

		//	if (key < 0)
		//	{
		//		key = Add(str, hash);
		//	}

		//	return new StringTabRef(key, str.Length);
		//}

		/// <summary>
		/// Get the string for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetValue(int key)
		{
			return Get(key);
		}

		/// <summary>
		/// Equality test using the key returned by Get()
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AreEqual(int key, string value)
		{
			return key == IndexOf(StringHelper.HashSimple32(value));
		}

		/// <summary>
		/// Equality test using the key returned by Get()
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AreEqual(int key, String8 value)
		{
			return key == IndexOf(StringHelper.HashSimple32(value));
		}

		/// <summary>
		/// Equality test using the key returned by Get()
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AreEqual(int key, StringBuilder value)
		{
			return key == IndexOf(StringHelper.HashSimple32(value));
		}

		/// <summary>
		/// Equality test using the key returned by Get()
		/// </summary>
		public bool AreEqual(int key, object obj)
		{
			if (obj is String str)
			{
				return AreEqual(key, str);
			}
			if (obj is String8 str8)
			{
				return AreEqual(key, str8);
			}
			if (obj is StringBuilder sb)
			{
				return AreEqual(key, sb);
			}

			return false;
		}
	}
}

