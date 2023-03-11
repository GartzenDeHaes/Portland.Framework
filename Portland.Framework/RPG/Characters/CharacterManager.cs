using System;
using System.Collections.Generic;

using Portland.AI;
using Portland.AI.Utility;
using Portland.Interp;
using Portland.Text;
using Portland.Types;

namespace Portland.RPG
{
	public sealed class CharacterManager : ICharacterManager
	{
		readonly IPropertyManager _props;
		Dictionary<String, Effect> _effectsById = new Dictionary<String, Effect>();
		Dictionary<String, EffectGroup> _effectGroupByName = new Dictionary<String, EffectGroup>();
		Dictionary<String, CharacterDefinition> _charDefs = new Dictionary<String, CharacterDefinition>();

		readonly ItemFactory _items;
		//StringTable _strings;

		public CharacterManager(IPropertyManager propMan, ItemFactory items)
		{
			//_strings = strings;
			_props = propMan;
			_items = items;

			_effectGroupByName.Add(String.Empty, new EffectGroup { Effects = Array.Empty<Effect>() });
			_props.DefinePropertySet(String.Empty, Array.Empty<String>(), String.Empty);
		}

		public bool HasStatDefined(in string statName)
		{
			return _props.HasPropertyDefined(statName);
		}

		public void DefineAlertForStatDefinition(in String propertyId, PropertyDefinition.AlertType type, in Variant8 value, string flagName)
		{
			if (_props.TryGetDefinition(propertyId, out var def))
			{
				def.DefineAlert(propertyId, type, value, flagName);
			}
			else
			{
				throw new Exception($"DefineAlertForPropertyDefinition: {propertyId} not found");
			}
		}

		public CharacterManagerBuilder GetBuilder()
		{
			return new CharacterManagerBuilder(this, _props, _items);
		}

		#region Characters

		public CharacterSheet CreateCharacter
		(
			in String charId,
			in String raceEffectGroup,
			in String classEffectGroup,
			in String factionEffectGroup,
			UtilitySet utilityProperties,
			ExecutionContext basCtx
		)
		{
			var def = _charDefs[charId];

			CharacterSheet chr = new CharacterSheet
			(
				def,
				_props.CreatePropertySet(def.PropertyGroupId, utilityProperties),
				basCtx,
				_effectGroupByName[raceEffectGroup],
				_effectGroupByName[classEffectGroup],
				_effectGroupByName[factionEffectGroup]
			);

			CreateDefaultItems(def, raceEffectGroup, chr);
			CreateDefaultItems(def, classEffectGroup, chr);

			return chr;
		}

		void CreateDefaultItems(CharacterDefinition def, in String raceOrClass, CharacterSheet chr)
		{
			if (def.DefaultItems.TryGetValue(raceOrClass, out var items))
			{
				for (int i = 0; i < items.Count; i++)
				{
					var itemdef = items[i];
					var item = _items.CreateItem(0, itemdef.ItemId, itemdef.Count);

					if (!chr.InventoryWindow.TrySetSectionItem(itemdef.WindowSectionName, itemdef.WindowSectionIndex, item))
					{
						throw new Exception($"Window section {itemdef.WindowSectionName} not found, or invalid window index {itemdef.WindowSectionIndex}");
					}
				}
			}
		}

		public CharacterDefinitionBuilder CreateCharacterDefinition(in String charId)
		{
			CharacterDefinition def = new CharacterDefinition() { CharId = charId };
			_charDefs.Add(charId, def);

			return new CharacterDefinitionBuilder(def, _props, _effectGroupByName);
		}

		public bool HasCharacterDefinition(in String charId)
		{
			return _charDefs.ContainsKey(charId);
		}

		#endregion

		#region Effects

		public bool HasEffect(in String name)
		{
			//int id = StringHelper.HashMurmur32(name);
			//var id = String8.FromTruncate(name);

			return _effectsById.ContainsKey(name);
		}

		void DefineEffect(in String name, in String appliesToPropId, in Variant8 value, float duration, EffectValueType isNumOrPct, PropertyRequirement[] requirements)
		{
			//int id = StringHelper.HashMurmur32(name);
			//var id = String8.FromTruncate(name);

			_effectsById.Add
			(
				name,
				new Effect { EffectId = name, EffectName = name, PropertyId = appliesToPropId, Value = value, Duration = duration, Op = isNumOrPct/*, Requirements = requirements*/ }
			);
		}

		//public void DefineEffect(string name, String8 appliesToPropId, in Variant8 value, EffectValueType isNumOrPct, PropertyRequirement[] requirements)
		//{
		//	DefineEffect(name, appliesToPropId, value, 0f, isNumOrPct, requirements);
		//}

		/// <summary>
		/// Can be used for STAT's and derived stats
		/// </summary>
		public void DefineEffect_RangeDelta(string name, in String appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.MaxDelta, Array.Empty<PropertyRequirement>());
		}

		/// <summary>
		/// Can be used for STAT's and derived stats
		/// </summary>
		public void DefineEffect_RangeMax(string name, in String appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.MaxDelta, Array.Empty<PropertyRequirement>());
		}

		/// <summary>
		/// Only use for STAT's, not derived values
		/// </summary>
		public void DefineStatEffect_Set(string name, in String appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.CurrentAbs, Array.Empty<PropertyRequirement>());
		}

		/// <summary>
		/// Only use for STAT's, not derived values
		/// </summary>
		public void DefineStatEffect_Delta(string name, in String appliesToPropId, in Variant8 value)
		{
			DefineEffect(name, appliesToPropId, value, 0f, EffectValueType.CurrentDelta, Array.Empty<PropertyRequirement>());
		}

		//public void DefineEffect(string name, String8 appliesToPropId, in Variant8 value, float duration, EffectValueType isNumOrPct)
		//{
		//	DefineEffect(name, appliesToPropId, value, duration, isNumOrPct, Array.Empty<PropertyRequirement>());
		//}

		public void DefineEffectGroup(in String groupName, String[] effectNames)
		{
			List<Effect> effects = new List<Effect>(effectNames.Length);

			for (int i = 0; i < effectNames.Length; i++)
			{
				effects.Add(_effectsById[effectNames[i]]);
			}

			_effectGroupByName.Add(groupName, new EffectGroup { EffectGroupId = groupName, Effects = effects.ToArray()/*, Requirements = requirements*/ });
		}

		#endregion

		public void Parse(XmlLex lex)
		{
			if (!lex.Lexum.IsEqualTo("character_types"))
			{
				return;
			}

			lex.MatchTag("character_types");

			while (lex.Lexum.IsEqualTo("character_def"))
			{
				lex.MatchTagStart("character_def");

				var builder = CreateCharacterDefinition(lex.MatchProperty("char_id"))
					.PropertyGroupId(lex.MatchProperty("property_set"))
					.AutoCountInventory(true);
				lex.MatchTagClose();

				while (lex.Token != XmlLex.XmlLexToken.CLOSE && !lex.IsEOF)
				{
					if (lex.Lexum.IsEqualTo("inventory"))
					{
						ParseInventory(lex, builder);
					}
					else if (lex.Lexum.IsEqualTo("items"))
					{
						ParseItems(lex, builder);
					}
					else
					{
						throw new Exception($"Unknown section characters/character_def/{lex.Lexum} on line {lex.LineNum}");
					}
				}

				lex.MatchTagClose("character_def");
			}

			lex.MatchTagClose("character_types");
		}

		void ParseInventory(XmlLex lex, CharacterDefinitionBuilder builder)
		{
			lex.MatchTag("inventory");

			while (lex.Lexum.IsEqualTo("section"))
			{
				lex.MatchTagStart("section");

				string name = String.Empty;
				int sectionTypeId = 0;
				bool isReadOnly = false;
				int width = 1;
				int height = 1;

				while (lex.Token != XmlLex.XmlLexToken.CLOSE && !lex.IsEOF)
				{
					if (lex.Lexum.IsEqualTo("name"))
					{
						name = lex.MatchProperty("name");
					}
					else if (lex.Lexum.IsEqualTo("type_id"))
					{
						sectionTypeId = Int32.Parse(lex.MatchProperty("type_id"));
					}
					else if (lex.Lexum.IsEqualTo("width"))
					{
						width = Int32.Parse(lex.MatchProperty("width"));
					}
					else if (lex.Lexum.IsEqualTo("height"))
					{
						height = Int32.Parse(lex.MatchProperty("height"));
					}
					else if (lex.Lexum.IsEqualTo("readonly"))
					{
						isReadOnly = Boolean.Parse(lex.MatchProperty("readonly"));
					}
					else
					{
						throw new Exception($"Unknown inventory window property {lex.Lexum} on line {lex.LineNum}");
					}
				}

				builder.AddInventorySection(name, sectionTypeId, isReadOnly, width, height);

				lex.MatchTagClose();
			}

			lex.MatchTagClose("inventory");
		}

		void ParseItems(XmlLex lex, CharacterDefinitionBuilder builder)
		{
			lex.MatchTag("items");

			while (lex.Lexum.IsEqualTo("default_item"))
			{
				lex.MatchTagStart("default_item");

				string raceOrClass = String.Empty;
				string itemId = String.Empty;
				int count = 1;
				string windowSectionName = String.Empty;
				int windowSectionIndex = 0;

				while (lex.Token != XmlLex.XmlLexToken.TAG_END && !lex.IsEOF)
				{
					if (lex.Lexum.IsEqualTo("race_or_class"))
					{
						raceOrClass = lex.MatchProperty("race_or_class");
					}
					else if (lex.Lexum.IsEqualTo("item_id"))
					{
						itemId = lex.MatchProperty("item_id");
					}
					else if (lex.Lexum.IsEqualTo("count"))
					{
						count = Int32.Parse(lex.MatchProperty("count"));
					}
					else if (lex.Lexum.IsEqualTo("window_section"))
					{
						windowSectionName = lex.MatchProperty("window_section");
					}
					else if (lex.Lexum.IsEqualTo("window_index"))
					{
						windowSectionIndex = Int32.Parse(lex.MatchProperty("window_index"));
					}
					else
					{
						throw new Exception($"Unknown default_item property {lex.Lexum} on line {lex.LineNum}");
					}
				}

				lex.MatchTagEnd();

				List<ItemPropertyDefault> props = new List<ItemPropertyDefault>();

				while (lex.Lexum.IsEqualTo("property"))
				{
					lex.MatchTagStart("property");

					props.Add(new ItemPropertyDefault { PropId = lex.MatchProperty("property_id"), Default = Variant8.Parse(lex.MatchProperty("default")) });

					lex.MatchTagEnd();
				}

				builder.AddDefaultItem(raceOrClass, new DefaultItemSpec
				{
					ItemId = itemId,
					Count = count,
					WindowSectionName = windowSectionName,
					WindowSectionIndex = windowSectionIndex,
					Properties = props.ToArray()
				});

				lex.MatchTagClose("default_item");
			}

			lex.MatchTagClose("items");
		}
	}
}
