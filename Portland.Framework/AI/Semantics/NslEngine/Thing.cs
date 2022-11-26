using System;
using System.Collections.Generic;

using Portland.Mathmatics;

namespace Portland.AI.Semantics
{
	public class Thing
	{
		public Int32Guid ClassId;
		public Thing Super;
		public string ClassName;
		public ThingAttribute Attributes;
		public Dictionary<string, Slot> Slots = new Dictionary<string, Slot>();
		public Dictionary<string, ValueSlot> Values = new Dictionary<string, ValueSlot>();

		public Thing()
		{
			ClassId.Init();
		}

		public bool IsNamedOrHasBase(string className)
		{
			if (ClassName.Equals(className))
			{
				return true;
			}

			if (Super == null)
			{
				return false;
			}

			return Super.IsNamedOrHasBase(className);
		}
	}
}
