using System;

using Portland.AI;
using Portland.AI.Utility;
using Portland.Text;

namespace Portland.Types
{
	public interface IPropertyManager
	{
		PropertySet CreatePropertySet(in string setId, UtilitySet utilityProps);
		void DefinePropertySet(in string setName, string[] propIds, string onUpdateScript);
		bool TryGetDefinitionSet(in string setId, out PropertyDefinitionSet def);
		bool HasPropertyDefined(in string statName);
		void ParseDefinitionSets(string xml);
		void ParseDefinitionSets(XmlLex lex);
		void LoadPropertyDefinitions(string xml);
		void ParsePropertyDefinitions(XmlLex lex);
		bool TryGetDefinition(in string id, out PropertyDefinition def);

		IBlackboard<string> GetGlobalProperties();

		PropertyDefinitionBuilder DefineProperty(in String id, string name, in string category, in bool isGlobal = false);

		/// <summary>
		/// 0 to 100 decreasing, such as satiation, health, hydration, sleepyness
		/// </summary>
		PropertyDefinitionBuilder DefineProperty_0to100_Descreasing(in String propId, bool isGlobal = false);

		/// <summary>
		/// 0 to 100 increasing, such as hunger, thirst, tiredness
		/// </summary>
		PropertyDefinitionBuilder DefineProperty_0to100_Increasing(in String propId, bool isGlobal = false);

		/// <summary>
		/// 0+ such as money, gold, XP
		/// </summary>
		PropertyDefinitionBuilder DefineProperty_Positive_Unbounded(in String propId, bool isGlobal = false);

		/// <summary>
		/// 24 hour clock, so 0000 to 2399. First two digits are hour, second two are 1/100 hour (0.6 minutes)
		/// </summary>
		PropertyDefinitionBuilder DefineProperty_Time_Military(in String propId, bool isGlobal = false);

		/// <summary>
		/// 0 to 23 increasing
		/// </summary>
		PropertyDefinitionBuilder DefineProperty_HourOfDay(in String propId, bool isGlobal = false);

		/// <summary>
		/// 0 to 1 increasing
		/// </summary>
		PropertyDefinitionBuilder DefineProperty_Time_Normalized(in String propId, bool isGlobal = false);
	}
}
