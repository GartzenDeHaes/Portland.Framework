using System;
using System.Collections.Generic;
using System.Text;

using Portland.Collections;
using Portland.Interp;

namespace Portland.CodeDom
{
	public class Expression
	{
		public Expression Parent;

		private Expression _left;
		public Expression Left
		{
			get { return _left; }
			set
			{
				_left = value;
				if (_left != null)
				{
					_left.Parent = this;
				}
			}
		}

		private Expression _right;
		public Expression Right
		{
			get { return _right; }
			set
			{
				_right = value;
				if (_right != null)
				{
					_right.Parent = this;
				}
			}
		}

		public DomNode Operator;

		public Expression()
		{
			// probably need to change this to collect, or recurse.
			Operator = DomNode.GetDoNothing();
		}

		public Expression(DomNode unary)
		{
			Operator = unary;
		}

		public Expression(DomNode unary, Expression leftHandSide)
		{
			Operator = unary;
			Left = leftHandSide;
		}

		public Expression(DomNode op, Expression leftHandSide, Expression right)
		{
			Operator = op;
			Left = leftHandSide;
			Right = right;
		}

		public virtual Variant Execute(ExecutionContext ctx)
		{
			return Operator.Execute(ctx, _left, _right);
		}
	}
}
