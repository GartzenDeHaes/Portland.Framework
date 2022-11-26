using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Mathmatics
{
	public class TimeSeries
	{
		private Sample m_sample;
		private Dictionary<Date, int> m_dateIdx = new Dictionary<Date,int>();

		public TimeSeries(TimeValue[] tvs)
		{
			m_sample = new Sample((from tv in tvs select tv.Value).ToArray());

			for (int x = 0; x < tvs.Count(); x++)
			{
				m_dateIdx.Add(tvs[x].Date, x);
				Debug.Assert(m_sample[x] == tvs[x].Value);
			}
		}

		public Sample Sample
		{
			get { return m_sample; }
		}

		public double this[int idx]
		{
			get { return m_sample[idx]; }
		}

		public double this[Date dt]
		{
			get { return m_sample[m_dateIdx[dt]]; }
		}

		public void ReRange(double max, double min)
		{
			m_sample = m_sample.ReRange(max, min);
		}

		public void TransformATan()
		{
			m_sample = m_sample.TransformATan();
		}

		public void TransformSquash()
		{
			m_sample = m_sample.TransformSquash();
		}

		public int IndexOfDate(Date dt)
		{
			return m_dateIdx[dt];
		}

		public bool HasValueAtDate(Date dt)
		{
			return m_dateIdx.ContainsKey(dt);
		}

		public double ValueAtDate(Date dt)
		{
			return m_sample[m_dateIdx[dt]];
		}

		public int Count
		{
			get { return m_sample.N; }
		}

		public double[] ToArray()
		{
			return m_sample.ToArray();
		}

		public Date[] ToDateArray()
		{
			return (from dt in m_dateIdx.Keys orderby dt ascending select dt).ToArray();
		}

		public TimeSeries ToDeltas()
		{
			Date[] dates = ToDateArray();
			TimeValue[] data = new TimeValue[m_sample.N];

			Parallel.For(1, dates.Length, x =>
			{
				data[x] = new TimeValue(dates[x], m_sample[m_dateIdx[dates[x]]] - m_sample[m_dateIdx[dates[x - 1]]]);
			});

			return new TimeSeries(data);
		}
	}
}
