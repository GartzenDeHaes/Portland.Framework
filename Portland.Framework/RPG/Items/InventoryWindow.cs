using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public class InventoryWindow : IDisposable
	{
		InventoryWindowGrid[] _windowAreas;

		public readonly int WindowTypeId;
		public readonly int WindowInstanceId;
		public readonly string WindowTitle;

		public int SlotCount
		{
			get
			{
				int size = 0;
				for (int i = 0; i < _windowAreas.Length; i++)
				{
					size += _windowAreas[i].Count;
				}
				return size;
			}
		}

		public ItemStack this[int index]
		{
			get
			{
				int len = _windowAreas.Length;
				for (int i = 0; i < len; i++)
				{
					var area = _windowAreas[i];

					if (index >= area.WindowStartIndex && index < area.WindowStartIndex + area.Count)
					{
						return area[index - area.WindowStartIndex];
					}
				}

				throw new IndexOutOfRangeException();
			}
			set
			{
				int len = _windowAreas.Length;
				for (int i = 0; i < len; i++)
				{
					var area = _windowAreas[i];

					if (index >= area.WindowStartIndex && index < area.WindowStartIndex + area.Count)
					{
						if (!area[index - area.WindowStartIndex].Equals(value))
						{
							area[index - area.WindowStartIndex] = value;
						}
						return;
					}
				}

				throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void MoveOrMergeItem(InventoryWindowGrid fromArea, int fromItemIndex)
		{
			for (int i = 0; i < _windowAreas.Length; i++)
			{
				if (_windowAreas[i] == fromArea)
				{
					continue;
				}
				if (TryMoveOrMerge(fromArea[fromItemIndex], _windowAreas[i]))
				{
					return;
				}
			}
		}

		bool TryMoveOrMerge(ItemStack item, InventoryWindowGrid area)
		{
			if (area.IsReadOnly)
			{
				return false;
			}

			area.MoveOrMergeItem(item);
			return item.StackCount == 0;
		}

		public InventoryWindow(string title, int windowTypeId, int windowInstanceId, InventoryWindowGrid[] sections)
		{
			WindowTypeId = windowTypeId;
			WindowInstanceId = windowInstanceId;
			WindowTitle = title;
			_windowAreas = sections;
		}

		public void Dispose()
		{
			_windowAreas = null;
		}
	}
}
