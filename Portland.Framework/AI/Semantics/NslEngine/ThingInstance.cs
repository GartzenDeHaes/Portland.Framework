using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Mathmatics;

namespace Portland.AI.Semantics
{
	public class ThingInstance
	{
		public Int32Guid InstanceId;
		public Thing MyClass;
		public Dictionary<string, SlotInstance> Slots = new Dictionary<string, SlotInstance>();
		public List<Link> LinksToThis = new List<Link>();
		public Dictionary<string, ValueSlotInstance> ValueSlots = new Dictionary<string, ValueSlotInstance>();

		public ThingInstance()
		{
			InstanceId.Init();
		}

		public void LinkTo(string slotName, ThingInstance target)
		{
			var slot = Slots[slotName];

			slot.Links.Add(new Link() { From = slot, To = target });
		}
	}
}
