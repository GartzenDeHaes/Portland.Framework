using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
using UnityEngine.Rendering;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Helper class for procedural mesh generation
	/// https://github.com/Syomus/ProceduralToolkit
	/// </summary>
	[Serializable]
	public partial class Mesh
	{
		private string _name = String.Empty;
		public List<Vector3> Vertices = new List<Vector3>();
		public List<int> Triangles = new List<int>();
		public List<Vector3> Normals = new List<Vector3>();
		public List<Vector4> Tangents = new List<Vector4>();
		public List<Vector2> Uv = new List<Vector2>();
		//public List<Vector2> Uv2 = new List<Vector2>();
		//public List<Vector2> Uv3 = new List<Vector2>();
		//public List<Vector2> Uv4 = new List<Vector2>();
		public List<Color> Colors = new List<Color>();

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Shortcut for vertices.Count
		/// </summary>
		public int VertexCount => Vertices.Count;

		/// <summary>
		/// Creates an empty MeshDraft
		/// </summary>
		public Mesh()
		{
		}

		public Mesh(string name)
		{
			_name = name;
		}

#if UNITY_5_3_OR_NEWER
		/// <summary>
		/// Creates a new MeshDraft with vertex data from the <paramref name="mesh"/>>
		/// </summary>
		public Mesh(UnityEngine.Mesh mesh)
		{
			if (mesh == null)
				throw new ArgumentNullException(nameof(mesh));

			_name = mesh.name;

			var uvects = new List<UnityEngine.Vector3>();
			mesh.GetVertices(uvects);
			foreach (var v in uvects)
			{
				Vertices.Add(new Vector3f(v.x, v.y, v.z));
			}

			mesh.GetTriangles(Triangles, 0);

			uvects.Clear();
			mesh.GetNormals(uvects);
			foreach (var v in uvects)
			{
				Normals.Add(new Vector3(v.x, v.y, v.z));
			}
			uvects.Clear();

			var uv4 = new List<UnityEngine.Vector4>();
			mesh.GetTangents(uv4);
			foreach (var v in uv4)
			{
				Tangents.Add(new Vector4(v.x, v.y, v.z, v.w));
			}
			uv4.Clear();

			var uv2 = new List<UnityEngine.Vector2>();
			mesh.GetUVs(0, uv2);
			foreach (var v in uv2)
			{
				Uv.Add(new Vector2(v.x, v.y));
			}
			uv2.Clear();

			//mesh.GetUVs(1, uv2);
			//foreach (var v in uv2)
			//{
			//	Uv2.Add(new Vector2(v.x, v.y));
			//}
			//uv2.Clear();

			//mesh.GetUVs(2, uv2);
			//foreach (var v in uv2)
			//{
			//	Uv3.Add(new Vector2(v.x, v.y));
			//}
			//uv2.Clear();

			//mesh.GetUVs(3, uv2);
			//foreach (var v in uv2)
			//{
			//	Uv4.Add(new UnityEngine.Vector2(v.x, v.y));
			//}
			//uv2.Clear();

			var cl = new List<UnityEngine.Color>();
			mesh.GetColors(cl);
			foreach (var c in cl)
			{
				Colors.Add(new Color(c.r, c.g, c.b, c.a));
			}
			cl.Clear();
		}
#endif
		/// <summary>
		/// Adds vertex data from the <paramref name="draft"/>
		/// </summary>
		public Mesh Add(Mesh draft)
		{
			if (draft == null) throw new ArgumentNullException(nameof(draft));

			for (var i = 0; i < draft.Triangles.Count; i++)
			{
				Triangles.Add(draft.Triangles[i] + Vertices.Count);
			}
			Vertices.AddRange(draft.Vertices);
			Normals.AddRange(draft.Normals);
			Tangents.AddRange(draft.Tangents);
			Uv.AddRange(draft.Uv);
			//Uv2.AddRange(draft.Uv2);
			//Uv3.AddRange(draft.Uv3);
			//Uv4.AddRange(draft.Uv4);
			Colors.AddRange(draft.Colors);
			return this;
		}

		#region AddTriangle

		/// <summary>
		/// Adds a triangle to the draft
		/// </summary>
		public Mesh AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, bool calculateNormal)
		{
			if (calculateNormal)
			{
				Vector3 normal = Vector3.Normalize(Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0));
				return AddTriangle(vertex0, vertex1, vertex2, normal, normal, normal);
			}
			return _AddTriangle(vertex0, vertex1, vertex2);
		}

		/// <summary>
		/// Adds a triangle to the draft
		/// </summary>
		public Mesh AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 normal)
		{
			return AddTriangle(vertex0, vertex1, vertex2, normal, normal, normal);
		}

		/// <summary>
		/// Adds a triangle to the draft
		/// </summary>
		public Mesh AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 normal0, Vector3 normal1, Vector3 normal2)
		{
			Normals.Add(normal0);
			Normals.Add(normal1);
			Normals.Add(normal2);
			return _AddTriangle(vertex0, vertex1, vertex2);
		}

		/// <summary>
		/// Adds a triangle to the draft
		/// </summary>
		public Mesh AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, bool calculateNormal,
			 Vector2 uv0, Vector2 uv1, Vector2 uv2)
		{
			Uv.Add(uv0);
			Uv.Add(uv1);
			Uv.Add(uv2);
			return AddTriangle(vertex0, vertex1, vertex2, calculateNormal);
		}

		/// <summary>
		/// Adds a triangle to the draft
		/// </summary>
		public Mesh AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 normal,
			 Vector2 uv0, Vector2 uv1, Vector2 uv2)
		{
			Uv.Add(uv0);
			Uv.Add(uv1);
			Uv.Add(uv2);
			return AddTriangle(vertex0, vertex1, vertex2, normal, normal, normal);
		}

		/// <summary>
		/// Adds a triangle to the draft
		/// </summary>
		public Mesh AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 normal0, Vector3 normal1, Vector3 normal2,
			 Vector2 uv0, Vector2 uv1, Vector2 uv2)
		{
			Uv.Add(uv0);
			Uv.Add(uv1);
			Uv.Add(uv2);
			return AddTriangle(vertex0, vertex1, vertex2, normal0, normal1, normal2);
		}

		private Mesh _AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
		{
			Triangles.Add(0 + Vertices.Count);
			Triangles.Add(1 + Vertices.Count);
			Triangles.Add(2 + Vertices.Count);
			Vertices.Add(vertex0);
			Vertices.Add(vertex1);
			Vertices.Add(vertex2);
			return this;
		}

		#endregion AddTriangle

		#region AddQuad

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 origin, Vector3 width, Vector3 height, bool calculateNormal)
		{
			Vector3 vertex0 = origin;
			Vector3 vertex1 = origin + height;
			Vector3 vertex2 = origin + height + width;
			Vector3 vertex3 = origin + width;
			if (calculateNormal)
			{
				Vector3 normal = Vector3.Normalize(Vector3.Cross(height, width));
				return AddQuad(vertex0, vertex1, vertex2, vertex3, normal, normal, normal, normal);
			}
			return _AddQuad(vertex0, vertex1, vertex2, vertex3);
		}

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, bool calculateNormal)
		{
			if (calculateNormal)
			{
				Vector3 normal = Vector3.Normalize(Vector3.Cross(vertex1 - vertex0, vertex3 - vertex0));

				// Fix for degenerate triangle-like quads
				if (normal.SqrMagnitude < MathHelper.Epsilonf)
				{
					normal = Vector3.Normalize(Vector3.Cross(vertex3 - vertex2, vertex1 - vertex2));
				}
				return AddQuad(vertex0, vertex1, vertex2, vertex3, normal, normal, normal, normal);
			}
			return _AddQuad(vertex0, vertex1, vertex2, vertex3);
		}

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 normal)
		{
			return AddQuad(vertex0, vertex1, vertex2, vertex3, normal, normal, normal, normal);
		}

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3,
			 Vector3 normal0, Vector3 normal1, Vector3 normal2, Vector3 normal3)
		{
			Normals.Add(normal0);
			Normals.Add(normal1);
			Normals.Add(normal2);
			Normals.Add(normal3);
			return _AddQuad(vertex0, vertex1, vertex2, vertex3);
		}

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 origin, Vector3 width, Vector3 height, bool calculateNormal,
			 Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			Uv.Add(uv0);
			Uv.Add(uv1);
			Uv.Add(uv2);
			Uv.Add(uv3);
			return AddQuad(origin, width, height, calculateNormal);
		}

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, bool calculateNormal,
			 Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			Uv.Add(uv0);
			Uv.Add(uv1);
			Uv.Add(uv2);
			Uv.Add(uv3);
			return AddQuad(vertex0, vertex1, vertex2, vertex3, calculateNormal);
		}

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 normal,
			 Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			Uv.Add(uv0);
			Uv.Add(uv1);
			Uv.Add(uv2);
			Uv.Add(uv3);
			return AddQuad(vertex0, vertex1, vertex2, vertex3, normal, normal, normal, normal);
		}

		/// <summary>
		/// Adds a quad to the draft
		/// </summary>
		public Mesh AddQuad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3,
			 Vector3 normal0, Vector3 normal1, Vector3 normal2, Vector3 normal3,
			 Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			Uv.Add(uv0);
			Uv.Add(uv1);
			Uv.Add(uv2);
			Uv.Add(uv3);
			return AddQuad(vertex0, vertex1, vertex2, vertex3, normal0, normal1, normal2, normal3);
		}

		private Mesh _AddQuad(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
		{
			Triangles.Add(0 + Vertices.Count);
			Triangles.Add(1 + Vertices.Count);
			Triangles.Add(2 + Vertices.Count);
			Triangles.Add(0 + Vertices.Count);
			Triangles.Add(2 + Vertices.Count);
			Triangles.Add(3 + Vertices.Count);
			Vertices.Add(vertex0);
			Vertices.Add(vertex1);
			Vertices.Add(vertex2);
			Vertices.Add(vertex3);
			return this;
		}

		#endregion AddQuad

		#region AddTriangleFan

		/// <summary>
		/// Adds a triangle fan to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_fan
		/// </remarks>
		public Mesh AddTriangleFan(IList<Vector3> fan, bool reverseTriangles = false)
		{
			Vector3 normal = Vector3.Normalize(Vector3.Cross(fan[1] - fan[0], fan[fan.Count - 1] - fan[0]));
			return AddTriangleFan(fan, normal, reverseTriangles);
		}

		/// <summary>
		/// Adds a triangle fan to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_fan
		/// </remarks>
		public Mesh AddTriangleFan(IList<Vector3> fan, Vector3 normal, bool reverseTriangles = false)
		{
			AddTriangleFanVertices(fan, reverseTriangles);
			for (int i = 0; i < fan.Count; i++)
			{
				Normals.Add(normal);
			}
			return this;
		}

		/// <summary>
		/// Adds a triangle fan to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_fan
		/// </remarks>
		public Mesh AddTriangleFan(IList<Vector3> fan, IList<Vector3> normals, bool reverseTriangles = false)
		{
			AddTriangleFanVertices(fan, reverseTriangles);
			this.Normals.AddRange(normals);
			return this;
		}

		/// <summary>
		/// Adds a triangle fan to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_fan
		/// </remarks>
		public Mesh AddTriangleFan(IList<Vector3> fan, IList<Vector2> uv, bool reverseTriangles = false)
		{
			this.Uv.AddRange(uv);
			return AddTriangleFan(fan, reverseTriangles);
		}

		/// <summary>
		/// Adds a triangle fan to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_fan
		/// </remarks>
		public Mesh AddTriangleFan(IList<Vector3> fan, Vector3 normal, IList<Vector2> uv, bool reverseTriangles = false)
		{
			this.Uv.AddRange(uv);
			return AddTriangleFan(fan, normal, reverseTriangles);
		}

		/// <summary>
		/// Adds a triangle fan to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_fan
		/// </remarks>
		public Mesh AddTriangleFan(IList<Vector3> fan, IList<Vector3> normals, IList<Vector2> uv, bool reverseTriangles = false)
		{
			this.Uv.AddRange(uv);
			return AddTriangleFan(fan, normals, reverseTriangles);
		}

		private void AddTriangleFanVertices(IList<Vector3> fan, bool reverseTriangles)
		{
			int count = Vertices.Count;
			if (reverseTriangles)
			{
				for (int i = fan.Count - 1; i > 1; i--)
				{
					Triangles.Add(0 + count);
					Triangles.Add(i + count);
					Triangles.Add(i - 1 + count);
				}
			}
			else
			{
				for (int i = 1; i < fan.Count - 1; i++)
				{
					Triangles.Add(0 + count);
					Triangles.Add(i + count);
					Triangles.Add(i + 1 + count);
				}
			}
			Vertices.AddRange(fan);
		}

		#endregion AddTriangleFan

		#region AddTriangleStrip

		/// <summary>
		/// Adds a triangle strip to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_strip
		/// </remarks>
		public Mesh AddTriangleStrip(IList<Vector3> strip)
		{
			Vector3 normal = Vector3.Normalize(Vector3.Cross(strip[1] - strip[0], strip[2] - strip[0]));
			return AddTriangleStrip(strip, normal);
		}

		/// <summary>
		/// Adds a triangle strip to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_strip
		/// </remarks>
		public Mesh AddTriangleStrip(IList<Vector3> strip, Vector3 normal)
		{
			for (int i = 0, j = 1, k = 2;
				 i < strip.Count - 2;
				 i++, j += i % 2 * 2, k += (i + 1) % 2 * 2)
			{
				Triangles.Add(i + Vertices.Count);
				Triangles.Add(j + Vertices.Count);
				Triangles.Add(k + Vertices.Count);
			}
			Vertices.AddRange(strip);
			for (int i = 0; i < strip.Count; i++)
			{
				Normals.Add(normal);
			}
			return this;
		}

		/// <summary>
		/// Adds a triangle strip to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_strip
		/// </remarks>
		public Mesh AddTriangleStrip(IList<Vector3> strip, IList<Vector3> normals)
		{
			for (int i = 0, j = 1, k = 2;
				 i < strip.Count - 2;
				 i++, j += i % 2 * 2, k += (i + 1) % 2 * 2)
			{
				Triangles.Add(i + Vertices.Count);
				Triangles.Add(j + Vertices.Count);
				Triangles.Add(k + Vertices.Count);
			}
			Vertices.AddRange(strip);
			this.Normals.AddRange(normals);
			return this;
		}

		/// <summary>
		/// Adds a triangle strip to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_strip
		/// </remarks>
		public Mesh AddTriangleStrip(IList<Vector3> strip, IList<Vector2> uv)
		{
			this.Uv.AddRange(uv);
			return AddTriangleStrip(strip);
		}

		/// <summary>
		/// Adds a triangle strip to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_strip
		/// </remarks>
		public Mesh AddTriangleStrip(IList<Vector3> strip, Vector3 normal, IList<Vector2> uv)
		{
			this.Uv.AddRange(uv);
			return AddTriangleStrip(strip, normal);
		}

		/// <summary>
		/// Adds a triangle strip to the draft
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Triangle_strip
		/// </remarks>
		public Mesh AddTriangleStrip(IList<Vector3> strip, IList<Vector3> normals, IList<Vector2> uv)
		{
			this.Uv.AddRange(uv);
			return AddTriangleStrip(strip, normals);
		}

		#endregion AddTriangleStrip

		#region AddBaselessPyramid

		/// <summary>
		/// Adds a baseless pyramid to the draft
		/// </summary>
		public Mesh AddBaselessPyramid(Vector3 apex, IList<Vector3> ring, bool generateUV, bool reverseTriangles = false)
		{
			if (generateUV)
			{
				var uv00 = new Vector2(0, 0);
				var uvApex = new Vector2(0.5f, 1);
				var uv10 = new Vector2(1, 0);
				if (reverseTriangles)
				{
					for (int i = ring.Count - 1; i > 0; i--)
					{
						AddTriangle(ring[i - 1], apex, ring[i], true, uv00, uvApex, uv10);
					}
					AddTriangle(ring[ring.Count - 1], apex, ring[0], true, uv00, uvApex, uv10);
				}
				else
				{
					for (var i = 0; i < ring.Count - 1; i++)
					{
						AddTriangle(ring[i + 1], apex, ring[i], true, uv00, uvApex, uv10);
					}
					AddTriangle(ring[0], apex, ring[ring.Count - 1], true, uv00, uvApex, uv10);
				}
			}
			else
			{
				if (reverseTriangles)
				{
					for (int i = ring.Count - 1; i > 0; i--)
					{
						AddTriangle(ring[i - 1], apex, ring[i], true);
					}
					AddTriangle(ring[ring.Count - 1], apex, ring[0], true);
				}
				else
				{
					for (var i = 0; i < ring.Count - 1; i++)
					{
						AddTriangle(ring[i + 1], apex, ring[i], true);
					}
					AddTriangle(ring[0], apex, ring[ring.Count - 1], true);
				}
			}
			return this;
		}

		#endregion AddBaselessPyramid

		#region AddFlatTriangleBand

		/// <summary>
		/// Adds a band made from triangles to the draft
		/// </summary>
		public Mesh AddFlatTriangleBand(IList<Vector3> lowerRing, IList<Vector3> upperRing, bool generateUV)
		{
			if (lowerRing.Count != upperRing.Count)
			{
				throw new ArgumentException("Array sizes must be equal");
			}
			if (lowerRing.Count < 3)
			{
				throw new ArgumentException("Array sizes must be greater than 2");
			}

			Vector2 uv00 = new Vector2(0, 0);
			Vector2 uvBottomCenter = new Vector2(0.5f, 0);
			Vector2 uv10 = new Vector2(1, 0);
			Vector2 uv01 = new Vector2(0, 1);
			Vector2 uvTopCenter = new Vector2(0.5f, 1);
			Vector2 uv11 = new Vector2(1, 1);

			Vector3 lower0, upper0, lower1, upper1;
			for (int i = 0; i < lowerRing.Count - 1; i++)
			{
				lower0 = lowerRing[i];
				lower1 = lowerRing[i + 1];
				upper0 = upperRing[i];
				upper1 = upperRing[i + 1];
				if (generateUV)
				{
					AddTriangle(lower1, upper0, lower0, true, uv00, uvTopCenter, uv10);
					AddTriangle(lower1, upper1, upper0, true, uvBottomCenter, uv01, uv11);
				}
				else
				{
					AddTriangle(lower1, upper0, lower0, true);
					AddTriangle(lower1, upper1, upper0, true);
				}
			}

			lower0 = lowerRing[lowerRing.Count - 1];
			lower1 = lowerRing[0];
			upper0 = upperRing[upperRing.Count - 1];
			upper1 = upperRing[0];
			if (generateUV)
			{
				AddTriangle(lower1, upper0, lower0, true, uv00, uvTopCenter, uv10);
				AddTriangle(lower1, upper1, upper0, true, uvBottomCenter, uv01, uv11);
			}
			else
			{
				AddTriangle(lower1, upper0, lower0, true);
				AddTriangle(lower1, upper1, upper0, true);
			}
			return this;
		}

		#endregion AddFlatTriangleBand

		#region AddFlatQuadBand

		/// <summary>
		/// Adds a band made from quads to the draft
		/// </summary>
		public Mesh AddFlatQuadBand(IList<Vector3> lowerRing, IList<Vector3> upperRing, bool generateUV)
		{
			if (lowerRing.Count != upperRing.Count)
			{
				throw new ArgumentException("Array sizes must be equal");
			}
			if (lowerRing.Count < 3)
			{
				throw new ArgumentException("Array sizes must be greater than 2");
			}

			Vector2 uv00 = new Vector2(0, 0);
			Vector2 uv10 = new Vector2(1, 0);
			Vector2 uv01 = new Vector2(0, 1);
			Vector2 uv11 = new Vector2(1, 1);

			Vector3 lower0, upper0, lower1, upper1;
			for (int i = 0; i < lowerRing.Count - 1; i++)
			{
				lower0 = lowerRing[i];
				lower1 = lowerRing[i + 1];
				upper0 = upperRing[i];
				upper1 = upperRing[i + 1];
				if (generateUV)
				{
					AddQuad(lower1, upper1, upper0, lower0, true, uv00, uv01, uv11, uv10);
				}
				else
				{
					AddQuad(lower1, upper1, upper0, lower0, true);
				}
			}

			lower0 = lowerRing[lowerRing.Count - 1];
			lower1 = lowerRing[0];
			upper0 = upperRing[upperRing.Count - 1];
			upper1 = upperRing[0];
			if (generateUV)
			{
				AddQuad(lower1, upper1, upper0, lower0, true, uv00, uv01, uv11, uv10);
			}
			else
			{
				AddQuad(lower1, upper1, upper0, lower0, true);
			}
			return this;
		}

		#endregion AddFlatQuadBand

		/// <summary>
		/// Clears all vertex data and all triangle indices
		/// </summary>
		public void Clear()
		{
			Vertices.Clear();
			Triangles.Clear();
			Normals.Clear();
			Tangents.Clear();
			Uv.Clear();
			//Uv2.Clear();
			//Uv3.Clear();
			//Uv4.Clear();
			Colors.Clear();
		}

		/// <summary>
		/// Moves draft vertices by <paramref name="vector"/>
		/// </summary>
		public Mesh Move(Vector3 vector)
		{
			for (int i = 0; i < Vertices.Count; i++)
			{
				Vertices[i] += vector;
			}
			return this;
		}

		/// <summary>
		/// Rotates draft vertices by <paramref name="rotation"/>
		/// </summary>
		public Mesh Rotate(Quaternion rotation)
		{
			for (int i = 0; i < Vertices.Count; i++)
			{
				Vertices[i] = rotation * Vertices[i];
				Normals[i] = rotation * Normals[i];
			}
			return this;
		}

		/// <summary>
		/// Scales draft vertices uniformly by <paramref name="scale"/>
		/// </summary>
		public Mesh Scale(float scale)
		{
			for (int i = 0; i < Vertices.Count; i++)
			{
				Vertices[i] *= scale;
			}
			return this;
		}

		/// <summary>
		/// Scales draft vertices non-uniformly by <paramref name="scale"/>
		/// </summary>
		public Mesh Scale(Vector3 scale)
		{
			for (int i = 0; i < Vertices.Count; i++)
			{
				Vertices[i] = Vector3.Multiply(Vertices[i], scale);
				Normals[i] = Vector3.Normalize(Vector3.Multiply(Normals[i], scale));
			}
			return this;
		}

		/// <summary>
		/// Paints draft vertices with <paramref name="color"/>
		/// </summary>
		public Mesh Paint(Color color)
		{
			Colors.Clear();
			for (int i = 0; i < Vertices.Count; i++)
			{
				Colors.Add(color);
			}
			return this;
		}

		/// <summary>
		/// Flips draft faces
		/// </summary>
		public Mesh FlipFaces()
		{
			FlipTriangles();
			FlipNormals();
			return this;
		}

		/// <summary>
		/// Reverses the winding order of draft triangles
		/// </summary>
		public Mesh FlipTriangles()
		{
			for (int i = 0; i < Triangles.Count; i += 3)
			{
				var temp = Triangles[i];
				Triangles[i] = Triangles[i + 1];
				Triangles[i + 1] = temp;
			}
			return this;
		}

		/// <summary>
		/// Reverses the direction of draft normals
		/// </summary>
		public Mesh FlipNormals()
		{
			for (int i = 0; i < Normals.Count; i++)
			{
				Normals[i] = -Normals[i];
			}
			return this;
		}

		/// <summary>
		/// Flips the UV map horizontally in the selected <paramref name="channel"/>
		/// </summary>
		public Mesh FlipUVHorizontally(int channel = 0)
		{
			List<Vector2> list;
			switch (channel)
			{
				case 0:
					list = Uv;
					break;
				//case 1:
				//	list = Uv2;
				//	break;
				//case 2:
				//	list = Uv3;
				//	break;
				//case 3:
				//	list = Uv4;
				//	break;
				default:
					throw new ArgumentOutOfRangeException(nameof(channel));
			}
			for (var i = 0; i < list.Count; i++)
			{
				list[i] = new Vector2(1 - list[i].X, list[i].Y);
			}
			return this;
		}

		/// <summary>
		/// Flips the UV map vertically in the selected <paramref name="channel"/>
		/// </summary>
		public Mesh FlipUVVertically(int channel = 0)
		{
			List<Vector2> list;
			switch (channel)
			{
				case 0:
					list = Uv;
					break;
				//case 1:
				//	list = Uv2;
				//	break;
				//case 2:
				//	list = Uv3;
				//	break;
				//case 3:
				//	list = Uv4;
				//	break;
				default:
					throw new ArgumentOutOfRangeException(nameof(channel));
			}
			for (var i = 0; i < list.Count; i++)
			{
				list[i] = new Vector2(list[i].X, 1 - list[i].Y);
			}
			return this;
		}

		/// <summary>
		/// Projects vertices on a sphere with the given <paramref name="radius"/> and <paramref name="center"/>, recalculates normals
		/// </summary>
		public Mesh Spherify(float radius, Vector3 center = default)
		{
			for (var i = 0; i < Vertices.Count; i++)
			{
				Normals[i] = Vector3.Normalize(Vertices[i] - center);
				Vertices[i] = Normals[i] * radius;
			}
			return this;
		}

#if UNITY_5_3_OR_NEWER
		/// <summary>
		/// Creates a new mesh from the data in the draft
		/// </summary>
		/// <param name="calculateBounds"> Calculate the bounding box of the Mesh after setting the triangles. </param>
		/// <param name="autoIndexFormat"> Use 16 bit or 32 bit index buffers based on vertex count. </param>
		public UnityEngine.Mesh ToMesh(bool calculateBounds = true, bool autoIndexFormat = true)
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
		public void ToMesh(ref UnityEngine.Mesh mesh, bool calculateBounds = true, bool autoIndexFormat = true)
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
			if (VertexCount > 65535)
			{
				if (autoIndexFormat)
				{
					mesh.indexFormat = IndexFormat.UInt32;
				}
				else
				{
					UnityEngine.Debug.LogError("A mesh can't have more than 65535 vertices with 16 bit index buffer. Vertex count: " + VertexCount);
					mesh.indexFormat = IndexFormat.UInt16;
				}
			}
			else
			{
				if (autoIndexFormat)
				{
					mesh.indexFormat = IndexFormat.UInt16;
				}
			}
			mesh.name = Name;

			//mesh.SetVertices(Vertices);
			//mesh.SetTriangles(Triangles, 0, calculateBounds);
			//mesh.SetNormals(Normals);
			//mesh.SetTangents(Tangents);
			//mesh.SetUVs(0, Uv);
			//mesh.SetUVs(1, Uv2);
			//mesh.SetUVs(2, Uv3);
			//mesh.SetUVs(3, Uv4);
			//mesh.SetColors(Colors);

			var uvects = new List<UnityEngine.Vector3>();
			foreach (var v in Vertices)
			{
				uvects.Add(new UnityEngine.Vector3(v.X, v.Y, v.Z));
			}
			mesh.SetVertices(uvects);
			uvects.Clear();

			mesh.SetTriangles(Triangles, 0, calculateBounds);

			foreach (var v in Normals)
			{
				uvects.Add(new UnityEngine.Vector3(v.X, v.Y, v.Z));
			}
			mesh.SetNormals(uvects);
			uvects.Clear();

			var uv4 = new List<UnityEngine.Vector4>();
			foreach (var v in Tangents)
			{
				uv4.Add(new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W));
			}
			mesh.SetTangents(uv4);
			uv4.Clear();

			var uv2 = new List<UnityEngine.Vector2>();
			foreach (var v in Uv)
			{
				uv2.Add(new UnityEngine.Vector2(v.X, v.Y));
			}
			mesh.SetUVs(0, uv2);
			uv2.Clear();

			//foreach (var v in Uv2)
			//{
			//	uv2.Add(new UnityEngine.Vector2(v.X, v.Y));
			//}
			//mesh.SetUVs(1, uv2);
			//uv2.Clear();

			//foreach (var v in Uv3)
			//{
			//	uv2.Add(new UnityEngine.Vector2(v.X, v.Y));
			//}
			//mesh.SetUVs(2, uv2);
			//uv2.Clear();

			//foreach (var v in Uv4)
			//{
			//	uv2.Add(new UnityEngine.Vector2(v.X, v.Y));
			//}
			//mesh.SetUVs(3, uv2);
			//uv2.Clear();

			var cl = new List<UnityEngine.Color>();
			foreach (var c in Colors)
			{
				cl.Add(new UnityEngine.Color(c.R, c.G, c.B, c.A));
			}
			mesh.SetColors(cl);
			cl.Clear();
		}

#endif

		public override string ToString()
		{
			return _name + " (ProceduralToolkit.MeshDraft)";
		}
	}
}
