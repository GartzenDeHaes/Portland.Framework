using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public partial class HeightMap
	{
		protected readonly float[,] _map;

		public int RawWidth
		{
			get { return _map.GetLength(0); }
		}

		public int RawHeight
		{
			get { return _map.GetLength(1); }
		}

		public Vector3 SizeInWorldUnits
		{
			get; set;
		}

		public HeightMap(int width, int height)
		{
			_map = new float[width, height];
		}

		private HeightMap(float[,] map)
		{
			_map = map;
		}

		private int XWorldToLocal(int x)
		{
			return (int)((float)RawWidth * (x / SizeInWorldUnits.x));
		}

		private int ZWorldToLocal(int z)
		{
			return (int)((float)RawHeight * (z / SizeInWorldUnits.z));
		}

		private float YLocalToWorld(float y)
		{
			// y [0, 1]
			return y * SizeInWorldUnits.y;
		}

		/// <summary>
		/// x and z are in world units, but do not account for Transform's, ie [0 ... SizeInWorldUnits)
		/// </summary>
		/// <param name="x">left right</param>
		/// <param name="z">forward back</param>
		/// <returns></returns>
		public float GetWorldHeightAt(int x, int z)
		{
			return YLocalToWorld(_map[XWorldToLocal(x), ZWorldToLocal(z)]);	
		}

		public float GetWorldSteepnessAt(int wx, int wz)
		{
			int x = XWorldToLocal(wx);
			int z = ZWorldToLocal(wz);

			float height = _map[x, z];

			// Compute the differentials by stepping over 1 in both directions.
			// TODO: Ensure these are inside the heightmap before sampling.
			float dx = _map[x + 1, z] - height;
			float dy = _map[x, z + 1] - height;

			// The "steepness" is the magnitude of the gradient vector
			// For a faster but not as accurate computation, you can just use abs(dx) + abs(dy)
			return MathF.Sqrt(dx * dx + dy * dy);
		}

		public string ToBinHex()
		{
			float[] arrayHeight = new float[RawWidth * RawHeight];

			for (int y = 0; y < RawHeight; y++)
			{
				for (int x = 0; x < RawWidth; x++)
				{
					arrayHeight[y * RawWidth + x] = _map[x, y];
				}
			}

			var byteHeightArray = new byte[arrayHeight.Length * 4];
			Buffer.BlockCopy(arrayHeight, 0, byteHeightArray, 0, byteHeightArray.Length);

			return Convert.ToBase64String(byteHeightArray, 0, byteHeightArray.Length);
		}

		public static HeightMap Parse(string binhexv3, int width, int height)
		{
			var binmap1d = Convert.FromBase64String(binhexv3);

			//var prod = width * height * 4;
			//Debug.Assert(binhexv3.Length == prod);

			var half1d = new ushort[width * height];

			Buffer.BlockCopy(binmap1d, 0, half1d, 0, binmap1d.Length);

			var float2d = new float[width, height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					float2d[x, y] = Half.FromRaw(half1d[y * width + x]);
					Debug.Assert(float2d[x, y] <= 1.000001f);
				}
			}

			return new HeightMap(float2d);
		}
	}
}
