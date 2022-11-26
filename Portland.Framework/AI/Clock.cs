using System;

namespace Portland.AI
{
	/// <summary>
	/// Provides a global scaled clock, but does not affect Time.time.  Allows for a
	/// short day/night cycle.
	/// </summary>
	[Serializable]
	public class Clock : IClock
	{
		private DateTime _startDtm;
		/// <summary>1440 for real time</summary>
		private float _minutesPerDay = 30;
		private float _updateFreqInSeconds = 5f;
		private int _hourNightStarts = 19;
		private int _hourNightEnds = 6;

		private float _ratio;
		private int _lastHour;

		private float _lastUpdate;
		private float _updateAcc;
		private bool _lastIsNightime;

		public DateTime Now { get; protected set; }
		public bool IsNightTime { get; protected set; }
		public bool IsWeekend { get; protected set; }
		public float SecondsPerHour { get; protected set; }

		public TimeSpan TimeElapsed
		{
			get { return TimeSpan.FromSeconds(Time); }
		}

		public float Time { get { return _updateAcc * _ratio; } }

		public float TimeOfDayNormalized01
		{
			get
			{
				float seconds = (float)(TimeSpan.FromHours(Now.Hour) + TimeSpan.FromMinutes(Now.Minute)).TotalSeconds;
				return seconds / (1440f * 60f);
			}
		}

		DateTime InGameTime
		{
			get { return _startDtm + TimeElapsed; }
		}

		public event Action OnNightfall;
		public event Action OnDaybreak;

		/// <summary>
		/// A scalable clock allowing for a short/long simulated time.
		/// </summary>
		/// <param name="baseDate">Inception date/time for basis</param>
		/// <param name="minutesPerDay">1440 for realtime.</param>
		public Clock(DateTime baseDate, float minutesPerDay)
		{
			_minutesPerDay = minutesPerDay;
			_startDtm = baseDate;
			_ratio = 1440f / _minutesPerDay;
			_lastUpdate = -60f;

			SecondsPerHour = 60f * 60f * (_minutesPerDay / 1440f);

			Update(0);
		}

		public void Update(float elapsedTimeInSeconds)
		{
			_updateAcc += elapsedTimeInSeconds;

			if (_updateAcc >= _lastUpdate + _updateFreqInSeconds)
			{
				_lastUpdate = _updateAcc;

				Now = InGameTime;

				if (_lastHour != Now.Hour)
				{
					_lastHour = Now.Hour;
					IsNightTime = (_lastHour >= _hourNightStarts) || !(_lastHour >= _hourNightEnds);
					IsWeekend = Now.DayOfWeek == DayOfWeek.Saturday || Now.DayOfWeek == DayOfWeek.Sunday;

					if (IsNightTime != _lastIsNightime)
					{
						if (IsNightTime)
						{
							OnNightfall?.Invoke();
						}
						else
						{
							OnDaybreak?.Invoke();
						}
						_lastIsNightime = IsNightTime;
					}
				}
			}
		}

		public TimeSpan GameTimeToRealTime(TimeSpan gameTimeSpan)
		{
			return TimeSpan.FromSeconds(gameTimeSpan.TotalSeconds * _ratio);
		}

		public void SetNow(DateTime now)
		{
			_startDtm = now;
			_lastUpdate = 0;
			_updateAcc = 0;
			_lastHour = 0;
		}
	}
}
