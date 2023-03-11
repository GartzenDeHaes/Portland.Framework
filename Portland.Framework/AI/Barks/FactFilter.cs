﻿using System;

using Portland.Text;

namespace Portland.AI.Barks
{
	public enum ComparisionOp
	{
		Equals,
		NotEquals,
		GreaterThan,
		GreaterThenEquals,
		LessThan,
		LessThanOrEquals,
		Exists,
		NotExists,
		PaternMatch
	}

	public sealed class FactFilter
	{
		public String ActorName;
		public string FactName;
		public ComparisionOp Op;
		public Variant8 Value;

		public bool IsMatch(IBlackboard<String> facts)
		{
			bool ret = false;

			if (facts.TryGetValue(FactName, out var fvalue))
			{
				switch (Op)
				{
					case ComparisionOp.Equals:
						ret = fvalue.Value == Value;
						break;
					case ComparisionOp.NotEquals:
						ret = fvalue.Value != Value;
						break;
					case ComparisionOp.GreaterThan:
						ret = fvalue.Value > Value;
						break;
					case ComparisionOp.GreaterThenEquals:
						ret = fvalue.Value >= Value;
						break;
					case ComparisionOp.LessThan:
						ret = fvalue.Value < Value;
						break;
					case ComparisionOp.LessThanOrEquals:
						ret = fvalue.Value <= Value;
						break;
					case ComparisionOp.Exists:
						ret = true;
						break;
					case ComparisionOp.NotExists:
						ret = false;
						break;
					case ComparisionOp.PaternMatch:
						ret = StringHelper.Like(fvalue.ToString(), Value.ToString());
						break;
				}
			}
			else if (Op == ComparisionOp.NotExists)
			{
				ret = true;
			}

			return ret;
		}
	}
}
