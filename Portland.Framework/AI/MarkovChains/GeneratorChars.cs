using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.MarkovChain;

namespace Portland.AI.MarkovChain
{
	public class GeneratorChars
	{
		/// <summary>
		/// Represents a name and associated weight (how many times that name counts).
		/// </summary>
		public struct WeightedName
		{
			public string Name;
			public int Weight;

			public WeightedName(string name, int weight)
			{
				Name = name;
				Weight = weight;
			}
		}

		private Random _rand;
		private readonly MarkovChainChars _chain;

		private HashSet<string> _originalNames = new HashSet<string>();
		private HashSet<string> _seenNames = new HashSet<string>();

		/// <summary>
		/// Create a generator given a list of names
		/// </summary>
		public GeneratorChars(int order, IEnumerable<string> names, int randSeed = 1337)
		{
			_chain = new MarkovChainChars(order);
			_rand = new Random(randSeed);

			foreach (var name in names)
			{
				_chain.Add(name, 1);
				_originalNames.Add(name);
			}
		}
		/// <summary>
		/// Create a generator given a weighted list of names
		/// </summary>
		public GeneratorChars(int order, IEnumerable<WeightedName> wnames, int randSeed = 1337)
		{
			_chain = new MarkovChainChars(order);
			_rand = new Random(randSeed);

			foreach (var wn in wnames)
			{
				_chain.Add(wn.Name, wn.Weight);
				_originalNames.Add(wn.Name);
			}
		}

		/// <summary>
		/// Returns a random name.
		/// </summary>
		public string Next()
		{
			var name = _chain.Chain(_rand);
			return name;
		}

		/// <summary>
		/// Returns a random name, making sure not to generate a name that was given in the training data.
		/// </summary>
		public string NextNew()
		{
			string name;

			while (true)
			{
				name = Next();
				if (!_originalNames.Contains(name))
				{
					break;
				}
			}
			return name;
		}

		/// <summary>
		/// Returns a random name, making sure not to generate a name that was given in the training data or that was previously returned by this function.
		/// </summary>
		/// <returns></returns>
		public string NextUnique()
		{
			string name;

			while (true)
			{
				name = NextNew();
				if (!_seenNames.Contains(name))
				{
					_seenNames.Add(name);
					break;
				}
			}
			return name;
		}

		/// <summary>
		/// Returns all generatable names of a certain length or less.
		/// </summary>
		/// <param name="maxlen">Max returned name length</param>
		public IEnumerable<string> AllRaw(int maxlen)
		{
			return _chain.AllRaw(maxlen);
		}
	}
}
