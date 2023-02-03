using System;
using System.Diagnostics;

using Portland.Collections;
using Portland.Text;

namespace Portland.RPG
{
	/// <summary>
	/// </summary>
	public sealed class PropertyManager
	{
		Vector<float>[] _values;
		Vector<PropertyDefinition> _definitions;
		Vector<PropertyDefinitionSet> _defSets = new Vector<PropertyDefinitionSet>();
		//Vector<PropertySetInstance> _pool = new Vector<PropertySetInstance>();

		public void Update(float deltaTimeInSeconds)
		{
			float changePerSecond;
			Vector<float> values;

			for (int i = 0; i < _definitions.Count; i++)
			{
				changePerSecond = _definitions[i].ChangePerSecond;
				values = _values[i];

				for (int j = 0; j < values.Count; j++)
				{
					values[i] += deltaTimeInSeconds * changePerSecond;
				}
			}
		}

		public float GetPropertyValue(in PropertyInstanceKey key)
		{
			return _values[key.DefinitonIndex][key.Index];
		}

		public void SetPropertyValue(in PropertyInstanceKey key, float value)
		{
			_values[key.DefinitonIndex].SetElementAt(key.Index, value);
		}

		public void ModifyPropertyValue(in PropertyInstanceKey key, float delta)
		{
			_values[key.DefinitonIndex].SetElementAt(key.Index, GetPropertyValue(key) + delta);
		}

		public AsciiId4 GetPropertyDefinitonId(in PropertyInstanceKey key)
		{
			return _definitions[key.DefinitonIndex].PropertyId;
		}

		public string GetPropertyName(in PropertyInstanceKey key)
		{
			return _definitions[key.DefinitonIndex].LongName;
		}

		public PropertyDefinitionBuilder DefineProperty(AsciiId4 id, string name)
		{
			var def = new PropertyDefinition { PropertyId = id, LongName = name, Minimum = 0, Maximum = 100 };

			for (int i = 0; i < _definitions.Count; i++)
			{
				if (_definitions[i] == null)
				{
					_definitions[i] = def;
					def.ValueIndex = i;
					return new PropertyDefinitionBuilder { Property = def };
				}
			}

			def.ValueIndex = _definitions.Add(def);
			return new PropertyDefinitionBuilder { Property = def };
		}

		bool TryGetDefinition(AsciiId4 id, out PropertyDefinition def)
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

		public void DefinePropertySet(String8 setName, AsciiId4[] propIds)
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
		}

		bool TryGetSetDef(String8 setId, out PropertyDefinitionSet setDef)
		{
			for (int i = 0; i < _defSets.Count; i++)
			{
				setDef = _defSets[i];
				if (setDef.SetId == setId)
				{
					return true;
				}
			}

			setDef = default;
			return false;
		}

		public PropertySetKeys CreateSetKeysInstance(String8 setId)
		{
			if (!TryGetSetDef(setId, out var setDef))
			{
				throw new Exception($"Definition for set {setId} not found");
			}

			var inst = new PropertySetKeys { SetId = setId, Properties = new PropertyInstanceKey[setDef.Properties.Length] };
			PropertyDefinition def;

			for (int i = 0; i < setDef.Properties.Length; i++)
			{
				def = setDef.Properties[i];
				inst.Properties[i] = new PropertyInstanceKey
				{
					PropertyId = def.PropertyId,
					Index = (short)_values[def.ValueIndex].AddElement(def.DefaultValue),
					DefinitonIndex = (short)i,
				};
			}

			return inst;
		}

		public PropertySet CreateSetInstance(String8 setId)
		{
			return new PropertySet(CreateSetKeysInstance(setId), this);
		}

		public PropertyManager(int numPropDefs = 8)
		{
			_definitions = new Vector<PropertyDefinition>(numPropDefs);
			_values = new Vector<float>[numPropDefs];

			for (int i = 0; i < numPropDefs; i++)
			{
				_values[i] = new Vector<float>();
			}
		}
	}
}
