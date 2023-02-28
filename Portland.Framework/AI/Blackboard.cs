using System;
using System.Collections.Generic;

using Portland.AI;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Types;

namespace Portland.Framework.AI
{
	public sealed class Blackboard<TKEY> : IBlackboard<TKEY>
	{
		Dictionary<TKEY, PropertyValue> _facts = new Dictionary<TKEY, PropertyValue>();
		IBlackboard<TKEY> _parent;

		public bool TryGetValue(in TKEY key, out PropertyValue value)
		{
			if (_facts.TryGetValue(key, out value))
			{
				return true;
			}
			return _parent?.TryGetValue(key, out value) ?? false;
		}

		public PropertyValue Get(in TKEY key)
		{
			if (_facts.TryGetValue(key, out var val))
			{
				return val;
			}
			return _parent?.Get(key) ?? null;
		}

		public void Set(in TKEY key, in Variant8 value)
		{
			_facts[key].Set(value);
		}

		public bool ContainsKey(in TKEY key)
		{
			return _facts.ContainsKey(key) || (_parent?.ContainsKey(key) ?? false);
		}

		public void Add(in TKEY id, PropertyValue value)
		{
			_facts.Add(id, value);
		}

		public int Count { get { return _facts.Count; } }

		public Blackboard()
		{
		}

		public Blackboard(IBlackboard<TKEY> parent)
		{
			_parent = parent;
		}

		//		public const int ViewBufferSize = 8;

		//		public AgentStateFlags Flags;
		//		public string Faction;			//!< Set by PrimitiveActor
		//		public string Goal;				// Purpose of activity such as MEAL, REST, DEFEND
		//		public string Frame;			// Activity group or context such as WORK, HOME, VIOLENCE
		//		public string Verb;				// *, GO, TAKE, ATTACK

		//		public RaycastHit RayAimTarget;		//!< Set by AgentRaycaster
		//		public RaycastHit RayGround;        //!< Set by AgentRaycaster
		//		public float RayUpDistance;         //!< Set by AgentRaycaster
		//		public float RaySteerLeft;				//!< Set by AgentRaycaster
		//		public float RaySteerRight;         //!< Set by AgentRaycaster
		//		public float RaySteerBack;				//!< Set by AgentRaycaster

		//		public Vector3 Position;
		//		public float AgentHeight;
		//		public bool IsGrounded = true;		//!< Set by AgentRaycaster
		//		public float Velocity;
		//		public float SpeedWalk;
		//		public float SpeedRun;

		//		public GameObject DirectObject;
		//		public Vector3 NavPoint;

		//		// These are set by AgentRaycaster
		//		public Collider[] NearbyColliders = new Collider[ViewBufferSize];
		//		public short NearbyCollidersNum;
		//		public float[] NearbyDistance = new float[ViewBufferSize];			
		//		public short[] FovIdx = new short[ViewBufferSize];					//!< Note, these will always be in ascending order	
		//		public short FovNum;
		//		public short[] FovNpcIdx = new short[ViewBufferSize];				//!< NPC's
		//		public short FovNpcNum;
		//		public short[] FovResIdx = new short[ViewBufferSize];				//!< Resources
		//		public short FovResNum;
		//		public short[] FovPickupIdx = new short[ViewBufferSize];			//!< Pickups
		//		public short FovPickupNum;

		//		public DateTime GameTime { get { return SceneContext.Instance.GameTime; } }   //!< Set by GameClockManager on SceneManagers object
		//		public int HourOfDay { get { return SceneContext.Instance.GameTime.Hour; } }	//!< Set by GameClockManager on SceneManagers object
		//		public bool IsNightTime { get { return SceneContext.Instance.IsNightTime; } } //!< Set by GameClockManager on SceneManagers object
		//		public bool IsWeekend { get { return SceneContext.Instance.IsWeekend; } }		//!< Set by GameClockManager on SceneManagers object

		List<string> _keys = new List<string>();
	}
}
