using System;
using System.Collections.Generic;


namespace Portland.RPG
{
	public interface IFactionManager
	{
		//FactionRelation Check(SemanticTag observer, SemanticTag target);

		public void DefineFaction(string factionId, string desc, string alignment, string grouping);

		void SetFactionRelation(string factionName, string otherFaction, float value);
	}
}
