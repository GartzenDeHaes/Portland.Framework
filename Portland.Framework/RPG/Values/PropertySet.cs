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
