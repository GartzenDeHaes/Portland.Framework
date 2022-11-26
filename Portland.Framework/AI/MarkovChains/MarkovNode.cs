using System;
using System.Collections.Generic;
using System.Linq;

namespace Portland.AI.MarkovChain
{
	public class MarkovNode<T> : IEquatable<MarkovNode<T>>
	{
		private readonly T[] _items;

		public MarkovNode(IEnumerable<T> items)
		{
			_items = items.ToArray();
		}

		public bool Equals(MarkovNode<T> other)
		{
			if (other == null)
			{
				return false;
			}

			if (_items.Length != other._items.Length)
			{
				return false;
			}

			for (int i = 0; i < _items.Length; i++)
			{
				if (!_items[i].Equals(other._items[i]))
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			int hash = 19;
			foreach (T item in _items)
			{
				hash = hash * 31 + item.GetHashCode();
			}
			return hash;
		}
	}
}
