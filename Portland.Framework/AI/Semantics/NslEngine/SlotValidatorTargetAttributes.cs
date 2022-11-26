using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics
{
	public class SlotValidatorTargetAttributes : ISlotGuard
	{
		private ThingAttribute _attribs;

		public SlotValidatorTargetAttributes(ThingAttribute attribs)
		{
			_attribs = attribs;
		}

		public bool ValidateOperation(ThingInstance from, SlotInstance slot, ThingInstance to)
		{
			return (to.MyClass.Attributes & _attribs) == _attribs;
		}
	}
}
