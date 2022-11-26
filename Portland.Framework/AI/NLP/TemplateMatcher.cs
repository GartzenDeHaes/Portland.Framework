using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.Semantics;
using Portland.ComponentModel;
using Portland.Text;

namespace Portland.AI.NLP
{
	/// <summary>
	/// Example:
	/// var db = new TemplateMatcher("Act");
	/// db.DefineOutputSlots(new string[] { "AgentName", "Act", "DoName", "IoName", "LocationName" });
	/// db.DefineSymbolClasses(new string[] { "V", "N", "ADJ", "DET", "ADV", "PRO", "PREP", "CON", "IMP", "*" });
	///
	/// db.DefineSymbolSet("CON", new string[] { "and", "but", "for", "or", "nor", "so", "yet" });
	/// db.DefineSymbolSet("DET", new string[] { "a", "an", "the" });
	/// db.DefineSymbolSet("PRP", new string[] { "at", "by", "in", "from", "into", "of", "off", "on", "with" });
	/// db.DefineSymbolSet("PRO", new string[] { "i", "me", "they", "them", "their", "to", "us", "we", "you" });
	///
	/// db.DefineTemplateKey("examine", new string[] { "examined", "look", "looked", "stare", "stared" });
	/// db.DefineTemplateKey("say", new string[] { "said" });
	/// db.DefineTemplateKey("see", new string[] { "saw" });
	/// db.DefineTemplateKey("take", new string[] { "took", "grabbed", "picked" });
	/// db.DefineTemplateKey("attack", new string[] { "attacked", "hit" });
	///
	/// db.DefineTemplate("AgentName=N:human Act=V:attack DET DoName=N PREP DET IoName=N:weapon [.|]");
	///
	/// string sen4 = "Rochelle hit the clown with the axe.";
	/// 
	/// Assert.IsTrue(db.Match(sen4, out paev));
	/// Assert.AreEqual("attack", paev.Act);
	/// Assert.AreEqual("rochelle", paev.AgentName);
	/// Assert.AreEqual("clown", paev.DoName);
	/// Assert.AreEqual("axe", paev.IoName);
	///
	/// </summary>
	public class TemplateMatcher
	{
		class MatchItem
		{
			public string Slot;					// ACT, AGENT, DOBJ
			public string SymbolClass;			// V, N, DET, PREP
			public string[] SymbolFilters;	// SymbolClass or literal
			public string SemanticType;		// human, weapon, door
			public bool IsOptional;
		}

		class MatchGroup
		{
			public MatchItem[] Items;
			public int MinMatchSize;
		}

		private string _keySymbolSetName;
		private string[] _slots;
		private Dictionary<string, HashSet<Word>> _symbolSets = new Dictionary<string, HashSet<Word>>();
		private Dictionary<string, string[]> _keys = new Dictionary<string, string[]>();
		private List<MatchGroup> _matchGroups = new List<MatchGroup>();

		public TemplateMatcher()
		{
			_keySymbolSetName = String.Empty;
		}

		/// <summary>
		/// Match word lists based on templates
		/// Key name maps a slot to a templates key. This lets the template work without a noun list
		/// </summary>
		/// <param name="keyName">Maps output slot (_slots) to a templdate key (_keys)</param>
		public TemplateMatcher(string keyName)
		{
			_keySymbolSetName = keyName;
		}

		public void DefineOutputSlots(string[] slots)
		{
			_slots = (string[])slots.Clone();
		}

		/// <summary>
		/// Primitive acts for NLP case.
		/// </summary>
		public void DefineTemplateKey(string present, string[] synonyms = null)
		{
			_keys.Add(present, synonyms);
		}

		/// <summary>
		/// Defines a named group of symbols.  For example, DET -> [a,the]
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="symbols"></param>
		public void DefineSymbolSet(string cls, string[] symbols)
		{
			if (!_symbolSets.TryGetValue(cls, out var words))
			{
				words = new HashSet<Word>();
				_symbolSets.Add(cls, words);
			}

			for (int x = 0; x < symbols.Length; x++)
			{
				words.Add(new Word(symbols[x]));
			}
		}

		public void DefineSymbolClasses(string[] clss)
		{
			for (int x = 0; x < clss.Length; x++)
			{
				DefineSymbolClass(clss[x]);
			}
		}

		public void DefineSymbolClass(string cls)
		{
			if (!_symbolSets.ContainsKey(cls))
			{
				_symbolSets.Add(cls, new HashSet<Word>());
			}
		}

		public void DefineTemplate(string clslst)
		{
			DefineTemplate(clslst.Split(new char[] { ' ' }));
		}

		/// <summary>
		/// Each class spec in the list has the form SLOT=CLS[lits|morelists]:type where SLOT and type are optional
		/// </summary>
		public void DefineTemplate(string[] clslst)
		{
			MatchItem[] items = new MatchItem[clslst.Length];
			MatchItem keyItem = null;
			int minMatchCount = 0;

			for (int x = 0; x < clslst.Length; x++)
			{
				string clsspec = clslst[x];
				var item = ParseMatchItem(clsspec);
				items[x] = item;

				if (item.Slot == _keySymbolSetName)
				{
					keyItem = item;
				}

				minMatchCount += item.IsOptional ? 0 : 1;

				Validate(item);
			}

			if (keyItem == null)
			{
				throw new Exception("Template spec must include entry for KEY slot " + _keySymbolSetName);
			}

			_matchGroups.Add(new MatchGroup() { Items = items, MinMatchSize = minMatchCount });
		}

		private void Validate(MatchItem item)
		{
			if (!String.IsNullOrWhiteSpace(item.Slot))
			{
				if (!_slots.Contains(item.Slot))
				{
					throw new Exception(item.Slot + " not found in slots defined by DefineOutputSlots()");
				}
			}

			if (!String.IsNullOrWhiteSpace(item.SymbolClass))
			{
				if (! _symbolSets.ContainsKey(item.SymbolClass))
				{
					throw new Exception(item.SymbolClass + " not found in symbol sets defined by DefineSymbolSet()");
				}
			}

			for (int x = 0; x < item.SymbolFilters.Length; x++)
			{
				string filter = item.SymbolFilters[x];
				if (StringHelper.IsUpperAlphaOnly(filter))
				{
					if (!_symbolSets.ContainsKey(filter))
					{
						throw new Exception("Literal class " + filter + " of slot " + item.Slot + " not found in symbol sets defined by DefineSymbolSet()");
					}
				}
			}

			if (!String.IsNullOrWhiteSpace(item.SemanticType))
			{
				// TODO: must name semantic type or instance in SemanticDb
			}

			if (item.Slot == _keySymbolSetName)
			{
				var pa = item.SemanticType;

				if (!String.IsNullOrWhiteSpace(pa))
				{
					//item.SymbolFilters = new string[] { pa };
					//item.SemanticType = null;

					if (!SearchKeys(pa, out var nkey))
					{
						throw new Exception("Key " + pa + " not found in collections defined by DefineSymbolSet()");
					}
				}
			}
		}

		private bool SearchKeys(string name, out string primaryKeyName)
		{
			if (_keys.ContainsKey(name))
			{
				primaryKeyName = name;
				return true;
			}
			foreach (string key in _keys.Keys)
			{
				if (_keys[key].Contains(name))
				{
					primaryKeyName = key;
					return true;
				}
			}
			primaryKeyName = String.Empty;
			return false;
		}

		private MatchItem ParseMatchItem(string clsspec)
		{
			MatchItem mitem = new MatchItem();

			if (clsspec.IndexOf('[') > -1)
			{
				mitem.SymbolFilters = new string[StringHelper.CountOccurancesOf(clsspec, '|') + 1];
			}
			else
			{
				mitem.SymbolFilters = new string[0];
			}

			using (SimpleLex lex = new SimpleLex(clsspec))
			{
				lex.Next();
				if (lex.TypeIs != SimpleLex.TokenType.PUNCT || lex.Lexum.Length == 0 || lex.Lexum[0] != '[')
				{
					string slotOrCls = lex.Lexum.ToString();
					lex.Next();
					if (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '=')
					{
						mitem.Slot = slotOrCls;
						lex.Next();
						mitem.SymbolClass = lex.Lexum.ToString();
						lex.Next();
					}
					else if (_slots.Contains(slotOrCls))
					{
						mitem.Slot = slotOrCls;
						lex.Next();
					}
					else
					{
						mitem.SymbolClass = slotOrCls;
						mitem.IsOptional = true;
					}
				}
				if (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '[')
				{
					lex.Next();

					for (int i = 0; i < mitem.SymbolFilters.Length; i++)
					{
						if (!lex.Lexum.IsEqualTo("|") && !lex.Lexum.IsEqualTo("]"))
						{
							mitem.SymbolFilters[i] = lex.Lexum.ToString();
							lex.Next();
						}
						else
						{
							mitem.SymbolFilters[i] = String.Empty;
							mitem.IsOptional = true;
						}

						if (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '|')
						{
							lex.Next();
						}
					}
					lex.Match(SimpleLex.TokenType.PUNCT);
				}

				if (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == ':')
				{
					lex.Next();
					mitem.SemanticType = lex.Lexum.ToString();
					lex.Match(SimpleLex.TokenType.ID);
				}
			}

			return mitem;
		}

		public bool Match(string sentence, out SemanticEvent paev)
		{
			paev = new SemanticEvent();

			WordGroup group = WordGroup.FromSimpleEnglishSentence(sentence);
			int count = group.Count;

			foreach (MatchGroup matchg in _matchGroups)
			{
				if (count < matchg.MinMatchSize || count > matchg.Items.Length)
				{
					continue;
				}

				if (Accept(matchg, group, paev))
				{
					return true;
				}
			}

			return false;
		}

		private bool Accept(MatchGroup group, WordGroup words, SemanticEvent paev)
		{
			Word word = words.Words;
			word = word.WordNext;

			int matchCount = 0;
			paev.Reset();

			for (int x = 0; word != (Word)null && x < group.Items.Length; x++)
			{
				MatchItem item = group.Items[x];

				if (item.SymbolClass != null && _symbolSets.TryGetValue(item.SymbolClass, out var hashset))
				{
					if (hashset.Count > 0 && !hashset.Contains(word))
					{
						break;
					}
				}

				bool foundAny = item.SymbolFilters.Length == 0;
				bool skipWord = false;

				for (int y = 0; y < item.SymbolFilters.Length; y++)
				{
					string tok = item.SymbolFilters[y];

					if (StringHelper.IsUpperAlphaOnly(tok))
					{
						// SymbolClass V, N, DET, PREP, ...
						if (_symbolSets.TryGetValue(tok, out var hashset2))
						{
							if (hashset2.Contains(word))
							{
								foundAny = true;
								skipWord = false;
								break;
							}
						}
					}
					else if (String.IsNullOrEmpty(tok))
					{
						foundAny = true;
						skipWord = true;
					}
					else if (word == tok)
					{
						// literal
						foundAny = true;
						skipWord = false;
						break;
					}
				}

				if (!foundAny)
				{
					break;
				}

				if (skipWord)
				{
					matchCount++;
					continue;
				}

				// TODO: lookup semantic type in SemanticDB

				if (!String.IsNullOrWhiteSpace(item.Slot))
				{
					if (_keys.ContainsKey(word.Lexum))
					{
						paev.SetProperty(item.Slot, word);
					}
					else if (item.Slot == _keySymbolSetName)
					{
						bool didSet = false;

						foreach (var key in _keys.Keys)
						{
							if (_keys[key].Contains(word.Lexum))
							{
								paev.SetProperty(item.Slot, key);
								didSet = true;
								break;
							}
						}

						if (!didSet)
						{
							return false;
						}
					}
					else
					{
						paev.SetProperty(item.Slot, word);
					}
				}
				matchCount++;
				word = word.WordNext;
			}

			return matchCount >= group.MinMatchSize && matchCount <= group.Items.Length;
		}

		public void LoadXml(string xmltxt)
		{
			XmlLex xml = new XmlLex(xmltxt, true);
			xml.MatchTag("document");

			if (xml.Lexum.IsEqualTo("outputs"))
			{
				xml.MatchTagStart("outputs");

				if (xml.Lexum.IsEqualTo("key"))
				{
					_keySymbolSetName = xml.MatchProperty("key");
				}

				List<string> slots = _slots == null ? new List<string>() : _slots.ToList();

				while (xml.Token == XmlLex.XmlLexToken.STRING)
				{
					slots.Add(xml.Lexum.ToString());
					xml.Next();
				}
				xml.Match(XmlLex.XmlLexToken.TAG_END);

				_slots = slots.ToArray();
			}

			xml.MatchTagStart("symbol_classes");
			while (xml.Token == XmlLex.XmlLexToken.STRING)
			{
				//Assert.IsTrue("V N ADJ DET ADV PRO PREP CON IMP * ".IndexOf(xml.Lexum.ToString() + " ") > -1);
				DefineSymbolClass(xml.Lexum.ToString());

				xml.Next();
			}
			xml.Match(XmlLex.XmlLexToken.TAG_END);

			xml.MatchTag("symbol_sets");
			while (xml.Token == XmlLex.XmlLexToken.TAG_START)
			{
				xml.MatchTagStart("symbol_set");

				string symbol = xml.MatchProperty("name");

				if (!_symbolSets.TryGetValue(symbol, out var words))
				{
					throw new Exception(symbol + " must be defined in symbol_classes");
				}

				while (xml.Token == XmlLex.XmlLexToken.STRING)
				{
					string word = xml.Lexum.ToString();
					words.Add(new Word(word));

					xml.Next();
				}

				xml.Match(XmlLex.XmlLexToken.TAG_END);
			}
			xml.MatchTagClose("symbol_sets");

			xml.MatchTag("keys");
			while (xml.Lexum.IsEqualTo("key"))
			{
				xml.Next();

				string name = xml.MatchProperty("name");
				List<string> syns = new List<string>();

				while (xml.Token == XmlLex.XmlLexToken.STRING)
				{
					string word = xml.Lexum.ToString();
					syns.Add(word);
					xml.Next();
				}

				_keys.Add(name, syns.ToArray());

				xml.Match(XmlLex.XmlLexToken.TAG_END);
			}
			xml.MatchTagClose("keys");

			xml.MatchTag("templates");
			while (xml.Lexum.IsEqualTo("template"))
			{
				xml.Next();

				string text = xml.MatchProperty("text");
				DefineTemplate(text);

				xml.Match(XmlLex.XmlLexToken.TAG_END);
			}
			xml.MatchTagClose("templates");

			xml.MatchTagClose("document");
		}
	}
}
