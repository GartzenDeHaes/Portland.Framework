using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Portland.RPG
{
	public class InventoryWindow : IDisposable
	{
		InventoryWindowGrid[] _windowAreas;

		public Action<int> OnSelectionChanged;

		public readonly int WindowTypeId;
		public readonly int WindowInstanceId;
		public readonly string WindowTitle;

		int _selected;

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

		public int SelectedSlot
		{
			get { return _selected; }
			set 
			{
				_selected = value;
				OnSelectionChanged?.Invoke(value);
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

		public bool TryGetWindowSection(string name, out InventoryWindowGrid grid)
		{
			for (int i = 0; i < _windowAreas.Length; i++)
			{
				grid = _windowAreas[i];
				if (grid.SectionName.Equals(name))
				{
					return true;
				}
			}

			grid = default(InventoryWindowGrid);
			return false;
		}

		public bool TrySetSectionItem(string winGridName, int winIndex, ItemStack item)
		{
			if (TryGetWindowSection(winGridName, out var grid))
			{
				if (winIndex < grid.Count)
				{
					grid[winIndex] = item;
					return true;
				}
			}

			return false;
		}

		public bool TryMergSectionItem(string winGridName, ItemStack item)
		{
			if (TryGetWindowSection(winGridName, out var grid))
			{
				grid.MoveOrMergeItem(item);
				return true;
			}

			return false;
		}

		public Variant8 GetProperty(int index, in String propName)
		{
			var item = this[index];
			return item.GetPropertyVariant(propName);
		}

		public bool TryGetProperty(int index, in String propName, out Variant8 value)
		{
			var item = this[index];
			return item.TryGetProperty(propName, out value);
		}

		public bool TrySumItemProp(string winGridName, in String propName, out float amt)
		{
			amt = 0;
			bool found = false;
			ItemStack item;

			if (TryGetWindowSection(winGridName, out var grid))
			{
				for (int i = 0; i < grid.Count; i++)
				{
					item = grid[i];
					if (item.TryGetProperty(propName, out var value))
					{
						amt += (float)value;
						found = true;
					}
				}
			}

			return found;
		}

		public bool TrySumItemProp(in String propName, out float amt)
		{
			amt = 0;
			bool found = false;
			ItemStack item;

			for (int x = 0; x < _windowAreas.Length; x++)
			{
				var grid = _windowAreas[x];

				for (int i = 0; i < grid.Count; i++)
				{
					item = grid[i];
					if (item.TryGetProperty(propName, out var value))
					{
						amt += (float)value;
						found = true;
					}
				}
			}

			return found;
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
