using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG.Factions
{
	public enum GroupCombatReaction
	{
		Neutral = 0,
		Enemy = 1,
		Ally = 2,
		Friend = 3,
	}

	public struct FactionRelation
	{
		public String8 OtherFactionId;
		public int Modifier;
		public GroupCombatReaction Relation;
	}
}
