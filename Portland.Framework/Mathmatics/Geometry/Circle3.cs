using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Representation of a 3D circle
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	[Serializable]
	public struct Circle3 : IEquatable<Circle3>
	{
		public Vector3 Center;
		public Vector3 Normal;
		public float Radius;

		/// <summary>
		/// Returns the perimeter of the circle
		/// </summary>
		public float Perimeter => 2 * MathF.PI * Radius;

		/// <summary>
		/// Returns the area of the circle
		/// </summary>
		public float Area => MathF.PI * Radius * Radius;

		public static Circle3 unitXY => new Circle3(Vector3.zero, Vector3.Backward, 1);
		public static Circle3 unitXZ => new Circle3(Vector3.zero, Vector3.up, 1);
		public static Circle3 unitYZ => new Circle3(Vector3.zero, Vector3.left, 1);

		public Circle3(float radius) : this(Vector3.zero, Vector3.Backward, radius)
		{
		}

		public Circle3(Vector3 center, float radius) : this(center, Vector3.Backward, radius)
		{
		}

		public Circle3(Vector3 center, Vector3 normal, float radius)
		{
			this.Center = center;
			this.Normal = normal;
			this.Radius = radius;
		}

		/// <summary>
		/// Linearly interpolates between two circles
		/// </summary>
		public static Circle3 Lerp(Circle3 a, Circle3 b, float t)
		{
			t = MathHelper.Clamp01(t);
			return new Circle3(
				 center: a.Center + (b.Center - a.Center) * t,
				 normal: Vector3.LerpUnclamped(a.Normal, b.Normal, t),
				 radius: a.Radius + (b.Radius - a.Radius) * t);
		}

		/// <summary>
		/// Linearly interpolates between two circles without clamping the interpolant
		/// </summary>
		public static Circle3 LerpUnclamped(Circle3 a, Circle3 b, float t)
		{
			return new Circle3(
				 center: a.Center + (b.Center - a.Center) * t,
				 normal: Vector3.LerpUnclamped(a.Normal, b.Normal, t),
				 radius: a.Radius + (b.Radius - a.Radius) * t);
		}

		public static explicit operator Sphere(Circle3 circle)
		{
			return new Sphere(circle.Center, circle.Radius);
		}

		public static explicit operator Circle2(Circle3 circle)
		{
			return new Circle2(circle.Center.ToVector2(), circle.Radius);
		}

		public static Circle3 operator +(Circle3 circle, Vector3 vector)
		{
			return new Circle3(circle.Center + vector, circle.Normal, circle.Radius);
		}

		public static Circle3 operator -(Circle3 circle, Vector3 vector)
		{
			return new Circle3(circle.Center - vector, circle.Normal, circle.Radius);
		}

		public static bool operator ==(Circle3 a, Circle3 b)
		{
			return a.Center == b.Center && a.Normal == b.Normal && a.Radius == b.Radius;
		}

		public static bool operator !=(Circle3 a, Circle3 b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return Center.GetHashCode() ^ (Normal.GetHashCode() << 2) ^ (Radius.GetHashCode() >> 2);
		}

		public override bool Equals(object other)
		{
			return other is Circle3 && Equals((Circle3)other);
		}

		public bool Equals(Circle3 other)
		{
			return Center.Equals(other.Center) && Normal.Equals(other.Normal) && Radius.Equals(other.Radius);
		}

		public override string ToString()
		{
			return string.Format("Circle3(center: {0}, normal: {1}, radius: {2})", Center, Normal, Radius);
		}
	}
}
