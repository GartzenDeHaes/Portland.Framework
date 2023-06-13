using System;

namespace Portland.PGC
{
	public class TiledNoise : INoise
	{
		const double _simplexDensity = 0.015f;
		const double offset = 1f;

		HeightMap _buffer;

		public double Value2D(double x, double y)
		{
			return _buffer[(int)x, (int)y];
		}

		public double Value3D(double x, double y, double z)
		{
			throw new NotImplementedException();
		}

		public TiledNoise(HeightMap buffer)
		{
			_buffer = buffer;
		}

		public static INoise TileNoise(int size)
		{
			OpenSimplex noiseGen = new OpenSimplex();
			return TileNoise(size, noiseGen);
		}

		public static INoise TileNoise(int size, INoise noiseGen)
		{
			double _x, _y, u, v;
			double noise00, noise01, noise10, noise11, noisea;

			HeightMap buffer = new HeightMap(size, size);

			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					_x = x;
					_y = y;

					u = _x / size;
					v = _y / size;

					noise00 = noiseGen.Value3D(_x * _simplexDensity, _y * _simplexDensity, offset);
					noise01 = noiseGen.Value3D(_x * _simplexDensity, (_y + size) * _simplexDensity, offset);
					noise10 = noiseGen.Value3D((_x + size) * _simplexDensity, _y * _simplexDensity, offset);
					noise11 = noiseGen.Value3D((_x + size) * _simplexDensity, (_y + size) * _simplexDensity, offset);

					noisea = u * v * noise00 + u * (1f - v) * noise01 + (1f - u) * v * noise10 + (1f - u) * (1f - v) * noise11;
					noisea /= 4f;

					noisea += 1f;
					noisea *= 0.5f;

					buffer[x, y] = (float)noisea;
				}
			}

			return new TiledNoise(buffer);
		}
	}
}
