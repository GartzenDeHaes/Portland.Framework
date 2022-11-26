using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics.Query
{
	public struct VariantCompare
	{
		public enum Operator
		{
			NONE = 0,
			BIT_AND,
			BIT_OR,
			BIT_XOR,
			EQ,
			NEQ,
			GT,
			GTEQ,
			LT,
			LTEQ,
		}

		private Operator _op;

		public VariantCompare(Operator op)
		{
			_op = op;
		}

		public bool Compare(Variant8 left, Variant8 right)
		{
			switch (_op)
			{
				case Operator.NONE:
					return false;
				case Operator.BIT_AND:
					return (left.ToInt() & right.ToInt()) != 0;
				case Operator.BIT_OR:
					return (left.ToInt() | right.ToInt()) != 0;
				case Operator.BIT_XOR:
					return (left.ToInt() ^ right.ToInt()) != 0;
				case Operator.EQ:
					return left.Equals(right);
				case Operator.NEQ:
					return !left.Equals(right);
				case Operator.GT:
					if (left.TypeIs == VariantType.Float || right.TypeIs == VariantType.Float)
					{
						return left.ToFloat() > right.ToFloat();
					}
					return left.ToInt() > right.ToInt();
				case Operator.GTEQ:
					if (left.TypeIs == VariantType.Float || right.TypeIs == VariantType.Float)
					{
						return left.ToFloat() >= right.ToFloat();
					}
					return left.ToInt() >= right.ToInt();
				case Operator.LT:
					if (left.TypeIs == VariantType.Float || right.TypeIs == VariantType.Float)
					{
						return left.ToFloat() < right.ToFloat();
					}
					return left.ToInt() < right.ToInt();
				case Operator.LTEQ:
					if (left.TypeIs == VariantType.Float || right.TypeIs == VariantType.Float)
					{
						return left.ToFloat() <= right.ToFloat();
					}
					return left.ToInt() <= right.ToInt();
			}
			return false;
		}

		public static VariantCompare Parse(string txt)
		{
			Operator op;

			switch (txt)
			{
				case "&":
					op = Operator.BIT_AND;
					break;
				case "|":
					op = Operator.BIT_OR;
					break;
				case "^":
					op = Operator.BIT_XOR;
					break;
				case "==":
					op = Operator.EQ;
					break;
				case "!=":
					op = Operator.NEQ;
					break;
				case ">":
					op = Operator.GT;
					break;
				case ">=":
					op = Operator.GTEQ;
					break;
				case "<":
					op = Operator.LT;
					break;
				case "<=":
					op = Operator.LTEQ;
					break;
				default:
					throw new Exception($"Unknown operator: {txt}");
			}

			return new VariantCompare(op);
		}
	}
}
