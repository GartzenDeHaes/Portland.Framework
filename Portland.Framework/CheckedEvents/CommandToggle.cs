using System;

#if UNITY_2019_1_OR_NEWER
using UnityEngine;
#else
using Portland.Mathmatics;
#endif

namespace Portland.CheckedEvents
{
	public sealed class CommandToggle : Marshallable
	{
		public bool Active { get; private set; }
		public bool Debounce { get; set; }
		public float StartTime { get; private set; }

		private Func<bool> _startTryers;
		private Func<bool> _stopTryers;
		private Action _onStart;
		private Action _onStop;

		/// <summary>
		/// This will register a method that will approve or disapprove the starting of this activity.
		/// </summary>
		public void AddStartTryer(Func<bool> tryer)
		{
			_startTryers += tryer;
		}

		public void RemoveStartTryer(Func<bool> tryer)
		{
			_startTryers -= tryer;
		}

		/// <summary>
		/// This will register a method that will approve or disapprove the stopping of this activity.
		/// </summary>
		public void AddStopTryer(Func<bool> tryer)
		{
			_stopTryers += tryer;
		}

		public void RemoveStopTryer(Func<bool> tryer)
		{
			_stopTryers -= tryer;
		}

		/// <summary>
		/// Will be called when this activity starts.
		/// </summary>
		public void AddStartListener(Action listener)
		{
			_onStart += listener;
		}

		public void RemoveStartListener(Action listener)
		{
			_onStart -= listener;
		}

		/// <summary>
		/// Will be called when this activity stops.
		/// </summary>
		public void AddStopListener(Action listener)
		{
			_onStop += listener;
		}

		public void RemoveStopListener(Action listener)
		{
			_onStop -= listener;
		}

		/// <summary>
		/// Calls TryStart or TryStop depending on current active state
		/// </summary>
		public void ToggleState()
		{
			if (Active)
			{
				TryStop();
			}
			else
			{
				TryStart();
			}
		}

		/// <summary>
		///
		/// </summary>
		public void ForceStart()
		{
			if (Active)
			{
				return;
			}

			Active = true;

			if (RequiresMarshalling())
			{
				SendToMarshaller(() => { _onStart?.Invoke(); });
			}
			else
			{
				_onStart?.Invoke();
			}

#if UNITY_2019_1_OR_NEWER
			StartTime = UnityEngine.Time.time;
#else
			StartTime = (float)(DateTime.Now.Ticks / MathHelper.StartupTick);
#endif
		}

		/// <summary>
		///
		/// </summary>
		public bool TryStart()
		{
			if (Active)
			{
				return false;
			}

			if (_startTryers != null)
			{
				Active = CallStartApprovers();

				if (Active)
				{
					if (RequiresMarshalling())
					{
						SendToMarshaller(() => { _onStart?.Invoke(); });
					}
					else
					{
						_onStart?.Invoke();
					}

#if UNITY_2019_1_OR_NEWER
					StartTime = UnityEngine.Time.time;
#else
					StartTime = (float)(DateTime.Now.Ticks / MathHelper.StartupTick);
#endif
				}
			}
			else
			{
				Active = true;

				if (RequiresMarshalling())
				{
					SendToMarshaller(() => { _onStart?.Invoke(); });
				}
				else
				{
					_onStart?.Invoke();
				}

#if UNITY_2019_1_OR_NEWER
				StartTime = UnityEngine.Time.time;
#else
				StartTime = (float)(DateTime.Now.Ticks / MathHelper.StartupTick);
#endif
				//Debug.LogWarning("[Activity] - You tried to start an activity which has no tryer (if no one checks if the activity can start, it won't start).");
			}

			return Active;
		}

		/// <summary>
		///
		/// </summary>
		public bool TryStop()
		{
			if (!Active)
			{
				return false;
			}

			if (_stopTryers == null || CallStopApprovers())
			{
				Active = false;
				if (RequiresMarshalling())
				{
					SendToMarshaller(() => { _onStop?.Invoke(); });
				}
				else
				{
					_onStop?.Invoke();
				}
				return true;
			}

			return false;
		}

		/// <summary>
		/// The activity will stop immediately.
		/// </summary>
		public void ForceStop()
		{
			if (!Active)
			{
				return;
			}

			Active = false;

			//SendToMarshaller(() => { _onStop?.Invoke(); });
			//_onStop?.Invoke();
		}

		public void ResetState()
		{
			Active = false;
		}

		private bool CallStartApprovers()
		{
			var invocationList = _startTryers.GetInvocationList();

			for (int i = 0; i < invocationList.Length; i++)
			{
				if (!(bool)invocationList[i].DynamicInvoke())
					return false;
			}

			return true;
		}

		private bool CallStopApprovers()
		{
			var invocationList = _stopTryers.GetInvocationList();

			for (int i = 0; i < invocationList.Length; i++)
			{
				if (!(bool)invocationList[i].DynamicInvoke())
					return false;
			}

			return true;
		}
	}
}
