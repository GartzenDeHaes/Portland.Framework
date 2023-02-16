using System;
using System.Collections.Generic;

using Portland.Collections;

namespace Portland.ComponentModel
{
	public interface IObservableMap<TKEY, TVALUE> : IMap<TKEY, TVALUE>
	{
		void AddListener(in TKEY valueKey, string listenerKey, Action<TKEY, TVALUE> onChange);
		void AddListener(string listenerKey, Action<TKEY, TVALUE> onAnyChange);
		void RemoveListener(string listenerKey);
		void RemoveListener(string listenerKey, in TKEY key);
	}
}