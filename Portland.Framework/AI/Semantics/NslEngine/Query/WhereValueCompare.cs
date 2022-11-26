using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public class WhereValueCompare : IFilter
	{
		private string _slotName;
		private VariantCompare _comp;
		private Variant8 _value;

		public WhereValueCompare(string slotName, string compareOp, Variant8 value)
		{
			_slotName = slotName;
			_comp = VariantCompare.Parse(compareOp);
			_value = value;
		}

		public bool Accept(ThingInstance rec)
		{
			return _comp.Compare(rec.ValueSlots[_slotName].AsVariant(), _value);
		}
	}
}
