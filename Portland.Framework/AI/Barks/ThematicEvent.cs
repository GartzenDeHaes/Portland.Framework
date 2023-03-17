using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.Collections;
using Portland.Text;
using Portland.Types;

namespace Portland.AI.Barks
{
    public struct ThematicEvent
	{
		public static AsciiId4 ActionSee = "SEE";
		public static AsciiId4 ActionSay = "SAY";
		public static AsciiId4 ActionIdle = "IDLE";

		/// <summary>Actor performing the action</summary>
		public String Agent;
		/// <summary>Verb</summary>
		public AsciiId4 Action;
		/// <summary>Direct object</summary>
		public string Concept;
		/// <summary>Indirect object</summary>
		//public String Instrument;
	}
}
