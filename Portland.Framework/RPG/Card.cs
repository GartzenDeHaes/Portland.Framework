using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public enum CardType
	{
		Unknown = 0,
		PropertyEffect,
		StatEffect,
		Race,
		Class,
		Item,
	}

	public class Card
	{
		public CardType CardType;
		public StatEffect[] StatEffects;
		public PropertyEffect[] PropertyEffects;
	}
}
