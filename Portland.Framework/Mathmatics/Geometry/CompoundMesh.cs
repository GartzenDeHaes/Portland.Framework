using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

#if UNITY_5_3_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Helper class for mesh generation supporting large meshes and submeshes
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public class CompoundMeshDraft : IEnumerable<Mesh>
	{
		public string name = "";

		public int VertexCount
		{
			get
			{
				int count = 0;
				foreach (var meshDraft in _meshDrafts)
				{
					count += meshDraft.VertexCount;
				}
				return count;
			}
		}

		private readonly List<Mesh> _meshDrafts = new List<Mesh>();

		public IEnumerator<Mesh> GetEnumerator()
		{
			return _meshDrafts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public CompoundMeshDraft Add(Mesh draft)
		{
			if (draft == null) throw new ArgumentNullException(nameof(draft));

			_meshDrafts.Add(draft);
			return this;
		}

		public CompoundMeshDraft Add(CompoundMeshDraft compoundDraft)
		{
			if (compoundDraft == null) throw new ArgumentNullException(nameof(compoundDraft));

			_meshDrafts.AddRange(compoundDraft._meshDrafts);
			return this;
		}

		/// <summary>
		/// Clears all vertex data and all triangle indices
		/// </summary>
		public void Clear()
		{
			_meshDrafts.Clear();
		}

		/// <summary>
		/// Moves draft vertices by <paramref name="vector"/>
		/// </summary>
		public CompoundMeshDraft Move(Vector3 vector)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.Move(vector);
			}
			return this;
		}

		/// <summary>
		/// Rotates draft vertices by <paramref name="rotation"/>
		/// </summary>
		public CompoundMeshDraft Rotate(Quaternion rotation)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.Rotate(rotation);
			}
			return this;
		}

		/// <summary>
		/// Scales draft vertices uniformly by <paramref name="scale"/>
		/// </summary>
		public CompoundMeshDraft Scale(float scale)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.Scale(scale);
			}
			return this;
		}

		/// <summary>
		/// Scales draft vertices non-uniformly by <paramref name="scale"/>
		/// </summary>
		public CompoundMeshDraft Scale(Vector3 scale)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.Scale(scale);
			}
			return this;
		}

		/// <summary>
		/// Paints draft vertices with <paramref name="color"/>
		/// </summary>
		public CompoundMeshDraft Paint(Color color)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.Paint(color);
			}
			return this;
		}

		/// <summary>
		/// Flips draft faces
		/// </summary>
		public CompoundMeshDraft FlipFaces()
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.FlipFaces();
			}
			return this;
		}

		/// <summary>
		/// Reverses the winding order of draft triangles
		/// </summary>
		public CompoundMeshDraft FlipTriangles()
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.FlipTriangles();
			}
			return this;
		}

		/// <summary>
		/// Reverses the direction of draft normals
		/// </summary>
		public CompoundMeshDraft FlipNormals()
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.FlipNormals();
			}
			return this;
		}

		/// <summary>
		/// Flips the UV map horizontally in the selected <paramref name="channel"/>
		/// </summary>
		public CompoundMeshDraft FlipUVHorizontally(int channel = 0)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.FlipUVHorizontally(channel);
			}
			return this;
		}

		/// <summary>
		/// Flips the UV map vertically in the selected <paramref name="channel"/>
		/// </summary>
		public CompoundMeshDraft FlipUVVertically(int channel = 0)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.FlipUVVertically(channel);
			}
			return this;
		}

		/// <summary>
		/// Projects vertices on a sphere with the given <paramref name="radius"/> and <paramref name="center"/>, recalculates normals
		/// </summary>
		public CompoundMeshDraft Spherify(float radius, Vector3 center = default)
		{
			foreach (var meshDraft in _meshDrafts)
			{
				meshDraft.Spherify(radius, center);
			}
			return this;
		}

		public void MergeDraftsWithTheSameName()
		{
			for (int i = 0; i < _meshDrafts.Count; i++)
			{
				var merged = MergeDraftsWithName(_meshDrafts[i].Name);
				_meshDrafts.Insert(i, merged);
			}
		}

		private Mesh MergeDraftsWithName(string draftName)
		{
			var merged = new Mesh(draftName);
			for (int i = 0; i < _meshDrafts.Count; i++)
			{
				var meshDraft = _meshDrafts[i];
				if (meshDraft.Name == draftName)
				{
					merged.Add(meshDraft);
					_meshDrafts.RemoveAt(i);
					i--;
				}
			}
			return merged;
		}

		public void SortDraftsByName()
		{
			_meshDrafts.Sort((a, b) => a.Name.CompareTo(b.Name));
		}

		public Mesh ToMesh()
		{
			var finalDraft = new Mesh();
			foreach (var meshDraft in _meshDrafts)
			{
				finalDraft.Add(meshDraft);
			}
			return finalDraft;
		}

#if UNITY_5_3_OR_NEWER
		/// <summary>
		/// Creates a new mesh from the data in the draft
		/// </summary>
		/// <param name="calculateBounds"> Calculate the bounding box of the Mesh after setting the triangles. </param>
		/// <param name="autoIndexFormat"> Use 16 bit or 32 bit index buffers based on vertex count. </param>
		public UnityEngine.Mesh ToMeshWithSubMeshes(bool calculateBounds = true, bool autoIndexFormat = true)
		{
			var mesh = new UnityEngine.Mesh();
			FillMesh(ref mesh, calculateBounds, autoIndexFormat);
			return mesh;
		}

		/// <summary>
		/// Fills the <paramref name="mesh"/> with the data in the draft
		/// </summary>
		/// <param name="mesh"> Resulting mesh. Cleared before use. </param>
		/// <param name="calculateBounds"> Calculate the bounding box of the Mesh after setting the triangles. </param>
		/// <param name="autoIndexFormat"> Use 16 bit or 32 bit index buffers based on vertex count. </param>
		public void ToMeshWithSubMeshes(ref UnityEngine.Mesh mesh, bool calculateBounds = true, bool autoIndexFormat = true)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException(nameof(mesh));
			}
			mesh.Clear(false);
			FillMesh(ref mesh, calculateBounds, autoIndexFormat);
		}

		private void FillMesh(ref UnityEngine.Mesh mesh, bool calculateBounds, bool autoIndexFormat)
		{
			int vCount = VertexCount;
			if (vCount > 65535)
			{
				if (autoIndexFormat)
				{
					mesh.indexFormat = IndexFormat.UInt32;
				}
				else
				{
					UnityEngine.Debug.LogError("A mesh can't have more than 65535 vertices with 16 bit index buffer. Vertex count: " + vCount);
					mesh.indexFormat = IndexFormat.UInt16;
				}
			}
			else
			{
				mesh.indexFormat = IndexFormat.UInt16;
			}

			var finalDraft = new Mesh();
			foreach (var meshDraft in _meshDrafts)
			{
				finalDraft.Add(meshDraft);
			}

			mesh.name = name;

			var vl = new List<UnityEngine.Vector3>();
			foreach (var v in finalDraft.Vertices)
			{
				vl.Add(new UnityEngine.Vector3(v.X, v.Y, v.Z));
			}
			mesh.SetVertices(vl);
			vl.Clear();

			mesh.subMeshCount = _meshDrafts.Count;

			int baseVertex = 0;
			for (int i = 0; i < _meshDrafts.Count; i++)
			{
				var draft = _meshDrafts[i];
				mesh.SetTriangles(draft.Triangles, i, false, baseVertex);
				baseVertex += draft.VertexCount;
			}
			if (calculateBounds)
			{
				mesh.RecalculateBounds();
			}

			foreach (var v in finalDraft.Normals)
			{
				vl.Add(new UnityEngine.Vector3(v.X, v.Y, v.Z));
			}
			mesh.SetNormals(vl);
			vl.Clear();

			var v4 = new List<UnityEngine.Vector4>();
			foreach (var v in finalDraft.Tangents)
			{
				v4.Add(new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W));
			}
			mesh.SetTangents(v4);
			v4.Clear();

			var uvl = new List<UnityEngine.Vector2>();
			foreach (var v in finalDraft.Uv)
			{
				uvl.Add(new UnityEngine.Vector2(v.X, v.Y));
			}
			mesh.SetUVs(0, uvl);
			uvl.Clear();

			foreach (var v in finalDraft.Uv2)
			{
				uvl.Add(new UnityEngine.Vector2(v.X, v.Y));
			}
			mesh.SetUVs(1, uvl);
			uvl.Clear();

			foreach (var v in finalDraft.Uv3)
			{
				uvl.Add(new UnityEngine.Vector2(v.X, v.Y));
			}
			mesh.SetUVs(2, uvl);
			uvl.Clear();

			foreach (var v in finalDraft.Uv4)
			{
				uvl.Add(new UnityEngine.Vector2(v.X, v.Y));
			}
			mesh.SetUVs(3, uvl);
			uvl.Clear();

			var lc = new List<UnityEngine.Color>();
			foreach (var c in finalDraft.Colors)
			{
				lc.Add(new UnityEngine.Color(c.R, c.G, c.B, c.A));
			}
			mesh.SetColors(lc);
			lc.Clear();
		}

#endif

		public override string ToString()
		{
			return name + " (ProceduralToolkit.CompoundMeshDraft)";
		}
	}
}
