using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public class SelectInstSingle : ISelector
	{
		private ThingInstance[] _list = new ThingInstance[1];

		public void Init()
		{
			_list[0] = null;
		}

		public bool Select(ThingInstance rec)
		{
			_list[0] = rec;

			return true; // complete
		}

		public IEnumerable SelectedObjects()
		{
			return _list;
		}
	}
}
