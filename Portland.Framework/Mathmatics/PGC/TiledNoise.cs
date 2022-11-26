using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Portland.PGC
{
	public class TiledNoise
	{
		const double _simplexDensity = 0.015f;
		const double offset = 1f;

		public static HeightMap SimplexTile(int size)
		{
			double _x, _y, u, v;
			double noise00, noise01, noise10, noise11, noisea;

			HeightMap noise = new HeightMap(size, size);
			OpenSimplex SimplexNoise = new OpenSimplex();

			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					_x = (double)x;
					_y = (double)y;

					u = _x / size;
					v = _y / size;

					noise00 = SimplexNoise.Value3D(_x * _simplexDensity, _y * _simplexDensity, offset);
					noise01 = SimplexNoise.Value3D(_x * _simplexDensity, (_y + size) * _simplexDensity, offset);
					noise10 = SimplexNoise.Value3D((_x + size) * _simplexDensity, _y * _simplexDensity, offset);
					noise11 = SimplexNoise.Value3D((_x + size) * _simplexDensity, (_y + size) * _simplexDensity, offset);

					noisea = u * v * noise00 + u * (1f - v) * noise01 + (1f - u) * v * noise10 + (1f - u) * (1f - v) * noise11;
					noisea /= 4f;

					noisea += 1f;
					noisea *= 0.5f;

					noise[x, y] = (float)noisea;
				}
			}

			return noise;
		}
	}
}
