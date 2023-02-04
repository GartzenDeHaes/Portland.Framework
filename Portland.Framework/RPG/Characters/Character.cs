using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.RPG
{
	[Serializable]
	public class Character
	{
		public StatSet StatsAndSkils;
		public PropertySetKeys Properties;
		public ItemCollection Inventory;
		// derived stats (AP, AC, HP
		// blackboard
		// achivements
		// active/completed quests (challenges) https://fallout.fandom.com/wiki/Fallout:_New_Vegas_challenges

		public Vector<int> PassiveEffects = new Vector<int>();
		public Vector<ActiveEffect> ActiveEffects = new Vector<ActiveEffect>();
	}
}
