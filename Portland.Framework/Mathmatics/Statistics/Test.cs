using System;

namespace Portland.Mathmatics
{
    public class Test
    {
        private Sample m_test;
        private Sample m_reference;

        public Test(Sample test, Sample reference)
        {
            m_test = test;
            m_reference = reference;
        }

		public void Clear()
		{
			m_test.Clear();
			m_test = null;
			m_reference.Clear();
			m_reference = null;
		}

        public double CrossCorrelationFunction(int lagK)
        {
            if (m_test.N != m_reference.N)
            {
                throw new ArgumentException("Sample sizes must be equal for CCF.");
            }

            double ccf = 0;

            for (int i = 0; i < m_test.N - lagK; i++)
            {
                if (lagK > -1)
                {
                    ccf += (m_test[i] - m_test.Mean) * (m_reference[i + lagK] - m_reference.Mean);
                }
                else
                {
                    ccf += (m_reference[i] - m_reference.Mean) * (m_test[i - lagK] - m_test.Mean);
                }
            }

            return ccf / (m_test.StDevSample * m_reference.StDevSample);
        }
    }
}
