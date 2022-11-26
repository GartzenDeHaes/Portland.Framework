using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Portland.Text
{
	/// <summary>
	/// A 7 byte character string.  This allows an extra byte for flags in Variant.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct String3 : IEquatable<String>
	{
		public const int MAX_LEN = 3;
		public static String3 Empty;

		/// <summary></summary>
		[FieldOffset(0)]
		public char c0;
		/// <summary></summary>
		[FieldOffset(2)]
		public char c1;
		/// <summary></summary>
		[FieldOffset(4)]
		public char c2;

		/// <summary></summary>
		[FieldOffset(0)]
		public uint Bits1;
		/// <summary>7 bytes total, so some overlap</summary>
		[FieldOffset(4)]
		public ushort Bits2;

		/// <summary>String length</summary>
		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (c0 == 0 ? 0 : 1) +
					(c1 == 0 ? 0 : 1) +
					(c2 == 0 ? 0 : 1);
			}
		}

		/// <summary></summary>
		public char this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return (char)c0;
					case 1: return (char)c1;
					case 2: return (char)c2;
					default:
						throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
					case 0: c0 = value; break;
					case 1: c1 = value; break;
					case 2: c2 = value; break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Update string in-place
		/// </summary>
		/// <exception cref="ArgumentException">Throws exception if that argument is longer than 8 chars.</exception>
		private void Parse(string s, bool checkLen = false)
		{
			int len = s.Length;

			if (checkLen && s.Length > MAX_LEN)
			{
				throw new ArgumentException("String3 string can only be up to 3 characters long");
			}

			c0 = len > 0 ? s[0] : '\0';
			c1 = len > 1 ? s[1] : '\0';
			c2 = len > 2 ? s[2] : '\0';
		}

		/// <summary></summary>
		public bool StartsWith(string str)
		{
			int len = str.Length;

			return (len <= Length) &&
				(char)c0 == (len > 0 ? str[0] : (char)c0) &&
				(char)c1 == (len > 1 ? str[1] : (char)c1) &&
				(char)c2 == (len > 2 ? str[2] : (char)c2)
				;
		}

		/// <summary></summary>
		public bool EndsWith(string str)
		{
			int slen = str.Length;
			int len = Length;

			if (slen > len)
			{
				return false;
			}

			for (int x = 0; x < slen; x++)
			{
				if (this[len - 1 - x] != str[slen - 1 - x])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary></summary>
		public int IndexOf(char ch)
		{
			if (ch == c0) return 0;
			if (ch == c1) return 1;
			if (ch == c2) return 2;
			return -1;
		}

		/// <summary></summary>
		public int IndexOf(string str, int start = 0)
		{
			int slen = str.Length;

			if (slen == 0)
			{
				return -1;
			}

			int len = Length;
			for (int x = start; x < len; x++)
			{
				if (this[x] == str[0])
				{
					int y = 0;
					for (y = 1; y < slen; y++)
					{
						int x2 = x + y;
						if (x2 >= len)
						{
							break;
						}
						if (this[x2] != str[y])
						{
							break;
						}
					}

					if (y == slen)
					{
						return x;
					}
				}
			}

			return -1;
		}

		/// <summary></summary>
		public String3 SubString(int start, int len = 32000)
		{
			String3 ret = String3.Empty;
			int length = Length;
			int end = start + len;
			end = end > length ? length : end;
			int pos = 0;

			for (int x = start; x < end; x++)
			{
				ret[pos++] = this[x];
			}

			return ret;
		}

		/// <summary></summary>
		public String3 TrimStart()
		{
			int pos = 0;
			int len = Length;
			String3 ret = String3.Empty;

			for (int x = 0; x < len; x++)
			{
				char ch = this[x];
				if (pos == 0 && Char.IsWhiteSpace(ch))
				{
					continue;
				}
				ret[pos++] = ch;
			}

			return ret;
		}

		/// <summary></summary>
		public String3 TrimEnd()
		{
			int len = Length;
			String3 ret = this;

			for (int x = len - 1; x >= 0; x--)
			{
				if (!Char.IsWhiteSpace(this[x]))
				{
					return ret;
				}

				ret[x] = '\0';
			}

			return ret;
		}

		/// <summary></summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String3 Trim()
		{
			return TrimEnd().TrimStart();
		}

		/// <summary>
		/// Simple hash function, may not work well with non-ASCII character sets.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return (c0 | c2 << 16)
				^ (c1 << 8);
		}

		/// <summary>
		/// Equals
		/// </summary>
		public bool Equals(string str)
		{
			int len = Length;

			return (len == str.Length) &&
				(char)c0 == (len > 0 ? str[0] : '\0') &&
				(char)c1 == (len > 1 ? str[1] : '\0') &&
				(char)c2 == (len > 2 ? str[2] : '\0')
				;
		}

		/// <summary>
		/// Equals
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(char c)
		{
			return c0 == c &&
				c1 == '\0';
		}

		/// <summary>
		/// Equals
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(String3 s)
		{
			return Bits1 == s.Bits1 && Bits2 == s.Bits2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(String8 s)
		{
			return s.Length <= MAX_LEN &&
				c0 == s.c0 &&
				c1 == s.c1 &&
				c2 == s.c2;
		}

		/// <summary>
		/// Equals
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is String3 s)
			{
				return Equals(s);
			}
			if (obj is String8 s8)
			{
				return Equals(s8);
			}
			if (obj is String str)
			{
				return Equals(str);
			}
			if (obj is char ch)
			{
				return Equals(ch);
			}

			return false;
		}

		/// <summary>==</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(String3 s1, String3 s2)
		{
			return s1.Equals(s2);
		}

		/// <summary>!=</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(String3 s1, String3 s2)
		{
			return !s1.Equals(s2);
		}

		/// <summary>==</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(String3 s1, String s2)
		{
			return s1.Equals(s2);
		}

		/// <summary>!=</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(String3 s1, String s2)
		{
			return !s1.Equals(s2);
		}

		/// <summary>==</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(String3 s1, String8 s2)
		{
			return s1.Equals(s2);
		}

		/// <summary>!=</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(String3 s1, String8 s2)
		{
			return !s1.Equals(s2);
		}

		/// <summary>==</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(String3 s1, char ch)
		{
			return s1.Equals(ch);
		}

		/// <summary>==</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(String3 s1, char ch)
		{
			return !s1.Equals(ch);
		}

		/// <summary>
		/// To a regular string
		/// </summary>
		public override string ToString()
		{
			StringBuilder buf = new StringBuilder(4);
			_ = c0 == '\0' ? buf : buf.Append((char)c0);
			_ = c1 == '\0' ? buf : buf.Append((char)c1);
			_ = c2 == '\0' ? buf : buf.Append((char)c2);
			return buf.ToString();
		}

		/// <summary>
		/// Static constructor
		/// </summary>
		public static String3 From(string s)
		{
			String3 s3 = String3.Empty;
			s3.Parse(s);
			return s3;
		}

		/// <summary>
		/// Static constructor; take the first 8 chars
		/// </summary>
		public static String3 FromTruncate(string s)
		{
			String3 s7 = String3.Empty;
			s7.Parse(s, false);
			return s7;
		}

		/// <summary>
		/// Static constructor
		/// </summary>
		public static String3 From(String8 s)
		{
			int len = s.Length;
			if (len > MAX_LEN)
			{
				throw new IndexOutOfRangeException("String8 was 8 chars, cannot covert to String7");
			}
			String3 s7 = new String3();
			s7.c0 = len > 0 ? (s[0]) : '\0';
			s7.c1 = len > 1 ? (s[1]) : '\0';
			s7.c2 = len > 2 ? (s[2]) : '\0';
			return s7;
		}

		/// <summary>
		/// Static constructor
		/// </summary>
		public static String3 From(StringBuilder sb)
		{
			String3 s = String3.Empty;
			int len = sb.Length;

			s.c0 = len > 0 ? (sb[0]) : '\0';
			s.c1 = len > 1 ? (sb[1]) : '\0';
			s.c2 = len > 2 ? (sb[2]) : '\0';
			return s;
		}

		/// <summary>Implicit String8 to string cast</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator string(String3 s) => s.ToString();
		/// <summary>Implicit string to String8 cast</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator String3(string x) => String3.From(x);

		/// <summary></summary>
		public static bool IsNullEmptyOrWhiteSpace(String3 s)
		{
			return s.Length == 0 ||
			(
				(s.c0 == '\0' || Char.IsWhiteSpace((s.c0))) &&
				(s.c1 == '\0' || Char.IsWhiteSpace((s.c1))) &&
				(s.c2 == '\0' || Char.IsWhiteSpace((s.c2)))
			);
		}

		/// <summary></summary>
		public static bool IsWhiteSpace(String3 s)
		{
			return
			(
				(Char.IsWhiteSpace((s.c0))) &&
				(s.c1 == '\0' || Char.IsWhiteSpace((s.c1))) &&
				(s.c2 == '\0' || Char.IsWhiteSpace((s.c2)))
			);
		}
	}
}
