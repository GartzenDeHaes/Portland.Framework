using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics
{
	public interface ISlotGuard
	{
		bool ValidateOperation(ThingInstance from, SlotInstance slot, ThingInstance to);
	}
}
