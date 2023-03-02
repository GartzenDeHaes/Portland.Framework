using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Text;

namespace Portland.AI.Dialogue
{
	public sealed class TextTemplate
	{
		public interface ITextItem
		{
			string Get(IBlackboard<string> blackboard);
		}

		public sealed class TextItem : ITextItem
		{
			public string Text;

			public string Get(IBlackboard<string> blackboard)
			{
				return Text;
			}
		}

		public sealed class TextVariable : ITextItem
		{
			public string PropertyId;

			public string Get(IBlackboard<string> blackboard)
			{
				return blackboard.Get(PropertyId).ToString();
			}
		}

		Vector<ITextItem> _texts = new Vector<ITextItem>(3);

		TextTemplate()
		{

		}

		public TextTemplate(string text)
		{
			_texts.Add(new TextItem { Text = text });
		}

		public static TextTemplate Parse(XmlLex lex)
		{
			TextTemplate t = new TextTemplate();

			while (!lex.IsEOF && lex.Token != XmlLex.XmlLexToken.TAG_START)
			{
				switch (lex.Token)
				{
					case XmlLex.XmlLexToken.EOF:
						return t;
					case XmlLex.XmlLexToken.STRING:
						lex.NextText();
						t._texts.Add(new TextItem { Text = lex.Lexum.ToString() });
						lex.Match(XmlLex.XmlLexToken.TEXT);
						break;
					case XmlLex.XmlLexToken.CODE_START:
						lex.Next();
						if (lex.Token == XmlLex.XmlLexToken.EQUAL)
						{
							lex.Next();
							t._texts.Add(new TextVariable { PropertyId = lex.Lexum.ToString().Substring(0, lex.Lexum.Length - 2) });
							lex.Match(XmlLex.XmlLexToken.STRING);
							lex.Match(XmlLex.XmlLexToken.CLOSE);
						}
						break;
					default:
						throw new Exception($"XML parser error on line {lex.LineNum}: '{lex.Lexum.ToString()}'");
				}
			}

			return t;
		}
	}
}
