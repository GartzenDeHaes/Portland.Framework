//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Portland.Collections;

//namespace Portland.Interp
//{
//	public class Program
//	{
//		public Instruction[] Code;
//		public string[] ConstStrings;
//		public int StartPC;
//	}

//	public struct PreparedProgram
//	{
//		public Program Program;
//		public StringTableToken[] ConstStrings;

//		public PreparedProgram(Program prog, StringTable strings)
//		{
//			Program = prog;
//			ConstStrings = new StringTableToken[prog.ConstStrings.Length];

//			for (int i = 0; i < prog.ConstStrings.Length; i++)
//			{
//				ConstStrings[i] = strings.Get(prog.ConstStrings[i]);
//			}
//		}
//	}
//}
