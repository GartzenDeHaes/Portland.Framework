using System;
using System.Diagnostics;

using Maximum;

using Portland.Collections;
using Portland.Text;

namespace Portland
{
	/// <summary>
	/// Date with no time
	/// </summary>
	[Serializable]
	public struct ShortDateTime : IComparable
	{
		static readonly int[] m_daysPerMonth = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
		BitSet32 _bits;

		/// <summary>
		/// Initialize to current date.
		/// </summary>
		public ShortDateTime()
		{
			_bits = MinValue._bits;
		}

		/// <summary>
		/// Initialize with the date portion of a datetime.
		/// </summary>
		/// <param name="dt"></param>
		public ShortDateTime(in DateTime dt)
		{
			_bits = new BitSet32();
			Year = dt.Year;
			Month = dt.Month;
			Day = dt.Day;
			Hour24 = dt.Hour;
			Minute = dt.Minute;

			Validate(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
		}

		public ShortDateTime(object date)
		{
			_bits = new BitSet32();

			if (null == date || date is DBNull)
			{
				return;
			}
			else if (date is DateTime dtm)
			{
				Year = dtm.Year;
				Month = dtm.Month;
				Day = dtm.Day;
				Hour24 = dtm.Hour;
				Minute = dtm.Minute;
			}
			else if (date is Date dt)
			{
				Year = dt.Year;
				Month = dt.Month;
				Day = dt.Day;
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
		public ShortDateTime(int year, int month, int day)
		{
			_bits = new BitSet32();

			Year = year;
			Month = month;
			Day = day;

			Validate(year, month, day, 0, 0);
		}

		public ShortDateTime(int year, int month, int day, int hour, int minute)
		{
			_bits = new BitSet32();

			Year = year;
			Month = month;
			Day = day;
			Hour24 = hour;
			Minute = minute;

			Validate(year, month, day, hour, minute);
		}

		/// <summary>
		/// initialize a date from 20040101 format
		/// </summary>
		/// <param name="revdate"></param>
		public ShortDateTime(int revdate)
		{
			_bits = new BitSet32();

			Year = (revdate / 10000);
			Month = ((revdate - (Year * 10000)) / 100);
			Day = (sbyte)((revdate - (Year * 10000)) - (Month * 100));

			if (!IsDate(Year, Month, Day))
			{
				throw new ArgumentException("Invalid date");
			}
		}

		/// <summary>
		/// year part
		/// </summary>
		public int Year
		{
			get { return (int)_bits.BitsAt(20, 11) + 1000; }
			private set
			{ 
				if (value < 1000 || value > 3047)
				{
					throw new ArgumentException("Short year must be [1048,3048)");
				}
				_bits.SetBitsAt(20, 11, (uint)(value - 1000));
			}
		}

		/// <summary>
		/// month part
		/// </summary>
		public int Month
		{
			get { return (int)_bits.BitsAt(16, 4); }
			private set { _bits.SetBitsAt(16, 4, (uint)value); }
		}

		/// <summary>
		/// day part
		/// </summary>
		public int Day
		{
			get { return (int)_bits.BitsAt(11, 5); }
			private set { _bits.SetBitsAt(11, 5, (uint)value); }
		}

		public int Hour24
		{
			get { return (int)_bits.BitsAt(6, 5); }
			private set { _bits.SetBitsAt(6, 5, (uint)value); }
		}

		public int Minute
		{
			get { return (int)_bits.BitsAt(0, 6); }
			private set { _bits.SetBitsAt(0, 6, (uint)value); }
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
		/// Add days to the date.
		/// </summary>
		public ShortDateTime AddDays(int days)
		{
			return new ShortDateTime(ToDateTime().AddDays(days));
		}

		/// <summary>
		/// Add months to the date.  The day will be set to the end of the
		/// result month.
		/// </summary>
		/// <param name="months">Can be negative</param>
		public ShortDateTime AddMonths(int months)
		{
			return new ShortDateTime(ToDateTime().AddMonths(months));
		}

		/// <summary>
		/// Returns the number of days in the month accounting for leap years.
		/// </summary>
		static int DaysInMonth(int month, int year)
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
			return new DateTime(Year, Month, Day, Hour24, Minute, 0);
		}

		public int ToRevInt()
		{
			return Year * 10000 + Month * 100 + Day;
		}

		public static bool operator >(in ShortDateTime d1, in ShortDateTime d2)
		{
			return d1._bits.RawBits > d2._bits.RawBits;
		}

		public static bool operator <(in ShortDateTime d1, in ShortDateTime d2)
		{
			return d1._bits.RawBits < d2._bits.RawBits;
		}

		public static bool operator >(in ShortDateTime d1, in DateTime d2)
		{
			return d1.ToDateTime() > d2;
		}

		public static bool operator >(in ShortDateTime d1, in DateTime? d2)
		{
			if (!d2.HasValue)
			{
				return false;
			}
			return d1.ToDateTime() > d2.Value;
		}

		public static bool operator <(in ShortDateTime d1, in DateTime d2)
		{
			return d1.ToDateTime() < d2;
		}

		public static bool operator <(in ShortDateTime d1, in DateTime? d2)
		{
			if (!d2.HasValue)
			{
				return false;
			}
			return d1.ToDateTime() < d2.Value;
		}

		public static bool operator <=(in ShortDateTime d1, in DateTime d2)
		{
			return d1 < d2 || d1 == d2;
		}

		public static bool operator >=(in ShortDateTime d1, in DateTime d2)
		{
			return d1 > d2 || d1 == d2;
		}

		public static bool operator <=(in ShortDateTime d1, in ShortDateTime d2)
		{
			return d1 < d2 || d1 == d2;
		}

		public static bool operator >=(in ShortDateTime d1, in ShortDateTime d2)
		{
			return d1 > d2 || d1 == d2;
		}

		/// <summary>
		/// Are the two dates equal?
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator ==(in ShortDateTime d1, in ShortDateTime d2)
		{
			return d1._bits == d2._bits;
		}

		/// <summary>
		/// Are the two dates equal?
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator ==(in ShortDateTime d1, in DateTime d2)
		{
			return d1.ToDateTime() == d2;
		}

		/// <summary>
		/// Are the two date different
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator !=(in ShortDateTime d1, in ShortDateTime d2)
		{
			return d1._bits != d2._bits;
		}

		/// <summary>
		/// Are the two date different
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static bool operator !=(in ShortDateTime d1, in DateTime d2)
		{
			return d1.ToDateTime() != d2;
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
			if (o is ShortDateTime sdt)
			{
				return Equals(sdt);
			}
			if (o is DateTime dtm)
			{
				return ToDateTime() == dtm;
			}
			return false;
		}

		public bool Equals(ShortDateTime dt)
		{
			return _bits == dt._bits;
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
			get { return Year * 10000 + Month * 100 + Day; }
		}

		public override string ToString()
		{
			return Month.ToString("00") + "/" + Day.ToString("00") + "/" + Year.ToString();
		}

		void Validate(in int year, in int month, in int day, in int hour, in int minute)
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
				if (hour < 0 || hour > 23)
				{
					throw new ArgumentException($"Invalidate hour of {hour}");
				}
				if (minute < 0 || minute > 59)
				{
					throw new ArgumentException($"Invalidate minute of {minute}");
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
				int century = (Year / 100);
				int c = 2 * (3 - century % 4);

				int year = Year - century * 100;
				int y = year + (year / 4);

				int month = 0;

				if (IsLeapYear())
				{
					switch (Month)
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
					switch (Month)
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

				int sum = c + y + month + Day;
				return (DayOfWeek)(sum % 7);
			}
		}

		public bool IsLeapYear()
		{
			return IsLeapYear(Year);
		}

		public int CompareTo(object obj)
		{
			if (obj is ShortDateTime sdt)
			{
				return (this < sdt) ? -1 : (this > sdt) ? 1 : 0;
			}
			if (obj is DateTime dtm)
			{
				return (this < dtm) ? -1 : (this > dtm) ? 1 : 0;
			}
			throw new ArgumentException("Cannot convert " + obj.GetType().Name + " to Date");
		}

		static bool IsLeapYear(int year)
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
		public static ShortDateTime MaxValue
		{
			get { return new ShortDateTime(3047, 12, 31, 23, 59); }
		}

		/// <summary>
		/// Returns the minimum possible date.
		/// </summary>
		public static ShortDateTime MinValue
		{
			get { return new ShortDateTime(1000, 1, 1); }
		}

		/// <summary>
		/// Attempt to parse the value.
		/// </summary>
		/// <param name="value">Date string</param>
		/// <param name="date">The parsed date</param>
		/// <returns>Returns true if successful.</returns>
		public static bool TryParse(string value, out ShortDateTime date)
		{
			if (DateTime.TryParse(value, out DateTime dtm))
			{
				date = new ShortDateTime(dtm);
				return true;
			}
			date = default;
			return false;
		}

		/// <summary>
		/// Convert the DateTime to a Date.
		/// </summary>
		/// <param name="dtm">Can be null</param>
		/// <returns>A Date or null.</returns>
		public static ShortDateTime Parse(DateTime? dtm)
		{
			if (dtm.HasValue)
			{
				return new ShortDateTime(dtm.Value);
			}
			return default;
		}

		/// <summary>
		/// Parse the date string.
		/// </summary>
		public static ShortDateTime Parse(string dt)
		{
			if (TryParse(dt, out ShortDateTime date))
			{
				return date;
			}
			return default;
		}

		/// <summary>
		/// Returns true if the argument is a valid date format.
		/// </summary>
		public static bool IsDate(string dt)
		{
			return TryParse(dt, out var date);
		}

		/// <summary>
		/// The current date.
		/// </summary>
		public static ShortDateTime Now
		{
			get { return new ShortDateTime(DateTime.Now); }
		}

		public static ShortDateTime UtcNow
		{
			get { return new ShortDateTime(DateTime.UtcNow); }
		}
	}
}
