using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Portland.Text;

namespace Portland.Collections
{
	/// <summary>
	/// A string table keyed with StringHelper.HashSimple32().
	/// </summary>
	public sealed class StringTable : ValueTable<string>
	{
		/// <summary>
		/// Get or create the index for the string
		/// </summary>
		public StringInStrTab Get(string str)
		{
			int hash = StringHelper.HashSimple32(str);
			int key = IndexOf(hash);

			if (key < 0)
			{
				key = Add(str, hash);
			}

			return new StringInStrTab(key, str.Length);
		}

		/// <summary>
		/// Get or create the index for the string.  No alloc if the string is already in the table.
		/// </summary>
		public StringInStrTab Get(StringBuilder buf)
		{
			int hash = StringHelper.HashSimple32(buf);
			int key = IndexOf(hash);

			if (key < 0)
			{
				key = Add(buf.ToString(), hash);
			}

			return new StringInStrTab(key, buf.Length);
		}

		/// <summary>
		/// Get or create the index for the string
		/// </summary>
		public StringInStrTab Get(String3 str3)
		{
			string str = str3;
			int hash = StringHelper.HashSimple32(str);
			int key = IndexOf(hash);

			if (key < 0)
			{
				key = Add(str, hash);
			}

			return new StringInStrTab(key, str.Length);
		}

		/// <summary>
		/// Get the string for the specified index.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetValue(int key)
		{
			return base.Get(key);
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
