//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Portland.Collections;

//namespace Portland.Interp
//{
//	public struct DataIndex
//	{
//		public ushort CellNumber;
//		public byte CellIndex;
//		public DataType Type;

//		public int ToInt()
//		{
//			return CellNumber | CellIndex << 16 | (byte)Type << 24;
//		}

//		public static DataIndex FromInt(int data)
//		{
//			return new DataIndex { CellNumber = (ushort)(data & 0xFFFFu), CellIndex = (byte)((data >> 16) & 0xFF), Type = (DataType)(data >> 24) };
//		}
//	}

//	public sealed class DataRef<T>
//	{
//		readonly DataTable<T>.DataCell _cell;
//		public readonly DataIndex Index;

//		internal DataRef(DataTable<T>.DataCell cell, DataIndex index)
//		{
//			_cell = cell;
//			Index = index;
//		}

//		public T Value
//		{
//			get { return _cell.GetValue(Index.CellIndex); }
//			set { _cell.SetValue(Index.CellIndex, value); }
//		}
//	}

//	public sealed class DataTable<T>
//	{
//		internal sealed class DataCell
//		{
//			BitSet32 _allocations;
//			T[] _data = new T[32];
//			public readonly ushort CellNumber;

//			public DataCell(ushort cellNum)
//			{
//				CellNumber = cellNum;
//			}

//			public bool IsFull()
//			{
//				return _allocations.RawBits == UInt32.MaxValue;
//			}

//			public T GetValue(byte cellIndex)
//			{
//				return _data[cellIndex];
//			}

//			public void SetValue(byte cellIndex, T value)
//			{
//				_data[cellIndex] = value;
//			}

//			public byte Allocate(T value)
//			{
//				for (int i = 0; i < 32; i++)
//				{
//					if (! _allocations.IsSet(i))
//					{
//						_allocations.SetBit(i);
//						_data[i] = value;
//						return (byte)i;
//					}
//				}

//				throw new Exception($"Internal error: DataCell has no free space (IsFull is {IsFull})");
//			}

//			public void Free(byte cellIndex)
//			{
//				Debug.Assert(_allocations.IsSet(cellIndex));

//				_allocations.ClearBit(cellIndex);
//			}
//		}

//		Vector<DataCell> _data = new Vector<DataCell>();
//		public readonly DataType TableDataType;

//		public DataTable(DataType typeId)
//		{
//			FindCellWithFreeSpace();
//			TableDataType = typeId;
//		}

//		DataCell FindCellWithFreeSpace()
//		{
//			// Last cell pushed is likely to have free space
//			if (! _data.Tail().IsFull())
//			{
//				return _data.Tail();
//			}
//			for (int i = 0; i < _data.Count; i++)
//			{
//				if (! _data[i].IsFull())
//				{
//					return _data[i];
//				}
//			}

//			var cell = new DataCell((ushort)_data.Count);
//			_data.Add(cell);
//			return cell;
//		}

//		public T GetValue(in DataIndex index)
//		{
//			return _data[index.CellNumber].GetValue(index.CellIndex);
//		}

//		public void SetValue(in DataIndex index, T value)
//		{
//			_data[index.CellNumber].SetValue(index.CellIndex, value);
//		}

//		public DataRef<T> AllocateRef(T value)
//		{
//			var cell = FindCellWithFreeSpace();
//			return new DataRef<T>(cell, new DataIndex { CellNumber = cell.CellNumber, CellIndex = cell.Allocate(value), Type = TableDataType });
//		}

//		public DataIndex Allocate(T value)
//		{
//			var cell = FindCellWithFreeSpace();
//			return new DataIndex { CellNumber = cell.CellNumber, CellIndex = cell.Allocate(value), Type = TableDataType };
//		}

//		public void DeAllocate(DataRef<T> item)
//		{
//			_data[item.Index.CellNumber].Free(item.Index.CellIndex);
//		}

//		public void DeAllocate(in DataIndex item)
//		{
//			_data[item.CellNumber].Free(item.CellIndex);
//		}
//	}
//}
