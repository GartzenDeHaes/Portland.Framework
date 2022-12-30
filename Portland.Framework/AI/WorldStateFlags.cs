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
		public bool IsCharacter01Alive { get { return Bits.IsSet(2); } set { Bits.SetTest(2, value); } }
		public bool IsCharacter02Alive { get { return Bits.IsSet(3); } set { Bits.SetTest(3, value); } }
		public bool IsCharacter03Alive { get { return Bits.IsSet(4); } set { Bits.SetTest(4, value); } }
		public bool IsCharacter04Alive { get { return Bits.IsSet(5); } set { Bits.SetTest(5, value); } }

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
			return Bits.GetHashCode();
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
				case "IS_CHARACTER_SPEAKING": ret = 1; break;
				case "CHARACTER_01_ALIVE": ret = 2; break;
				case "CHARACTER_02_ALIVE": ret = 3; break;
				case "CHARACTER_03_ALIVE": ret = 4; break;
				case "CHARACTER_04_ALIVE": ret = 5; break;
			}

			return ret;
		}
	}
}
