using System;
using System.Collections.Generic;
using System.Text;

using Portland.Mathmatics;

namespace Portland.PGC
{
	public sealed class Voronoi : NoiseGen
	{
		#region Fields

		public enum DistributionType
		{
			EUCLIDEAN = 1,
			CITYBLOCK = 2,
			MANHATTAN = 3,
			QUADRATIC = 4,
		}

		private double m_displacement = 1.0;
		private double m_frequency = 1.0;
		private int m_seed = 0;
		private bool m_distance = false;
		private bool m_showcells = true;
		private DistributionType m_dist_type;
		private double w = 1.0, h = 1.0, dpt = 1.0;
		private bool m_wrap = true;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Voronoi.
		/// </summary>
		public Voronoi(DistributionType distType = DistributionType.EUCLIDEAN)
		{
			m_dist_type = distType;
		}

		/// <summary>
		/// Initializes a new instance of Voronoi.
		/// </summary>
		/// <param name="frequency">The frequency of the first octave.</param>
		/// <param name="displacement">The displacement of the ridged-multifractal noise.</param>
		/// <param name="persistence">The persistence of the ridged-multifractal noise.</param>
		/// <param name="seed">The seed of the ridged-multifractal noise.</param>
		/// <param name="distance">Indicates whether the distance from the nearest seed point is applied to the output value.</param>
		public Voronoi(double frequency, double displacement, int seed, bool distance)
		{
			this.Frequency = frequency;
			this.Displacement = displacement;
			this.Seed = seed;
			this.UseDistance = distance;
			this.Seed = seed;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the displacement value of the Voronoi cells.
		/// </summary>
		public double Displacement
		{
			get { return this.m_displacement; }
			set { this.m_displacement = value; }
		}

		/// <summary>
		/// Gets or sets the frequency of the seed points.
		/// </summary>
		public double Frequency
		{
			get { return this.m_frequency; }
			set { this.m_frequency = value; }
		}

		/// <summary>
		/// Gets or sets the seed value used by the Voronoi cells.
		/// </summary>
		public int Seed
		{
			get { return this.m_seed; }
			set { this.m_seed = value; }
		}

		/// <summary>
		/// Gets or sets a value whether the distance from the nearest seed point is applied to the output value.
		/// </summary>
		public bool UseDistance
		{
			get { return this.m_distance; }
			set { this.m_distance = value; }
		}

		public DistributionType DistType
		{
			get { return this.m_dist_type; }
			set { this.m_dist_type = value; }
		}
		public bool ShowCells
		{
			get { return this.m_showcells; }
			set { this.m_showcells = value; }
		}

		#endregion

		#region ModuleBase Members

		public override double Value2D(double x, double y)
		{
			return Value3D(x, y, 1f);
		}

		private const int GeneratorNoiseX = 1619;
		private const int GeneratorNoiseY = 31337;
		private const int GeneratorNoiseZ = 6971;
		private const int GeneratorSeed = 1013;
		//private const int GeneratorShift = 8; 
		
		internal static double ValueNoise3D(int x, int y, int z, int seed)
		{
			return 1.0 - (ValueNoise3DInt(x, y, z, seed) / 1073741824.0);
		}

		internal static long ValueNoise3DInt(int x, int y, int z, int seed)
		{
			long n = (GeneratorNoiseX * x + GeneratorNoiseY * y + GeneratorNoiseZ * z +
						 GeneratorSeed * seed) & 0x7fffffff;
			n = (n >> 13) ^ n;
			return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
		}     
		
		public override double Value3D(double x, double y, double z)
		{
			x *= this.m_frequency;
			y *= this.m_frequency;
			z *= this.m_frequency;
			w = 1.0;
			h = 1.0;
			//dpt = this.m_frequency;
			int xi = (x > 0.0 ? (int)x : (int)x - 1);
			int iy = (y > 0.0 ? (int)y : (int)y - 1);
			int iz = (z > 0.0 ? (int)z : (int)z - 1);
			double md = 2147483647.0;
			double xc = 0;
			double yc = 0;
			double zc = 0;
			for (int zcu = iz - 2; zcu <= iz + 2; zcu++)
			{
				for (int ycu = iy - 2; ycu <= iy + 2; ycu++)
				{
					for (int xcu = xi - 2; xcu <= xi + 2; xcu++)
					{
						double xp = xcu + ValueNoise3D(xcu, ycu, zcu, this.m_seed);
						double yp = ycu + ValueNoise3D(xcu, ycu, zcu, this.m_seed + 1);
						double zp = zcu + ValueNoise3D(xcu, ycu, zcu, this.m_seed + 2);
						double xd = xp - x;
						double yd = yp - y;
						double zd = zp - z;
						double d;

						if (m_wrap)
						{
							xd = Math.Abs(xd);
							yd = Math.Abs(yd);
							zd = Math.Abs(zd);

							if (xd > w / 2.0)
							{
								if (xp < x)
								{
									xd = xp + (w - x);
								}
								else
								{
									xd = x + (w - xp);
								}
							}
							if (yd > h / 2.0)
							{
								if (yp < y)
								{
									yd = yp + (h - y);
								}
								else
								{
									yd = y + (h - yp);
								}
							}
							if (zd > dpt / 2.0)
							{
								if (zp < z)
								{
									zd = zp + (dpt - z);
								}
								else
								{
									zd = z + (dpt - zp);
								}
							}
						}

						if (m_dist_type == DistributionType.CITYBLOCK)
						{
							d = Math.Max(Math.Max(Math.Abs(xd), Math.Abs(yd)), Math.Abs(zd));
							d *= d;
						}
						else if (m_dist_type == DistributionType.MANHATTAN)
						{
							d = Math.Abs(xd) + Math.Abs(yd) + Math.Abs(zd);
							d *= d;
						}
						else if (m_dist_type == DistributionType.QUADRATIC)
						{
							d = xd * xd + yd * yd + zd * zd + xd * yd + xd * zd + yd * zd;
							d *= d;
						}
						else
						{
							d = xd * xd + yd * yd + zd * zd;
						}

						if (d < md)
						{
							md = d;
							xc = xp;
							yc = yp;
							zc = zp;
						}
					}
				}
			}
			double v;
			if (this.m_distance)
			{

				double xd = xc - x;
				double yd = yc - y;
				double zd = zc - z;

				if (m_wrap)
				{
					xd = Math.Abs(xd);
					yd = Math.Abs(yd);
					zd = Math.Abs(zd);

					if (xd > w / 2.0)
					{
						if (xc < x)
						{
							xd = xc + (w - x);
						}
						else
						{
							xd = x + (w - xc);
						}
					}
					if (yd > h / 2.0)
					{
						if (yc < y)
						{
							yd = yc + (h - y);
						}
						else
						{
							yd = y + (h - yc);
						}
					}
					if (zd > dpt / 2.0)
					{
						if (zc < z)
						{
							zd = zc + (dpt - z);
						}
						else
						{
							zd = z + (dpt - zc);
						}
					}
				}


				v = (Math.Sqrt(xd * xd + yd * yd + zd * zd)) * MathHelper.Sqrt3 - 1.0;
			}
			else
			{
				v = 0.0;
			}
			if (this.m_showcells)
			{
				return v + (this.m_displacement * (double)ValueNoise3D((int)(Math.Floor(xc)), (int)(Math.Floor(yc)),
				  (int)(Math.Floor(zc)), 0));
			}
			return v;
		}

		#endregion
	}
}
