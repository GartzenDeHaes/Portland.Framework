using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Portland.Mathmatics
{
	public class Sample
	{
		private double m_min = Double.PositiveInfinity;
		private double m_max = Double.NegativeInfinity;
		private double m_mean;
		private int m_n;
		private double m_ssd;
		private double[] m_data;

		public Sample()
		{
		}

		public Sample(List<double> list)
		{
			Recalc(list);
		}

        public Sample(double[] list)
		{
			Recalc(list);
		}

		public void Clear()
		{
			m_data = null;
			m_min = Double.PositiveInfinity;
			m_max = Double.NegativeInfinity;
		}

        public double this [int idx]
        {
            get 
			{
				Debug.Assert(idx > -1 && idx < m_data.Length);
				return m_data[idx]; 
			}
        }

		public void Append(double val)
		{
			List<double> vals = m_data.ToList();
			vals.Add(val);
			Recalc(vals);
		}

		public void Recalc(List<double> list)
		{
			Recalc(list.ToArray());
		}

		public void Recalc(double[] list)
		{
			double sum = 0;
            m_min = Double.MaxValue;
            m_max = Double.MinValue;
			m_n = list.Length;

			for (int x = 0; x < m_n; x++)
			{
				double val = list[x];
				
				Debug.Assert(!Double.IsInfinity(val));
				Debug.Assert(!Double.IsNaN(val));

				if (val < m_min)
				{
					m_min = val;
				}
				if (val > m_max)
				{
					m_max = val;
				}
				sum += val;
			}
			m_mean = sum / m_n;

			m_data = new double[list.Length];
			sum = 0;
			double mean2 = m_mean * m_mean;

			for (int x = 0; x < m_n; x++)
			{
				double val = list[x];
				m_data[x] = val;
				sum += (val * val) - mean2;
			}
			
			m_ssd = System.Math.Sqrt(sum / m_n);
		}

		public double Min
		{
			get { return m_min; }
		}

		public double Max
		{
			get { return m_max; }
		}

		public double Mean
		{
			get { return m_mean; }
		}

		public int N
		{
			get { return m_n; }
		}

		public double StDevSample
		{
			get { return m_ssd; }
		}

		public double AverageVariance
		{
			get
			{
				double ep = 0;
				double s;
				double var = 0;

				for (int j = 0; j < m_n; j++)
				{
					s = m_data[j] - m_mean;
					ep = ep + s;
					var = var + s * s;
				}

				return (var - ep * ep / m_n) / (m_n - 1);
			}
		}

		public double Skew
		{
			get
			{
				double sum = 0;
				for ( int x = 0; x < m_n; x++ )
				{
					sum += ((m_data[x] - m_mean) / StDevSample) * ((m_data[x] - m_mean) / StDevSample) * ((m_data[x] - m_mean) / StDevSample);
				}
				return sum / m_n;
			}
		}

		public double Kurtosis
		{
			get
			{
				double sum = 0;
				for ( int x = 0; x < m_n; x++ )
				{
					sum = sum + ((m_data[x] - m_mean) / StDevSample) * ((m_data[x] - m_mean) / StDevSample) * ((m_data[x] - m_mean) / StDevSample) * ((m_data[x] - m_mean) / StDevSample);
				}
				return sum / m_n - 3;
			}
		}

		public double Median
		{
			get
			{
				return (from d in m_data.AsParallel() orderby d ascending select d).ElementAt(m_n / 2);
			}
		}

        public Sample TransformASin()
		{
			return new Sample((from d in m_data.AsParallel() select System.Math.Asin(System.Math.Sqrt(d))).ToArray());
		}

        public Sample TransformSqrt375()
		{
			return new Sample((from d in m_data.AsParallel() select System.Math.Sqrt(d + 0.375)).ToArray());
		}

        public Sample TransformLog10()
		{
			return new Sample((from d in m_data.AsParallel() select System.Math.Log10(d)).ToArray());
		}

		public Sample TransformATan()
		{
			return new Sample((from d in m_data.AsParallel() select System.Math.Atan(d)).ToArray());
		}

		public Sample TransformSquash()
		{
			return new Sample((from d in m_data.AsParallel() select 1.0 / (1.0 + System.Math.Exp(-(d - Mean) / StDevSample))).ToArray());
		}

		public Sample ReRange(double max, double min)
		{
			double scale = (max - min) / (Max - Min);
			var scaled = (from d in m_data select d * scale);
			double scaledMin = scaled.Min();
			double transform = -scaledMin + min;

			return new Sample((from d in scaled select d + transform).ToArray());
		}

		public double[] ToArray()
		{
			return (double[])m_data.Clone();
		}
	}
}
