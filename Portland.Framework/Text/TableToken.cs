using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.AI.Barks
{
	public struct TextTableToken
	{
		public int Index;
		public int HashCode;
		//public AsciiId4 StartsWith;

		public bool Equals(in TextTableToken stok)
		{
			return Index == stok.Index;
		}

		public override bool Equals(object obj)
		{
			if (obj is TextTableToken stok)
			{
				return Index == stok.Index;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode;
		}

		public override string ToString()
		{
			//return $"{StartsWith}({Index})";
			return $"({Index})";
		}

		public static bool operator ==(in TextTableToken a, in TextTableToken b)
		{
			return a.Index == b.Index;// && a.HashCode == b.HashCode;
		}

		public static bool operator !=(in TextTableToken a, in TextTableToken b)
		{
			return a.Index != b.Index;// && a.HashCode != b.HashCode;
		}
	}

}
