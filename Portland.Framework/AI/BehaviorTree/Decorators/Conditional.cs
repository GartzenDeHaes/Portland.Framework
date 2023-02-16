using System;
using System.Collections.Generic;

using Portland.AI.Barks;
using Portland.Basic;
using Portland.Text;

namespace Portland.Framework.AI.BehaviorTree.Decorators
{
	public class Conditional : Decorator
	{
		ComparisionOp _op;

		public NodeState StateOnConditionOnFalse = NodeState.Failure;

		//public Blackboard AgentBlackboard;

		//private BasicProgram _prog;

		public Conditional()
		{
			Description = "Aborts when Code evaluates to false";
		}

		//public override BtNode Clone(Blackboard bb)
		//{
		//	var con = (Conditional)base.Clone(bb);
		//	con.name = con.name.Replace("(Clone)", "*");

		//	con._jskey = "BtConditional" + _jskeyGen++;
		//	con.AgentBlackboard = bb;

		//	return con;
		//}

		protected override void OnStart()
		{
			// JsEngine singleton
			//JsContext.Instance.JS.NativeObjectAdd(this, _jskey);

			//_prog = JsContext.Instance.JS.PrepareProgram(Code, _jskey);
		}

		protected override void OnStop()
		{
			//JsContext.Instance.JS.GlobalRemove(_jskey);
			//_prog = null;

			Child?.Stop();
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			// Note: using Eval is an easy way to do live updates without writing an editor with a reload button
			//var ret = JsContext.Instance.JS.Eval(Code);

			//var ret = JsContext.Instance.JS.Exec(_prog);

			//if (Child != null && (bool)ret.ToNativeType(typeof(bool)))
			//{
			//	return Child.Update();
			//}
			//else
			//{
			//	Child.Stop();
			//	return StateOnConditionOnFalse;
			//}

			//bool ret = false;

			//if (facts.TryGetValue(FactName, out var fvalue))
			//{
			//	switch (Op)
			//	{
			//		case ComparisionOp.Equals:
			//			ret = Value == fvalue.Value;
			//			break;
			//		case ComparisionOp.NotEquals:
			//			ret = Value != fvalue.Value;
			//			break;
			//		case ComparisionOp.GreaterThan:
			//			ret = Value > fvalue.Value;
			//			break;
			//		case ComparisionOp.GreaterThenEquals:
			//			ret = Value >= fvalue.Value;
			//			break;
			//		case ComparisionOp.LessThan:
			//			ret = Value < fvalue.Value;
			//			break;
			//		case ComparisionOp.LessThanOrEquals:
			//			ret = Value <= fvalue.Value;
			//			break;
			//		case ComparisionOp.Exists:
			//			ret = true;
			//			break;
			//		case ComparisionOp.NotExists:
			//			ret = false;
			//			break;
			//		case ComparisionOp.PaternMatch:
			//			ret = StringHelper.Like(fvalue.ToString(), Value.ToString());
			//			break;
			//	}
			//}
			//else if (Op == ComparisionOp.NotExists)
			//{
			//	ret = true;
			//}

			//return ret;

			return NodeState.Success;
		}
	}
}
