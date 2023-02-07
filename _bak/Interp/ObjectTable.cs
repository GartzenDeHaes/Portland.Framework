//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//using Portland.Collections;

//namespace Portland.Interp
//{
//	public enum DataType : byte
//	{
//		Int = 0,
//		Float = 1,
//		String = 2,
//		ObjectIndex = 3,
//		Object = 16
//	}

//	[StructLayout(LayoutKind.Explicit)]
//	public struct Variant4
//	{
//		[FieldOffset(0)]
//		public int Int;

//		[FieldOffset(0)]
//		public float Float;

//		[FieldOffset(0)]
//		public StringTableToken Str;

//		[FieldOffset(0)]
//		public DataIndex Index;
//	}

//	public class ObjectInst
//	{
//		public DataIndex ObjectVar;
//		public Vector<StringTableToken> FieldNames = new Vector<StringTableToken>();
//		public Vector<DataIndex> Fields = new Vector<DataIndex>();
//	}

//	public sealed class ObjectTable
//	{
//		public readonly DataTable<Variant4>[] Data = {
//			new DataTable<Variant4>(DataType.Int),
//			new DataTable<Variant4>(DataType.Float),
//			new DataTable<Variant4>(DataType.String),
//			new DataTable<Variant4>(DataType.ObjectIndex),
//		};

//		DataTable<ObjectInst> _objects = new DataTable<ObjectInst>(DataType.Object);

//		StringTable _strings;

//		public float GetValueFloat(in DataIndex idx)
//		{
//			float value = 0;
//			Variant4 data = Data[(int)idx.Type].GetValue(idx);

//			if (idx.Type == DataType.Float)
//			{
//				value = data.Float;
//			}
//			else if (idx.Type == DataType.Int)
//			{
//				value = data.Int;
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to float");
//			}

//			return value;
//		}

//		public int GetValueInt(in DataIndex idx)
//		{
//			int value = 0;
//			Variant4 data = Data[(int)idx.Type].GetValue(idx);

//			if (idx.Type == DataType.Float)
//			{
//				value = (int)data.Float;
//			}
//			else if (idx.Type == DataType.Int)
//			{
//				value = data.Int;
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to int");
//			}

//			return value;
//		}

//		public StringTableToken GetValueString(in DataIndex idx)
//		{
//			StringTableToken value;
//			Variant4 data = Data[(int)idx.Type].GetValue(idx);

//			if (idx.Type == DataType.Float)
//			{
//				value = _strings.Get(data.Float.ToString());
//			}
//			else if (idx.Type == DataType.Int)
//			{
//				value = _strings.Get(data.Int.ToString());
//			}
//			else if (idx.Type == DataType.String)
//			{
//				value = data.Str;
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to string");
//			}

//			return value;
//		}

//		// dosent work
//		public void SetValue(in DataIndex idx, int value)
//		{
//			if (idx.Type == DataType.Int)
//			{
//				Data[(int)idx.Type].SetValue(idx, CreateValue(value));
//			}
//			else
//			{
//				ReAllocate(ref idx, value);
//			}
//		}

//		public ObjectInst GetObject(in DataIndex idx)
//		{
//			if (idx.Type == DataType.ObjectIndex)
//			{
//				int oidx = Data[(int)DataType.ObjectIndex].GetValue(idx).Int;
//				return _objects.GetValue(DataIndex.FromInt(oidx));
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to object");
//			}
//		}

//		bool TryFindField(in DataIndex objIndex, in StringTableToken name, out ObjectInst obj, out int index)
//		{
//			int oidx = Data[(int)DataType.ObjectIndex].GetValue(objIndex).Int;
//			obj = _objects.GetValue(DataIndex.FromInt(oidx));
//			for (index = 0; index < obj.FieldNames.Count; index++)
//			{
//				if (obj.FieldNames[index] == name)
//				{
//					return true;
//				}
//			}

//			return false;
//		}

//		public void SetFieldInObject(in DataIndex idx, in StringTableToken name, in DataIndex objValue)
//		{
//			if (idx.Type == DataType.ObjectIndex)
//			{
//				if (TryFindField(idx, name, out var obj, out var pos))
//				{
//					obj.Fields[pos] = objValue;
//				}
//				else
//				{
//					throw new RuntimeException($"Cannot find field {_strings.GetString(name)} in object");
//				}
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to object");
//			}
//		}

//		public void SetFieldInObject(in DataIndex idx, in StringTableToken name, int ival)
//		{
//			if (idx.Type == DataType.ObjectIndex)
//			{
//				if (TryFindField(idx, name, out var obj, out var pos))
//				{
//					if (obj.Fields[pos].Type == DataType.Int)
//					{
						
//					}
//				}
//				else
//				{
//					throw new RuntimeException($"Cannot find field {_strings.GetString(name)} in object");
//				}
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to object");
//			}
//		}

//		public void AddFieldToObject(in DataIndex idx, in StringTableToken name, in DataIndex objValue)
//		{
//			if (idx.Type == DataType.ObjectIndex && objValue.Type == DataType.ObjectIndex)
//			{
//				int oidx = Data[(int)DataType.ObjectIndex].GetValue(idx).Int;
//				ObjectInst obj = _objects.GetValue(DataIndex.FromInt(oidx));
//				obj.FieldNames.Add(name);
//				obj.Fields.Add(objValue);
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to object");
//			}
//		}

//		public void AddFieldToObject(in DataIndex idx, in StringTableToken name)
//		{
//			if (idx.Type == DataType.ObjectIndex)
//			{
//				int oidx = Data[(int)DataType.ObjectIndex].GetValue(idx).Int;
//				ObjectInst obj = _objects.GetValue(DataIndex.FromInt(oidx));
//				obj.FieldNames.Add(name);
//				obj.Fields.Add(Allocate(0));
//			}
//			else
//			{
//				throw new TypeMismatchException($"Cannot covert {idx.Type} to object");
//			}
//		}

//		public ObjectInst AllocateObject()
//		{
//			ObjectInst oi = new ObjectInst();
//			DataIndex objIndex = _objects.Allocate(oi);
//			oi.ObjectVar = Data[(int)DataType.ObjectIndex].Allocate(CreateValue(objIndex.ToInt()));

//			return oi;
//		}

//		public DataIndex Allocate(float value)
//		{
//			return Data[(int)DataType.Float].Allocate(CreateValue(value));
//		}

//		public DataIndex Allocate(int value)
//		{
//			return Data[(int)DataType.Int].Allocate(CreateValue(value));
//		}

//		public DataIndex Allocate(string s)
//		{
//			return Data[(int)DataType.String].Allocate(CreateValue(s));
//		}

//		public DataIndex Allocate(in StringTableToken s)
//		{
//			return Data[(int)DataType.String].Allocate(CreateValue(s));
//		}

//		Variant4 CreateValue(int i)
//		{
//			return new Variant4 { Int = i };
//		}

//		Variant4 CreateValue(float f)
//		{
//			return new Variant4 { Float = f };
//		}

//		Variant4 CreateValue(string s)
//		{
//			StringTableToken tok = _strings.Get(s);
//			return new Variant4 { Str = tok };
//		}

//		Variant4 CreateValue(in StringTableToken s)
//		{
//			return new Variant4 { Str = s };
//		}

//		public void DeAllocate(in DataIndex i)
//		{
//			Debug.Assert(i.Type != DataType.Object);

//			if (i.Type == DataType.ObjectIndex)
//			{
//				_objects.DeAllocate(DataIndex.FromInt(Data[(int)DataType.ObjectIndex].GetValue(i).Int));
//			}

//			Data[(int)i.Type].DeAllocate(i);
//		}

//		public void ReAllocate(ref DataIndex i, int value)
//		{
//			Debug.Assert(i.Type != DataType.Object);

//			if (i.Type == DataType.ObjectIndex)
//			{
//				_objects.DeAllocate(DataIndex.FromInt(Data[(int)DataType.ObjectIndex].GetValue(i).Int));
//			}

//			Data[(int)i.Type].DeAllocate(i);

//			var n = Allocate(value);
//			i.CellIndex = n.CellIndex;
//			i.CellNumber = n.CellNumber;
//			i.Type = n.Type;
//		}

//		public void ReAllocate(ref DataIndex i, float value)
//		{
//			Debug.Assert(i.Type != DataType.Object);

//			if (i.Type == DataType.ObjectIndex)
//			{
//				_objects.DeAllocate(DataIndex.FromInt(Data[(int)DataType.ObjectIndex].GetValue(i).Int));
//			}

//			Data[(int)i.Type].DeAllocate(i);

//			var n = Allocate(value);
//			i.CellIndex = n.CellIndex;
//			i.CellNumber = n.CellNumber;
//			i.Type = n.Type;
//		}

//		public void ReAllocate(ref DataIndex i, in StringTableToken value)
//		{
//			Debug.Assert(i.Type != DataType.Object);

//			if (i.Type == DataType.ObjectIndex)
//			{
//				_objects.DeAllocate(DataIndex.FromInt(Data[(int)DataType.ObjectIndex].GetValue(i).Int));
//			}

//			Data[(int)i.Type].DeAllocate(i);

//			var n = Allocate(value);
//			i.CellIndex = n.CellIndex;
//			i.CellNumber = n.CellNumber;
//			i.Type = n.Type;
//		}

//		public ObjectTable(StringTable strings)
//		{
//			_strings = strings;
//		}
//	}
//}
