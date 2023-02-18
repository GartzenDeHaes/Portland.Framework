﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Framework.AI;
using Portland.Text;
using Portland.Types;

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
						ret = Value == fvalue.Value;
						break;
					case ComparisionOp.NotEquals:
						ret = Value != fvalue.Value;
						break;
					case ComparisionOp.GreaterThan:
						ret = Value > fvalue.Value;
						break;
					case ComparisionOp.GreaterThenEquals:
						ret = Value >= fvalue.Value;
						break;
					case ComparisionOp.LessThan:
						ret = Value < fvalue.Value;
						break;
					case ComparisionOp.LessThanOrEquals:
						ret = Value <= fvalue.Value;
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
