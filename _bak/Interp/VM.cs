//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Portland.Collections;

//namespace Portland.Interp
//{
//	/// <summary>
//	/// R0 is always the target of LVAL and STO
//	/// R1 is the accumulator (destination of math type instructions)
//	/// 
//	/// </summary>
//	public sealed class VM
//	{
//		struct Context
//		{
//			public ObjectInst Scope;
//			public BitSet32 RTypes;
//			public int Pc;
//			public ObjectInst OuterScope;
//			public Variant4 r0;
//			public Variant4 r1;
//			public Variant4 r2;
//			public Variant4 r3;
//			public Variant4 r4;
//			public Variant4 r5;
//			public Variant4 r6;
//			public Variant4 r7;
//			public Variant4 r8;
//			public Variant4 r9;
//			public Variant4 ra;
//			public Variant4 rb;
//			public Variant4 rc;
//			public Variant4 rd;
//			public Variant4 re;
//			public Variant4 rf;
//		}

//		Variant4[] _registers = new Variant4[16];
//		BitSet32 _registerTypes;
//		Vector<Context> _context = new Vector<Context>(4);
//		Vector<Variant8> _stack = new Vector<Variant8>(8);
//		int _pc;

//		ObjectTable _table;
//		StringTable _strings;
//		Instruction[] _prog;
//		StringTableToken[] _progConstStrings;
//		Instruction _current;
//		int _temp;

//		public bool Step()
//		{
//			_current = _prog[_pc++];

//			switch (_current.Op)
//			{
//				case OpCode.NOP:
//					break;
//				case OpCode.MOV:
//					_registers[_current.RegNum] = _registers[_current.Data];
//					SetRegisterDataType(_current.RegNum, GetRegisterDataType(_current.Data));
//					break;
//				case OpCode.ADD:
//					Add(_current.RegNum, _current.Data);
//					break;
//				case OpCode.SUB:
//					Sub(_current.RegNum, _current.Data);
//					break;
//				case OpCode.MUL:
//					Mul(_current.RegNum, _current.Data);
//					break;
//				case OpCode.DIV:
//					Div(_current.RegNum, _current.Data);
//					break;
//				case OpCode.MOD:
//					Mod(_current.RegNum, _current.Data);
//					break;
//				case OpCode.LVAL:
//					Lval();
//					break;
//				case OpCode.LCTX:
//					SetRegister(_current.RegNum, _context.Tail().Scope.ObjectVar);
//					break;
//				case OpCode.VAR:
//					_table.AddFieldToObject(_registers[_current.RegNum].Index, GetStrByMode());
//					break;
//				case OpCode.OBJ:
//					_table.AddFieldToObject(_registers[_current.RegNum].Index, GetStrByMode(), _table.AllocateObject().ObjectVar);
//					break;
//				case OpCode.RVAL:
//					break;
//				case OpCode.STO:
//					break;
//				case OpCode.LOAD:
//					break;
//				case OpCode.ENTER:
//					break;
//				case OpCode.LEAVE:
//					break;
//				case OpCode.PUSH:
//					_stack.Add(RegisterToVariant(_current.RegNum));
//					break;
//				case OpCode.POP:
//					SetRegister(_current.RegNum, _stack.Pop());
//					break;
//				case OpCode.JMP:
//					_pc = GetIntByMode();
//					break;
//				case OpCode.JMPZ:
//					_temp = GetIntByMode();
//					if (_registers[_current.RegNum].Int == 0)
//					{
//						_pc = _temp;
//					}
//					break;
//				case OpCode.JMPNZ:
//					_temp = GetIntByMode();
//					if (_registers[_current.RegNum].Int != 0)
//					{
//						_pc = _temp;
//					}
//					break;
//				case OpCode.JMPGT:
//					break;
//				case OpCode.JMPGTEQ:
//					break;
//				case OpCode.JMPLT:
//					break;
//				case OpCode.JMPLTEQ:
//					break;
//				case OpCode.CALL:
//					break;
//				case OpCode.STOP:
//					return false;
//				default:
//					throw new RuntimeException($"Unknown op code {_current.Op}");
//			}
//			return true;
//		}

//		int GetIntByMode()
//		{
//			int ret = 0;
//			switch (_current.Mode)
//			{
//				case Mode.IMMI:
//					ret = _current.Data;
//					break;
//				case Mode.NXTI:
//					ret = _prog[++_pc].Int;
//					break;
//				default:
//					throw new Exception($"Invalid addressing mode for {_current.Op}");
//			}

//			return ret;
//		}

//		StringTableToken GetStrByMode()
//		{
//			switch (_current.Mode)
//			{
//				case Mode.IMMI:
//					return _progConstStrings[_current.Data];
//				case Mode.NXTI:
//					return _progConstStrings[_prog[++_pc].Int];
//				default:
//					throw new Exception($"Invalid addressing mode for {_current.Op}");
//			}
//		}

//		public void Run(PreparedProgram prog)
//		{
//			_prog = prog.Program.Code;
//			_progConstStrings = prog.ConstStrings;
//			_pc = prog.Program.StartPC;
//			_stack.Clear();

//			while(Step())
//			{

//			}
//		}

//		public void Run(Program prog)
//		{
//			var prepared = new PreparedProgram(prog, _strings);
//			Run(prepared);
//		}

//		void Add(int rl, int rr)
//		{
//			if (GetRegisterDataType(rl) == DataType.Int && GetRegisterDataType(rr) == DataType.Int)
//			{
//				SetRegister(0, _registers[rl].Int + _registers[rr].Int);
//			}
//			else
//			{
//				SetRegister(0, GetRegFloat(rl) + GetRegFloat(rr));
//			}
//		}

//		void Sub(int rl, int rr)
//		{
//			if (GetRegisterDataType(rl) == DataType.Int && GetRegisterDataType(rr) == DataType.Int)
//			{
//				SetRegister(0, _registers[rl].Int - _registers[rr].Int);
//			}
//			else
//			{
//				SetRegister(0, GetRegFloat(rl) - GetRegFloat(rr));
//			}
//		}

//		void Mul(int rl, int rr)
//		{
//			if (GetRegisterDataType(rl) == DataType.Int && GetRegisterDataType(rr) == DataType.Int)
//			{
//				SetRegister(0, _registers[rl].Int * _registers[rr].Int);
//			}
//			else
//			{
//				SetRegister(0, GetRegFloat(rl) * GetRegFloat(rr));
//			}
//		}

//		void Div(int rl, int rr)
//		{
//			if (GetRegisterDataType(rl) == DataType.Int && GetRegisterDataType(rr) == DataType.Int)
//			{
//				SetRegister(0, _registers[rl].Int / _registers[rr].Int);
//			}
//			else
//			{
//				SetRegister(0, GetRegFloat(rl) / GetRegFloat(rr));
//			}
//		}

//		void Mod(int rl, int rr)
//		{
//			if (GetRegisterDataType(rl) == DataType.Int && GetRegisterDataType(rr) == DataType.Int)
//			{
//				SetRegister(0, _registers[rl].Int % _registers[rr].Int);
//			}
//			else
//			{
//				SetRegister(0, GetRegFloat(rl) % GetRegFloat(rr));
//			}
//		}

//		void Lval()
//		{
//			if (_current.Mode == Mode.NXTI)
//			{
//				Debug.Assert(GetRegisterDataType(0) == DataType.ObjectIndex);

//				int objIdx = GetIntByMode();
//				StringTableToken name = _progConstStrings[objIdx];

//				ObjectInst context = _table.GetObject(_registers[0].Index);



//				//inst = _table.GetObject(DataIndex.FromInt(objIdx));
//			}
//			else
//			{
//				SetRegister(0, _context.Tail().Scope.ObjectVar);
//			}
//		}

//		void EnterContext(ObjectInst obj)
//		{
//			_context.Add
//			(
//				new Context { 
//					Scope = obj,
//					RTypes = _registerTypes,
//					Pc = _pc,
//					OuterScope = _context.Tail().Scope,
//					r0 = _registers[0],
//					r1 = _registers[1],
//					r2 = _registers[2],
//					r3 = _registers[3],
//					r4 = _registers[4],
//					r5 = _registers[5],
//					r6 = _registers[6],
//					r7 = _registers[7],
//					r8 = _registers[8],
//					r9 = _registers[9],
//					ra = _registers[10],
//					rb = _registers[11],
//					rc = _registers[12],
//					rd = _registers[13],
//					re = _registers[14],
//					rf = _registers[15],
//				}
//			);
//		}

//		void LeaveContext()
//		{
//			var ctx = _context.Pop();
//			_registerTypes = ctx.RTypes;
//			_pc = ctx.Pc;
//			_registers[0] = ctx.r0;
//			_registers[1] = ctx.r1;
//			_registers[2] = ctx.r2;
//			_registers[3] = ctx.r3;
//			_registers[4] = ctx.r4;
//			_registers[5] = ctx.r5;
//			_registers[6] = ctx.r6;
//			_registers[7] = ctx.r7;
//			_registers[8] = ctx.r8;
//			_registers[9] = ctx.r9;
//			_registers[10] = ctx.ra;
//			_registers[11] = ctx.rb;
//			_registers[12] = ctx.rc;
//			_registers[13] = ctx.rd;
//			_registers[14] = ctx.re;
//			_registers[15] = ctx.rf;
//		}

//		DataType GetRegisterDataType(int regNum)
//		{
//			int bitStart = (regNum) * 4;
//			uint bits = _registerTypes.BitsAt(bitStart, 2);

//			return (DataType)bits;
//		}

//		void SetRegisterDataType(int regNum, DataType type)
//		{
//			int bitStart = (regNum) * 4;
//			_registerTypes.SetBitsAt(bitStart, 2, (uint)type);
//		}

//		void SetRegister(int regNum, int i)
//		{
//			_registers[regNum].Int = i;
//			SetRegisterDataType(regNum, DataType.Int);
//		}

//		void SetRegister(int regNum, float i)
//		{
//			_registers[regNum].Float = i;
//			SetRegisterDataType(regNum, DataType.Float);
//		}

//		void SetRegister(int regNum, StringTableToken s)
//		{
//			_registers[regNum].Str = s;
//			SetRegisterDataType(regNum, DataType.String);
//		}

//		void SetRegister(int regNum, in Variant8 val)
//		{
//			switch(val.TypeIs)
//			{
//				case VariantType.Int:
//					SetRegister(regNum, val.Int);
//					break;
//				case VariantType.Float:
//					SetRegister(regNum, val.Float);
//					break;
//				case VariantType.Bool:
//					SetRegister(regNum, val.ToInt());
//					break;
//				case VariantType.String:
//					SetRegister(regNum, _strings.Get(val.ToString()));
//					break;
//				default:
//					throw new RuntimeException($"Cannot set variant type {val.TypeName} to a register)");
//			}
//		}

//		void SetRegister(int regNum, DataIndex i)
//		{
//			_registers[regNum].Index = i;
//			SetRegisterDataType(regNum, DataType.ObjectIndex);
//		}

//		Variant8 RegisterToVariant(int regNum)
//		{
//			switch (GetRegisterDataType(regNum))
//			{
//				case DataType.Int:
//					return new Variant8(_registers[regNum].Int);
//				case DataType.Float:
//					return new Variant8(_registers[regNum].Float);
//				case DataType.String:
//					return new Variant8(_strings.GetString(_registers[regNum].Str));
//				case DataType.ObjectIndex:
//					return new Variant8(_registers[regNum].Index.ToInt());
//			}

//			throw new RuntimeException($"Cannot convert register {regNum}({GetRegisterDataType(regNum)} to variant)");
//		}

//		public float GetRegFloat(int regNum) 
//		{
//			DataType type = GetRegisterDataType(regNum);
//			if (type == DataType.Float)
//			{
//				return _registers[regNum].Float;
//			}
//			if (type == DataType.Int)
//			{
//				return _registers[regNum].Int;
//			}
//			throw new TypeMismatchException($"Cannot covert register {regNum}({type}) to float");
//		}

//		public int GetRegInt(int regNum)
//		{
//			DataType type = GetRegisterDataType(regNum);
//			if (type == DataType.Int)
//			{
//				return _registers[regNum].Int;
//			}
//			if (type == DataType.Float)
//			{
//				return (int)_registers[regNum].Float;
//			}
//			throw new TypeMismatchException($"Cannot covert register {regNum}({type}) to int");
//		}

//		public StringTableToken GetRegStr(int regNum)
//		{
//			DataType type = GetRegisterDataType(regNum);
//			if (type == DataType.String)
//			{
//				return _registers[regNum].Str;
//			}
//			if (type == DataType.Int)
//			{
//				return _strings.Get(_registers[regNum].Int.ToString());
//			}
//			if (type == DataType.Float)
//			{
//				return _strings.Get(_registers[regNum].Float.ToString());
//			}
//			throw new TypeMismatchException($"Cannot covert register {regNum}({type}) to string");
//		}

//		public VM(StringTable strings)
//		{
//			_strings = strings;
//			_table = new ObjectTable(strings);

//			var ctx = new Context { Pc = 0, RTypes = new BitSet32() };
//			ctx.Scope = _table.AllocateObject();
//			_context.Add(ctx);

//			SetRegisterDataType(0, DataType.ObjectIndex);
//		}
//	}
//}
