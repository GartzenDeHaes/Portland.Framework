using System;
using System.Diagnostics;

using Maximum;

using Portland.Text;

namespace Portland
{
	/// <summary>
	/// Date with no time
	/// </summary>
	[Serializable]
	public struct Date : IComparable
	{
		private static readonly int[] m_daysPerMonth = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
		private static Date m_minDate = new Date(Int32.MinValue, 1, 1);
		private static Date m_maxDate = new Date(Int32.MaxValue, 12, 31);

		private short m_year;
		private sbyte m_month;
		private sbyte m_day;

		/// <summary>
		/// Initialize to current date.
		/// </summary>
		public Date()
		{
			DateTime now = DateTime.Now;
			m_year = (short)now.Year;
			m_month = (sbyte)now.Month;
			m_day = (sbyte)now.Day;

			Validate(now.Year, now.Month, now.Day);
		}

		/// <summary>
		/// Initialize with the date portion of a datetime.
		/// </summary>
		/// <param name="dt"></param>
		public Date(DateTime dt)
		{
			m_year = (short)dt.Year;
			m_month = (sbyte)dt.Month;
			m_day = (sbyte)dt.Day;

			Validate(dt.Year, dt.Month, dt.Day);
		}

		public Date(object date)
		{
			if (null == date || date is DBNull)
			{
				m_year = (short)DateTime.MinValue.Year;
				m_month = (sbyte)DateTime.MinValue.Month;
				m_day = (sbyte)DateTime.MinValue.Day;
			}
			else if (date is DateTime dtm)
			{
				m_year = (short)dtm.Year;
				m_month = (sbyte)dtm.Month;
				m_day = (sbyte)dtm.Day;
			}
			else if (date is Date dt)
			{
				m_year = (short)dt.Year;
				m_month = (sbyte)dt.Month;
				m_day = (sbyte)dt.Day;
			}
			else
			{
				throw new Exception("Cannot convert " + date.GetType().Name + " to Date");
			}
		}

		/// <summary>
		/// Create a new date
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Date(int year, int month, int day)
		{
			m_year = (short)year;
			m_month = (sbyte)month;
			m_day = (sbyte)day;

			Validate(year, month, day);
		}

		/// <summary>
		/// initialize a date from 20040101 format
		/// </summary>
		/// <param name="revdate"></param>
		public Date(int revdate)
		{
			m_year = (short)(revdate / 10000);
			m_month = (sbyte)((revdate - (m_year * 10000)) / 100);
			m_day = (sbyte)((revdate - (m_year * 10000)) - (m_month * 100));

			if (!IsDate(m_year, m_month, m_day))
			{
				throw new ArgumentException("Invalid date");
			}
		}

		/// <summary>
		/// year part
		/// </summary>
		public int Year
		{
			get
			{
				return m_year;
			}
		}

		/// <summary>
		/// month part
		/// </summary>
		public int Month
		{
			get
			{
				return m_month;
			}
		}

		/// <summary>
		/// day part
		/// </summary>
		public int Day
		{
			get
			{
				return m_day;
			}
		}

		public int Quarter
		{
			get
			{
				switch (Month)
				{
					case 1:
					case 2:
					case 3:
						return 1;
					case 4:
					case 5:
					case 6:
						return 2;
					case 7:
					case 8:
					case 9:
						return 3;
					default:
						return 4;
				}
			}
		}

		/// <summary>
		/// Add n business days to the date and return a new Date
		/// </summary>
		/// <param name="daysToAdd"></param>
		/// <returns></returns>
		public Date AddBusinessDays(int days)
		{
			Date returnDate = this;
			int multiplier = 1;
			if (days < 0)
			{
				multiplier = -1;
				days *= multiplier;
			}
			for (int i = days; i > 0; i--)
			{
				do
				{
					returnDate = returnDate.AddDays(1 * multiplier);
				}
				while (returnDate.IsHoliday() || returnDate.IsWeekend());
			}
			return returnDate;
		}

		/// <summary>
		/// Add days to the date.
		/// </summary>
		public Date AddDays(int days)
		{
			return new Date(ToDateTime().AddDays(days));
		}

		/// <summary>
		/// Add months to the date.  The day will be set to the end of the
		/// result month.
		/// </summary>
		/// <param name="months">Can be negative</param>
		public Date AddMonths(int months)
		{
			int absMonths = System.Math.Abs(months);
			int monthDiff = absMonths % 12;
			int yearDiff = absMonths / 12;

			int month = (months > 0) ? m_month + monthDiff : m_month - monthDiff;
			int year = (months > 0) ? m_year + yearDiff : m_year - yearDiff;
			int day = m_day;

			if (month > 12)
			{
				month -= 12;
				year++;
			}
			else if (month < 1)
			{
				month += 12;
				year--;
			}

			// According to the framework docs, if the resulting month has fewer
			// days than the current day, set the day to the last day of the month.
			int daysInMonth = DaysInMonth(month, year);
			if (day > daysInMonth)
			{
				day = daysInMonth;
			}

			return new Date(year, month, day);
		}

		/// <summary>
		/// Returns the number of days in the month accounting for leap years.
		/// </summary>
		private static int DaysInMonth(int month, int year)
		{
			if (2 == month && IsLeapYear(year))
			{
				return m_daysPerMonth[1] + 1;
			}
			return m_daysPerMonth[month - 1];
		}

		/// <summary>
		/// convert to datetime
		/// </summary>
		public DateTime ToDateTime()
		{
			if (Equals(MinValue))
			{
				return DateTime.MinValue;
			}
			if (Equals(MaxValue))
			{
				return DateTime.MaxValue;
			}
			return new DateTime(m_year, m_month, m_day);
		}

		/// <summary>
		/// convert to datetime 11:59:59 PM
		/// </summary>
		public DateTime ToDateTimeEndOfDay()
		{
			if (Equals(MinValue))
			{
				return DateTime.MinValue;
			}
			if (Equals(MaxValue))
			{
				return DateTime.MaxValue;
			}
			return new DateTime(m_year, m_month, m_day, 23, 59, 59, 999);
		}

		public int ToRevInt()
		{
			return m_year * 10000 + m_month * 100 + m_day;
		}

		/// <summary>
		/// Extremely limited format cap
		/// </summary>
		/// <param name="frmt">"MMDDYY"</param>
		/// <returns></returns>
		public string Format(string frmt)
		{
			if (frmt == "MMDDYY")
			{
				Debug.Assert(1.ToString("00") == "01", "ToString format assumption failure");
				return m_month.ToString("00") + m_day.ToString("00") + TwoDigitYear(m_year).ToString("00");
			}
			throw new ArgumentException("Can't format to " + frmt);
		}

		/// <summary>
		/// Return a two digit year from a four digit one
		/// </summary>
		/// <param name="year"></param>
		/// <returns></returns>
		public static int TwoDigitYear(int year)
		{
			Debug.Assert(Int32.Parse(year.ToString().Substring(2, 2)) == year % 100, "Conversion assumption failure");
			return year % 100;
		}

		public static bool operator >(Date d1, Date d2)
		{
			if (null == (object)d1 || null == (object)d2)
			{
				return false;
			}
			if (d1.m_year > d2.Year)
			{
				return true;
			}
			if (d1.m_year == d2.Year && (d1.m_month > d2.Month || (d1.m_month == d2.Month && d1.Day > d2.Day)))
			{
				return true;
			}
			return false;
		}

		public static bool operator <(Date d1, Date d2)
		{
			if (null == (object)d1 || null == (object)d2)
			{
				return false;
			}
			if (d1.m_year < d2.Year)
			{
				return true;
			}
			if (d1.m_year == d2.Year && (d1.m_month < d2.Month || (d1.m_month == d2.Month && d1.Day < d2.Day)))
			{
				return true;
			}
			return false;
		}

		public static bool operator >(Date d1, DateTime d2)
		{
			if (null == (object)d1)
			{
				return false;
			}
			if (d1.m_year > d2.Year)
			{
				return true;
			}
			if (d1.m_year == d2.Year && (d1.m_month > d2.Month || (d1.m_month == d2.Month && d1.Day > d2.Day)))
			{
				return true;
			}
			return false;
		}

		public static bool operator >(Date d1, DateTime? d2)
		{
			if (null == (object)d1 || !d2.HasValue)
			{
				return false;
			}
			if (d1.m_year > d2.Value.Year)
			{
				return true;
			}
			if (d1.m_year == d2.Value.Year && (d1.m_month > d2.Value.Month || (d1.m_month == d2.Value.Month && d1.Day > d2.Value.Day)))
			{
				return true;
			}
			return false;
		}

		public static bool operator <(Date d1, DateTime d2)
		{
			if (null == (object)d1)
			{
				return false;
			}
			if (d1.m_year < d2.Year)
			{
				return true;
			}
			if (d1.m_year == d2.Year && (d1.m_month < d2.Month || (d1.m_month == d2.Month && d1.Day < d2.Day)))
			{
				return true;
			}
			return false;
		}

		public static bool operator <(Date d1, DateTime? d2)
		{
			if (null == (object)d1 || !d2.HasValue)
			{
				return false;
			}
			if (d1.m_year < d2.Value.Year)
			{
				return true;
			}
			if (d1.m_year == d2.Value.Year && (d1.m_month < d2.Value.Month || (d1.m_month == d2.Value.Month && d1.Day < d2.Value.Day)))
			{
				return true;
			}
			return false;
		}

		public static bool operator <=(Date d1, DateTime d2)
		{
			return d1 < d2 || d1 == d2;
		}

		public static bool operator >=(Date d1, DateTime d2)
		{
			return d1 > d2 || d1 == d2;
		}

		public static bool operator <=(Date d1, Date d2)
		{
			return d1 < d2 || d1 == d2;
		}

		public static bool operator >=(Date d1, Date d2)
		{
			return d1 > d2 || d1 == d2;
		}

		/// <summary>
		/// Are the two dates equal?
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator ==(Date d1, Date d2)
		{
			if ((object)d1 == null)
			{
				return (object)d2 == null;
			}
			else if ((object)d2 == null)
			{
				return false;
			}
			return d1.m_day == d2.Day && d1.m_month == d2.Month && d1.m_year == d2.Year;
		}

		/// <summary>
		/// Are the two dates equal?
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator ==(Date d1, DateTime d2)
		{
			if ((object)d1 == null)
			{
				return null == (object)d2;
			}
			return d1.Equals(d2);
		}

		/// <summary>
		/// Are the two date different
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator !=(Date d1, Date d2)
		{
			if (null == (object)d1)
			{
				return null != (object)d2;
			}
			return !d1.Equals(d2);
		}

		/// <summary>
		/// Are the two date different
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator !=(Date d1, DateTime d2)
		{
			if (null == (object)d1)
			{
				return null != (object)d2;
			}
			else if ((object)d2 == null)
			{
				return true;
			}
			return d1.m_day != d2.Day || d1.m_month != d2.Month || d1.m_year != d2.Year;
		}

		/// <summary>
		/// same as ==
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals(object o)
		{
			if (null == o)
			{
				return false;
			}
			if (o is Date)
			{
				return Equals((Date)o);
			}
			if (o is DateTime)
			{
				DateTime dt = (DateTime)o;
				return m_day == dt.Day && m_month == dt.Month && m_year == dt.Year;
			}
			return false;
		}

		public bool Equals(Date dt)
		{
			if (null == (object)dt)
			{
				return false;
			}
			return m_day == dt.m_day && m_month == dt.m_month && m_year == dt.m_year;
		}

		/// <summary>
		/// required override
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return AsRevInt.GetHashCode();
		}

		public int AsRevInt
		{
			get { return m_year * 10000 + m_month * 100 + m_day; }
		}

		public string AsUsString
		{
			get { return ToString(); }
		}

		public override string ToString()
		{
			return m_month.ToString("00") + "/" + m_day.ToString("00") + "/" + m_year.ToString();
		}

		private void Validate(in int year, in int month, in int day)
		{
			if (0 != year || 0 != month || 0 != day)
			{
				if (month <= 0 || month > 12 || day <= 0 || day > 31)
				{
					throw new ArgumentOutOfRangeException("Invalid date (" + month + "/" + day + "/" + year + ")");
				}
				if (day > DaysInMonth(month, year))
				{
					throw new ArgumentOutOfRangeException("Invalid date (" + month + "/" + day + "/" + year + ")");
				}
			}
		}

		/// <summary>
		/// Returns true if the specified values are valid.
		/// </summary>
		public static bool IsDate(int year, int mo, int day)
		{
			return (mo > 0 && mo <= 12 && day > 0 && day < 32 && day <= DaysInMonth(mo, year));
		}

		/// <summary>
		/// If the date is a holiday or weekend, return the previous date that is not a holiday or weekend.
		/// </summary>
		public Date RegressToWorkday()
		{
			Date dt = this;

			while (dt.IsHoliday() || dt.IsWeekend())
			{
				dt = dt.AddDays(-1);
			}

			return dt;
		}

		/// <summary>
		/// If the date is a holiday or weekend, return the next date that is not a holiday or weekend.
		/// </summary>
		public Date AdvanceToWorkday()
		{
			Date dt = this;

			while (dt.IsHoliday() || dt.IsWeekend())
			{
				dt = dt.AddDays(1);
			}

			return dt;
		}

		/// <summary>
		/// Returns true if this date is a weekend.
		/// </summary>
		/// <returns></returns>
		public bool IsWeekend()
		{
			return DayOfWeek == DayOfWeek.Sunday || DayOfWeek == DayOfWeek.Saturday;
		}

		/// <summary>
		/// Returns the day of week for this date.
		/// </summary>
		public DayOfWeek DayOfWeek
		{
			get
			{
				int century = (m_year / 100);
				int c = 2 * (3 - century % 4);

				int year = m_year - century * 100;
				int y = year + (year / 4);

				int month = 0;

				if (IsLeapYear())
				{
					switch (m_month)
					{
						case 1: month = 6; break;
						case 2: month = 2; break;
						case 3: month = 3; break;
						case 4: month = 6; break;
						case 5: month = 1; break;
						case 6: month = 4; break;
						case 7: month = 6; break;
						case 8: month = 2; break;
						case 9: month = 5; break;
						case 10: month = 0; break;
						case 11: month = 3; break;
						case 12: month = 5; break;
					}
				}
				else
				{
					switch (m_month)
					{
						case 1: month = 0; break;
						case 2: month = 3; break;
						case 3: month = 3; break;
						case 4: month = 6; break;
						case 5: month = 1; break;
						case 6: month = 4; break;
						case 7: month = 6; break;
						case 8: month = 2; break;
						case 9: month = 5; break;
						case 10: month = 0; break;
						case 11: month = 3; break;
						case 12: month = 5; break;
					}
				}

				int sum = c + y + month + m_day;
				return (DayOfWeek)(sum % 7);
			}
		}

		/// <summary>
		/// Returns true if the date is a holiday.
		/// </summary>
		public bool IsHoliday()
		{
			switch (m_month)
			{
				case 1:
					//	New Years
					//	RULE
					//	January 1.  If on a Saturday, the previous friday is off. If on a Sunday then Monday is off
					if (Day == 1 && (DayOfWeek != DayOfWeek.Saturday && DayOfWeek != DayOfWeek.Sunday))
					{
						return true;
					}
					if (Day == 2 && DayOfWeek == DayOfWeek.Monday)
						return true;

					//	MLK Day
					//	RULE
					//	celebrated as third Monday in January, King's birthday is Jan 18.
					if (DayOfWeek == DayOfWeek.Monday)
					{
						Date mlkDay = new Date(Year, 1, 1);
						while (mlkDay.DayOfWeek != DayOfWeek.Monday)
						{
							mlkDay = mlkDay.AddDays(1);
						}
						mlkDay = mlkDay.AddDays(14);

						if (this.Equals(mlkDay))
						{
							return true;
						}
					}
					break;
				case 2:
					//	Presidents' Day
					//	RULE
					//	Third Monday in February.  Replaced Lincoln and Washington's birthday in 1971.
					if (DayOfWeek == DayOfWeek.Monday)
					{
						Date presDay = new Date(Year, 2, 1);
						while (presDay.DayOfWeek != DayOfWeek.Monday)
						{
							presDay = presDay.AddDays(1);
						}
						presDay = presDay.AddDays(14);

						if (this.Equals(presDay))
						{
							return true;
						}
					}
					break;
				case 5:
					//	Memorial Day
					//	RULE
					//	Last Monday in May. Originally May 31
					if (DayOfWeek == DayOfWeek.Monday)
					{
						if (AddDays(7).Month == 6)
						{
							return true;
						}
					}
					break;
				case 7:
					// July 4th
					// RULE
					//	If on a Saturday, then friday is off
					//	If on a Sunday, then monday is off
					if (Day == 4 && (DayOfWeek != DayOfWeek.Saturday && DayOfWeek != DayOfWeek.Sunday))
					{
						return true;
					}
					if (Day == 3 && DayOfWeek == DayOfWeek.Friday)
					{
						return true;
					}
					if (Day == 5 && DayOfWeek == DayOfWeek.Monday)
					{
						return true;
					}
					break;
				case 9:
					//	Labor Day
					//	RULE
					//	First Monday in September. In 1882..1883 it was celebrated on September 5.
					if (DayOfWeek == DayOfWeek.Monday)
					{
						if (AddDays(-7).Month == 8)
						{
							return true;
						}
					}
					break;
				case 10:
					//	Columbus Day
					//	RULE
					//	1905 .. 1970 -> October 12
					//	1971 .. now  -> Second Monday in October.
					if (DayOfWeek == DayOfWeek.Monday)
					{
						Date columbusDay = new Date(Year, 10, 1);
						while (columbusDay.DayOfWeek != DayOfWeek.Monday)
						{
							columbusDay = columbusDay.AddDays(1);
						}
						columbusDay = columbusDay.AddDays(7);

						if (this.Equals(columbusDay))
						{
							return true;
						}
					}
					break;
				case 11:
					// Veterans' Day
					// RULE
					// November 11.  If on a Saturday, the previous friday is off. If on a Sunday then Monday is off
					if (Day == 11 && DayOfWeek != DayOfWeek.Saturday && DayOfWeek != DayOfWeek.Sunday)
					{
						return true;
					}
					if (Day == 10 && DayOfWeek == DayOfWeek.Friday)
					{
						return true;
					}
					if (Day == 12 && DayOfWeek == DayOfWeek.Monday)
					{
						return true;
					}

					//  Thanksgiving 
					//	RULE
					//	1621         -> first Thanksgiving, precise date unknown.
					//	1622         -> was no Thanksgiving.
					//	1623 .. 1675 -> precise date unknown.
					//	1676 .. 1862 -> June 29.
					//	1863 .. 1938 -> last Thursday of November.
					//	1939 .. 1941 -> 2nd to last Thursday of November.
					//	1942 .. now  -> 4th Thursday of November.
					if (DayOfWeek == DayOfWeek.Thursday)
					{
						DateTime thanksgivingDay = new DateTime(Year, 11, 1);
						while (thanksgivingDay.DayOfWeek != DayOfWeek.Thursday)
						{
							thanksgivingDay = thanksgivingDay.AddDays(1);
						}
						thanksgivingDay = thanksgivingDay.AddDays(21);

						if (this.Equals(thanksgivingDay.Date))
						{
							return true;
						}
					}
					else if (DayOfWeek == DayOfWeek.Friday)
					{
						// the day after is also a bank holiday
						Date dayAfterTg = new Date(Year, 11, 1);

						while (dayAfterTg.DayOfWeek != DayOfWeek.Thursday)
						{
							dayAfterTg = dayAfterTg.AddDays(1);
						}
						dayAfterTg = dayAfterTg.AddDays(22);

						if (this.Equals(dayAfterTg))
						{
							return true;
						}
					}
					break;
				case 12:
					//	Christmas
					//	RULE
					//	 If on a saturday, then friday is off
					//	 If on a sunday, then monday is off
					if (Day == 25 && (DayOfWeek != DayOfWeek.Saturday && DayOfWeek != DayOfWeek.Sunday))
					{
						return true;
					}

					if (Day == 24 && DayOfWeek == DayOfWeek.Friday)
					{
						// the 25th is on saturday
						return true;
					}

					if (Day == 26 && DayOfWeek == DayOfWeek.Monday)
					{
						// the 25th is on sunday
						return true;
					}

					//	New Years
					//	RULE
					//	January 1.  If on a Saturday, the previous friday is off
					if (Day == 31 && DayOfWeek == DayOfWeek.Friday)
					{
						// the friday of Dec 31 is a holiday (jan 1 is on saturday)
						return true;
					}
					break;
			}
			return false;
		}

		public bool IsLeapYear()
		{
			return IsLeapYear(m_year);
		}

		public int CompareTo(object obj)
		{
			if (obj is Date)
			{
				return (this < (Date)obj) ? -1 : (this > (Date)obj) ? 1 : 0;
			}
			if (obj is DateTime)
			{
				return (this < (DateTime)obj) ? -1 : (this > (DateTime)obj) ? 1 : 0;
			}
			throw new ArgumentException("Cannot convert " + obj.GetType().Name + " to Date");
		}

		/// <summary>
		/// Parse the date string.
		/// </summary>
		public string MonthAbbr
		{
			get
			{
				switch (m_month)
				{
					case 1:
						return "Jan";
					case 2:
						return "Feb";
					case 3:
						return "Mar";
					case 4:
						return "Apr";
					case 5:
						return "May";
					case 6:
						return "Jun";
					case 7:
						return "Jul";
					case 8:
						return "Aug";
					case 9:
						return "Sep";
					case 10:
						return "Oct";
					case 11:
						return "Nov";
					case 12:
						return "Dec";
				}
				throw new FormatException("Internal error in Date, month was " + m_month);
			}
		}

		private static bool IsLeapYear(int year)
		{
			if ((year % 4) != 0)
			{
				return false;
			}
			else if ((year % 400) == 0)
			{
				return true;
			}
			else if ((year % 100) == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Returns the maximum possible date.
		/// </summary>
		public static Date MaxValue
		{
			get { return m_maxDate; }
		}

		/// <summary>
		/// Returns the minimum possible date.
		/// </summary>
		public static Date MinValue
		{
			get { return m_minDate; }
		}

		/// <summary>
		/// Attempt to parse the value.
		/// </summary>
		/// <param name="value">Date string</param>
		/// <param name="date">The parsed date</param>
		/// <returns>Returns true if successful.</returns>
		public static bool TryParse(string value, out Date date)
		{
			date = default;
			if (IsDate(value))
			{
				date = Parse(value);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parse the arguemnt into a date.
		/// </summary>
		/// <param name="o"></param>
		/// <returns>Returns null if o is null or DBNull.  </returns>
		public static Date Parse(object o)
		{
			if (null == o || o is DBNull)
			{
				return default;
			}
			if (o is string)
			{
				return Parse((string)o);
			}
			if (o is DateTime)
			{
				return new Date((DateTime)o);
			}
			if (o is DateTime?)
			{
				if (!((DateTime?)o).HasValue)
				{
					return default;
				}
				return new Date(((DateTime?)o).Value);
			}
			if (o is Date)
			{
				return (Date)o;
			}
			if (o is int)
			{
				// Tandem reverse int date
				if ((int)o == 0)
				{
					return default;
				}
				return new Date((int)o);
			}
			throw new ArgumentException(o.ToString() + " is not a valid date format");
		}

		/// <summary>
		/// Convert the DateTime to a Date.
		/// </summary>
		/// <param name="dtm">Can be null</param>
		/// <returns>A Date or null.</returns>
		public static Date Parse(DateTime? dtm)
		{
			if (dtm.HasValue)
			{
				return new Date(dtm.Value);
			}
			return default;
		}

		/// <summary>
		/// Parse the date string.
		/// </summary>
		public static Date Parse(string dt)
		{
			int mo, dy, yr;
			string[] parts;

			if (dt.IndexOf('-') > -1)
			{
				parts = dt.Split(new char[] { '-' });
				if (parts.Length != 3)
				{
					throw new FormatException("Invalid European format, should be YYYY-MM-DD.");
				}
				if (!StringHelper.IsInt(parts[0]))
				{
					throw new FormatException("Invalid European format year.");
				}
				if (!StringHelper.IsInt(parts[1]))
				{
					throw new FormatException("Invalid European format month.");
				}
				if (!StringHelper.IsInt(parts[2]))
				{
					throw new FormatException("Invalid European format day.");
				}
				return new Date(Int32.Parse(parts[0]), Int32.Parse(parts[1]), Int32.Parse(parts[2]));
			}

			if (dt.IndexOf(',') > -1)
			{
				switch (dt.Substring(0, 3))
				{
					case "Jan":
						mo = 1;
						break;
					case "Feb":
						mo = 2;
						break;
					case "Mar":
						mo = 3;
						break;
					case "Apr":
						mo = 4;
						break;
					case "May":
						mo = 5;
						break;
					case "Jun":
						mo = 6;
						break;
					case "Jul":
						mo = 7;
						break;
					case "Aug":
						mo = 8;
						break;
					case "Sep":
						mo = 9;
						break;
					case "Oct":
						mo = 10;
						break;
					case "Nov":
						mo = 11;
						break;
					case "Dec":
						mo = 12;
						break;
					default:
						throw new FormatException();
				}
				int pos = dt.IndexOf(' ') + 1;
				int cmaidx = dt.IndexOf(',');
				dy = Int32.Parse(StringHelper.MidStr(dt, pos, cmaidx));
				yr = Int32.Parse(dt.Substring(cmaidx + 1));

				return new Date(yr, mo, dy);
			}
			if (dt == "0" || String.IsNullOrEmpty(dt))
			{
				// For parsing Tandem dates.
				return default;
			}
			if (StringHelper.IsInt(dt) && dt.Length == 8)
			{
				// Tandem reverse int format
				string day = dt.Substring(6);
				if (day == "00")
				{
					// Tandem data is crap.
					day = "01";
				}
				return new Date(Int32.Parse(dt.Substring(0, 4)), Int32.Parse(dt.Substring(4, 2)), Int32.Parse(day));
			}
			if (StringHelper.CountOccurancesOf(dt, '/') != 2)
			{
				throw new FormatException();
			}

			parts = dt.Split(new char[] { '/' });
			if (!Int32.TryParse(parts[0], out mo))
			{
				throw new FormatException();
			}
			if (!Int32.TryParse(parts[1], out dy))
			{
				throw new FormatException();
			}
			if (!Int32.TryParse(parts[2], out yr))
			{
				throw new FormatException();
			}
			return new Date(yr, mo, dy);
		}

		/// <summary>
		/// Returns true if the argument is a valid date format.
		/// </summary>
		public static bool IsDate(string dt)
		{
			return TryParse(dt, out Date date);
		}

		/// <summary>
		/// The current date.
		/// </summary>
		public static Date Now
		{
			get { return new Date(); }
		}

		public static Date UtcNow
		{
			get { return new Date(DateTime.UtcNow); }
		}
	}
}
