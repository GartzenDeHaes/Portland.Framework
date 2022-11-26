using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.Semantics.Query;

namespace Portland.AI.Semantics
{
	public class DataCommand
	{
		public List<Variant8> Parameters = new List<Variant8>();

		private ISelector _select;
		private List<IFilter> _filters = new List<IFilter>();

		public DataCommand()
		{
		}

		public IEnumerable Query(ICollection<ThingInstance> data)
		{
			_select.Init();

			foreach (var inst in data)
			{
				bool rejected = false;

				for (int x = 0; x < _filters.Count; x++)
				{
					if (!_filters[x].Accept(inst))
					{
						rejected = true;
						break;
					}
				}
				if (rejected)
				{
					continue;
				}
				if (_select.Select(inst))
				{
					break;
				}
			}

			return _select.SelectedObjects();
		}

		public bool Include(ThingInstance inst)
		{
			foreach (var filter in _filters)
			{
				if (!filter.Accept(inst))
				{
					return false;
				}
			}

			return true;
		}

		public DataCommand SelectAll()
		{
			if (_select != null)
			{
				throw new Exception("Only on selector allowed");
			}
			_select = new SelectInst();

			return this;
		}

		public DataCommand WhereClassNamed(string className)
		{
			var value = new Variant8(className);
			Parameters.Add(value);

			_filters.Add(new WhereClassNamed(value));

			return this;
		}

		public DataCommand WhereClassOrBaseNamed(string className)
		{
			var value = new Variant8(className);
			Parameters.Add(value);

			_filters.Add(new WhereClassOrBaseClassNamed(value));

			return this;
		}

		public DataCommand WhereValueCompare(string slotName, string compareOp, Variant8 value)
		{
			Parameters.Add(value);

			_filters.Add(new WhereValueCompare(slotName, compareOp, value));

			return this;
		}
	}
}
