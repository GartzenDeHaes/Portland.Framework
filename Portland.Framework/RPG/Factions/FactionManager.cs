using System;
using System.Collections.Generic;

using Portland.Mathmatics;

namespace Portland.RPG
{
	public class FactionManager : IFactionManager
	{
		Dictionary<string, Dictionary<string, float>> _factionMatrix = new Dictionary<string, Dictionary<string, float>>();

		public FactionManager() 
		{ 
		}

		public void DefineFaction(string factionId, string desc, string alignment, string grouping)
		{
			if (! _factionMatrix.TryGetValue(factionId, out var dict))
			{
				_factionMatrix.Add(factionId, new Dictionary<string, float>());
			}
		}

		public void SetFactionRelation(string factionName, string otherFaction, float value)
		{
			value = MathHelper.Clamp(value, -1f, 1f);

			var dict = _factionMatrix[factionName];
			if (dict.ContainsKey(otherFaction))
			{
				dict[otherFaction] = value;
			}
			else
			{
				dict.Add(otherFaction, value);
			}
		}
	}
}
