using System;
using System.Diagnostics;

using Portland.AI.Utility;
using Portland.Collections;
using Portland.Text;
using Portland.Types;

namespace Portland.RPG
{
	public struct PropertyDefinitionSet
	{
		public String SetId;
		public PropertyDefinition[] Properties;
	}  
	
	/// <summary>
	/// </summary>
	public sealed class PropertyManager
	{
		//Vector<float>[] _values;
		Vector<PropertyDefinition> _definitions;
		Vector<PropertyDefinitionSet> _defSets = new Vector<PropertyDefinitionSet>();
		//Vector<PropertySetInstance> _pool = new Vector<PropertySetInstance>();

		//public void Update(float deltaTimeInSeconds)
		//{
		//	float changePerSecond;
		//	float minimum;
		//	Vector<float> values;

		//	for (int i = 0; i < _definitions.Count; i++)
		//	{
		//		changePerSecond = _definitions[i].ChangePerSecond;
		//		minimum = _definitions[i].Minimum;

		//		if (changePerSecond != 0f)
		//		{
		//			values = _values[i];

		//			for (int j = 0; j < values.Count; j += 2)
		//			{
		//				ref float val = ref values.ElementAtRef(j);
		//				val += deltaTimeInSeconds * changePerSecond;
		//				if (val > values[j+1])
		//				{
		//					val = values[j+1];
		//				}
		//				else if (val < minimum)
		//				{
		//					val = minimum;
		//				}
		//			}
		//		}
		//	}
		//}

		public PropertyDefinitionBuilder DefineProperty(in string id, string name, in string category)
		{
			var def = new PropertyDefinition { PropertyId = id, Category = category, DisplayName = name, Minimum = 0, Maximum = 100 };

			for (int i = 0; i < _definitions.Count; i++)
			{
				if (_definitions[i] == null)
				{
					_definitions[i] = def;
					return new PropertyDefinitionBuilder { Property = def };
				}
			}

			_definitions.Add(def);
			return new PropertyDefinitionBuilder { Property = def };
		}

		bool TryGetDefinition(in String id, out PropertyDefinition def)
		{
			for (int i = 0; i < _definitions.Count; i++)
			{
				def = _definitions[i];
				if (def.PropertyId == id)
				{
					return true;
				}
			}

			def = null;
			return false;
		}

		public void DefinePropertySet(in String setName, String[] propIds)
		{
			var set = new PropertyDefinitionSet { SetId = setName, Properties = new PropertyDefinition[propIds.Length] };

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

			//if (_defSets.Count > _values.Length)
			//{
			//	throw new Exception($"Increase size of numProperties parameter to PropertyManager");
			//}
		}

		bool TryGetSetDef(in String setId, out PropertyDefinitionSet setDef)
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

		PropertyValue[] CreatePropertySetValues(in String setId)
		{
			if (!TryGetSetDef(setId, out var setDef))
			{
				throw new Exception($"Definition for set {setId} not found");
			}

			var values = new PropertyValue[setDef.Properties.Length];
			//= new PropertySetKeys { SetId = setId, Properties = new PropertyInstanceKey[setDef.Properties.Length] };
			PropertyDefinition def;

			for (int i = 0; i < setDef.Properties.Length; i++)
			{
				def = setDef.Properties[i];
				values[i] = new PropertyValue(def);
			}

			return values;
		}

		public PropertySet CreatePropertySet(in String setId)
		{
			if (!TryGetSetDef(setId, out var setDef))
			{
				throw new Exception($"Definition for set {setId} not found");
			}
			return new PropertySet(setDef, CreatePropertySetValues(setId));
		}

		public PropertyManager(int numPropDefs = 24)
		{
			_definitions = new Vector<PropertyDefinition>(numPropDefs);
			//_values = new Vector<float>[numPropDefs];

			//for (int i = 0; i < numPropDefs; i++)
			//{
			//	_values[i] = new Vector<float>(32);
			//}
		}
	}
}
