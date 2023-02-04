using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Mathmatics;
using Portland.Text;

namespace Portland.RPG
{
	public sealed class StatFactory
	{
		Vector<StatDefinition> _statDefinitions = new Vector<StatDefinition>();
		Vector<StatDefinitionSet> _statDefinitionSets = new Vector<StatDefinitionSet>();

		public StatSet CreateStats(String8 statDefSetId)
		{
			if (TryGetDefinitionSet(statDefSetId, out var defs))
			{
				var set = new StatSet { SetId = statDefSetId, Stats = new Stat[defs.Stats.Length] };
				StatDefinition def;

				for (int i = 0; i < defs.Stats.Length; i++)
				{
					def = defs.Stats[i];
					set.Stats[i] = new Stat { StatId = def.StatId, Value = def.Default };
				}

				return set;
			}
			else
			{
				throw new Exception($"Unknown stat definition set {statDefSetId}");
			}
		}

		public bool TryGetStatIndex(String8 statDefId, AsciiId4 statId, out int index)
		{
			if (TryGetDefinitionSet(statDefId, out var def))
			{
				for (index = 0; index < def.Stats.Length; index++)
				{
					if (def.Stats[index].StatId == statId)
					{
						return true;
					}
				}
			}

			index = -1;
			return false;
		}

		public bool TryGetDefinitionSet(String8 setId, out StatDefinitionSet set)
		{
			for (int i = 0; i < _statDefinitionSets.Count; i++)
			{
				set = _statDefinitionSets[i];
				if (set.SetId == setId)
				{
					return true;
				}
			}

			set = default(StatDefinitionSet);
			return false;
		}

		public bool TryGetDefinition(AsciiId4 statId, out StatDefinition def)
		{
			for (int i = 0; i < _statDefinitions.Count; i++)
			{
				def = _statDefinitions[i];
				if (def.StatId == statId)
				{
					return true;
				}
			}

			def = default(StatDefinition);
			return false;
		}

		public void DefineStat(string name, AsciiId4 id, int min, int max, DiceTerm propability)
		{
			DefineStat(name, id, min, max, -1, propability);
		}

		public void DefineStat(string name, AsciiId4 id, int min, int max)
		{
			DefineStat(name, id, min, max, (max - min) / 2 + min, new DiceTerm(0, 0, 0));
		}
		public void DefineStat(string name, AsciiId4 id, int min, int max, int defaultValue)
		{
			DefineStat(name, id, min, max, defaultValue, new DiceTerm(0, 0, 0));
		}

		public void DefineStat(string name, AsciiId4 id, int min, int max, int defaultValue, DiceTerm probability)
		{
			_statDefinitions.Add(new StatDefinition { 
				StatId = id, 
				Minimum = min, 
				Maximum = max, 
				DisplayName = name, 
				Default = defaultValue,
				Probability = probability
			});
		}

		public void DefineStatSet(String8 id, AsciiId4[] statIds)
		{
			var def = new StatDefinitionSet { SetId = id, Stats = new StatDefinition[statIds.Length] };
			_statDefinitionSets.Add(def);

			for (int i = 0; i < statIds.Length; i++)
			{
				if (TryGetDefinition(statIds[i], out var stat))
				{
					def.Stats[i] = stat;
				}
				else
				{
					throw new Exception($"Stat {statIds[i]} not found");
				}
			}
		}
	}
}
