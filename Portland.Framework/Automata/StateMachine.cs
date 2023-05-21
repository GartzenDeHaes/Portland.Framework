using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

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

		//Dictionary<STATE, State> _states = new();
		State[] _states;
		Vector<StateTransition> StateTransitions = new ();

		public STATE StartState;
		public State CurrentState;
		public State NextState;

		StateTransition CurrentTransition;

		public bool DebuggingEnabled { get; private set; }

		public StateMachine()
		{
			var states = Enum.GetValues<STATE>();
			_states = new State[states.Length];

			for (int i = 0; i < states.Length; i++)
			{
				_states[i] = new State() { StateId = states[i] };
			}

			CurrentState = _states[0];
		}

		public StateMachine(string name)
		: this()
		{
			StateMachineName = name;
		}

		public StateMachine(STATE start)
		: this()
		{
			StartState = start;

			CurrentState = _states[Convert.ToInt32(start)];
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

			//StateTransition triggerTransition = StateTransitions.Find(transition => EqualityComparer<STATE>.Default.Equals(transition.EntryState.StateId, currentStateTag) && EqualityComparer<EVENT>.Default.Equals(transition.TriggerName, trigger));

			StateTransition triggerTransition = null;

			for (int i = 0; i < StateTransitions.Count; i++)
			{
				var transition = StateTransitions[i];

				if (EqualityComparer<STATE>.Default.Equals(transition.EntryState.StateId, currentStateTag) && EqualityComparer<EVENT>.Default.Equals(transition.TriggerName, trigger))
				{
					triggerTransition = transition;
					break;
				}
			}

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

		public StateBuilder BuildState(STATE stateId)
		{
			return new StateBuilder(_states[Convert.ToInt32(stateId)], this);
		}

		public void SetStartState(STATE name)
		{
			StartState = name;
			CurrentState = _states[Convert.ToInt32(name)];
		}

		public void GoToState(STATE state)
		{
			CurrentState = _states[Convert.ToInt32(state)];
		}

		public void GoToState(State state)
		{
			CurrentState = state;
		}

		public State GetState(STATE name)
		{
			return _states[Convert.ToInt32(name)];
		}

		public void ClearCurrentTransition() => CurrentTransition = null;

		public void SetDebugging(bool debuggingEnabled) => DebuggingEnabled = debuggingEnabled;
	}
}
