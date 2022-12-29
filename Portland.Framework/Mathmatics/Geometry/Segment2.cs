using System;
using System.Collections.Generic;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Representation of a 2D line segment
	/// </summary>
	[Serializable]
	public struct Segment2 : IEquatable<Segment2>
	{
		public Vector2 a;
		public Vector2 b;

		/// <summary>
		/// Returns the normalized direction of the segment
		/// </summary>
		public Vector2 Direction => Vector2.Normalize(b - a);
		/// <summary>
		/// Returns the length of the segment
		/// </summary>
		public double Length => (b - a).Magnitude;
		/// <summary>
		/// Returns the center of the segment
		/// </summary>
		public Vector2 Center => GetPoint(0.5f);

		/// <summary>
		/// Returns the axis-aligned bounding box of the segment
		/// </summary>
		public Rect aabb
		{
			get
			{
				Vector2 min = Vector2.Min(a, b);
				Vector2 max = Vector2.Max(a, b);
				return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
			}
		}

		public Segment2(Vector2 a, Vector2 b)
		{
			this.a = a;
			this.b = b;
		}

		/// <summary>
		/// Access the a or b component using [0] or [1] respectively
		/// </summary>
		public Vector2 this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return a;
					case 1: return b;
					default:
						throw new IndexOutOfRangeException("Invalid Segment2 index!");
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
						throw new IndexOutOfRangeException("Invalid Segment2 index!");
				}
			}
		}

		/// <summary>
		/// Returns a point on the segment at the given normalized position
		/// </summary>
		/// <param name="position">Normalized position</param>
		public Vector2 GetPoint(float position)
		{
			return Geometry.PointOnSegment2(a, b, position);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on the segment
		/// </summary>
		/// <param name="count">Number of points</param>
		public List<Vector2> GetPoints(int count)
		{
			return Geometry.PointsOnSegment2(a, b, count);
		}

		/// <summary>
		/// Linearly interpolates between two segments
		/// </summary>
		public static Segment2 Lerp(Segment2 a, Segment2 b, float t)
		{
			t = MathHelper.Clamp01(t);
			return new Segment2(a.a + (b.a - a.a) * t, a.b + (b.b - a.b) * t);
		}

		/// <summary>
		/// Linearly interpolates between two segments without clamping the interpolant
		/// </summary>
		public static Segment2 LerpUnclamped(Segment2 a, Segment2 b, float t)
		{
			return new Segment2(a.a + (b.a - a.a) * t, a.b + (b.b - a.b) * t);
		}

		#region Casting operators

		public static explicit operator Line2(Segment2 segment)
		{
			return new Line2(segment.a, Vector2.Normalize(segment.b - segment.a));
		}

		public static explicit operator Ray2(Segment2 segment)
		{
			return new Ray2(segment.a, Vector2.Normalize(segment.b - segment.a));
		}

		public static explicit operator Segment3(Segment2 segment)
		{
			return new Segment3(segment.a.ToVector3(), segment.b.ToVector3());
		}

		#endregion Casting operators

		public static Segment2 operator +(Segment2 segment, Vector2 vector)
		{
			return new Segment2(segment.a + vector, segment.b + vector);
		}

		public static Segment2 operator -(Segment2 segment, Vector2 vector)
		{
			return new Segment2(segment.a - vector, segment.b - vector);
		}

		public static bool operator ==(Segment2 a, Segment2 b)
		{
			return a.a == b.a && a.b == b.b;
		}

		public static bool operator !=(Segment2 a, Segment2 b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return a.GetHashCode() ^ (b.GetHashCode() << 2);
		}

		public override bool Equals(object other)
		{
			return other is Segment2 && Equals((Segment2)other);
		}

		public bool Equals(Segment2 other)
		{
			return a.Equals(other.a) && b.Equals(other.b);
		}

		public override string ToString()
		{
			return string.Format("Segment2(a: {0}, b: {1})", a, b);
		}
	}
}
