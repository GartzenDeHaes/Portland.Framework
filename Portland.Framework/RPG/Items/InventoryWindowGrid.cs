using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public class InventoryWindowGrid : IDisposable
	{
		ItemCollection _inventory;
		int _startSlotInInventory;
		public readonly int GridWidth;
		public readonly int GridHeight;
		public readonly int WindowAreaTypeId;
		public readonly int WindowStartIndex;

		public int Count { get { return GridWidth * GridHeight; } }

		public readonly string Title;
		public readonly bool IsReadOnly;
		public readonly ItemRequirement[] Requirements;

		public Action<int> OnSlotChanged;

		public ItemStack this[int gridIndex]
		{
			get { return _inventory[gridIndex + _startSlotInInventory]; }
			set
			{
				int sindex = gridIndex + _startSlotInInventory;

				if (IsAcceptableItem(value) && !_inventory[sindex].Equals(value))
				{
					_inventory[sindex] = value;

					OnSlotChanged?.Invoke(gridIndex);
				}
			}
		}

		public bool IsAcceptableItem(ItemStack item)
		{
			if (IsReadOnly)
			{
				return false;
			}
			for (int i = 0; i < Requirements.Length; i++)
			{
				if (!Requirements[i].MeetsRequirement(item))
				{
					return false;
				}
			}

			return true;
		}

		public bool IsEmpty()
		{
			for (int x = 0; x < Count; x++)
			{
				if (_inventory[x + _startSlotInInventory].StackCount != 0)
				{
					return false;
				}
			}

			return true;
		}

		public void ClearSlot(int gridIndex)
		{
			_inventory[gridIndex + _startSlotInInventory] = new ItemStack(gridIndex + _startSlotInInventory, ItemDefinitionBuilder.Empty, 0);
		}

		/// <summary>
		/// Merge an item with an existing stack, or put in an empty slot
		/// </summary>
		public int MoveOrMergeItem(ItemStack item)
		{
			if (! IsAcceptableItem(item))
			{
				return -1;
			}

			int emptyIndex = -1;

			var maximumStackSize = item.Definition.MaxStackSize;

			ItemStack current;

			for (int i = 0; i < Count; i++)
			{
				current = this[i];

				if (current.IsEmpty() && emptyIndex == -1)
				{
					emptyIndex = i;
				}
				else if (current.Definition.ItemId == item.Definition.ItemId && current.StackCount < maximumStackSize)
				{
					if (current.StackCount + item.StackCount > maximumStackSize)
					{
						// fill current slot and continue

						item.ChangeStackCount(item.StackCount - (maximumStackSize - current.StackCount));

						current.ChangeStackCount(maximumStackSize - current.StackCount);
						
						continue;
					}

					current.ChangeStackCount(item.StackCount);

					item.SetStackCount(0);

					return i;
				}
			}

			if (emptyIndex != -1)
			{
				this[emptyIndex] = item;
				item.SetStackCount(0);
			}

			return emptyIndex;
		}

		public void CopyTo(InventoryWindowGrid area)
		{
			// ItemCollection calls CloneAndZeroCount()

			for (int i = 0; i < area.Count && i < Count; i++)
			{
				area[i] = this[i];
			}
		}

		public InventoryWindowGrid
		(
			int windowAreaTypeId, 
			int windowStartIndex, 
			ItemCollection inventory, 
			int startSlotInInventory, 
			string gridName, 
			int gridWidth, 
			int gridHeight, 
			bool isReadonly, 
			ItemRequirement[] requirements
		)
		{
			WindowAreaTypeId = windowAreaTypeId;
			WindowStartIndex = windowStartIndex;
			_inventory = inventory;
			_startSlotInInventory = startSlotInInventory;
			GridWidth = gridWidth;
			GridHeight = gridHeight;
			Title = gridName;
			IsReadOnly = isReadonly;
			Requirements = requirements;
		}

		public void Dispose()
		{
			_inventory = null;
		}
	}
}
