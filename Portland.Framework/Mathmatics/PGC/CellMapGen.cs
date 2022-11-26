using System;
using System.Collections.Generic;

using Portland.Collections;
using Portland.Mathmatics;

namespace Portland.PGC
{
	// http://www.roguebasin.com/index.php/Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels
	public class CellMapGen : IIndexed2D<byte>
	{
		IRandom _rnd;

		byte[,] _map;

		public int MapWidth { get; }
		public int MapHeight { get; }
		public int PercentAreWalls { get; set; }

		public byte this[int x, int y] 
		{ 
			get { return _map[x, y]; }
			set { _map[x, y] = value; }
		}

		public CellMapGen(int mapWidth, int mapHeight, int percentWalls = 40)
		: this(MathHelper.Rnd, mapWidth, mapHeight, percentWalls)
		{
		}

		public CellMapGen(IRandom rnd, int mapWidth, int mapHeight, int percentWalls = 40)
		{
			_rnd = rnd;

			this.MapWidth = mapWidth;
			this.MapHeight = mapHeight;
			this.PercentAreWalls = percentWalls;
			this._map = new byte[this.MapWidth, this.MapHeight];

			RandomFillMap();
		}

		public CellMapGen(IRandom rnd, int mapWidth, int mapHeight, byte[,] map, int percentWalls = 40)
		{
			_rnd = rnd;

			this.MapWidth = mapWidth;
			this.MapHeight = mapHeight;
			this.PercentAreWalls = percentWalls;
			this._map = map;
		}

		public void MakeCaverns()
		{
			// By initilizing column in the outter loop, its only created ONCE
			for (int column, row = 0; row <= MapHeight - 1; row++)
			{
				for (column = 0; column <= MapWidth - 1; column++)
				{
					_map[column, row] = PlaceWallLogic(column, row);
				}
			}
		}

		public byte PlaceWallLogic(int x, int y)
		{
			int numWalls = GetAdjacentWalls(x, y, 1, 1);

			if (_map[x, y] == 1)
			{
				if (numWalls >= 4)
				{
					return 1;
				}
				if (numWalls < 2)
				{
					return 0;
				}

			}
			else
			{
				if (numWalls >= 5)
				{
					return 1;
				}
			}
			return 0;
		}

		public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
		{
			int startX = x - scopeX;
			int startY = y - scopeY;
			int endX = x + scopeX;
			int endY = y + scopeY;

			int iX = startX;
			int iY = startY;

			int wallCounter = 0;

			for (iY = startY; iY <= endY; iY++)
			{
				for (iX = startX; iX <= endX; iX++)
				{
					if (!(iX == x && iY == y))
					{
						if (IsWall(iX, iY))
						{
							wallCounter += 1;
						}
					}
				}
			}
			return wallCounter;
		}

		bool IsWall(int x, int y)
		{
			// Consider out-of-bound a wall
			if (IsOutOfBounds(x, y))
			{
				return true;
			}

			if (_map[x, y] == 1)
			{
				return true;
			}

			if (_map[x, y] == 0)
			{
				return false;
			}
			return false;
		}

		bool IsOutOfBounds(int x, int y)
		{
			if (x < 0 || y < 0)
			{
				return true;
			}
			else if (x > MapWidth - 1 || y > MapHeight - 1)
			{
				return true;
			}
			return false;
		}

		public void PrintMap()
		{
			Console.Clear();
			Console.Write(MapToString());
		}

		string MapToString()
		{
			string returnString = string.Join(" ", // Seperator between each element
														 "Width:",
														 MapWidth.ToString(),
														 "\tHeight:",
														 MapHeight.ToString(),
														 "\t% Walls:",
														 PercentAreWalls.ToString(),
														 Environment.NewLine
														);

			List<string> mapSymbols = new List<string>();
			mapSymbols.Add(".");
			mapSymbols.Add("#");
			mapSymbols.Add("+");

			for (int column, row = 0; row < MapHeight; row++)
			{
				for (column = 0; column < MapWidth; column++)
				{
					returnString += mapSymbols[_map[column, row]];
				}
				returnString += Environment.NewLine;
			}
			return returnString;
		}

		public void BlankMap()
		{
			for (int column, row = 0; row < MapHeight; row++)
			{
				for (column = 0; column < MapWidth; column++)
				{
					_map[column, row] = 0;
				}
			}
		}

		public void RandomFillMap()
		{
			// New, empty map
			//Map = new int[MapWidth, MapHeight];

			int mapMiddle; // Temp variable
			for (int column, row = 0; row < MapHeight; row++)
			{
				for (column = 0; column < MapWidth; column++)
				{
					// If coordinants lie on the the edge of the map (creates a border)
					if (column == 0)
					{
						_map[column, row] = 1;
					}
					else if (row == 0)
					{
						_map[column, row] = 1;
					}
					else if (column == MapWidth - 1)
					{
						_map[column, row] = 1;
					}
					else if (row == MapHeight - 1)
					{
						_map[column, row] = 1;
					}
					// Else, fill with a wall a random percent of the time
					else
					{
						mapMiddle = (MapHeight / 2);

						if (row == mapMiddle)
						{
							_map[column, row] = 0;
						}
						else
						{
							_map[column, row] = RandomPercent(PercentAreWalls);
						}
					}
				}
			}
		}

		byte RandomPercent(int percent)
		{
			if (percent >= _rnd.Range(1, 101))
			{
				return 1;
			}
			return 0;
		}

		public int GetLength(int ord)
		{
			return _map.GetLength(ord);
		}
	}
}
