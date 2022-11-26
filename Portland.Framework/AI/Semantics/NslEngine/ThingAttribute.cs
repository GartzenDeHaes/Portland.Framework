using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics
{
	public enum ThingAttribute
	{
		NONE = 0,
		WIELDABLE = (1<<0),	// Can be held in hands
		WEARABLE = (1<<1),	// Can be worn by character
		STATIC = (1<<2),		// Unmovable such a the ground
		MOBILE = (1<<3),		// Can move under its own power
		STORABLE = (1<<4),	// Can be placed in a contain or character inventory
	}
}
