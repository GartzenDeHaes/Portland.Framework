using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Text;

namespace Portland.RPG
{
	public sealed class CharacterManager
	{
		StatFactory _stats;
		PropertyManager _props;
		Dictionary<int, Effect> _effectsById = new Dictionary<int, Effect>();
		Dictionary<StringTableToken, Effect[]> _effectGroupByName = new Dictionary<StringTableToken, Effect[]>();
		StringTable _strings;

		public CharacterManager(StringTable strings, StatFactory stats, PropertyManager props)
		{
			_strings = strings;
			_stats = stats;
			_props = props;
		}

		public Character CreateCharacter(String8 statsSetId, String8 skillsStatSetId, String8 propertySetId)
		{
			// need definition for inventory layout
			return null;
		}

		public void DefineEffect(string name, EffectType appliesTo, in Variant8 value, float duration, EffectValueType isNumOrPct, Requirement[] requirements)
		{
			int id = StringHelper.HashMurmur32(name);

			_effectsById.Add
			(
				id,
				new Effect { EffectId = id, EffectName = name, AppliesTo = appliesTo, Value = value, Duration = duration, IsNumOrPct = isNumOrPct, Requirements = requirements }
			);
		}

		public void DefineEffect(string name, EffectType appliesTo, in Variant8 value, EffectValueType isNumOrPct, Requirement[] requirements)
		{
			DefineEffect(name, appliesTo, value, 0f, isNumOrPct, requirements);
		}

		public void DefineEffect(string name, EffectType appliesTo, in Variant8 value, EffectValueType isNumOrPct)
		{
			DefineEffect(name, appliesTo, value, 0f, isNumOrPct, Array.Empty<Requirement>());
		}

		public void DefineEffect(string name, EffectType appliesTo, in Variant8 value, float duration, EffectValueType isNumOrPct)
		{
			DefineEffect(name, appliesTo, value, duration, isNumOrPct, Array.Empty<Requirement>());
		}

		public void DefineEffectGroup(string groupName, string[] effectNames)
		{
			List<Effect> effects = new List<Effect>(effectNames.Length);

			for (int i = 0; i < effectNames.Length; i++)
			{
				effects.Add(_effectsById[StringHelper.HashMurmur32(effectNames[i])]);
			}

			_effectGroupByName.Add(_strings.Get(groupName), effects.ToArray());
		}
	}
}
