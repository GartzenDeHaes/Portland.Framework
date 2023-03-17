using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG.Dialogue
{
	// wait, set, add, send, stop
	public sealed class DialogueCommand
	{
		/// <summary>END the dialogue</summary>
		public readonly static String10 CommandNameStop = "STOP";
		/// <summary>Wait in seconds before continuing executing comamands</summary>
		public readonly static String10 CommandNameWait = "WAIT";
		/// <summary>Send an event/message</summary>
		public readonly static String10 CommandNameSend = "SEND";
		/// <summary>Set a blackboard value</summary>
		public readonly static String10 CommandNameVarSet = "SET";
		/// <summary>Clear a blackboard value</summary>
		public readonly static String10 CommandNameVarClear = "CLEAR";
		/// <summary>Add to a blackboard value, creating it (as zero) if it doesn't exist</summary>
		public readonly static String10 CommandNameAdd = "ADD";
		/// <summary>Load a new node</summary>
		public readonly static String10 CommandNameGoto = "JUMP";
		/// <summary>Waits, then stops</summary>
		public readonly static String10 CommandNameDuration = "DURATION";
		/// <summary>Queries the bark system</summary>
		public readonly static String10 CommandNameConcept = "CONCEPT";

		public String10 CommandName;
		public string ArgS;
		public Variant8 ArgV;
	}
}
