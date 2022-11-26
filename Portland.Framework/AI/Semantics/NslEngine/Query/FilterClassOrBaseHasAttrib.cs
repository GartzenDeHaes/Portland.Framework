using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public class FilterClassOrBaseHasAttrib : IFilter
	{
		private ThingAttribute _attrib;

		public FilterClassOrBaseHasAttrib(ThingAttribute attrib)
		{
			_attrib = attrib;
		}

		public bool Accept(ThingInstance rec)
		{
			return HasAttrib(rec.MyClass);
		}

		private bool HasAttrib(Thing cls)
		{
			if ((cls.Attributes & _attrib) != 0)
			{
				return true;
			}

			if (cls.Super != null)
			{
				return HasAttrib(cls.Super);
			}

			return false;
		}
	}
}
