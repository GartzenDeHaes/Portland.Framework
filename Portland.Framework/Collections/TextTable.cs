using System;
using System.Collections.Generic;
using System.Text;

using Portland.Mathmatics.Geometry;
using Portland.Text;

namespace Portland.Collections
{
	public struct StringTableToken
	{
		public short Index;
		public short Length;

		public bool Equals(in StringTableToken stok)
		{
			return Index == stok.Index;
		}

		public override bool Equals(object obj)
		{
			if (obj is StringTableToken stok)
			{
				return Index == stok.Index;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Length << 16 | (int)Index;
		}

		public override string ToString()
		{
			//return $"{StartsWith}({Index})";
			return $"({Index})";
		}

		public static bool operator ==(in StringTableToken a, in StringTableToken b)
		{
			return a.Index == b.Index;// && a.HashCode == b.HashCode;
		}

		public static bool operator !=(in StringTableToken a, in StringTableToken b)
		{
			return a.Index != b.Index;// && a.HashCode != b.HashCode;
		}
	}

	public sealed class StringTable
	{
		Dictionary<int, int> _hashIndex = new Dictionary<int, int>();
		Vector<string> _strings = new Vector<string>(64);

		public StringTable()
		{
			// Index zero is empty string
			_hashIndex.Add(StringHelper.HashMurmur32(String.Empty), 0);
			_strings.Add(String.Empty);
		}

		public bool TryGet(string lexum, out StringTableToken ret)
		{
			ret = new StringTableToken();
			int index;

			if (TryHash(lexum, out index) || ScanForString(lexum, out index))
			{
				ret.Index = (short)index;
				ret.Length = (short)lexum.Length;
				return true;
			}

			return false;
		}

		bool TryHash(string lexum, out int index)
		{
			int hash = StringHelper.HashMurmur32(lexum);
			if (_hashIndex.TryGetValue(hash, out index))
			{
				return _strings[index].Equals(lexum);
			}

			return false;
		}

		bool ScanForString(string lexum, out int index)
		{
			for (index = 0; index < _strings.Count; index++)
			{
				if (_strings[index].Equals(lexum))
				{
					return true;
				}
			}

			return false;
		}

		public StringTableToken Get(string lexum)
		{
			if (TryGet(lexum, out StringTableToken ret))
			{
				return ret;
			}

			return Add(lexum);
		}

		public StringTableToken Get(short index)
		{
			return new StringTableToken { Index = index, Length = (short)_strings[index].Length };
		}

		public string GetString(in StringTableToken token)
		{
			return _strings[token.Index];
		}

		bool TryHash(StringBuilder lexum, out int index)
		{
			int hash = StringHelper.HashMurmur32(lexum);
			if (_hashIndex.TryGetValue(hash, out index))
			{
				return lexum.Equals(_strings[index]);
			}

			return false;
		}

		bool ScanForString(StringBuilder lexum, out int index)
		{
			for (index = 0; index < _strings.Count; index++)
			{
				if (_strings[index].Equals(lexum))
				{
					return true;
				}
			}

			return false;
		}

		public bool TryGet(StringBuilder buf, out StringTableToken ret)
		{
			ret = new StringTableToken();
			int index;

			if (TryHash(buf, out index) || ScanForString(buf, out index))
			{
				ret.Index = (short)index;
				ret.Length = (short)buf.Length;
				return true;
			}

			return false;
		}

		public StringTableToken Get(StringBuilder buf)
		{
			if (TryGet(buf, out StringTableToken ret))
			{
				return ret;
			}

			return Add(buf.ToString());
		}

		public string GetString(StringBuilder buf)
		{
			if (TryGet(buf, out StringTableToken ret))
			{
				return GetString(ret);
			}

			return GetString(Add(buf.ToString()));
		}

		StringTableToken Add(string lexum)
		{
			var tok = new StringTableToken()
			{
				Index = (short)_strings.Add(lexum),
				Length = (short)lexum.Length,
			};

			_hashIndex.TryAdd(StringHelper.HashMurmur32(lexum), tok.Index);

			return tok;
		}

		public bool Contains(string lexum)
		{
			return TryHash(lexum, out int _) || ScanForString(lexum, out int _);
		}

		public bool Contains(StringBuilder buf)
		{
			return TryHash(buf, out int _) || ScanForString(buf, out int _);
		}
	}
}
