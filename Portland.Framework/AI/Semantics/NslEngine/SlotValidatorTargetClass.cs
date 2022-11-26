using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics
{
	public class SlotValidatorTargetClass : ISlotGuard
	{
		private string _className;

		public SlotValidatorTargetClass(string className)
		{
			_className = className;
		}

		public bool ValidateOperation(ThingInstance from, SlotInstance slot, ThingInstance to)
		{
			return to.MyClass.IsNamedOrHasBase(_className);
		}
	}
}
