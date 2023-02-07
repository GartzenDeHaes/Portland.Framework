using System;
using System.Text;

using Portland.Collections;

namespace Portland.Basic
{
	public sealed class BasicLex : IBasicLex
	{
		private static readonly StringTable _stab = new StringTable();

		private readonly StringBuilder _text = new StringBuilder();
		private int _textPos = 0;

		private readonly StringBuilder _lexum = new StringBuilder();

		private enum State
		{
			START,
			ID,
			STRING_TICK,
			STRING_QUOTE,
			NUM
		}

		public int LineNumber
		{
			get; private set;
		}

		public string Lexum
		{
			get 
			{
				lock (_stab)
				{
					return _stab.GetString(_lexum);
				}
			}
		}

		public LexToken Token
		{
			get; private set;
		}

		public bool TreatCrAsWs
		{
			get; set;
		}

		public bool IsCurrentTokenOperator
		{
			get
			{
				return Token == LexToken.AND ||
					Token == LexToken.GT ||
					Token == LexToken.GTEQ ||
					Token == LexToken.LT ||
					Token == LexToken.LTEQ ||
					Token == LexToken.MINUS ||
					Token == LexToken.MOD ||
					Token == LexToken.NEQ ||
					Token == LexToken.NOT ||
					Token == LexToken.OR ||
					Token == LexToken.PLUS ||
					Token == LexToken.SLASH ||
					Token == LexToken.STAR ||
					Token == LexToken.XOR;
			}
		}

		public BasicLex()
		{
		}

		public void Clear()
		{
			_lexum.Length = 0;
			_text.Length = 0;
			_textPos = 0;
		}

		public string RemainingText()
		{
			Token = LexToken.EOF;
			string txt = _text.ToString(_textPos, _text.Length - _textPos);
			_text.Length = 0;
			_textPos = 0;
			return txt;
		}

		public string RemainingLine()
		{
			_lexum.Length = 0;

			while (_text[_textPos] != '\n' && _textPos < _text.Length)
			{
				_lexum.Append(_text[_textPos++]);
			}

			if (_textPos < _text.Length)
			{
				Token = LexToken.CR;
			}
			else
			{
				Token = LexToken.EOF;
			}

			return _lexum.ToString();
		}

		public void AppendInput(string text)
		{
			lock (_text)
			{
				_text.Append(text);
			}
		}

		private char NextCh()
		{
			if (_textPos >= _text.Length)
			{
				_text.Length = 0;
				_textPos = 0;
				return '\0';
			}

			return _text[_textPos++];
		}

		private void SkipWs()
		{
			while (_textPos < _text.Length && Char.IsWhiteSpace(_text[_textPos]))
			{
				if (_text[_textPos] == '\n' && !TreatCrAsWs)
				{
					return;
				}
				_textPos++;
			}
		}

		public bool Next()
		{
			_lexum.Length = 0;

			if (_text.Length == 0)
			{
				Token = LexToken.EOF;
				return false;
			}

			SkipWs();

			State state = State.START;

			while (_textPos < _text.Length)
			{
				char ch = NextCh();

				switch (state)
				{
					case State.START:
						switch (ch)
						{
							case '[':
								Token = LexToken.LBRAC;
								return true;
							case ']':
								Token = LexToken.RBRAC;
								return true;
							case '\'':
								state = State.STRING_TICK;
								Token = LexToken.STRING;
								break;
							case '"':
								state = State.STRING_QUOTE;
								Token = LexToken.STRING;
								break;
							case '\n':
								LineNumber++;
								if (!TreatCrAsWs)
								{
									Token = LexToken.CR;
									return true;
								}
								continue;
							case '\0':
								Token = LexToken.EOF;
								return false;
							case '+':
								Token = LexToken.PLUS;
								return true;
							case '-':
								Token = LexToken.MINUS;
								return true;
							case ',':
								Token = LexToken.COMMA;
								return true;
							case '*':
								Token = LexToken.STAR;
								return true;
							case '/':
								Token = LexToken.SLASH;
								return true;
							case '#':
								Token = LexToken.HASH;
								return true;
							case '=':
								Token = LexToken.EQUALS;
								return true;
							case '(':
								Token = LexToken.LPAR;
								return true;
							case ')':
								Token = LexToken.RPAR;
								return true;
							case ':':
								Token = LexToken.COLON;
								return true;
							case '^':
								Token = LexToken.XOR;
								return true;
							case '!':
								if (_text[_textPos] == '=')
								{
									_textPos++;
									Token = LexToken.NEQ;
								}
								else
								{
									Token = LexToken.NOT;
								}
								return true;
							case '<':
								if (_text[_textPos] == '=')
								{
									_textPos++;
									Token = LexToken.LTEQ;
								}
								else
								{
									Token = LexToken.LT;
								}
								return true;
							case '>':
								if (_text[_textPos] == '=')
								{
									_textPos++;
									Token = LexToken.GTEQ;
								}
								else
								{
									Token = LexToken.GT;
								}
								return true;
						}

						if (Char.IsControl(ch) || Char.IsWhiteSpace(ch))
						{
							continue;
						}

						if (state == State.START)
						{
							_lexum.Append(ch);

							if (Char.IsDigit(ch))
							{
								state = State.NUM;
								Token = LexToken.NUM;
							}
							else
							{
								state = State.ID;
								Token = LexToken.ID;
							}
						}

						break;
					case State.ID:
						if (Char.IsLetterOrDigit(ch) || ch == '_')
						{
							_lexum.Append(ch);
						}
						else
						{
							_textPos--;

							if (_lexum.Length == 2)
							{
								if
								(
									(_lexum[0] == 'I' || _lexum[0] == 'i')
									&& (_lexum[1] == 'F' || _lexum[1] == 'f')
								)
								{
									Token = LexToken.IF;
								}
								else if
								(
									(_lexum[0] == 'O' || _lexum[0] == 'o')
									&& (_lexum[1] == 'R' || _lexum[1] == 'r')
								)
								{
									Token = LexToken.OR;
								}
								else if
								(
									(_lexum[0] == 'T' || _lexum[0] == 't')
									&& (_lexum[1] == 'O' || _lexum[1] == 'o')
								)
								{
									Token = LexToken.TO;
								}
							}
							else if (_lexum.Length == 3)
							{
								if
								(
									(_lexum[0] == 'F' || _lexum[0] == 'f')
									&& (_lexum[1] == 'O' || _lexum[1] == 'o')
									&& (_lexum[2] == 'R' || _lexum[2] == 'r')
								)
								{
									Token = LexToken.FOR;
								}
								else if
								(
									(_lexum[0] == 'S' || _lexum[0] == 's')
									&& (_lexum[1] == 'U' || _lexum[1] == 'u')
									&& (_lexum[2] == 'B' || _lexum[2] == 'b')
								)
								{
									Token = LexToken.SUB;
								}
								else if
								(
									(_lexum[0] == 'L' || _lexum[0] == 'l')
									&& (_lexum[1] == 'E' || _lexum[1] == 'e')
									&& (_lexum[2] == 'T' || _lexum[2] == 't')
								)
								{
									Token = LexToken.LET;
								}
								else if
								(
									(_lexum[0] == 'D' || _lexum[0] == 'd')
									&& (_lexum[1] == 'I' || _lexum[1] == 'i')
									&& (_lexum[2] == 'M' || _lexum[2] == 'm')
								)
								{
									Token = LexToken.DIM;
								}
								else if
								(
									(_lexum[0] == 'A' || _lexum[0] == 'a')
									&& (_lexum[1] == 'N' || _lexum[1] == 'n')
									&& (_lexum[2] == 'D' || _lexum[2] == 'd')
								)
								{
									Token = LexToken.AND;
								}
								else if
								(
									(_lexum[0] == 'X' || _lexum[0] == 'x')
									&& (_lexum[1] == 'O' || _lexum[1] == 'o')
									&& (_lexum[2] == 'R' || _lexum[2] == 'r')
								)
								{
									Token = LexToken.XOR;
								}
								else if
								(
									(_lexum[0] == 'N' || _lexum[0] == 'n')
									&& (_lexum[1] == 'O' || _lexum[1] == 'o')
									&& (_lexum[2] == 'T' || _lexum[2] == 't')
								)
								{
									Token = LexToken.NOT;
								}
								else if
								(
									(_lexum[0] == 'M' || _lexum[0] == 'm')
									&& (_lexum[1] == 'O' || _lexum[1] == 'o')
									&& (_lexum[2] == 'D' || _lexum[2] == 'd')
								)
								{
									Token = LexToken.MOD;
								}
								else if
								(
									(_lexum[0] == 'R' || _lexum[0] == 'r')
									&& (_lexum[1] == 'E' || _lexum[1] == 'e')
									&& (_lexum[2] == 'M' || _lexum[2] == 'm')
								)
								{
									Token = LexToken.REM;
								}
							}
							else if (_lexum.Length == 4)
							{
								if
								(
									(_lexum[0] == 'T' || _lexum[0] == 't')
									&& (_lexum[1] == 'H' || _lexum[1] == 'h')
									&& (_lexum[2] == 'E' || _lexum[2] == 'e')
									&& (_lexum[3] == 'N' || _lexum[3] == 'n')
								)
								{
									Token = LexToken.THEN;
								}
								else if
								(
									(_lexum[0] == 'C' || _lexum[0] == 'c')
									&& (_lexum[1] == 'A' || _lexum[1] == 'a')
									&& (_lexum[2] == 'L' || _lexum[2] == 'l')
									&& (_lexum[3] == 'L' || _lexum[3] == 'l')
								)
								{
									Token = LexToken.CALL;
								}
								else if
								(
									(_lexum[0] == 'N' || _lexum[0] == 'n')
									&& (_lexum[1] == 'U' || _lexum[1] == 'u')
									&& (_lexum[2] == 'L' || _lexum[2] == 'l')
									&& (_lexum[3] == 'L' || _lexum[3] == 'l')
								)
								{
									Token = LexToken.NULL;
								}
								else if
								(
									(_lexum[0] == 'T' || _lexum[0] == 't')
									&& (_lexum[1] == 'R' || _lexum[1] == 'r')
									&& (_lexum[2] == 'U' || _lexum[2] == 'u')
									&& (_lexum[3] == 'E' || _lexum[3] == 'e')
								)
								{
									Token = LexToken.TRUE;
								}
								else if
								(
									(_lexum[0] == 'E' || _lexum[0] == 'e')
									&& (_lexum[1] == 'L' || _lexum[1] == 'l')
									&& (_lexum[2] == 'S' || _lexum[2] == 's')
									&& (_lexum[3] == 'E' || _lexum[3] == 'e')
								)
								{
									Token = LexToken.ELSE;
								}
								else if
								(
									(_lexum[0] == 'E' || _lexum[0] == 'e')
									&& (_lexum[1] == 'X' || _lexum[1] == 'x')
									&& (_lexum[2] == 'I' || _lexum[2] == 'i')
									&& (_lexum[3] == 'T' || _lexum[3] == 't')
								)
								{
									Token = LexToken.EXIT;
								}
								else if
								(
									(_lexum[0] == 'N' || _lexum[0] == 'n')
									&& (_lexum[1] == 'E' || _lexum[1] == 'e')
									&& (_lexum[2] == 'X' || _lexum[2] == 'x')
									&& (_lexum[3] == 'T' || _lexum[3] == 't')
								)
								{
									Token = LexToken.NEXT;
								}
								else if
								(
									(_lexum[0] == 'S' || _lexum[0] == 's')
									&& (_lexum[1] == 'T' || _lexum[1] == 't')
									&& (_lexum[2] == 'E' || _lexum[2] == 'e')
									&& (_lexum[3] == 'P' || _lexum[3] == 'p')
								)
								{
									Token = LexToken.STEP;
								}
								else if
								(
									(_lexum[0] == 'W' || _lexum[0] == 'w')
									&& (_lexum[1] == 'E' || _lexum[1] == 'e')
									&& (_lexum[2] == 'N' || _lexum[2] == 'n')
									&& (_lexum[3] == 'D' || _lexum[3] == 'd')
								)
								{
									Token = LexToken.WEND;
								}
								//else if
								//(
								//	(_lexum[0] == 'S' || _lexum[0] == 's')
								//	&& (_lexum[1] == 'E' || _lexum[1] == 'e')
								//	&& (_lexum[2] == 'N' || _lexum[2] == 'n')
								//	&& (_lexum[3] == 'D' || _lexum[3] == 'd')
								//)
								//{
								//	Token = LexToken.SEND;
								//}
							}
							else if (_lexum.Length == 5)
							{
								if
								(
									(_lexum[0] == 'W' || _lexum[0] == 'w')
									&& (_lexum[1] == 'H' || _lexum[1] == 'h')
									&& (_lexum[2] == 'I' || _lexum[2] == 'i')
									&& (_lexum[3] == 'L' || _lexum[3] == 'l')
									&& (_lexum[4] == 'E' || _lexum[4] == 'e')
								)
								{
									Token = LexToken.WHILE;
								}
								else if
								(
									(_lexum[0] == 'E' || _lexum[0] == 'e')
									&& (_lexum[1] == 'R' || _lexum[1] == 'r')
									&& (_lexum[2] == 'R' || _lexum[2] == 'r')
									&& (_lexum[3] == 'O' || _lexum[3] == 'o')
									&& (_lexum[4] == 'R' || _lexum[4] == 'r')
								)
								{
									Token = LexToken.ERROR;
								}
								else if
								(
									(_lexum[0] == 'F' || _lexum[0] == 'f')
									&& (_lexum[1] == 'A' || _lexum[1] == 'a')
									&& (_lexum[2] == 'L' || _lexum[2] == 'l')
									&& (_lexum[3] == 'S' || _lexum[3] == 's')
									&& (_lexum[4] == 'E' || _lexum[4] == 'e')
								)
								{
									Token = LexToken.FALSE;
								}
								else if
								(
									(_lexum[0] == 'P' || _lexum[0] == 'p')
									&& (_lexum[1] == 'R' || _lexum[1] == 'r')
									&& (_lexum[2] == 'I' || _lexum[2] == 'i')
									&& (_lexum[3] == 'N' || _lexum[3] == 'n')
									&& (_lexum[4] == 'T' || _lexum[4] == 't')
								)
								{
									Token = LexToken.PRINT;
								}
								else if
								(
									(_lexum[0] == 'E' || _lexum[0] == 'e')
									&& (_lexum[1] == 'N' || _lexum[1] == 'n')
									&& (_lexum[2] == 'D' || _lexum[2] == 'd')
									&& (_lexum[3] == 'I' || _lexum[3] == 'i')
									&& (_lexum[4] == 'F' || _lexum[4] == 'f')
								)
								{
									Token = LexToken.ENDIF;
								}
							}
							else if (_lexum.Length == 6)
							{
								if (_lexum.ToString().ToUpper().Equals("RETURN"))
								{
									Token = LexToken.RETURN;
								}
								if (_lexum.ToString().ToUpper().Equals("ENDSUB"))
								{
									Token = LexToken.ENDSUB;
								}
							}
							else if (_lexum.Length == 8)
							{
								if (_lexum.ToString().ToUpper().Equals("CONTINUE"))
								{
									Token = LexToken.CONTINUE;
								}
							}
							
							return true;
						}
						break;
					case State.NUM:
						if (Char.IsDigit(ch) || ch == '.')
						{
							_lexum.Append(ch);
						}
						else
						{
							_textPos--;
							return true;
						}
						break;
					case State.STRING_QUOTE:
						if (ch == '\0' || ch == '"')
						{
							return true;
						}
						else
						{
							_lexum.Append(ch);
						}
						break;
					case State.STRING_TICK:
						if (ch == '\0' || ch == '\'')
						{
							return true;
						}
						else
						{
							_lexum.Append(ch);
						}
						break;
				}
			}

			if (state == State.START)
			{
				_text.Length = 0;
				_textPos = 0;
				Token = LexToken.EOF;
			}

			return _lexum.Length > 0;
		}
	}
}
