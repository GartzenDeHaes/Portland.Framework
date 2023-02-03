using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.RPG
{
	public struct StatDefinition
	{
		public AsciiId4 StatId;
		public int Minimum;
		public int Maximum;
		public int Default;
		public string Name;
	}
}
