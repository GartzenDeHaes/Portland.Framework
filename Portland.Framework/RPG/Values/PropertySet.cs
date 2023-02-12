using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Portland.Collections;
using Portland.Mathmatics;

namespace Portland.RPG
{
	public class PropertySet : IIndexed<float> 
	{
		readonly PropertyDefinitionSet _def;
		readonly PropertySetKeys _keys;
		readonly PropertyManager _manager;

		public int Count
		{
			get { return _keys.Properties.Length; }
		}

		public float this[int index] 
		{
			get { return _manager.GetPropertyValue(_keys.Properties[index]); }
			set { _manager.SetPropertyValue(_keys.Properties[index], value); }
		}

		bool FindKey(in String8 id, out int index)
		{
			for (index = 0; index < _keys.Properties.Length; index++)
			{
				if (_keys.Properties[index].Definiton.PropertyId == id)
				{
					return true;
				}
			}

			return false;
		}

		public bool HasProperty(in String8 id)
		{
			return FindKey(id, out int _);
		}

		public float GetValue(in String8 id)
		{
			if (TryGetValue(id, out float value)) 
			{ 
				return value;
			}
			return -1;
		}

		public float GetMaximum(in String8 id)
		{
			if (FindKey(id, out int index))
			{
				return _manager.GetPropertyMaximum(_keys.Properties[index]);
			}
			return -1;
		}

		public bool TryGetValue(in String8 id, out float value)
		{
			if (FindKey(id, out int index))
			{
				value = this[index];
				return true;
			}
			value = 0f;
			return false;
		}

		public bool TrySetValue(in String8 id, float value)
		{
			if (FindKey(id, out int index))
			{
				this[index] = value;
				return true;
			}
			return false;
		}

		public bool TryGetMaximum(in String8 id, out float maximum)
		{
			if (FindKey(id, out int index))
			{
				maximum = _manager.GetPropertyMaximum(_keys.Properties[index]);
				return true;
			}

			maximum = 100f;
			return false;
		}

		public bool TrySetMaximum(in String8 id, float maximum)
		{
			if (FindKey(id, out int index))
			{
				_manager.SetPropertyMaximum(_keys.Properties[index], maximum);
				return true;
			}

			return false;
		}

		public void ModifyValueAt(int index, float delta)
		{
			_manager.ModifyPropertyValue(_keys.Properties[index], delta);
		}

		public bool TryGetProbability(in String8 id, out DiceTerm dice)
		{
			if (FindKey(id, out int index))
			{
				dice = _keys.Properties[index].Definiton.Probability;
				return true;
			}

			dice = default(DiceTerm);
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

		public String8 IdAt(int index)
		{
			return _keys.Properties[index].Definiton.PropertyId;
		}

		public string DisplayNameAt(int index)
		{
			return _manager.GetPropertyName(_keys.Properties[index]);
		}

		public String8 SetName()
		{
			return _keys.SetId;
		}

		public PropertySet(PropertyDefinitionSet def, PropertySetKeys keys, PropertyManager manager)
		{
			_def = def;
			_keys = keys;
			_manager = manager;
		}
	}
}
