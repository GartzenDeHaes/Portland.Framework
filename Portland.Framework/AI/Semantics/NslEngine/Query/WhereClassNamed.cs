using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public class WhereClassNamed : IFilter
	{
		private Variant8 _className;

		public WhereClassNamed(Variant8 className)
		{
			_className = className;
		}

		public bool Accept(ThingInstance rec)
		{
			return rec.MyClass.ClassName.Equals(_className.ToString());
		}
	}
}
