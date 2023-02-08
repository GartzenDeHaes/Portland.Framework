using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Text;

namespace Portland.RPG
{
	public class PropertySet : IIndexed<float> 
	{
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

		bool FindKey(in AsciiId4 id, out int index)
		{
			for (index = 0; index < _keys.Properties.Length; index++)
			{
				if (_keys.Properties[index].PropertyId == id)
				{
					return true;
				}
			}

			return false;
		}

		public bool TryGetValue(in AsciiId4 id, out float value)
		{
			if (FindKey(id, out int index))
			{
				value = this[index];
				return true;
			}
			value = 0f;
			return false;
		}

		public bool TrySetValue(in AsciiId4 id, float value)
		{
			if (FindKey(id, out int index))
			{
				this[index] = value;
				return true;
			}
			return false;
		}

		public void ModifyValueAt(int index, float delta)
		{
			_manager.ModifyPropertyValue(_keys.Properties[index], delta);
		}

		public AsciiId4 IdAt(int index)
		{
			return _keys.Properties[index].PropertyId;
		}

		public string NameAt(int index)
		{
			return _manager.GetPropertyName(_keys.Properties[index]);
		}

		public String8 SetName()
		{
			return _keys.SetId;
		}

		public PropertySet(PropertySetKeys keys, PropertyManager manager)
		{
			_keys = keys;
			_manager = manager;
		}
	}
}
