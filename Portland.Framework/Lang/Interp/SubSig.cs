using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Interp
{
	public struct SubSig : IEquatable<SubSig>
	{
		public String Name;
		public int ArgCount;

		public override int GetHashCode()
		{
			return Name.GetHashCode() + ArgCount << 24;
		}

		public override bool Equals(object obj)
		{
			if (obj is SubSig sig)
			{
				return Name == sig.Name && ArgCount == sig.ArgCount;
			}
			return false;
		}

		public override string ToString()
		{
			return Name.ToString() + "_" + ArgCount;
		}

		public bool Equals(SubSig other)
		{
			return ArgCount == other.ArgCount && Name == other.Name;
		}

		public bool Equals(in SubSig other)
		{
			return ArgCount == other.ArgCount && Name == other.Name;
		}
	}
}
