using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Text;

namespace Portland.AI
{
	public struct WorldStateFlags
	{
		public BitSet32 Bits;

		public bool Daylight { get { return Bits.IsSet(0); } set { Bits.SetTest(0, value); } }
		public bool IsCharacterSpeaking { get { return Bits.IsSet(1); } set { Bits.SetTest(1, value); } }

		public override string ToString()
		{
			return Bits.ToString();
		}

		public bool Equals(WorldStateFlags fl)
		{
			return fl.Bits.RawBits == Bits.RawBits;
		}

		public override bool Equals(object obj)
		{
			if (obj is WorldStateFlags fl)
			{
				return fl.Bits.RawBits == Bits.RawBits;
			}

			return Bits.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Bits.RawBits.GetHashCode();
		}

		public void Reset()
		{
			Bits.RawBits = 0;
		}

		public void SetByName(string bitName, bool state)
		{
			int bit = BitNameToNum(bitName);
			if (bit < 0)
			{
				throw new ArgumentOutOfRangeException(bitName + " is not valid for WorldStateFlags");
			}

			Bits.SetTest(bit, state);
		}

		public static int BitNameToNum(string bitName)
		{
			if (!StringHelper.IsUpper(bitName))
			{
				bitName = bitName.ToUpper();
			}

			int ret = -1;
			switch (bitName)
			{
				case "DAYLIGHT": ret = 0; break;
				case "ISCHARACTERSPEAKING": ret = 1; break;
			}

			return ret;
		}
	}
}
