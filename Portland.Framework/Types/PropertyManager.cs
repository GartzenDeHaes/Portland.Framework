using System;
using System.Collections.Generic;
using System.Diagnostics;

using Portland.AI;
using Portland.AI.Utility;
using Portland.Basic;
using Portland.Collections;
using Portland.Mathmatics;
using Portland.Text;

using static Portland.Types.PropertyDefinition;

namespace Portland.Types
{
	/// <summary>
	/// </summary>
	public sealed class PropertyManager : IPropertyManager
	{
		Dictionary<string, PropertyDefinition> _definitions = new Dictionary<string, PropertyDefinition>();
		Vector<PropertyDefinitionSet> _defSets = new Vector<PropertyDefinitionSet>();
		Blackboard<string> _globalProperties;

		public bool HasPropertyDefined(in string statName)
		{
			return _definitions.ContainsKey(statName);
		}

		public bool TryGetDefinition(in string id, out PropertyDefinition def)
		{
			if (_definitions.TryGetValue(id, out def))
			{
				return true;
			}

			return false;
		}

		public void DefinePropertySet(in string setName, string[] propIds, string onUpdateScript)
		{
			var set = new PropertyDefinitionSet { 
				SetId = setName, 
				Properties = new PropertyDefinition[propIds.Length],
				OnLevelScript = onUpdateScript
			};

			for (int i = 0; i < propIds.Length; i++)
			{
				if (TryGetDefinition(propIds[i], out var def))
				{
					set.Properties[i] = def;
				}
				else
				{
					throw new Exception($"Property definition {propIds[i]} not found");
				}
			}

			_defSets.Add(set);
		}

		public bool TryGetDefinitionSet(in string setId, out PropertyDefinitionSet setDef)
		{
			for (int i = 0; i < _defSets.Count; i++)
			{
				if (_defSets[i].SetId == setId)
				{
					setDef = _defSets[i];
					return true;
				}
			}

			setDef = default;
			return false;
		}

		PropertyValue[] CreatePropertySetValues(in string setId, UtilitySet utilityProps)
		{
			if (!TryGetDefinitionSet(setId, out var setDef))
			{
				throw new Exception($"Definition for set {setId} not found");
			}

			var values = new PropertyValue[setDef.Properties.Length];
			//= new PropertySetKeys { SetId = setId, Properties = new PropertyInstanceKey[setDef.Properties.Length] };
			PropertyDefinition def;

			for (int i = 0; i < setDef.Properties.Length; i++)
			{
				def = setDef.Properties[i];

				if (def.GetFromUtility)
				{
					if (utilityProps.TryGetProperty(def.PropertyId, out var uval))
					{
						values[i] = uval;
					}
					else
					{
						uval = new PropertyValue(def);
						values[i] = uval;
						utilityProps.AddProperty(uval);
					}
				}
				else
				{
					values[i] = new PropertyValue(def);
				}
			}

			return values;
		}

		public PropertySet CreatePropertySet(in string setId, UtilitySet utilityProps)
		{
			if (!TryGetDefinitionSet(setId, out var setDef))
			{
				throw new Exception($"Definition for set {setId} not found");
			}
			return new PropertySet(setDef, CreatePropertySetValues(setId, utilityProps));
		}

		public IBlackboard<string> GetGlobalProperties()
		{
			return _globalProperties;
		}

		public PropertyManager()
		{
			_globalProperties = new Blackboard<string>();
		}

		public PropertyDefinitionBuilder DefineProperty(in String id, string name, in string category, in bool isGlobal = false)
		{
			var def = new PropertyDefinition { PropertyId = id, Category = category, DisplayName = name, Minimum = 0, Maximum = 100, IsGlobalValue = isGlobal };

			_definitions.Add(def.PropertyId, def);

			if (def.IsGlobalValue && !_globalProperties.ContainsKey(id))
			{
				_globalProperties.Add(id, new PropertyValue(def));
			}

			return new PropertyDefinitionBuilder { Property = def, GlobalProperties = _globalProperties };
		}

		/// <summary>
		/// 0 to 100 decreasing, such as satiation, health, hydration, sleepyness
		/// </summary>
		public PropertyDefinitionBuilder DefineProperty_0to100_Descreasing(in String propId, bool isGlobal = false)
		{
			return DefineProperty(propId, propId, String.Empty, isGlobal)
				.Minimum(0f)
				.Maximum(100f)
				.ChangePerSecond(-(20f / 60f) / 60f)
				.SetDefault(100f)
				.TypeName("float") // doesn't seem to be used
			;
		}

		/// <summary>
		/// 0 to 100 increasing, such as hunger, thirst, tiredness
		/// </summary>
		public PropertyDefinitionBuilder DefineProperty_0to100_Increasing(in String propId, bool isGlobal = false)
		{
			return DefineProperty_0to100_Descreasing(propId, isGlobal)
				.ChangePerSecond((20f / 60f) / 60f)
				.SetDefault(0)
				.TypeName("float")
			;
		}

		/// <summary>
		/// 0+ such as money, gold, XP
		/// </summary>
		public PropertyDefinitionBuilder DefineProperty_Positive_Unbounded(in String propId, bool isGlobal = false)
		{
			return DefineProperty(propId, propId, String.Empty, isGlobal)
				.Minimum(0f)
				.ChangePerSecond(0f)
				.SetDefault(0)
				.TypeName("float")
			;
		}

		/// <summary>
		/// 24 hour clock, so 0000 to 2399. First two digits are hour, second two are 1/100 hour (0.6 minutes)
		/// </summary>
		public PropertyDefinitionBuilder DefineProperty_Time_Military(in String propId, bool isGlobal = false)
		{
			return DefineProperty(propId, propId, String.Empty, isGlobal)
				.Minimum(0f)
				.Maximum(2399)
				.ChangePerSecond(0.6f / 60f)
				.SetDefault(0800)
				.TypeName("float")
			;
		}

		/// <summary>
		/// 0 to 23 increasing
		/// </summary>
		public PropertyDefinitionBuilder DefineProperty_HourOfDay(in String propId, bool isGlobal = false)
		{
			return DefineProperty_0to100_Increasing(propId, isGlobal)
				.Minimum(0)
				.Maximum(23)
				//.ChangePerSecond((1f / 60f) / 60f)
				.ChangePerSecond(0f)
				.SetDefault(8)
			;
		}

		/// <summary>
		/// 0 to 1 increasing
		/// </summary>
		public PropertyDefinitionBuilder DefineProperty_Time_Normalized(in String propId, bool isGlobal = false)
		{
			return DefineProperty_0to100_Increasing(propId, isGlobal)
				.Minimum(0)
				.Maximum(1)
				//.ChangePerSecond((1f * _clock.SecondsPerHour / 1440f) / 1440f)
				.ChangePerSecond(0f)
				.SetDefault(0.5f)
			;
		}

		public void ParseDefinitionSets(string xml)
		{
			ParseDefinitionSets(new XmlLex(xml));
		}

		public void ParseDefinitionSets(XmlLex lex)
		{
			lex.MatchTag("property_sets");

			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("set");

				ParseSet(lex);
			}
			lex.MatchTagClose("property_sets");
		}

		void ParseSet(XmlLex lex)
		{
			string setId = lex.MatchProperty("id");
			List<PropertyDefinition> defs = new List<PropertyDefinition>();

			while (!lex.IsEOF && lex.Token != XmlLex.XmlLexToken.TAG_END && lex.Token != XmlLex.XmlLexToken.CLOSE)
			{
				string propId = lex.Lexum.ToString();
				lex.Next();

				if (_definitions.TryGetValue(propId, out var def))
				{
					defs.Add(def);
				}
				else
				{
					throw new ArgumentException("Unknown property definition " + propId);
				}
			}

			var set = new PropertyDefinitionSet { SetId = setId, Properties = defs.ToArray() };
			_defSets.Add(set);

			if (lex.Token == XmlLex.XmlLexToken.TAG_END)
			{
				lex.Match(XmlLex.XmlLexToken.TAG_END);
				return;
			}

			lex.Match(XmlLex.XmlLexToken.CLOSE);

			while (lex.Lexum.IsEqualTo("script"))
			{
				lex.MatchTagStart("script");
				string onEvent = lex.MatchProperty("event");
				lex.MatchTagClose();

				lex.NextText();

				if (onEvent.Equals("on_inventory"))
				{
					set.OnInventoryScript = lex.Lexum.ToString();
				}
				else if (onEvent.Equals("on_effect"))
				{
					set.OnEffectScript = lex.Lexum.ToString();
				}
				else if (onEvent.Equals("on_level"))
				{
					set.OnLevelScript = lex.Lexum.ToString();

				}
				lex.Next();

				lex.MatchTagClose("script");
			}
			lex.MatchTagClose("set");
		}

		public void LoadPropertyDefinitions(string xml)
		{
			ParsePropertyDefinitions(new XmlLex(xml));
		}

		public void ParsePropertyDefinitions(XmlLex lex)
		{
			lex.MatchTag("properties");
			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("property");

				ParseProperty(lex);
			}
			lex.MatchTagClose("properties");
		}

		private void ParseProperty(XmlLex lex)
		{
			var prop = new PropertyDefinition();

			while (!lex.IsEOF && lex.Token != XmlLex.XmlLexToken.TAG_END && lex.Token != XmlLex.XmlLexToken.CLOSE)
			{
				var lexum = lex.Lexum.ToString();
				var val = lex.MatchProperty(lexum);

				switch (lexum)
				{
					case "name":
						prop.PropertyId = val; break;
					case "type":
						prop.TypeName = val; break;
					case "desc":
						prop.DisplayName = val; break;
					case "category":
						prop.Category = val; break;
					case "global":
						prop.IsGlobalValue = Boolean.Parse(val); break;
					case "min":
						prop.Minimum = Single.Parse(val); break;
					case "max":
						prop.Maximum = Single.Parse(val); break;
					case "start":
						prop.DefaultValue = Single.Parse(val); break;
					case "start_rand":
						prop.DefaultRandomize = Boolean.Parse(val); break;
					case "change_per_hour":
						prop.ChangePerSec = Single.Parse(val) / 60f / 60f; break;
					case "change_per_sec":
						prop.ChangePerSec = Single.Parse(val); break;
					case "from_utility":
						prop.GetFromUtility = Boolean.Parse(val); break;
					case "prob":
						prop.Probability = DiceTerm.Parse(val); break;
					default:
						throw new ArgumentException("Unknown property " + lexum);
				}
			}

			_definitions.Add(prop.PropertyId, prop);

			if (lex.Token == XmlLex.XmlLexToken.CLOSE)
			{
				lex.Match(XmlLex.XmlLexToken.CLOSE);

				if (lex.Token != XmlLex.XmlLexToken.CLOSE)
				{
					ParseAlerts(lex, prop);
				}
				lex.MatchTagClose("property");
			}
			else
			{
				lex.Match(XmlLex.XmlLexToken.TAG_END);
			}

			if (prop.IsGlobalValue)
			{
				if (_globalProperties.ContainsKey(prop.PropertyId))
				{
					throw new Exception($"Global property {prop.PropertyId} already defined.");
				}
				_globalProperties.Add(prop.PropertyId, new PropertyValue(prop));
			}
		}

		void ParseAlerts(XmlLex lex, PropertyDefinition prop)
		{
			lex.MatchTagStart("alert");

			AlertDefinition alert = new AlertDefinition { PropertId = prop.PropertyId };

			while (!lex.IsEOF && lex.Token != XmlLex.XmlLexToken.TAG_END && lex.Token != XmlLex.XmlLexToken.CLOSE)
			{
				var lexum = lex.Lexum.ToString();
				var val = lex.MatchProperty(lexum);

				switch (lexum)
				{
					case "type":
						alert.Type = (AlertType)Enum.Parse(typeof(AlertType), val, true); break;
					case "value":
						alert.Value = Variant8.Parse(val); break;
					case "flag":
						alert.FlagName = val; break;
					default:
						throw new ArgumentException("Unknown property " + lexum);
				}
			}

			prop.Alerts.Add(alert);

			lex.Match(XmlLex.XmlLexToken.TAG_END);
		}
	}
}
