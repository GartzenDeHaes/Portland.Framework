using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ItemCollection
	{
		ItemStack[] _items;
		
		public readonly string CollectionName;

		public Action<int> OnCollectionChanged;

		public ItemStack this[int index]
		{
			get { return _items[index]; }
			set 
			{
				var item = value.Clone(index);

				item.OnPropertyChanged = _items[index].OnPropertyChanged;
				item.OnStackCountChanged = _items[index].OnStackCountChanged;

				_items[index] = item;

				OnCollectionChanged?.Invoke(index);
			}
		}

		public int Count { get { return _items.Length; } }

		public void ClearSlot(int index)
		{
			_items[index] = new ItemStack(index, ItemDefinitionBuilder.Empty, 0);

			OnCollectionChanged?.Invoke(index);
		}

		public void DecrementStackCount(int index)
		{
			_items[index].ChangeStackCount(-1);
			if (_items[index].StackCount == 0)
			{
				ClearSlot(index);
			}
		}

		public ItemCollection(string name, int numSlots)
		{
			CollectionName = name;

			_items = new ItemStack[numSlots];

			for (int x = 0; x < numSlots; x++)
			{
				_items[x] = new ItemStack(x, ItemDefinitionBuilder.Empty, 0);
			}
		}
	}
}
