using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	[Serializable]
	public struct ItemRecipeIngredient
	{
		public String8 ItemId;
		public int Amount;
	}

	[Serializable]
	public class ItemRecipe
	{
		public String8 OutputItemId;
		public float BuildTime;
		public ItemRecipeIngredient[] Ingredients;
	}
}
