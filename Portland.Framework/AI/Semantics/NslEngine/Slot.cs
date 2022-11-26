using System;
using System.Collections.Generic;

namespace Portland.AI.Semantics
{
	public class Slot
	{
		public string Name;
		public List<ISlotGuard> LinkValidators = new List<ISlotGuard>();
	}
}
