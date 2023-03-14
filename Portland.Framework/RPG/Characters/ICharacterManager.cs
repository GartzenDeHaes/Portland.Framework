﻿using Portland.AI;
using Portland.AI.Utility;
using Portland.Interp;
using Portland.Text;
using Portland.Types;

namespace Portland.RPG
{
	public interface ICharacterManager
	{
		CharacterSheet CreateCharacter
		(
			in string charId, 
			in string raceEffectGroup, 
			in string classEffectGroup, 
			in string factionEffectGroup, 
			UtilitySet utilityProperties, 
			IBlackboard<string> facts,
			ExecutionContext basCtx
		);
		CharacterDefinitionBuilder CreateCharacterDefinition(in string charId);
		void DefineAlertForStatDefinition(in string propertyId, PropertyDefinition.AlertType type, in Variant8 value, string flagName);
		void DefineEffectGroup(in string groupName, string[] effectNames);
		void DefineEffect_RangeDelta(string name, in string appliesToPropId, in Variant8 value);
		void DefineEffect_RangeMax(string name, in string appliesToPropId, in Variant8 value);
		void DefineStatEffect_Delta(string name, in string appliesToPropId, in Variant8 value);
		void DefineStatEffect_Set(string name, in string appliesToPropId, in Variant8 value);
		CharacterManagerBuilder GetBuilder();
		bool HasCharacterDefinition(in string charId);
		CharacterDefinition GetCharacterDefinition(in string charId);
		bool HasEffect(in string name);
		bool HasStatDefined(in string statName);

		void Parse(XmlLex lex);
	}
}