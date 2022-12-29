using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

using Portland.Mathmatics.Geometry;
using Portland.Mathmatics;
using Half = Portland.Mathmatics.Half;

namespace Portland.SceneGraph
{
	/// <summary>
	///   The TerrainData class stores heightmaps, detail mesh positions, tree instances, and terrain texture alpha maps.
	/// </summary>
	public class Terrain : Component
	{
		//private WeakReference<Transform> _trans;
		private readonly HeightMap _heightMap;

		public Terrain(HeightMap hmap)
		{
			//_trans = new WeakReference<Transform>(trans);
			_heightMap = hmap;
		}

		public Terrain(int width, int height)
		{
			//_trans = new WeakReference<Transform>(trans);
			_heightMap = new HeightMap(width, height);
		}

		public void SetSizeInWorldUnits(Vector3 size)
		{
			_heightMap.SizeInWorldUnits = size;
		}

		public override BoundingBox GetBoundingBox()
		{
			float widthDiv2 = _heightMap.SizeInWorldUnits.x / 2;
			float depthDiv2 = _heightMap.SizeInWorldUnits.z / 2;
			return new BoundingBox(new Vector3(Transform.Position.x - widthDiv2, Transform.Position.y, Transform.Position.z - depthDiv2), new Vector3(Transform.Position.x + widthDiv2, Transform.Position.y, Transform.Position.z + depthDiv2));
		}

		/// <summary>
		///   <para>Samples the height at the given position defined in world space, relative to the terrain space.</para>
		/// </summary>
		/// <param name="worldPosition"></param>
		public float SampleHeight(Vector3 worldPosition)
		{
			throw new NotImplementedException();
		}

		public void AddTree(Vector2 position, float widthScale, float heightScale)
		{
		}

		public void SetDetailInfo(int detailWidth, int detailHeight, int detailPatchCount, float detailResolution, float detailResolutionPerPatch)
		{
		}

		public void AddDetailLayer(short[,] data, string name, float minWidth, float minHeight, float maxWidth, float maxHeight)
		{
		}

		///// <summary>
		/////   <para>Adds a tree instance to the terrain.</para>
		///// </summary>
		///// <param name="instance"></param>
		//public void AddTreeInstance(TreeInstance instance)
		//{
		//	Terrain.INTERNAL_CALL_AddTreeInstance(this, ref instance);
		//}

		/// <summary>
		///   <para>Lets you setup the connection between neighboring Terrains.</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		public void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom)
		{
		}

		private string _alphamapLayers;
		private int _alphasLayerCnt;
		private float[,,] _alphas;

		public void ParseAlphas(int alphamapLayerCnt, string alphamapLayers, int width, int height, string binhex)
		{
			_alphamapLayers = alphamapLayers;
			_alphasLayerCnt = alphamapLayerCnt;

			var far = new ushort[width * height * alphamapLayerCnt];
			var bin = Convert.FromBase64String(binhex);

			Buffer.BlockCopy(bin, 0, far, 0, bin.Length);

			_alphas = new float[width, height, alphamapLayerCnt];

			for (int i = 0; i < alphamapLayerCnt; i++)
			{
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						_alphas[x, y, i] = Half.FromRaw(far[i * (height * width) + (y * width) + x]);
					}
				}
			}
		}
	}
}
