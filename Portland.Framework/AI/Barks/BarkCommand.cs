using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Text;

namespace Portland.AI.Barks
{
	public sealed class BarkCommand
	{
		public static AsciiId4 CommandNameSay = "SAY";
		/// <summary>Disable a rule</summary>
		public static AsciiId4 CommandNameDontSay = "NSAY";
		public static AsciiId4 CommandNameResetRule = "RSET";
		public static AsciiId4 CommandNameSetVar = "SET";
		public static AsciiId4 CommandNameRaise = "SEND";
		public static AsciiId4 CommandConcept = "CNPT";
		public static AsciiId4 CommandNameAdd = "ADD";

		public float DelayTime;
		public float DelayRemainingToRun;
		public AsciiId4 CommandName;
		public TextTableToken ActorName;
		public TextTableToken Arg1;
		public Variant8 Arg2;
		public float Duration = 5f;

		public Rule Rule;
		public Vector<string> DefaultTexts;
	}
}
