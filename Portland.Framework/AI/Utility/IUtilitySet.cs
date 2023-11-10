using System.Collections.Generic;

using Portland.Types;

namespace Portland.AI.Utility
{
	public interface IUtilitySet
	{
		PropertyValue this[in string propertyName] { get; set; }
		PropertyValue this[string propertyName] { get; set; }

		PropertyValue CurrentObjective { get; }
		string Name { get; }
		ObjectiveScore[] Objectives { get; }
		Dictionary<string, PropertyValue> Properties { get; }

		void AddProperty(PropertyDefinition def);
		void AddProperty(PropertyValue property);
		bool HasProperty(in string propertyName);
		bool TryGetProperty(in string propertyName, out PropertyValue value);
		void Update(float deltaTime);
	}
}