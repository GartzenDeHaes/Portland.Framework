using Portland.Collections;
using Portland.ComponentModel;

namespace Portland.Framework.AI
{
	public interface IBlackboard
	{
		void Add(in StringTableToken tok, IObservableValue<Variant8> value);
		bool ContainsKey(in StringTableToken tok);
		bool ContainsKey(string key);
		IObservableValue<Variant8> Get(in StringTableToken key);
		IObservableValue<Variant8> Get(string key);
		bool TryGetValue(in StringTableToken tok, out IObservableValue<Variant8> value);
		bool TryGetValue(string key, out IObservableValue<Variant8> value);
	}
}