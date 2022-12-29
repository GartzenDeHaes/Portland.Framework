using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Representation of a 3D line
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	[Serializable]
    public struct Line3 : IEquatable<Line3>
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public static Line3 xAxis => new Line3(Vector3.Zero, Vector3.Right);
        public static Line3 yAxis => new Line3(Vector3.Zero, Vector3.Up);
        public static Line3 zAxis => new Line3(Vector3.Zero, Vector3.Forward);

        public Line3(Ray ray)
        {
            Origin = ray.Origin;
            Direction = ray.Direction;
        }

        public Line3(Vector3 origin, Vector3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        /// <summary>
        /// Returns a point at <paramref name="distance"/> units from origin along the line
        /// </summary>
        public Vector3 GetPoint(float distance)
        {
            return Origin + Direction*distance;
        }

        /// <summary>
        /// Linearly interpolates between two lines
        /// </summary>
        public static Line3 Lerp(Line3 a, Line3 b, float t)
        {
            t = MathHelper.Clamp01(t);
            return new Line3(a.Origin + (b.Origin - a.Origin)*t, a.Direction + (b.Direction - a.Direction)*t);
        }

        /// <summary>
        /// Linearly interpolates between two lines without clamping the interpolant
        /// </summary>
        public static Line3 LerpUnclamped(Line3 a, Line3 b, float t)
        {
            return new Line3(a.Origin + (b.Origin - a.Origin)*t, a.Direction + (b.Direction - a.Direction)*t);
        }

        #region Casting operators

        public static explicit operator Line3(Ray ray)
        {
            return new Line3(ray);
        }

        public static explicit operator Ray(Line3 line)
        {
            return new Ray(line.Origin, line.Direction);
        }

        public static explicit operator Ray2(Line3 line)
        {
            return new Ray2(line.Origin.ToVector2(), line.Direction.ToVector2());
        }

        public static explicit operator Line2(Line3 line)
        {
            return new Line2(line.Origin.ToVector2(), line.Direction.ToVector2());
        }

        #endregion Casting operators

        public static Line3 operator +(Line3 line, Vector3 vector)
        {
            return new Line3(line.Origin + vector, line.Direction);
        }

        public static Line3 operator -(Line3 line, Vector3 vector)
        {
            return new Line3(line.Origin - vector, line.Direction);
        }

        public static bool operator ==(Line3 a, Line3 b)
        {
            return a.Origin == b.Origin && a.Direction == b.Direction;
        }

        public static bool operator !=(Line3 a, Line3 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Origin.GetHashCode() ^ (Direction.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            return other is Line3 && Equals((Line3) other);
        }

        public bool Equals(Line3 other)
        {
            return Origin.Equals(other.Origin) && Direction.Equals(other.Direction);
        }

        public override string ToString()
        {
            return string.Format("Line3(origin: {0}, direction: {1})", Origin, Direction);
        }
    }
}
