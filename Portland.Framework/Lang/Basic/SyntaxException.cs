using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Basic
{
	public sealed class SyntaxException : Exception
	{
		int _lineNumber;
		string _message;

		public SyntaxException(int line, string msg)
		: base($"{msg} on line {line}")
		{
			_lineNumber = line;
			_message = msg;
		}
	}
}
