using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public class SelectInst : ISelector
	{
		private List<ThingInstance> _list = new List<ThingInstance>();

		public void Init()
		{
			_list.Clear();
		}

		public bool Select(ThingInstance rec)
		{
			_list.Add(rec);

			return false; // continue
		}

		public IEnumerable SelectedObjects()
		{
			return _list;
		}
	}
}
