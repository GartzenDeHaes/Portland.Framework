using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics
{
	public class SlotInstance
	{
		public Slot SlotClass;
		public ThingInstance Holder;
		public List<Link> Links = new List<Link>();

		public List<ThingInstance> ListTargets()
		{
			var list = new List<ThingInstance>();

			foreach (var link in Links)
			{
				list.Add(link.To);
			}

			return list;
		}
	}
}
