﻿using Portland.AI.Utility;
using Portland.Collections;
using Portland.ComponentModel;

namespace Portland.Framework.AI
{
	public interface IBlackboard<TKEY>
	{
		void Add(in TKEY id, PropertyValue value);
		bool ContainsKey(in TKEY tok);
		PropertyValue Get(in TKEY key);
		void Set(in TKEY key, in Variant8 value);
		bool TryGetValue(in TKEY tok, out PropertyValue value);
	}
}
