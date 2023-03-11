using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI;

using Portland.Collections;
using Portland.Text;

namespace Portland.RPG.Dialogue
{
	public sealed class TextTemplate
	{
		public interface ITextItem
		{
			string Get(IBlackboard<string> blackboard);
		}

		public sealed class TextItem : ITextItem
		{
			public readonly string Text;

			public TextItem(string txt)
			{
				Text = txt;
			}

			public string Get(IBlackboard<string> blackboard)
			{
				return Text;
			}
		}

		public sealed class TextVariable : ITextItem
		{
			public string PropertyId;

			public TextVariable(string propId)
			{
				PropertyId = propId;
			}

			public string Get(IBlackboard<string> blackboard)
			{
				return blackboard.Get(PropertyId).ToString();
			}
		}

		public Vector<ITextItem> Texts = new Vector<ITextItem>(3);

		TextTemplate()
		{
		}

		public TextTemplate(string text)
		{
			Texts.Add(new TextItem(text));
		}

		public string Get(IBlackboard<string> blackboard)
		{
			StringBuilder buf = new StringBuilder();

			for (int i = 0; i < Texts.Count; i++)
			{
				buf.Append(Texts[i].Get(blackboard));
			}

			return buf.ToString();
		}

		public static TextTemplate Parse(string txt)
		{
			TextTemplate t = new TextTemplate();

			int pos;
			if ((pos = txt.IndexOf('{')) < 0)
			{
				return new TextTemplate(txt);
			}

			StringTokenizer lex = new StringTokenizer(txt);

			while (!lex.IsEOF)
			{
				if (lex.PeekChar() == '{')
				{
					lex.Match("{");

					lex.Match("$");

					t.Texts.Add(new TextVariable(lex.ReadToAny('}')));

					lex.Match("}");
				}
				else
				{
					t.Texts.Add(new TextItem(lex.ReadToAny('{')));
				}

				lex.SkipWhitespace();
			}

			return t;
		}

		//public static TextTemplate Parse(XmlLex lex)
		//{
		//	TextTemplate t = new TextTemplate();

		//	while (!lex.IsEOF && lex.Token != XmlLex.XmlLexToken.TAG_START)
		//	{
		//		switch (lex.Token)
		//		{
		//			case XmlLex.XmlLexToken.EOF:
		//				return t;
		//			case XmlLex.XmlLexToken.STRING:
		//				lex.NextText();
		//				t._texts.Add(new TextItem { Text = lex.Lexum.ToString() });
		//				lex.Match(XmlLex.XmlLexToken.TEXT);
		//				break;
		//			case XmlLex.XmlLexToken.CODE_START:
		//				lex.Next();
		//				if (lex.Token == XmlLex.XmlLexToken.EQUAL)
		//				{
		//					lex.Next();
		//					t._texts.Add(new TextVariable { PropertyId = lex.Lexum.ToString().Substring(0, lex.Lexum.Length - 2) });
		//					lex.Match(XmlLex.XmlLexToken.STRING);
		//					lex.Match(XmlLex.XmlLexToken.CLOSE);
		//				}
		//				break;
		//			default:
		//				throw new Exception($"XML parser error on line {lex.LineNum}: '{lex.Lexum.ToString()}'");
		//		}
		//	}

		//	return t;
		//}
	}
}
