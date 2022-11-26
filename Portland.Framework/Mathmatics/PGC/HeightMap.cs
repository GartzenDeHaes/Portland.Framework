using System;

using Portland.Collections;
using Portland.Mathmatics;

namespace Portland.PGC
{
	public class HeightMap : IIndexedArray2DWrap<float>
	{
		readonly float[] _cells;
		public readonly int Width;
		public readonly int Height;

		public HeightMap(int width, int height)
		{
			this.Width = width;
			this.Height = height;
			_cells = new float[width * height];
		}

		public override float this[int x, int y]
		{
			get
			{
				return _cells[WrapIndex(y, 1)*Width + WrapIndex(x, 0)];
			}
			set
			{
				_cells[WrapIndex(y, 1) * Width + WrapIndex(x, 0)] = value;
			}
		}

		public override int GetLength(int dim)
		{
			return dim == 0 ? Width : Height;
		}

		public float Max()
		{
			float max = 0f;
			float f;

			for (int i = 0; i < _cells.Length; i++)
			{
				f = _cells[i];
				max = f > max ? f : max;
			}
			return max;
		}

		public void Normalize()
		{
			float maxHeight = Max();

			for (int i = 0; i < _cells.Length; i++)
			{
				_cells[i] /= maxHeight;
			}
		}

		public void Scale(float scale)
		{
			for (int i = 0; i < _cells.Length; i++)
			{
				_cells[i] *= scale;
			}
		}

		public override string ToString()
		{
			string result = "";

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					result += String.Format("{0:0.00}", _cells[y * Width + x]) + " ";
				}
				result += "\n";
			}

			return result;
		}

		public void MakeSeamlessHorizontally(int stitchWidth)
		{
			// iterate on the stitch band (on the left
			// of the noise)
			for (int x = 0; x < stitchWidth; x++)
			{
				// get the transparency value from
				// a linear gradient
				float v = x / (float)stitchWidth;
				for (int y = 0; y < Height; y++)
				{
					// compute the "mirrored x position":
					// the far left is copied on the right
					// and the far right on the left
					int xo = Width - stitchWidth + x;
					// copy the value on the right of the noise
					_cells[y * Width + xo] = MathHelper.Lerp(_cells[y * Width + xo], _cells[y * Width + stitchWidth - x], v);
				}
			}
		}

		public void MakeSeamlessVertically(int stitchWidth)
		{
			// iterate through the stitch band (both
			// top and bottom sides are treated
			// simultaneously because its mirrored)
			for (int y = 0; y < stitchWidth; y++)
			{
				// number of neighbour pixels to
				// consider for the average (= kernel size)
				int k = stitchWidth - y;
				// go through the entire row
				for (int x = 0; x < Width; x++)
				{
					// compute the sum of pixel values
					// in the top and the bottom bands
					float s1 = 0.0f, s2 = 0.0f;
					int c = 0;
					for (int xo = x - k; xo < x + k; xo++)
					{
						if (xo < 0 || xo >= Width)
							continue;
						s1 += _cells[y * Width + xo];
						s2 += _cells[(Height - y - 1) * Width + xo];
						c++;
					}
					// compute the means and assign them to
					// the pixels in the top and the bottom
					// rows
					_cells[y * Width + x] = s1 / (float)c;
					_cells[(Height - y - 1) * Width + x] = s2 / (float)c;
				}
			}
		}
	}
}
