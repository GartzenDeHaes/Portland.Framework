using System;
using System.Collections.Generic;

using Portland.AI;
using Portland.Collections;
using Portland.ComponentModel;

namespace Portland.Framework.AI
{
	public sealed class Blackboard : IBlackboard
	{
		public StringTable Strings;

		Dictionary<StringTableToken, IObservableValue<Variant8>> _facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();

		public bool TryGetValue(in StringTableToken tok, out IObservableValue<Variant8> value)
		{
			return _facts.TryGetValue(tok, out value);
		}

		public bool TryGetValue(string key, out IObservableValue<Variant8> value)
		{
			return _facts.TryGetValue(Strings.Get(key), out value);
		}

		public IObservableValue<Variant8> Get(in StringTableToken key)
		{
			return _facts[key];
		}

		public IObservableValue<Variant8> Get(string key)
		{
			return _facts[Strings.Get(key)];
		}

		public bool ContainsKey(in StringTableToken tok)
		{
			return _facts.ContainsKey(tok);
		}

		public bool ContainsKey(string key)
		{
			return _facts.ContainsKey(Strings.Get(key));
		}

		public void Add(in StringTableToken tok, IObservableValue<Variant8> value)
		{
			_facts.Add(tok, value);
		}

		public Blackboard(StringTable strings)
		{
			Strings = strings;
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
