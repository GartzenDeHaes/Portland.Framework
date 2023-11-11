using System;

using Portland.AI;
using Portland.AI.Utility;
using Portland.Interp;
using Portland.Text;
using Portland.Types;

namespace Portland.RPG
{
	public interface ICharacterManager
	{
		public CharacterSheet CreateCharacter
		(
			in String10 uniqueCharName,
			in String charTypeId,
			in String raceEffectGroup,
			in String classEffectGroup,
			in String factionEffectGroup
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