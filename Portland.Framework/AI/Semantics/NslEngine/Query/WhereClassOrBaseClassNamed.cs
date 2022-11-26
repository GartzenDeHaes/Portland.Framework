using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public class WhereClassOrBaseClassNamed : IFilter
	{
		private Variant8 _className;

		public WhereClassOrBaseClassNamed(Variant8 className)
		{
			_className = className;
		}

		public bool Accept(ThingInstance rec)
		{
			return Accept(rec.MyClass);
		}

		private bool Accept(Thing cls)
		{
			if (cls.ClassName.Equals(_className.ToString()))
			{
				return true;
			}
			if (cls.Super != null)
			{
				return Accept(cls.Super);
			}
			return false;
		}
	}
}
