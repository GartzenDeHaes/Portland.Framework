using System;

namespace Portland.Mathmatics
{
	public struct DiceTerm
	{
		public sbyte Rolls;
		public short Sides;
		public sbyte PlusConst;

		public int Minimum
		{
			get { return Rolls + PlusConst; }
		}

		public int Maximum
		{
			get { return Rolls * Sides + PlusConst; }
		}

		public DiceTerm(sbyte rolls, short sides, sbyte constterm)
		{
			Rolls = rolls;
			Sides = sides;
			PlusConst = constterm;
		}

		public int Roll(IRandom rnd)
		{
			int total = 0;
			for (int i = 0; i < Rolls; i++)
			{
				total += rnd.Range(1, Sides);
			}

			return total + PlusConst;
		}

		public static int Roll(IRandom rnd, in String8 expression)
		{
			var dice = Parse(expression);
			return dice.Roll(rnd);
		}

		static short ScanInt(in String8 expression, ref int pos)
		{
			int tens = 1;
			int ret = 0;

			while (pos < expression.Length && Char.IsDigit(expression[pos]))
			{
				ret += (expression[pos++] - 48) * tens;
				tens *= 10;
			}

			return (short)ret;
		}

		public static DiceTerm Parse(in String8 expression)
		{
			DiceTerm dice = new DiceTerm { Rolls = 1, Sides = 1 };
			int pos = 0;

			if (pos < expression.Length)
			{
				if (Char.IsDigit(expression[pos]))
				{
					dice.Rolls = (sbyte)ScanInt(expression, ref pos);
				}

				if (pos < expression.Length)
				{
					if (expression[pos] == 'd' || expression[pos] == 'D')
					{
						pos++;
						
						dice.Sides = ScanInt(expression, ref pos);

						if (pos < expression.Length)
						{
							if (expression[pos] == '+')
							{
								pos++;
								dice.PlusConst= (sbyte)ScanInt(expression, ref pos);
							}
							else if (expression[pos] == '-')
							{
								pos++;
								dice.PlusConst = (sbyte)-ScanInt(expression, ref pos);
							}
							else
							{
								throw new Exception($"Expoected + or - but found {expression[pos]}");
							}
						}
					}
				}
			}
			if (pos < expression.Length)
			{
				throw new Exception($"Unexpected character '{expression[pos]}' found in dice expression {expression}");
			}
			if (dice.Sides == 1 && dice.Rolls != 1 && dice.PlusConst == 0)
			{
				dice.PlusConst = dice.Rolls;
				dice.Rolls = 0;
			}
			return dice;
		}
	}
}
