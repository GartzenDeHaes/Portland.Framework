using System.Text;

using Portland.Text;

namespace Portland.Collections
{
	public sealed class TextTable
	{
		public struct StringHolder
		{
			public string Lexum;
			public int MurmurHashCode;
		}

		Vector<StringHolder> _strings = new Vector<StringHolder>(64);

		public TextTable()
		{
			// Index zero is empty string
			_strings.Add(new StringHolder { Lexum = string.Empty, MurmurHashCode = StringHelper.HashMurmur32(string.Empty) });
		}

		public bool TryGet(string lexum, out TextTableToken ret)
		{
			ret = new TextTableToken();

			for (int x = 0; x < _strings.Count; x++)
			{
				if (_strings[x].Lexum.Equals(lexum))
				{
					ret.Index = x;
					ret.HashCode = _strings[x].MurmurHashCode;
					//ret.StartsWith = AsciiId4.ConstructStartsWith(lexum);
					return true;
				}
			}

			return false;
		}

		public TextTableToken Get(string lexum)
		{
			if (TryGet(lexum, out TextTableToken ret))
			{
				return ret;
			}

			return Add(lexum);
		}

		public TextTableToken Get(int index)
		{
			return new TextTableToken { Index = index, HashCode = _strings[index].MurmurHashCode/*, StartsWith = AsciiId4.ConstructStartsWith(_strings[index].Lexum)*/ };
		}

		public string GetString(in TextTableToken token)
		{
			return _strings[token.Index].Lexum;
		}

		public bool TryGet(StringBuilder buf, out TextTableToken ret)
		{
			ret = new TextTableToken();

			for (int x = 0; x < _strings.Count; x++)
			{
				if (StringHelper.AreEqual(buf, _strings[x].Lexum))
				{
					ret.Index = x;
					ret.HashCode = _strings[x].MurmurHashCode;
					//ret.StartsWith = AsciiId4.ConstructStartsWith(buf);
					return true;
				}
			}

			return false;
		}

		public TextTableToken Get(StringBuilder buf)
		{
			if (TryGet(buf, out TextTableToken ret))
			{
				return ret;
			}

			return Add(buf.ToString());
		}

		TextTableToken Add(string lexum)
		{
			var holder = new StringHolder { Lexum = lexum, MurmurHashCode = StringHelper.HashMurmur32(lexum) };
			return new TextTableToken()
			{
				Index = _strings.Add(holder),
				HashCode = holder.MurmurHashCode,
				//StartsWith = AsciiId4.ConstructStartsWith(lexum)
			};
		}

		public bool Contains(string lexum)
		{
			for (int x = 0; x < _strings.Count; x++)
			{
				if (_strings[x].Lexum.Equals(lexum))
				{
					return true;
				}
			}

			return false;
		}

		public bool Contains(StringBuilder buf)
		{
			for (int x = 0; x < _strings.Count; x++)
			{
				if (StringHelper.AreEqual(buf, _strings[x].Lexum))
				{
					return true;
				}
			}

			return false;
		}
	}
}
