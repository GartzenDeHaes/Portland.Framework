using System;
using System.Collections.Generic;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Representation of a 2D circle
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	[Serializable]
	public struct Circle2 : IEquatable<Circle2>
	{
		public Vector2 Center;
		public float Radius;

		/// <summary>
		/// Returns the perimeter of the circle
		/// </summary>
		public float perimeter => 2 * MathF.PI * Radius;

		/// <summary>
		/// Returns the area of the circle
		/// </summary>
		public float area => MathF.PI * Radius * Radius;

		public static Circle2 unit => new Circle2(Vector2.Zero, 1);

		public Circle2(float radius) : this(Vector2.Zero, radius)
		{
		}

		public Circle2(Vector2 center, float radius)
		{
			this.Center = center;
			this.Radius = radius;
		}

		public Circle2(float centerx, float centery, float radius)
		{
			this.Center = new Vector2(centerx, centery);
			this.Radius = radius;
		}

		/// <summary>
		/// Returns a point on the circle at the given <paramref name="angle"/>
		/// </summary>
		/// <param name="angle">Angle in degrees</param>
		public Vector2 GetPoint(float angle)
		{
			return Geometry.PointOnCircle2(Center, Radius, angle);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on the circle
		/// </summary>
		/// <param name="count">Number of points</param>
		public List<Vector2> GetPoints(int count)
		{
			return Geometry.PointsOnCircle2(Center, Radius, count);
		}

		/// <summary>
		/// Returns true if the point intersects the circle
		/// </summary>
		public bool Contains(Vector2 point)
		{
			return Intersect.PointCircle(point, Center, Radius);
		}

		/// <summary>
		/// Linearly interpolates between two circles
		/// </summary>
		public static Circle2 Lerp(Circle2 a, Circle2 b, float t)
		{
			t = MathHelper.Clamp01(t);
			return new Circle2(a.Center + (b.Center - a.Center) * t, a.Radius + (b.Radius - a.Radius) * t);
		}

		/// <summary>
		/// Linearly interpolates between two circles without clamping the interpolant
		/// </summary>
		public static Circle2 LerpUnclamped(Circle2 a, Circle2 b, float t)
		{
			return new Circle2(a.Center + (b.Center - a.Center) * t, a.Radius + (b.Radius - a.Radius) * t);
		}

		public static explicit operator Sphere(Circle2 circle)
		{
			return new Sphere(circle.Center.ToVector3(), circle.Radius);
		}

		public static explicit operator Circle3(Circle2 circle)
		{
			return new Circle3(circle.Center.ToVector3(), Vector3.Backward, circle.Radius);
		}

		public static Circle2 operator +(Circle2 circle, Vector2 vector)
		{
			return new Circle2(circle.Center + vector, circle.Radius);
		}

		public static Circle2 operator -(Circle2 circle, Vector2 vector)
		{
			return new Circle2(circle.Center - vector, circle.Radius);
		}

		public static bool operator ==(Circle2 a, Circle2 b)
		{
			return a.Center == b.Center && a.Radius == b.Radius;
		}

		public static bool operator !=(Circle2 a, Circle2 b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return Center.GetHashCode() ^ (Radius.GetHashCode() << 2);
		}

		public override bool Equals(object other)
		{
			return other is Circle2 && Equals((Circle2)other);
		}

		public bool Equals(Circle2 other)
		{
			return Center.Equals(other.Center) && Radius.Equals(other.Radius);
		}

		public override string ToString()
		{
			return string.Format("Circle2(center: {0}, radius: {1})", Center, Radius);
		}
	}
}
