using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.RPG.Factions
{
	[Flags]
	public enum FactionFlags
	{
		None = 0,
		HiddenFromPC = 0x1,
		Evil = 0x2,
		SpecialCombat = 0x4,
		TrackCrime = 0x10,
		AllowSell = 0x20,
	}

	public class Faction
	{
		public String8 Id;
		public FactionFlags Flags;
		public AlignmentOrder AlignOrder;
		public AlignmentGoodness AlignGoodness;
		public string Name;
		public Vector<FactionRelation> Relations = new();
		public Dictionary<String8, int> RelationLookup = new();
		public Vector<Rank> Ranks = new();

		public void AddRelation(in String8 otherFaction, in GroupCombatReaction reaction, int modifier)
		{
			var rel = new FactionRelation() { OtherFactionId = otherFaction, Modifier = modifier, Relation = reaction };
			var idx = Relations.AddAndGetIndex(rel);
			RelationLookup.Add(otherFaction, idx);
		}
	}
}
