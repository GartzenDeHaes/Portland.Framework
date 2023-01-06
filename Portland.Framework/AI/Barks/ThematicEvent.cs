using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.AI.Barks
{
	public struct ThematicEvent
	{
		public static AsciiId4 ActionSee = "SEE";
		public static AsciiId4 ActionSay = "SAY";

		public TextTableToken Agent;
		public AsciiId4 Action;
		public TextTableToken DirectObject;
		public TextTableToken Instrument;
	}
}
