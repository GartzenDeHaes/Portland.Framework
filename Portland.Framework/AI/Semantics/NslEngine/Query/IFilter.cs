using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public interface IFilter
	{
		/// <summary>
		/// Implements WHERE clause.
		/// </summary>
		/// <param name="rec">Current target</param>
		/// <returns>Returns true if the record is included in the results</returns>
		bool Accept(ThingInstance rec);
	}
}
