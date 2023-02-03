//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Portland.RPG.Items
//{
//	public class InventorySlot
//	{
//		public ItemStack Items;
//		public readonly int SlotNum;

//		public readonly ItemRequirement[] Requirements;

//		public Action<InventorySlot> OnStackSet;

//		public bool TrySetItem(ItemStack item)
//		{
//			for (int i = 0; i < Requirements.Length; i++)
//			{
//				if (!Requirements[i].MeetsRequirement(item))
//				{
//					return false;
//				}
//			}

//			Items = item;
			
//			OnStackSet?.Invoke(this);

//			return true;
//		}

//		public InventorySlot(int slotNum, ItemRequirement[] requirements)
//		{
//			SlotNum = slotNum;
//			Requirements = requirements;
//		}
//	}
//}
