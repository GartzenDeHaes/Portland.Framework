using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.AI.NLP
{
	public class Word : IEquatable<Word>, IEquatable<String>, IEquatable<StringBuilder>
	{
		public string Lexum;
		public int Hash;

		public Word WordNext;
		public Word WordPrev;

		public Word(string str)
		{
			Lexum = str;
			Hash = StringHelper.HashMurmur32(str);
		}

		public override bool Equals(object obj)
		{
			if (obj is Word w)
			{
				return Hash == w.Hash;
			}
			if (obj is String s)
			{
				return Lexum == s;
			}
			return Hash == obj.GetHashCode();
		}

		public override int GetHashCode()
		{
			return Hash;
		}

		public override string ToString()
		{
			return Lexum;
		}

		public bool Equals(Word other)
		{
			return Hash == other.Hash;
		}

		public bool Equals(string other)
		{
			return Lexum.Equals(other);
		}

		public bool Equals(StringBuilder other)
		{
			return other.IsEqualTo(Lexum);
		}

		///// <summary>==</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator ==(Word s1, Word s2)
		//{
		//	return null != s1 && null != s2 && s1.Hash == s2.Hash;
		//}

		///// <summary>!=</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator !=(Word s1, Word s2)
		//{
		//	return null != s1 && null != s2 && s1.Hash != s2.Hash;
		//}

		///// <summary>==</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator ==(Word s1, String s2)
		//{
		//	return s1.Lexum.Equals(s2);
		//}

		///// <summary>!=</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator !=(Word s1, String s2)
		//{
		//	return !s1.Lexum.Equals(s2);
		//}

		///// <summary>==</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator ==(Word s1, char ch)
		//{
		//	return s1.Lexum.Length == 1 && s1.Lexum[0] == ch;
		//}

		///// <summary>==</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator !=(Word s1, char ch)
		//{
		//	return s1.Lexum.Length != 1 || s1.Lexum[0] != ch;
		//}

		///// <summary>==</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator ==(Word s1, StringBuilder buf)
		//{
		//	return buf.IsEqualTo(s1.Lexum);
		//}

		///// <summary>==</summary>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool operator !=(Word s1, StringBuilder buf)
		//{
		//	return !buf.IsEqualTo(s1.Lexum);
		//}

		/// <summary>Implicit Word to string cast</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator string(Word s) => s.Lexum;
	}
}
