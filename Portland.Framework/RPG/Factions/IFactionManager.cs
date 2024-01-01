using System;
using System.Collections.Generic;

using Portland.RPG.Factions;


namespace Portland.RPG
{
	public interface IFactionManager
	{
		void DefineFaction(in String8 factionId, string desc, FactionFlags flags, AlignmentOrder order, AlignmentGoodness goodness);
		void SetFactionRelation(in String8 factionAId, in String8 factionBId, GroupCombatReaction rel, int modifier);
		void AddFactionRank(in String8 factionId, int rankNum, string maleTitle, string femaleTitle);
		bool TryGetRelation(in String8 factionAId, in String8 factionBId, out GroupCombatReaction reaction, out int modifier);
	}
}
