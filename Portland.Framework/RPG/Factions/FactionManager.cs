using System;
using System.Collections.Generic;

using Portland.Collections;
using Portland.Mathmatics;

namespace Portland.RPG.Factions
{
	public enum AlignmentOrder
	{
		Neutral,
		Lawful,
		Chaotic
	}

	public enum AlignmentGoodness
	{
		Neutral,
		Good,
		Evil
	}

	public class FactionManager : IFactionManager, IDisposable
	{
		Vector<Faction> _factions = new();
		Dictionary<String8, int> _factionLookup = new();

		public FactionManager() 
		{ 
		}

		public void DefineFaction(in String8 factionId, string desc, FactionFlags flags, AlignmentOrder order, AlignmentGoodness goodness)
		{
			if (_factionLookup.ContainsKey(factionId))
			{
				throw new Exception($"Duplicate faction {factionId}");
			}

			var faction = new Faction() { Id = factionId, Name = desc, Flags = flags, AlignOrder = order, AlignGoodness = goodness };
			var idx = _factions.AddAndGetIndex(faction);
			_factionLookup.Add(factionId, idx);

			//return faction;
		}

		public void SetFactionRelation(in String8 factionAId, in String8 factionBId, GroupCombatReaction rel, int modifier)
		{
			var factionA = _factionLookup[factionAId];
			var factionB = _factionLookup[factionBId];
			_factions[factionA].AddRelation(factionBId, rel, modifier);
			_factions[factionB].AddRelation(factionAId, rel, modifier);
		}

		public void AddFactionRank(in String8 factionId, int rankNum, string maleTitle, string femaleTitle)
		{
			var faction = _factionLookup[factionId];
			_factions[faction].Ranks.Add(new Rank() {
				RankNumber = rankNum,
				MaleTitle = maleTitle,
				FemaleTitle = femaleTitle
			});
		}

		public bool TryGetRelation(in String8 factionAId, in String8 factionBId, out GroupCombatReaction reaction, out int modifier)
		{
			if (_factionLookup.TryGetValue(factionAId, out var idx))
			{
				var factiona = _factions[idx];
				if (factiona.RelationLookup.TryGetValue(factionBId, out var ridx))
				{
					var relation = factiona.Relations[ridx];
					reaction = relation.Relation;
					modifier = relation.Modifier; 
					return true;
				}
			}

			reaction = GroupCombatReaction.Neutral;
			modifier = 0;
			return false;
		}

		public FactionsDto ToDataObject()
		{
			return new FactionsDto() { Factions = _factions.ToArray() };
		}

		public static FactionManager FromDataObject(FactionsDto dto)
		{
			var man = new FactionManager();
			man._factions.Add(dto.Factions);
			for (int i = 0; i < man._factions.Count; i++)
			{
				man._factionLookup.Add(man._factions[i].Id, i);
			}
			return man;
		}

		public static bool TryParseRelation(string str, out GroupCombatReaction relation)
		{
			str = str.ToLower();
			switch(str)
			{
				case "neutral":
					relation = GroupCombatReaction.Neutral;
					break;
				case "enemy":
					relation = GroupCombatReaction.Enemy;
					break;
				case "ally":
					relation = GroupCombatReaction.Ally;
					break;
				case "friend":
					relation = GroupCombatReaction.Friend;
					break;
				default:
					relation = GroupCombatReaction.Neutral;
					return false;
			}
			return true;
		}

		public static bool TryParseAlignment(string str, out AlignmentOrder acl, out AlignmentGoodness age)
		{
			acl = AlignmentOrder.Neutral;
			age = AlignmentGoodness.Neutral;

			int idx = str.IndexOf(' ');
			if (idx < 0)
			{
				idx = str.IndexOf('-');
				if (idx < 0)
				{
					return false;
				}	
			}

			string cl = str.Substring(0, idx);
			string ge = str.Substring(idx + 1);

			switch(cl.ToUpper())
			{
				case "CHAOTIC":
					acl = AlignmentOrder.Chaotic;
					break;
				case "LAWFUL":
					acl = AlignmentOrder.Lawful;
					break;
				case "NEUTRAL":
					break;
				default:
					return false;
			}

			switch(ge.ToUpper())
			{
				case "GOOD":
					age = AlignmentGoodness.Good;
					break;
				case "EVIL":
					age = AlignmentGoodness.Evil;
					break;
				case "NEUTRAL":
					age = AlignmentGoodness.Neutral;
					break;
				default:
					return false;
			}

			return true;
		}

		public void Dispose()
		{
			_factions.Dispose();
			_factions = null;
			_factionLookup.Clear();
			_factionLookup = null;
		}
	}

	public class FactionsDto
	{
		public Faction[] Factions { get; set; }
	}
}
