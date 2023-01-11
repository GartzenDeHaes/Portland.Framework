using System;

using Portland.Mathmatics;

namespace Portland.AI.Barks
{
	public class RandMax : IRandom
	{
		public int Next()
		{
			return Int32.MaxValue;
		}

		public int Next(int maxExclusive)
		{
			return maxExclusive - 1;
		}

		public bool NextBool()
		{
			return true;
		}

		public byte NextByte()
		{
			return 255;
		}

		public byte[] NextBytes(int count)
		{
			throw new NotImplementedException();
		}

		public double NextDouble()
		{
			return Double.MaxValue;
		}

		public double NextDouble(double maxInclusive)
		{
			return maxInclusive;
		}

		public double NextDouble(double minInclusive, double maxInclusive)
		{
			return maxInclusive;
		}

		public float NextFloat()
		{
			return Single.MaxValue;
		}

		public float NextFloat(float maxInclusive)
		{
			return maxInclusive;
		}

		public uint NextUInt()
		{
			return UInt32.MaxValue;
		}

		public uint NextUInt(uint maxExclusive)
		{
			return maxExclusive - 1;
		}

		public float Range(float minInclusive, float maxInclusive)
		{
			return maxInclusive;
		}

		public int Range(int minInclusive, int maxExclusive)
		{
			return maxExclusive - 1;
		}

		public uint Range(uint minInclusive, uint maxExclusive)
		{
			return maxExclusive - 1;
		}
	}

	public class RandMin : IRandom
	{
		public int Next()
		{
			return 0;
		}

		public int Next(int maxExclusive)
		{
			return 0;
		}

		public bool NextBool()
		{
			return false;
		}

		public byte NextByte()
		{
			return 0;
		}

		public byte[] NextBytes(int count)
		{
			throw new NotImplementedException();
		}

		public double NextDouble()
		{
			return 0;
		}

		public double NextDouble(double maxInclusive)
		{
			return 0;
		}

		public double NextDouble(double minInclusive, double maxInclusive)
		{
			return minInclusive;
		}

		public float NextFloat()
		{
			return 0;
		}

		public float NextFloat(float maxInclusive)
		{
			return 0;
		}

		public uint NextUInt()
		{
			return 0;
		}

		public uint NextUInt(uint maxExclusive)
		{
			return 0;
		}

		public float Range(float minInclusive, float maxInclusive)
		{
			return minInclusive;
		}

		public int Range(int minInclusive, int maxExclusive)
		{
			return minInclusive;
		}

		public uint Range(uint minInclusive, uint maxExclusive)
		{
			return minInclusive;
		}
	}
}
