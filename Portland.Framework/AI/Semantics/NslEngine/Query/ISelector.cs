using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public interface ISelector
	{
		/// <summary>
		/// Reset internal for re-use.
		/// </summary>
		void Init();

		/// <summary>
		/// Records that have passed the filter tests are then Select'ed
		/// </summary>
		/// <param name="rec">Current target</param>
		/// <returns>Returns true if the selection is complete and the query should end</returns>
		bool Select(ThingInstance rec);

		/// <summary>
		/// After Select() returns true, call SelectedObjects to get the items collected.  This also clears the results.
		/// </summary>
		IEnumerable SelectedObjects();
	}
}
