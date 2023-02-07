//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Portland.Interp
//{
//	public enum Mode : byte
//	{
//		IMMI = 0,	// One instruction, source data is in Data (one sbyte)
//		NXTI = 1,   // Two instructions, next instruction is an int 
//		NXTF = 2,   // Two instructions, next instruction is a float 
//		NXTS = 3,	// Two instructions, next instruction is a int index into the string contant table
//	}

//	public enum OpCode : byte
//	{
//		NOP = 0,
//		/// <summary>
//		/// Copy data between registers
//		/// MODE
//		///	IMMI: DATA holds right hand side register number 
//		/// RESULT
//		///	INSTR.REG = REG number in DATA
//		/// </summary>
//		MOV = 1,
//		/// <summary>
//		/// Add two registers and put the result in R0
//		/// MODE
//		///	IMMI: DATA holds right hand side register number 
//		/// RESULT
//		///	R0 = INSTR.REG + REG number in DATA
//		/// </summary>
//		ADD = 2,
//		/// <summary>
//		/// Subtract two registers and put the result in R0
//		/// MODE
//		///	IMMI: DATA holds right hand side register number 
//		/// RESULT
//		///	R0 = INSTR.REG - REG number in DATA
//		/// </summary>
//		SUB = 3,
//		/// <summary>
//		/// Multiply two registers and put the result in R0
//		/// MODE
//		///	IMMI: DATA holds right hand side register number 
//		/// RESULT
//		///	R0 = INSTR.REG * REG number in DATA
//		/// </summary>
//		MUL = 4,
//		/// <summary>
//		/// Divide two regiesters and put the result in R0
//		/// MODE
//		///	IMMI: DATA holds right hand side register number 
//		/// RESULT
//		///	R0 = INSTR.REG / REG number in DATA
//		/// </summary>
//		DIV = 5,
//		/// <summary>
//		/// Modulus two regiesters and put the result in R0
//		/// MODE
//		///	IMMI: DATA holds right hand side register number 
//		/// RESULT
//		///	R0 = INSTR.REG % REG number in DATA
//		/// </summary>
//		MOD = 6,
//		/// <summary>
//		/// Loads context LVALUE
//		/// MODE
//		/// 
//		/// RESULT
//		///	INSTR.REG = LVALUE
//		/// </summary>
//		LCTX = 7,
//		/// <summary>
//		/// Loads an object ref
//		/// MODE
//		///	IMMI: Data holds the const string instance of the field name
//		///	NXTS: Next instruction holds a index of a string to use for the name of the field to load from R0 
//		/// RESULT
//		///	INSTR.REG = LVALUE
//		/// </summary>
//		LVAL = 8,
//		/// <summary>
//		/// Creates a new field in the object LVALUE in INSTR.REG
//		/// MODE
//		///	IMMI: Data holds the const string instance of the field name
//		///	NXTS: Next instruction holds a index of a string to use for the name 
//		/// RESULT
//		///	
//		/// </summary>
//		VAR = 9,
//		/// <summary>
//		/// Create a new object as a field of the object RVALUE
//		/// MODE
//		///	IMMI: Data holds the const string instance of the field name
//		///	NXTS: Field name of the new object
//		/// RESULT
//		/// 
//		/// </summary>
//		OBJ = 10,
//		/// <summary>
//		/// Fetch the value for LVALUE and place it in a register (in place)
//		/// MODE
//		///	
//		/// RESULT
//		///	INSTR.REG = RVALUE of INSTR.REG
//		/// </summary>
//		RVAL = 11,
//		/// <summary>
//		/// Store a value from a R0 into a named field
//		/// MODE
//		///	IMMI: use DATA has string index
//		///	NXTI: use int in next instruction
//		///	NXTF: use float in next instruction
//		///	NXTS: use string in next instruction
//		/// RESULT
//		///	INSTRUCTION.REG = RVALUE of R0
//		/// </summary>
//		STO = 12,
//		/// <summary>
//		/// Store a const into a register
//		/// MODE
//		///	IMMI: use sbyte in DATA
//		///	NXTI: use int in next instruction
//		///	NXTF: use float in next instruction
//		///	NXTS: use string in next instruction
//		/// RESULT
//		///	INSTRUCTION.REG = value
//		/// </summary>
//		LOAD = 13,
//		/// <summary>
//		/// Enter a new context using the LVALUE in the specified register
//		/// MODE
//		///	
//		/// RESULT
//		///	No change to registers	
//		/// </summary>
//		ENTER = 14,
//		/// <summary>
//		/// Pop the context
//		/// MODE
//		/// 
//		/// RESULT
//		///	Registers and PC are loaded from the context stack
//		/// </summary>
//		LEAVE = 15,
//		/// <summary>
//		/// Push a register value onto the stack
//		/// MODE
//		///
//		/// RESULT
//		///	No change to registers	
//		/// </summary>
//		PUSH = 16,
//		/// <summary>
//		/// Pop a value
//		/// MODE
//		///
//		/// RESULT
//		///	INSTR.REG = POP
//		/// </summary>
//		POP = 17,
//		/// <summary>
//		/// Jump relative 
//		/// MODE
//		///	IMMI: PC += DATA
//		///	NXTI: PC += next instruction
//		/// RESULT
//		///	No change to registers
//		/// </summary>
//		JMP = 18,
//		/// <summary>
//		/// Jump relative if the specified register is zero 
//		/// MODE
//		///	IMMI: PC += DATA
//		///	NXTI: PC += next instruction
//		/// RESULT
//		///	No change to registers
//		/// </summary>
//		JMPZ = 19,
//		/// <summary>
//		/// Jump relative if the specified register is not zero 
//		/// MODE
//		///	IMMI: PC += DATA
//		///	NXTI: PC += next instruction
//		/// RESULT
//		///	No change to registers
//		/// </summary>
//		JMPNZ = 20,
//		JMPGT = 21,
//		JMPGTEQ = 22,
//		JMPLT = 23,
//		JMPLTEQ = 24,
//		CALL = 25,
//		/// <summary>
//		/// Stop execution
//		/// </summary>
//		STOP = 63,
//	}
//}
