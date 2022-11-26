using System;
using System.Collections.Generic;
using System.Linq;

namespace Portland.AI.MarkovChain
{
	public class MarkovChain<T> where T : IEquatable<T>
	{
		private readonly int _order;
		private Dictionary<MarkovNode<T>, Dictionary<T, int>> _chain;
		private Dictionary<MarkovNode<T>, int> _terminalWeights;
		private Random _random;

		public MarkovChain(int order, int seed = 1337)
		{
			_order = order;
			_chain = new Dictionary<MarkovNode<T>, Dictionary<T, int>>();
			_terminalWeights = new Dictionary<MarkovNode<T>, int>();
			_random = new Random(seed);
		}

		public void AddItems(IEnumerable<IEnumerable<T>> itemList)
		{
			foreach (var item in itemList)
			{
				AddItems(item);
			}
		}

		public void AddItems(IEnumerable<T> items)
		{
			Queue<T> previous = new Queue<T>();

			// Add each item to the chain.
			foreach (T item in items)
			{
				AddItem(previous, item);

				previous.Enqueue(item);
				if (previous.Count > _order)
				{
					previous.Dequeue();
				}
			}

			// Add to the terminal weights
			MarkovNode<T> key = new MarkovNode<T>(previous);
			if (!_terminalWeights.ContainsKey(key))
			{
				_terminalWeights.Add(key, 0);
			}

			_terminalWeights[key]++;
		}

		private void AddItem(Queue<T> previous, T item)
		{
			if (previous.Count > _order)
			{
				throw new ArgumentException("The queue is longer than the Markov chain order", "previous");
			}

			MarkovNode<T> key = new MarkovNode<T>(previous);
			if (!_chain.ContainsKey(key))
			{
				_chain.Add(key, new Dictionary<T, int>());
			}

			if (!_chain[key].ContainsKey(item))
			{
				_chain[key].Add(item, 0);
			}
			
			_chain[key][item]++;
		}

		/// <summary>
		/// Gets an series of items, generated from the Markov chain.
		/// </summary>
		/// <returns>An IEnumerable of items.</returns>
		public IEnumerable<T> Generate()
		{
			Queue<T> previous = new Queue<T>();

			while (true)
			{
				while (previous.Count > _order)
				{
					previous.Dequeue();
				}

				MarkovNode<T> key = new MarkovNode<T>(previous);

				// If terminus is reached.
				if (!_chain.TryGetValue(key, out var weights))
				{
					yield break;
				}

				_terminalWeights.TryGetValue(key, out var terminalWeight);

				int sumWeights = weights.Sum(w => w.Value);
				int randomValue = _random.Next(sumWeights + terminalWeight) + 1;

				// If terminus is chosen.
				if (randomValue > sumWeights)
				{
					yield break;
				}

				// Loop through the chain, adding the weights of each step.
				// When 'randomValue' is less than or equal to the cumulative
				// weights so far, the current item is chosen.

				int cumulativeWeight = 0;

				foreach (var pair in weights)
				{
					cumulativeWeight += pair.Value;

					if (randomValue <= cumulativeWeight)
					{
						yield return pair.Key;
						previous.Enqueue(pair.Key);
						break;
					}
				}
			}
		}
	}
}
