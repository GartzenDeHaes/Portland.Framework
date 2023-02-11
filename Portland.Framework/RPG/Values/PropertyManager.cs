﻿using System;
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
			float minimum;
			Vector<float> values;

			for (int i = 0; i < _definitions.Count; i++)
			{
				changePerSecond = _definitions[i].ChangePerSecond;
				minimum = _definitions[i].Minimum;

				if (changePerSecond != 0f)
				{
					values = _values[i];

					for (int j = 0; j < values.Count; j += 2)
					{
						ref float val = ref values.ElementAtRef(j);
						val += deltaTimeInSeconds * changePerSecond;
						if (val > values[j+1])
						{
							val = values[j+1];
						}
						else if (val < minimum)
						{
							val = minimum;
						}
					}
				}
			}
		}

		public float GetPropertyValue(in PropertyInstanceKey key)
		{
			return _values[key.DefinitonIndex][key.Index];
		}

		public float GetPropertyMaximum(in PropertyInstanceKey key)
		{
			return _values[key.DefinitonIndex][key.Index + 1];
		}

		public float GetPropertyMinimum(in PropertyInstanceKey key)
		{
			return _definitions[key.DefinitonIndex].Minimum;
		}

		public void SetPropertyMaximum(in PropertyInstanceKey key, float maximum)
		{
			_values[key.DefinitonIndex].SetElementAt(key.Index + 1, maximum);
		}

		public void SetPropertyValue(in PropertyInstanceKey key, float value)
		{
			var def = _definitions[key.DefinitonIndex];
			if (value < def.Minimum)
			{
				value = def.Minimum;
			}
			var values = _values[key.DefinitonIndex];
			var max = values[key.Index + 1];
			if (value > max)
			{
				value = max;
			}
			values.SetElementAt(key.Index, value);
		}

		public void ModifyPropertyValue(in PropertyInstanceKey key, float delta)
		{
			SetPropertyValue(key, _values[key.DefinitonIndex].ElementAt(key.Index) + delta);
		}

		public String8 GetPropertyDefinitonId(in PropertyInstanceKey key)
		{
			return _definitions[key.DefinitonIndex].PropertyId;
		}

		public string GetPropertyName(in PropertyInstanceKey key)
		{
			return _definitions[key.DefinitonIndex].LongName;
		}

		public PropertyDefinitionBuilder DefineProperty(in String8 id, string name, in String8 category)
		{
			var def = new PropertyDefinition { PropertyId = id, Category = category, LongName = name, Minimum = 0, Maximum = 100 };

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

		bool TryGetDefinition(in String8 id, out PropertyDefinition def)
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

		public void DefinePropertySet(in String8 setName, String8[] propIds)
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

			if (_defSets.Count > _values.Length)
			{
				throw new Exception($"Increase size of numProperties parameter to PropertyManager");
			}
		}

		bool TryGetSetDef(in String8 setId, out PropertyDefinitionSet setDef)
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

		public PropertySetKeys CreateSetKeysInstance(in String8 setId)
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
					Index = (short)_values[def.ValueIndex].AddElement(def.DefaultValueForInitialization),
					DefinitonIndex = (short)i,
					Probability = def.Probability,
				};
				_values[def.ValueIndex].AddElement(def.Maximum);
			}

			return inst;
		}

		public PropertySet CreateSetInstance(in String8 setId)
		{
			return new PropertySet(CreateSetKeysInstance(setId), this);
		}

		public PropertyManager(int numPropDefs = 24)
		{
			_definitions = new Vector<PropertyDefinition>(numPropDefs);
			_values = new Vector<float>[numPropDefs];

			for (int i = 0; i < numPropDefs; i++)
			{
				_values[i] = new Vector<float>(32);
			}
		}
	}
}
