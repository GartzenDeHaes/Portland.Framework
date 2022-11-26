using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public class SelectInstValue : ISelector
	{
		private List<Variant8> _values = new List<Variant8>();
		private string _slotName;

		public SelectInstValue(string slotName)
		{
			_slotName = slotName;
		}

		public void Init()
		{
			_values.Clear();
		}

		public bool Select(ThingInstance rec)
		{
			_values.Add(rec.ValueSlots[_slotName].AsVariant());

			return false; // continue;
		}

		public IEnumerable SelectedObjects()
		{
			return _values;
		}
	}
}
