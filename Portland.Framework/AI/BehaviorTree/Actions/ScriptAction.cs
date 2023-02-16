//using System;
//using System.Collections.Generic;

//namespace Portland.Framework.AI.BehaviorTree.Actions
//{
//	public class ScriptAction : ActionNode
//	{
//		private static int _jskeyGen;
//		private string _jskey;
//		private IJsObject _jsobj;
//		private float _startTime;

//		[NonSerialized]
//		public Blackboard AgentBlackboard;

//		[Tooltip("Will be ignored if zero")]
//		public float TimeoutInSeconds = 0f;
//		public NodeState StateReturnOnTimeout = NodeState.Success;
//		[Space]
//		[TextArea(10, 25)]
//		public string Code = @"function start()
//{
//}
//function stop()
//{
//}
//function update()
//{
//  return NodeState.Running;
//}
//";
		
//		public ScriptAction()
//		{
//			_jskey = "ScriptAction" + _jskeyGen++;
//		}

//		public override BtNode Clone(Blackboard bb)
//		{
//			var dup = base.Clone(bb);
//			dup.name = dup.name.Replace("(Clone)", "*");

//			((ScriptAction)dup).AgentBlackboard = bb;

//			return dup;
//		}

//		protected override void OnStart()
//		{
//			if (_jsobj == null)
//			{
//				_jsobj = JsContext.Instance.JS.NativeObjectAdd(this, _jskey);
//				JsContext.Instance.JS.Eval(Code, _jskey);
//			}
//			else
//			{
//				JsContext.Instance.JS.GlobalVarAdd(_jskey, _jsobj);
//			}

//			_startTime = Time.time;

//			JsContext.Instance.JS.TryRunFunction(_jskey, "start");
//		}

//		protected override void OnStop()
//		{
//			JsContext.Instance.JS.TryRunFunction(_jskey, "stop");

//			JsContext.Instance.JS.GlobalRemove(_jskey);
//		}

//		protected override NodeState OnUpdate()
//		{
//			if (TimeoutInSeconds > .001f && Time.time > _startTime + TimeoutInSeconds)
//			{
//				return StateReturnOnTimeout;
//			}

//			if (JsContext.Instance.JS.TryRunFunction(_jskey, "update"))
//			{
//				return (NodeState)JsContext.Instance.JS.ReturnValue.ToNativeType(typeof(NodeState));				
//			}

//			Debug.Log("update function missing in behavior tree ScriptAction");
//			return NodeState.Failure;
//		}
//	}
//}
