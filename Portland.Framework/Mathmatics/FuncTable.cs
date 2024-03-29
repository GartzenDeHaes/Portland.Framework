﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Portland.Mathmatics
{
	public sealed class FuncTablef
	{
		float _domainMin;
		//float _domainMax;
		float _step;
		float[] _cells;

		public float this[float x]
		{
			get
			{
				float a = x - _domainMin;
				float step = a / _step;
				//int pos = (int)step;
				return _cells[(int)step % (_cells.Length + 1)];
			}
		}

		//public int Count { get; }
		//public float DomainMin { get { return _domainMin; } }
		//public float DomainMax { get { return _domainMax; } }

		public FuncTablef(int cols, float domainMin, float domainMax, Func<float, float> func)
		{
			_domainMin = domainMin;
			//_domainMax = domainMax;
			//Count = cols;
			_cells = new float[cols + 1];

			float dsize = domainMax - domainMin;
			_step = dsize / cols;

			SetTable(0, cols, func);
			_cells[cols] = func(domainMax);
		}

		public void SetTable(int start, int length, Func<float, float> func)
		{
			for (int x = start; x < length; x++)
			{
				_cells[x] = func(_domainMin + _step * x);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetPoint(int column, float value)
		{
			_cells[column] = value;
		}

		public void SetTableToLine(int startCol, float startY, int endCol, float endY)
		{
			List<Tuple<int, int>> lineTable = new List<Tuple<int, int>>();

			int dx = Math.Abs(endCol - startCol);
			float dy = Math.Abs(endY - startY);
			int sx = startCol < endCol ? 1 : -1;
			int sy = startY < endY ? 1 : -1;
			float err = dx - dy;
			float e2;

			while (true)
			{
				SetPoint(startCol, startY);

				if (startCol == endCol && startY >= endY)
				{
					break;
				}

				e2 = 2 * err;
				if (e2 > -dy)
				{
					err -= dy;
					startCol += sx;
				}

				if (e2 < dx)
				{
					err += dx;
					startY += sy;
				}
			}
		}
	}

	public sealed class FuncTablef2D
	{
		float _domainMin;
		//float _domainMax;
		float _step;
		float[] _cells;

		public float this[float x, float y]
		{
			get
			{
				float x1 = x - _domainMin;
				float stepx = x1 / _step;
				int posx = (int)stepx;

				float y1 = y - _domainMin;
				float stepy = y1 / _step;
				int posy = (int)stepy;

				return _cells[(posy % _size) * _size + posx % _size];
			}
		}

		int _size;
		//public int Count { get { return _size; } }
		//public float DomainMin { get { return _domainMin; } }
		//public float DomainMax { get { return _domainMax; } }

		public FuncTablef2D(int size, float domainMin, float domainMax, Func<float, float, float> func)
		{
			_domainMin = domainMin;
			//_domainMax = domainMax;
			_size = size;
			_cells = new float[size * size + size];

			float dsize = domainMax - domainMin;
			_step = dsize / size;

			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					_cells[y * size + x] = func(domainMin + _step * x, domainMin + _step * y);
				}
			}
			_cells[size * size] = func(domainMax, domainMax);
		}
	}

	public sealed class FuncTabled
	{
		double _domainMin;
		//double _domainMax;
		double _step;
		double[] _cells;

		public double this[double x]
		{
			get
			{
				double a = x - _domainMin;
				double step = a / _step;
				int pos = (int)step;
				return _cells[pos];
			}
		}

		//public int Count { get; }
		//public double DomainMin { get { return _domainMin; } }
		//public double DomainMax { get { return _domainMax; } }

		public FuncTabled(int cols, double domainMin, double domainMax, Func<double, double> func)
		{
			_domainMin = domainMin;
			//_domainMax = domainMax;
			//Count = cols;
			_cells = new double[cols + 1];

			double dsize = domainMax - domainMin;
			_step = dsize / cols;

			for (int x = 0; x < cols; x++)
			{
				_cells[x] = func(domainMin + _step * x);
			}
			_cells[cols] = func(domainMax);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetPoint(int column, double value)
		{
			_cells[column] = value;
		}

		public void SetTableToLine(int startCol, double startY, int endCol, double endY)
		{
			List<Tuple<int, int>> lineTable = new List<Tuple<int, int>>();

			int dx = Math.Abs(endCol - startCol);
			double dy = Math.Abs(endY - startY);
			int sx = startCol < endCol ? 1 : -1;
			int sy = startY < endY ? 1 : -1;
			double err = dx - dy;
			double e2;

			while (true)
			{
				SetPoint(startCol, startY);

				if (startCol == endCol && startY >= endY)
				{
					break;
				}

				e2 = 2 * err;
				if (e2 > -dy)
				{
					err -= dy;
					startCol += sx;
				}

				if (e2 < dx)
				{
					err += dx;
					startY += sy;
				}
			}
		}
	}
}
