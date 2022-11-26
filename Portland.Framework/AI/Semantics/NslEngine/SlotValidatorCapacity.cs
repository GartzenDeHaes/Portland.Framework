using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics
{
	public class SlotValidatorCapacity : ISlotGuard
	{
		private int _min, _max;

		public SlotValidatorCapacity(int minSize, int maxSize)
		{
			_min = minSize;
			_max = maxSize;
		}

		public bool ValidateOperation(ThingInstance from, SlotInstance slot, ThingInstance to)
		{
			return slot.Links.Count >= _min && slot.Links.Count <= _max;
		}
	}
}
