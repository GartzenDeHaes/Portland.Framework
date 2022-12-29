using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Representation of a 2D line
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	[Serializable]
	public struct Line2 : IEquatable<Line2>
	{
		public Vector2 Origin;
		public Vector2 Direction;

		public static Line2 xAxis => new Line2(Vector2.Zero, Vector2.Right);
		public static Line2 yAxis => new Line2(Vector2.Zero, Vector2.Up);

		public Line2(Ray2 ray)
		{
			Origin = ray.Origin;
			Direction = ray.Direction;
		}

		public Line2(Vector2 origin, Vector2 direction)
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		/// <summary>
		/// Returns a point at <paramref name="distance"/> units from origin along the line
		/// </summary>
		public Vector2 GetPoint(float distance)
		{
			return Origin + Direction * distance;
		}

		/// <summary>
		/// Linearly interpolates between two lines
		/// </summary>
		public static Line2 Lerp(Line2 a, Line2 b, float t)
		{
			t = MathHelper.Clamp01(t);
			return new Line2(a.Origin + (b.Origin - a.Origin) * t, a.Direction + (b.Direction - a.Direction) * t);
		}

		/// <summary>
		/// Linearly interpolates between two lines without clamping the interpolant
		/// </summary>
		public static Line2 LerpUnclamped(Line2 a, Line2 b, float t)
		{
			return new Line2(a.Origin + (b.Origin - a.Origin) * t, a.Direction + (b.Direction - a.Direction) * t);
		}

		#region Casting operators

		public static explicit operator Line2(Ray2 ray)
		{
			return new Line2(ray);
		}

		public static explicit operator Ray2(Line2 line)
		{
			return new Ray2(line.Origin, line.Direction);
		}

		public static explicit operator Ray(Line2 line)
		{
			return new Ray(line.Origin.ToVector3(), line.Direction.ToVector3());
		}

		public static explicit operator Line3(Line2 line)
		{
			return new Line3(line.Origin.ToVector3(), line.Direction.ToVector3());
		}

		#endregion Casting operators

		public static Line2 operator +(Line2 line, Vector2 vector)
		{
			return new Line2(line.Origin + vector, line.Direction);
		}

		public static Line2 operator -(Line2 line, Vector2 vector)
		{
			return new Line2(line.Origin - vector, line.Direction);
		}

		public static bool operator ==(Line2 a, Line2 b)
		{
			return a.Origin == b.Origin && a.Direction == b.Direction;
		}

		public static bool operator !=(Line2 a, Line2 b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return Origin.GetHashCode() ^ (Direction.GetHashCode() << 2);
		}

		public override bool Equals(object other)
		{
			return other is Line2 && Equals((Line2)other);
		}

		public bool Equals(Line2 other)
		{
			return Origin.Equals(other.Origin) && Direction.Equals(other.Direction);
		}

		public override string ToString()
		{
			return string.Format("Line2(origin: {0}, direction: {1})", Origin, Direction);
		}
	}
}
