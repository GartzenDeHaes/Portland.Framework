using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Basic
{
	public enum LexToken
	{
		EOF,
		STRING,
		ID,
		NUM,
		FLOAT,
		COMMA,
		HASH,
		PLUS,
		MINUS,
		STAR,
		SLASH,
		MOD,
		EQUALS,
		NEQ,
		LT,
		GT,
		LTEQ,
		GTEQ,
		NOT,
		LPAR,
		RPAR,
		CR,
		COLON,
		IF,
		THEN,
		ELSE,
		ENDIF,
		FOR,
		TO,
		STEP,
		WHILE,
		WEND,
		CALL,
		SUB,
		ENDSUB,
		DIM,
		LET,
		NULL,
		AND,
		OR,
		XOR,
		TRUE,
		FALSE,
		ERROR,
		EXIT,
		NEXT,
		PRINT,
		//SEND,
		RETURN,
		CONTINUE,
		REM,
		RBRAC,
		LBRAC,
		LAST
	}

	public interface IBasicLex
	{
		int LineNumber
		{
			get;
		}

		LexToken Token
		{
			get;
		}

		string Lexum
		{
			get;
		}

		bool TreatCrAsWs
		{
			get; set;
		}

		bool IsCurrentTokenOperator
		{
			get;
		}

		void AppendInput(string text);
		bool Next();
		void Clear();

		string RemainingText();
		string RemainingLine();
	}
}
