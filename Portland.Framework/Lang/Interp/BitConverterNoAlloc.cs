using System;
using System.Diagnostics;
using System.Text;

using Portland.Mathmatics;

namespace Portland.Interp
{
	/// <summary>
	/// Bad
	/// </summary>
	public static class BitConverterNoAlloc
	{
#if BIGENDIAN
        public static readonly bool IsLittleEndian /* = false */;
#else
		public static readonly bool IsLittleEndian = true;
#endif

		// Converts a byte into an array of bytes with length one.
		public static void GetBytes(bool value, byte[] data, ref int pos)
		{
			if (data == null)
			{
				throw new ArgumentNullException();
			}
			if (data.Length >= pos + 1)
			{
				throw new ArgumentException("data too short");
			}

			data[pos++] = (value ? (byte)1 : (byte)0);
		}

		// Converts a char into an array of bytes with length two.
		public static void GetBytes(char value, byte[] data, ref int pos)
		{
			GetBytes((short)value, data, ref pos);
		}

		// Converts a short into an array of bytes with length
		// two.
		public static void GetBytes(short value, byte[] data, ref int pos)
		{
			if (data.Length >= pos + 2)
			{
				throw new ArgumentException("data too short");
			}

			IntFloat conv = new IntFloat { IntValue = value };
			data[pos++] = conv.b0;
			data[pos++] = conv.b1;
		}

		// Converts an int into an array of bytes with length 
		// four.
		public static void GetBytes(int value, byte[] bytes, ref int pos)
		{
			if (bytes.Length >= pos + 4)
			{
				throw new ArgumentException("data too short");
			}

			IntFloat conv = new IntFloat { IntValue = value };
			bytes[pos++] = conv.b0;
			bytes[pos++] = conv.b1;
			bytes[pos++] = conv.b2;
			bytes[pos++] = conv.b3;
		}

		// Converts a long into an array of bytes with length 
		// eight.
		public static void GetBytes(long value, byte[] bytes, ref int pos)
		{
			if (bytes.Length >= pos + 8)
			{
				throw new ArgumentException("data too short");
			}

			LongDouble conv = new LongDouble { LongValue = value };
			bytes[pos++] = conv.b0;
			bytes[pos++] = conv.b1;
			bytes[pos++] = conv.b2;
			bytes[pos++] = conv.b3;
			bytes[pos++] = conv.b4;
			bytes[pos++] = conv.b5;
			bytes[pos++] = conv.b6;
			bytes[pos++] = conv.b7;
		}

		public static void GetBytes(string value, byte[] data, ref int pos)
		{
			if (value.Length > 255)
			{
				throw new ArgumentException("string value too long for buffer");
			}

			GetBytes((short)value.Length, data, ref pos);

			for (int x = 0; x < value.Length; x++)
			{
				GetBytes(value[x], data, ref pos);
			}
		}

		public static string ToString(byte[] data, ref int pos, StringBuilder buf)
		{
			short len = BitConverter.ToInt16(data, pos);
			pos += 2;

			buf.Length = 0;

			char ch;

			for (int x = 0; x < len; x++)
			{
				ch = BitConverter.ToChar(data, pos);
				pos += 2;
				buf.Append(ch);
			}

			return buf.ToString();
		}

		// Converts an ushort into an array of bytes with
		// length two.
		public static void GetBytes(ushort value, byte[] data, ref int pos)
		{
			GetBytes((short)value, data, ref pos);
		}

		// Converts an uint into an array of bytes with
		// length four.
		public static void GetBytes(uint value, byte[] data, ref int pos)
		{
			GetBytes((int)value, data, ref pos);
		}

		// Converts an unsigned long into an array of bytes with
		// length eight.
		public static void GetBytes(ulong value, byte[] data, ref int pos)
		{
			GetBytes((long)value, data, ref pos);
		}

		// Converts a float into an array of bytes with length 
		// four.
		public static void GetBytes(float value, byte[] data, ref int pos)
		{
			IntFloat conv = new IntFloat { FloatValue = value };
			GetBytes(conv.IntValue, data, ref pos);
		}

		// Converts a double into an array of bytes with length 
		// eight.
		public static void GetBytes(double value, byte[] data, ref int pos)
		{
			LongDouble conv = new LongDouble { DoubleValue = value };
			GetBytes(conv.LongValue, data, ref pos);
		}
	}
}
