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
		PropertyManager _props;
		Dictionary<String8, Effect> _effectsById = new Dictionary<String8, Effect>();
		Dictionary<String8, EffectGroup> _effectGroupByName = new Dictionary<String8, EffectGroup>();
		StringTable _strings;

		public CharacterManager(StringTable strings, PropertyManager props)
		{
			_strings = strings;
			_props = props;
		}

		public Character CreateCharacter(String8 statsSetId, String8 skillsStatSetId, String8 propertySetId)
		{
			// need definition for inventory layout
			return null;
		}

		#region Effects

		public bool HasEffect(string name)
		{
			//int id = StringHelper.HashMurmur32(name);
			var id = String8.FromTruncate(name);

			return _effectsById.ContainsKey(id);
		}

		public void DefineEffect(string name, AsciiId4 appliesToPropId, in Variant8 value, float duration, EffectValueType isNumOrPct, PropertyRequirement[] requirements)
		{
			//int id = StringHelper.HashMurmur32(name);
			var id = String8.FromTruncate(name);

			_effectsById.Add
			(
				id,
				new Effect { EffectId = id, EffectName = name, PropertyId = appliesToPropId, Value = value, Duration = duration, IsNumOrPct = isNumOrPct, Requirements = requirements }
			);
		}

		public void DefineEffect(string name, AsciiId4 appliesToPropId, in Variant8 value, EffectValueType isNumOrPct, PropertyRequirement[] requirements)
		{
			DefineEffect(name, appliesToPropId, value, 0f, isNumOrPct, requirements);
		}

		public void DefineEffect(string name, AsciiId4 appliesToPropId, in Variant8 value, EffectValueType isNumOrPct)
		{
			DefineEffect(name, appliesToPropId, value, 0f, isNumOrPct, Array.Empty<PropertyRequirement>());
		}

		public void DefineEffect(string name, AsciiId4 appliesToPropId, in Variant8 value, float duration, EffectValueType isNumOrPct)
		{
			DefineEffect(name, appliesToPropId, value, duration, isNumOrPct, Array.Empty<PropertyRequirement>());
		}

		public void DefineEffectGroup(in String8 groupName, String8[] effectNames, PropertyRequirement[] requirements)
		{
			List<Effect> effects = new List<Effect>(effectNames.Length);

			for (int i = 0; i < effectNames.Length; i++)
			{
				effects.Add(_effectsById[effectNames[i]]);
			}

			_effectGroupByName.Add(groupName, new EffectGroup { EffectGroupId = groupName, Effects = effects.ToArray(), Requirements = requirements });
		}

		#endregion
	}
}
