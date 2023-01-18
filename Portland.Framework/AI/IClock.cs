using System;

namespace Portland.AI
{
	public interface IClock
	{
		DateTime Now { get; }
		bool IsNightTime { get; }
		bool IsWeekend { get; }
		float SecondsPerHour { get; }
		TimeSpan TimeElapsed { get; }
		float Time { get; }
		float RealTime { get; }
		float TimeOfDayNormalized01 { get; }

		event Action OnNightfall;
		event Action OnDaybreak;

		void Update(float elapsedTimeInSeconds);

		TimeSpan GameTimeToRealTime(TimeSpan gameTimeSpan);
		void SetNow(DateTime now);
	}
}