//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Portland.Text
//{
//	public struct StringDictRef
//	{
//		short _idx;
//		short _len;

//		public int Length
//		{
//			[MethodImpl(MethodImplOptions.AggressiveInlining)]
//			get { return _len; }
//		}

//		public char this[int idx]
//		{
//			[MethodImpl(MethodImplOptions.AggressiveInlining)]
//			get { return Variant8.StrTab.GetValue(_idx)[idx]; }
//		}

//		//public StringInStrTab()
//		//{
//		//	// Note, empty string is index zero set by Variant
//		//}

//		public StringDictRef(int idx, int len)
//		{
//			_idx = (short)idx;
//			_len = (short)len;
//		}

//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public int IndexOf(char ch)
//		{
//			return Variant8.StrTab.GetValue(_idx).IndexOf(ch);
//		}

//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public int IndexOf(string s)
//		{
//			return Variant8.StrTab.GetValue(_idx).IndexOf(s);
//		}

//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public bool StartsWith(string pre)
//		{
//			return Variant8.StrTab.GetValue(_idx).StartsWith(pre);
//		}

//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public string SubString(int start)
//		{
//			return Variant8.StrTab.GetValue(_idx).Substring(start);
//		}

//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public string SubString(int start, int len)
//		{
//			return Variant8.StrTab.GetValue(_idx).Substring(start, len);
//		}

//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public string ToUpper()
//		{
//			return Variant8.StrTab.GetValue(_idx).ToUpper();
//		}

//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public string ToLower()
//		{
//			return Variant8.StrTab.GetValue(_idx).ToLower();
//		}

//		public override bool Equals(object obj)
//		{
//			if (obj is StringDictRef st)
//			{
//				return _idx == st._idx;
//			}
//			if (obj is String s)
//			{
//				return ToString().Equals(s);
//			}
//			return obj.Equals(ToString());
//		}

//		public override string ToString()
//		{
//			return Variant8.StrTab.GetValue(_idx);
//		}

//		public override int GetHashCode()
//		{
//			return Variant8.StrTab.GetValue(_idx).GetHashCode();
//		}

//		/// <summary>==</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static bool operator ==(StringDictRef s1, String s2)
//		{
//			return s1.ToString().Equals(s2);
//		}

//		/// <summary>!=</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static bool operator !=(StringDictRef s1, String s2)
//		{
//			return !s1.ToString().Equals(s2);
//		}

//		/// <summary>==</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static bool operator ==(StringDictRef s1, String8 s2)
//		{
//			return s2.Equals(s1.ToString());
//		}

//		/// <summary>!=</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static bool operator !=(StringDictRef s1, String8 s2)
//		{
//			return !s2.Equals(s1.ToString());
//		}

//		/// <summary>Implicit String8 to string cast</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static implicit operator string(StringDictRef s) => s.ToString();
//		/// <summary>Implicit string to String8 cast</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public static implicit operator StringDictRef(string x) => Variant8.StrTab.Get(x);
//	}
//}
