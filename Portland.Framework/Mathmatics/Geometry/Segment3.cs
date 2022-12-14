using System;
using System.Collections.Generic;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Representation of a 3D line segment
	/// </summary>
	[Serializable]
	public struct Segment3 : IEquatable<Segment3>
	{
		public Vector3 a;
		public Vector3 b;

		/// <summary>
		/// Returns the normalized direction of the segment
		/// </summary>
		public Vector3 Direction => Vector3.Normalize((b - a));

		/// <summary>
		/// Returns the length of the segment
		/// </summary>
		public double Length => (b - a).magnitude;

		/// <summary>
		/// Returns the center of the segment
		/// </summary>
		public Vector2 Center => GetPoint2D(0.5f);

		/// <summary>
		/// Returns the axis-aligned bounding box of the segment
		/// </summary>
		public BoundingBox aabb
		{
			get
			{
				var bounds = new BoundingBox(Vector3.Min(a, b), Vector3.Max(a, b));
				return bounds;
			}
		}

		public Segment3(Vector3 a, Vector3 b)
		{
			this.a = a;
			this.b = b;
		}

		/// <summary>
		/// Access the a or b component using [0] or [1] respectively
		/// </summary>
		public Vector3 this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return a;
					case 1: return b;
					default:
						throw new IndexOutOfRangeException("Invalid Segment3 index!");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						a = value;
						break;
					case 1:
						b = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid Segment3 index!");
				}
			}
		}

		/// <summary>
		/// Returns a point on the segment at the given normalized position
		/// </summary>
		/// <param name="position">Normalized position</param>
		public Vector3 GetPoint(float position)
		{
			return Geometry.PointOnSegment3(a, b, position);
		}

		public Vector2 GetPoint2D(float position)
		{
			var v = Geometry.PointOnSegment3(a, b, position);
			return new Vector2(v.x, v.y);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on the segment
		/// </summary>
		/// <param name="count">Number of points</param>
		public List<Vector3> GetPoints(int count)
		{
			return Geometry.PointsOnSegment3(a, b, count);
		}

		/// <summary>
		/// Linearly interpolates between two segments
		/// </summary>
		public static Segment3 Lerp(Segment3 a, Segment3 b, float t)
		{
			t = MathHelper.Clamp01(t);
			return new Segment3(a.a + (b.a - a.a) * t, a.b + (b.b - a.b) * t);
		}

		/// <summary>
		/// Linearly interpolates between two segments without clamping the interpolant
		/// </summary>
		public static Segment3 LerpUnclamped(Segment3 a, Segment3 b, float t)
		{
			return new Segment3(a.a + (b.a - a.a) * t, a.b + (b.b - a.b) * t);
		}

		#region Casting operators

		public static explicit operator Line3(Segment3 segment)
		{
			return new Line3(segment.a, Vector3.Normalize((segment.b - segment.a)));
		}

		public static explicit operator Ray(Segment3 segment)
		{
			return new Ray(segment.a, Vector3.Normalize((segment.b - segment.a)));
		}

		public static explicit operator Segment2(Segment3 segment)
		{
			return new Segment2(new Vector2(segment.a.x, segment.a.y), new Vector2(segment.b.x, segment.b.y));
		}

		#endregion Casting operators

		public static Segment3 operator +(Segment3 segment, Vector3 vector)
		{
			return new Segment3(segment.a + vector, segment.b + vector);
		}

		public static Segment3 operator -(Segment3 segment, Vector3 vector)
		{
			return new Segment3(segment.a - vector, segment.b - vector);
		}

		public static bool operator ==(Segment3 a, Segment3 b)
		{
			return a.a == b.a && a.b == b.b;
		}

		public static bool operator !=(Segment3 a, Segment3 b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return a.GetHashCode() ^ (b.GetHashCode() << 2);
		}

		public override bool Equals(object other)
		{
			return other is Segment3 && Equals((Segment3)other);
		}

		public bool Equals(Segment3 other)
		{
			return a.Equals(other.a) && b.Equals(other.b);
		}

		public override string ToString()
		{
			return string.Format("Segment3(a: {0}, b: {1})", a, b);
		}
	}
}
