using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Interp;
using Portland.Text;

namespace Portland.RPG
{
	public sealed class CharacterManager
	{
		PropertyManager _props;
		Dictionary<String8, Effect> _effectsById = new Dictionary<String8, Effect>();
		Dictionary<String8, EffectGroup> _effectGroupByName = new Dictionary<String8, EffectGroup>();
		Dictionary<String8, CharacterDefinition> _charDefs = new Dictionary<String8, CharacterDefinition>();

		ItemFactory _items;
		StringTable _strings;

		public CharacterManager(StringTable strings, PropertyManager props, ItemFactory items)
		{
			_strings = strings;
			_props = props;
			_items = items;

			_effectGroupByName.Add(String8.Empty, new EffectGroup { Effects = Array.Empty<Effect>() });
		}

		public CharacterSheet CreateCharacter(in String8 charId, in String8 raceEffectGroup, in String8 classEffectGroup, in String8 factionEffectGroup)
		{
			var def = _charDefs[charId];

			CharacterSheet chr = new CharacterSheet
			(
				def, 
				_props.CreateSetInstance(def.PropertyGroupId), 
				_effectGroupByName[raceEffectGroup], 
				_effectGroupByName[classEffectGroup], 
				_effectGroupByName[factionEffectGroup]
			);

			CreateDefaultItems(def, raceEffectGroup, chr);
			CreateDefaultItems(def, classEffectGroup, chr);

			return chr;
		}

		void CreateDefaultItems(CharacterDefinition def, in String8 raceOrClass, CharacterSheet chr)
		{
			if (def.DefaultItems.TryGetValue(raceOrClass, out var items))
			{
				for (int i = 0; i < items.Count; i++)
				{
					var itemdef = items[i];
					var item = _items.CreateItem(0, itemdef.ItemId, itemdef.Count);

					if (! chr.InventoryWindow.TrySetSectionItem(itemdef.WindowSectionName, itemdef.WindowSectionIndex, item))
					{
						throw new Exception($"Window section {itemdef.WindowSectionName} not found, or invalid window index {itemdef.WindowSectionIndex}");
					}
				}
			}
		}

		public CharacterDefinitionBuilder CreateCharacterDefinition(in String8 id)
		{
			CharacterDefinition def = new CharacterDefinition() { CharId = id };
			_charDefs.Add(id, def);

			return new CharacterDefinitionBuilder(def, _effectGroupByName);
		}

		#region Effects

		public bool HasEffect(string name)
		{
			//int id = StringHelper.HashMurmur32(name);
			var id = String8.FromTruncate(name);

			return _effectsById.ContainsKey(id);
		}

		void DefineEffect(string name, String8 appliesToPropId, in Variant8 value, float duration, EffectValueType isNumOrPct, PropertyRequirement[] requirements)
		{
			//int id = StringHelper.HashMurmur32(name);
			var id = String8.FromTruncate(name);

			_effectsById.Add
			(
				id,
				new Effect { EffectId = id, EffectName = name, PropertyId = appliesToPropId, Value = value, Duration = duration, Op = isNumOrPct/*, Requirements = requirements*/ }
			);
		}

		//public void DefineEffect(string name, String8 appliesToPropId, in Variant8 value, EffectValueType isNumOrPct, PropertyRequirement[] requirements)
		//{
		//	DefineEffect(name, appliesToPropId, value, 0f, isNumOrPct, requirements);
		//}

		/// <summary>
		/// Can be used for STAT's and derived stats
		/// </summary>
		public void DefineEffect_RangeDelta(string name, String8 appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.MaxDelta, Array.Empty<PropertyRequirement>());
		}

		/// <summary>
		/// Can be used for STAT's and derived stats
		/// </summary>
		public void DefineEffect_RangeMax(string name, String8 appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.MaxDelta, Array.Empty<PropertyRequirement>());
		}

		/// <summary>
		/// Only use for STAT's, not derived values
		/// </summary>
		public void DefineStatEffect_Set(string name, String8 appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.CurrentAbs, Array.Empty<PropertyRequirement>());
		}

		/// <summary>
		/// Only use for STAT's, not derived values
		/// </summary>
		public void DefineStatEffect_Delta(string name, String8 appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.CurrentDelta, Array.Empty<PropertyRequirement>());
		}

		//public void DefineEffect(string name, String8 appliesToPropId, in Variant8 value, float duration, EffectValueType isNumOrPct)
		//{
		//	DefineEffect(name, appliesToPropId, value, duration, isNumOrPct, Array.Empty<PropertyRequirement>());
		//}

		public void DefineEffectGroup(in String8 groupName, String8[] effectNames)
		{
			List<Effect> effects = new List<Effect>(effectNames.Length);

			for (int i = 0; i < effectNames.Length; i++)
			{
				effects.Add(_effectsById[effectNames[i]]);
			}

			_effectGroupByName.Add(groupName, new EffectGroup { EffectGroupId = groupName, Effects = effects.ToArray()/*, Requirements = requirements*/ });
		}

		#endregion
	}
}
