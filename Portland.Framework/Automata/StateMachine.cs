using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Automata
{
	public sealed class StateMachine<STATE, EVENT> where STATE : struct, Enum where EVENT : struct, Enum
	{
		public sealed class State
		{
			public STATE StateId;

			public Action OnUpdate;
			public Action OnLateUpdate;
			public Action OnFixedUpdate;

			public Action OnEnter;
			public Action OnExit;
			public Action<float> OnEnterTransition;
			public Action<float> OnExitTransition;
		}

		public sealed class StateBuilder
		{
			public class WhenDo
			{
				StateBuilder _sb;
				EVENT _trigger;

				public WhenDo(StateBuilder sb, EVENT trigger)
				{
					_sb = sb;
					_trigger = trigger;
				}

				public StateBuilder Do(STATE targetState, float transitionDuration = 0f)
				{
					_sb._stateMachine.StateTransitions.Add(new StateTransition(_sb._stateMachine, _sb._state, _sb._stateMachine.GetState(targetState), _trigger, transitionDuration));
					return _sb;
				}
			}

			State _state;
			StateMachine<STATE, EVENT> _stateMachine;

			public StateBuilder(State state, StateMachine<STATE, EVENT> stateMachine)
			{
				_state = state;
				_stateMachine = stateMachine;
			}

			public StateBuilder OnUpdate(Action sub) { _state.OnUpdate = sub; return this; }
			public StateBuilder OnLateUpdate(Action sub) { _state.OnLateUpdate = sub; return this; }
			public StateBuilder OnFixedUpdate(Action sub) { _state.OnFixedUpdate = sub; return this; }
			public StateBuilder OnEnter(Action sub) { _state.OnEnter = sub; return this; }
			public StateBuilder OnExit(Action sub) { _state.OnExit = sub; return this; }
			public StateBuilder OnEnterTransition(Action<float> sub) { _state.OnEnterTransition = sub; return this; }
			public StateBuilder OnExitTransition(Action<float> sub) { _state.OnExitTransition = sub; return this; }

			public WhenDo When(EVENT trigger)
			{
				return new WhenDo(this, trigger);
			}
		}

		public sealed class StateTransition
		{
			public EVENT TriggerName;
			public float TransitionDuration;
			public State EntryState;
			public State ExitState;

			StateMachine<STATE, EVENT> StateMachine;

			float _currentDuration;
			bool _transitionComplete;

			public StateTransition(StateMachine<STATE, EVENT> stateMachine, State entryState, State exitState, EVENT trigger, float transitionDuration = 0)
			{
				EntryState = entryState;
				ExitState = exitState;
				TriggerName = trigger;
				TransitionDuration = Math.Abs(transitionDuration);

				StateMachine = stateMachine;
			}

			public void Update(float deltaTime)
			{
				// run the old state and new state during the transition
				if (TransitionDuration > 0)
				{
					// 0 to 1, one is complete
					float t = Math.Clamp(_currentDuration / TransitionDuration, 0, 1f);
					EntryState.OnExitTransition?.Invoke(t);
					ExitState.OnEnterTransition?.Invoke(t);
				}

				if (_currentDuration + deltaTime >= TransitionDuration && !_transitionComplete)
				{
					Complete();
				}
				else
				{
					_currentDuration += deltaTime;
				}
			}

			public bool IsDone { get { return _currentDuration >= TransitionDuration; } }

			public void Reset()
			{
				_currentDuration = 0;
				_transitionComplete = false;
			}

			void Complete()
			{
				EntryState.OnExit?.Invoke();

				_currentDuration = TransitionDuration;
				_transitionComplete = true;

				StateMachine.GoToState(ExitState);
				StateMachine.CurrentState.OnEnter?.Invoke();

				if (StateMachine.CurrentTransition == this)
				{
					StateMachine.ClearCurrentTransition();
				}

				Reset();

				if (this.StateMachine.DebuggingEnabled)
				{
#if UNITY_5_3_OR_NEWER
					UnityEngine.Debug.Log($"[{TriggerName}] StateTransition is complete");
#else
					Debug.WriteLine($"[{TriggerName}] StateTransition is complete");
#endif
				}
			}
		}

		public string StateMachineName = "[StateMachine]";

		Dictionary<STATE, State> States = new();
		List<StateTransition> StateTransitions = new List<StateTransition>();

		public State StartState;
		public State CurrentState;
		public State NextState;

		StateTransition CurrentTransition;

		public bool DebuggingEnabled { get; private set; }

		public StateMachine()
		{
		}

		public StateMachine(string name)
		{
			StateMachineName = name;
		}

		public StateMachine(STATE start)
		{
			CreateState(start);
			SetStartState(start);
		}

		public StateMachine(string name, STATE start)
		: this(start)
		{
			StateMachineName = name;
		}

		public bool Trigger(EVENT trigger)
		{
			var currentStateTag = this.CurrentState.StateId;
			
			if (CurrentTransition != null)
			{
				CurrentTransition.Reset();
				CurrentTransition = null;
			}

			//StateTransition triggerTransition = this.StateTransitions.Find(transition => transition.EntryState.StateId == CurrentState.StateId && transition.TriggerName == trigger);
			StateTransition triggerTransition = StateTransitions.Find(transition => EqualityComparer<STATE>.Default.Equals(transition.EntryState.StateId, currentStateTag) && EqualityComparer<EVENT>.Default.Equals(transition.TriggerName, trigger));

			if (triggerTransition != null)
			{
				CurrentTransition = triggerTransition;
				return true;
			}
			else
			{
				if (DebuggingEnabled)
				{
#if UNITY_5_3_OR_NEWER
					UnityEngine.Debug.Log($"{StateMachineName} Trigger '{trigger}' not found for current active state '{currentStateTag}'");
#else
					Debug.WriteLine($"{StateMachineName} Trigger '{trigger}' not found for current active state '{currentStateTag}'");
#endif
				}

				return false;
			}
		}

		public void DoUpdate(float deltaTime)
		{
			CurrentState?.OnUpdate();
			CurrentTransition?.Update(deltaTime);
		}

		public void DoFixedUpdate()
		{
			CurrentState.OnFixedUpdate?.Invoke();
		}

		public void DoLateUpdate()
		{
			CurrentState.OnLateUpdate?.Invoke();
		}

		public StateBuilder CreateState(STATE stateId)
		{
			if (HasState(stateId))
			{
				throw new Exception($"{StateMachineName} already has state {stateId}");
			}
			var state = new State() { StateId = stateId };
			States.Add(stateId, state);
			return new StateBuilder(state, this);
		}

		public StateBuilder BuildState(STATE stateId)
		{
			if (TryGetState(stateId, out var state))
			{
				return new StateBuilder(state, this);
			}
			else
			{
				return CreateState(stateId);
			}
		}

		public void SetStartState(STATE name)
		{
			if (!HasState(name))
			{
				throw new Exception($"{StateMachineName} Start state {name} not found.");
			}

			State state = GetState(name);
			StartState = state;
			CurrentState = state;
		}

		public void GoToState(State state) => CurrentState = state;

		//public void AddTransition(STATE entryState, STATE exitState, EVENT triggerName, float transitionDuration = 0)
		//{
		//	if (this.StateTransitions.Any(transition => EqualityComparer<EVENT>.Default.Equals(transition.TriggerName, triggerName)))
		//	{
		//		throw new Exception($"{StateMachineName} Error adding state transition {triggerName}. State transition with this trigger name already exist in StateMachine.");
		//	}

		//	this.StateTransitions.Add(new StateTransition(this, GetState(entryState), GetState(exitState), triggerName, transitionDuration));
		//}

//		public void AddState(State stateInstance)
//		{
//			var name = stateInstance.StateId;

//			if (this.States.ContainsKey(name))
//			{
//				throw new Exception($"{StateMachineName} State {name} already exists in the StateMachine.");
//			}
//			else
//			{
//				if (this.DebuggingEnabled)
//				{
//#if UNITY_5_3_OR_NEWER
//					Debug.Log($"{StateMachineName} Adding state {name} to the StateMachine.");
//#else
//					Debug.WriteLine($"{StateMachineName} Adding state {name} to the StateMachine.");
//#endif
//				}

//				this.States.Add(name, stateInstance);
//			}
//		}

		public State GetState(STATE name)
		{
			if (States.ContainsKey(name))
			{
				return States[name];
			}

			throw new Exception(name + " state alread exists in the StateMachine.");
		}

		public bool TryGetState(STATE tag, out State stateInstance)
		{
			stateInstance = default(State);

			if (States.ContainsKey(tag))
			{
				stateInstance = States[tag];
				return true;
			}

			return false;
		}

		//public string GetStateTag<T>() where T : State => this.States.FirstOrDefault(pair => pair.Key == typeof(T).Name).Key;

		//public string GetStateTag(State stateInstance) => this.States.FirstOrDefault(pair => pair.Value == stateInstance).Key;

		public bool HasState(STATE name)
		{
			return States.ContainsKey(name);
		}

		public void AddState(STATE state)
		{
			CreateState(state);
		}

		public void AddStates()
		{
			var states = Enum.GetValues<STATE>();
			for (int i = 0; i < states.Length; i++)
			{
				if (!HasState(states[i]))
				{
					CreateState(states[i]);
				}
			}
		}

		public void ClearCurrentTransition() => CurrentTransition = null;

		public void SetDebugging(bool debuggingEnabled) => DebuggingEnabled = debuggingEnabled;
	}
}
