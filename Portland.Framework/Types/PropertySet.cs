﻿using System;

using Portland.AI;
using Portland.Mathmatics;

namespace Portland.Types
{
	public class PropertySet //: IIndexed<float> 
	{
		readonly PropertyDefinitionSet _def;
		readonly PropertyValue[] _values;
		//readonly PropertyManager _manager;

		//public int Count
		//{
		//	get { return _values.Length; }
		//}

		//public float this[int index] 
		//{
		//	get { return _values[index].Amt; }
		//	set { _values[index].Set(value); }
		//}

		bool FindKey(in string id, out int index)
		{
			for (index = 0; index < _values.Length; index++)
			{
				if (_values[index].Definition.PropertyId == id)
				{
					return true;
				}
			}

			return false;
		}

		public bool HasProperty(in string id)
		{
			return FindKey(id, out int _);
		}

		public float GetValue(in string id)
		{
			if (TryGetValue(id, out float value))
			{
				return value;
			}
			return -1;
		}

		public float GetMaximum(in string id)
		{
			if (FindKey(id, out int index))
			{
				return _values[index].Max;
			}
			return -1;
		}

		public bool TryGetValue(in string id, out float value)
		{
			if (FindKey(id, out int index))
			{
				value = _values[index].Value;
				return true;
			}
			value = 0f;
			return false;
		}

		public bool TrySetValue(in string id, float value)
		{
			if (FindKey(id, out int index))
			{
				_values[index].Set(value);
				return true;
			}
			return false;
		}

		public bool TryGetMaximum(in string id, out float maximum)
		{
			if (FindKey(id, out int index))
			{
				maximum = _values[index].Max;
				return true;
			}

			maximum = 100f;
			return false;
		}

		public bool TrySetMaximum(in string id, float maximum)
		{
			if (FindKey(id, out int index))
			{
				_values[index].Max = maximum;
				return true;
			}

			return false;
		}

		public void ModifyValueAt(int index, float delta)
		{
			_values[index].AddToValue(delta);
		}

		public bool TryGetProbability(in string id, out DiceTerm dice)
		{
			if (FindKey(id, out int index))
			{
				dice = _values[index].Definition.Probability;
				return true;
			}

			dice = default;
			return false;
		}

		//public bool TrySetProbability(in String8 id, in DiceTerm dice)
		//{
		//	if (FindKey(id, out int index))
		//	{
		//		_keys.Properties[index].Probability = dice;
		//		return true;
		//	}
		//	return false;
		//}

		//public String8 IdAt(int index)
		//{
		//	return _values[index].Definition.PropertyId;
		//}

		public void AddToBlackBoard(IBlackboard<string> bb)
		{
			for (int i = 0; i < _values.Length; i++)
			{
				bb.Add(_values[i].Definition.PropertyId, _values[i]);
			}
		}

		public string GetDisplayName(in string propId)
		{
			if (FindKey(propId, out int index))
			{
				return _values[index].Definition.DisplayName;
			}

			return string.Empty;
		}

		public string GetSetId()
		{
			return _def.SetId;
		}

		public PropertySet(PropertyDefinitionSet def, PropertyValue[] values)
		{
			_def = def;
			_values = values;
		}
	}
}
