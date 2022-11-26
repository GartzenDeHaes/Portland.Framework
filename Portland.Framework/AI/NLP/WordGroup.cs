using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.AI.NLP
{
	public enum WordGroupMood
	{
		Inform = 0,
		Question = 1,
		Imperative = 2
	}

	public class WordGroup
	{
		public Word Words;
		public int Count;
		//public PartsOfSpeech[] PosTags;
		public WordGroupMood Mood;

		/// <summary>
		/// Basic sentences that do not include commas, semicolons, double dash (--), ellipsis, or quotes.
		/// </summary>
		/// <param name="sentence"></param>
		/// <returns></returns>
		public static WordGroup FromSimpleEnglishSentence(string sentence)
		{
			WordGroup group = new WordGroup() { Mood = WordGroupMood.Inform };
			group.Words = new Word(String.Empty);
			Word current = group.Words;

			var words = sentence.ToLower().Split(new char[] { ' ', '.' });
			string word;

			for (int x = 0; x < words.Length; x++)
			{
				group.Count++;

				word = words[x];
				if (word.EndsWith("?"))
				{
					word = word.Substring(0, word.Length - 1);
					group.Mood = WordGroupMood.Question;
				}
				else if (word.EndsWith("!"))
				{
					word = word.Substring(0, word.Length - 1);
					group.Mood = WordGroupMood.Imperative;
				}

				current.WordNext = new Word(word);
				current.WordNext.WordPrev = current;
				current = current.WordNext;
			}

			return group;
		}
	}
}
