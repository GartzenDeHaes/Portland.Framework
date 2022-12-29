using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Representation of a sphere
	/// </summary>
	[Serializable]
   public struct Sphere : IEquatable<Sphere>
   {
      public Vector3 Center;
      public float Radius;

      /// <summary>
      /// Returns the area of the sphere
      /// </summary>
      public float Area => 4 * MathF.PI * Radius * Radius;

      /// <summary>
      /// Returns the volume of the sphere
      /// </summary>
      public float Volume => 4f / 3f * MathF.PI * Radius * Radius * Radius;

      public static Sphere Unit => new Sphere(Vector3.Zero, 1);

      public Sphere(float radius)
      {
         Center = Vector3.Zero;
         this.Radius = radius;
      }

      public Sphere(Vector3 center, float radius)
      {
         this.Center = center;
         this.Radius = radius;
      }

      /// <summary>
      /// Returns a point on the sphere at the given coordinates
      /// </summary>
      /// <param name="horizontalAngle">Horizontal angle in degrees [0, 360]</param>
      /// <param name="verticalAngle">Vertical angle in degrees [-90, 90]</param>
      public Vector3 GetPoint(float horizontalAngle, float verticalAngle)
      {
         return Center + Geometry.PointOnSphere(Radius, horizontalAngle, verticalAngle);
      }

      /// <summary>
      /// Returns true if the point intersects the sphere
      /// </summary>
      public bool Contains(Vector3 point)
      {
         return Intersect.PointSphere(point, Center, Radius);
      }

      /// <summary>
      /// Linearly interpolates between two spheres
      /// </summary>
      public static Sphere Lerp(Sphere a, Sphere b, float t)
      {
         t = MathHelper.Clamp01(t);
         return new Sphere(a.Center + (b.Center - a.Center) * t, a.Radius + (b.Radius - a.Radius) * t);
      }

      /// <summary>
      /// Linearly interpolates between two spheres without clamping the interpolant
      /// </summary>
      public static Sphere LerpUnclamped(Sphere a, Sphere b, float t)
      {
         return new Sphere(a.Center + (b.Center - a.Center) * t, a.Radius + (b.Radius - a.Radius) * t);
      }

      public static explicit operator Circle2(Sphere sphere)
      {
         return new Circle2(sphere.Center.x, sphere.Center.y, sphere.Radius);
      }

      public static Sphere operator +(Sphere sphere, Vector3 vector)
      {
         return new Sphere(sphere.Center + vector, sphere.Radius);
      }

      public static Sphere operator -(Sphere sphere, Vector3 vector)
      {
         return new Sphere(sphere.Center - vector, sphere.Radius);
      }

      public static bool operator ==(Sphere a, Sphere b)
      {
         return a.Center == b.Center && a.Radius == b.Radius;
      }

      public static bool operator !=(Sphere a, Sphere b)
      {
         return !(a == b);
      }

      public override int GetHashCode()
      {
         return Center.GetHashCode() ^ (Radius.GetHashCode() << 2);
      }

      public override bool Equals(object other)
      {
         return other is Sphere && Equals((Sphere)other);
      }

      public bool Equals(Sphere other)
      {
         return Center.Equals(other.Center) && Radius.Equals(other.Radius);
      }

      public override string ToString()
      {
         return string.Format("Sphere(center: {0}, radius: {1})", Center, Radius);
      }
   }
}
