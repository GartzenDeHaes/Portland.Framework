//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//using Portland.Collections;

//namespace Portland.Interp
//{
//	[StructLayout(LayoutKind.Explicit)]
//	public struct Instruction
//	{
//		//[FieldOffset(0)]
//		//public BitSet32 Bits;

//		[FieldOffset(0)]
//		public OpCode Op;

//		[FieldOffset(1)]
//		public Mode Mode;

//		[FieldOffset(2)]
//		public byte RegNum;

//		[FieldOffset(3)]
//		public sbyte Data;

//		[FieldOffset(0)]
//		public int Int;

//		[FieldOffset(0)]
//		public int Float;

//		//public OpCode Op
//		//{
//		//	get { return (OpCode)Bits.BitsAt(0, 6); }
//		//	set { Bits.SetBitsAt(0, 6, (uint)value); }
//		//}

//		//public Mode AddressMode
//		//{
//		//	get { return (Mode)Bits.BitsAt(6, 2); }
//		//	set { Bits.SetBitsAt(6, 2, (uint)value); }
//		//}
//	}
//}
