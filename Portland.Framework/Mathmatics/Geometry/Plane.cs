// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	internal class PlaneHelper
	{
		/// <summary>
		/// Returns a value indicating what side (positive/negative) of a plane a point is
		/// </summary>
		/// <param name="point">The point to check with</param>
		/// <param name="plane">The plane to check against</param>
		/// <returns>Greater than zero if on the positive side, less than zero if on the negative size, 0 otherwise</returns>
		public static float ClassifyPoint(in Vector3 point, in Plane plane)
		{
			return point.x * plane.normal.x + point.y * plane.normal.y + point.z * plane.normal.z + plane.distance;
		}

		/// <summary>
		/// Returns the perpendicular distance from a point to a plane
		/// </summary>
		/// <param name="point">The point to check</param>
		/// <param name="plane">The place to check</param>
		/// <returns>The perpendicular distance from the point to the plane</returns>
		public static float PerpendicularDistance(in Vector3 point, in Plane plane)
		{
			// dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
			return MathF.Abs((plane.normal.x * point.x + plane.normal.y * point.y + plane.normal.z * point.z)
											/ MathF.Sqrt(plane.normal.x * plane.normal.x + plane.normal.y * plane.normal.y + plane.normal.z * plane.normal.z));
		}
	}

#if !UNITY_5_3_OR_NEWER
	/// <summary>
	/// A plane in 3d space, represented by its normal away from the origin and its distance from the origin, D.
	/// </summary>
	[DataContract]
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public struct Plane : IEquatable<Plane>
	{
		#region Public Fields

		/// <summary>
		/// The distance of the <see cref="Plane"/> to the origin.
		/// </summary>
		[DataMember]
		public float distance;

		/// <summary>
		/// The normal of the <see cref="Plane"/>.
		/// </summary>
		[DataMember]
		public Vector3 normal;

		#endregion Public Fields

		#region Constructors

		/// <summary>
		/// Create a <see cref="Plane"/> with the first three components of the specified <see cref="Vector4"/>
		/// as the normal and the last component as the distance to the origin.
		/// </summary>
		/// <param name="value">A vector holding the normal and distance to origin.</param>
		public Plane(in Vector4 value)
			 : this(new Vector3(value.x, value.y, value.z), value.w)
		{

		}

		/// <summary>
		/// Create a <see cref="Plane"/> with the specified normal and distance to the origin.
		/// </summary>
		/// <param name="normal">The normal of the plane.</param>
		/// <param name="d">The distance to the origin.</param>
		public Plane(in Vector3 normal, float d)
		{
			this.normal = normal;
			distance = d;
		}

		/// <summary>
		/// Create the <see cref="Plane"/> that contains the three specified points.
		/// </summary>
		/// <param name="a">A point the created <see cref="Plane"/> should contain.</param>
		/// <param name="b">A point the created <see cref="Plane"/> should contain.</param>
		/// <param name="c">A point the created <see cref="Plane"/> should contain.</param>
		public Plane(in Vector3 a, in Vector3 b, in Vector3 c)
		{
			Vector3 ab = b - a;
			Vector3 ac = c - a;

			Vector3 cross = Vector3.Cross(ab, ac);
			Vector3.Normalize(cross, out normal);
			distance = -(Vector3.Dot(normal, a));
		}

		/// <summary>
		/// Create a <see cref="Plane"/> with the first three values as the X, Y and Z
		/// components of the normal and the last value as the distance to the origin.
		/// </summary>
		/// <param name="a">The X component of the normal.</param>
		/// <param name="b">The Y component of the normal.</param>
		/// <param name="c">The Z component of the normal.</param>
		/// <param name="d">The distance to the origin.</param>
		public Plane(float a, float b, float c, float d)
			 : this(new Vector3(a, b, c), d)
		{

		}

		/// <summary>
		/// Create a <see cref="Plane"/> that contains the specified point and has the specified <see cref="normal"/> vector.
		/// </summary>
		/// <param name="pointOnPlane">A point the created <see cref="Plane"/> should contain.</param>
		/// <param name="normal">The normal of the plane.</param>
		public Plane(in Vector3 pointOnPlane, in Vector3 normal)
		{
			this.normal = normal;
			distance = -(
				 pointOnPlane.x * normal.x +
				 pointOnPlane.y * normal.y +
				 pointOnPlane.z * normal.z
			);
		}

		#endregion Constructors

		#region Public Methods

		/// <summary>
		/// Get the dot product of a <see cref="Vector4"/> with this <see cref="Plane"/>.
		/// </summary>
		/// <param name="value">The <see cref="Vector4"/> to calculate the dot product with.</param>
		/// <returns>The dot product of the specified <see cref="Vector4"/> and this <see cref="Plane"/>.</returns>
		public double Dot(in Vector4 value)
		{
			return ((((this.normal.x * value.x) + (this.normal.y * value.y)) + (this.normal.z * value.z)) + (this.distance * value.w));
		}

		/// <summary>
		/// Get the dot product of a <see cref="Vector4"/> with this <see cref="Plane"/>.
		/// </summary>
		/// <param name="value">The <see cref="Vector4"/> to calculate the dot product with.</param>
		/// <param name="result">
		/// The dot product of the specified <see cref="Vector4"/> and this <see cref="Plane"/>.
		/// </param>
		public void Dot(in Vector4 value, out double result)
		{
			result = (((this.normal.x * value.x) + (this.normal.y * value.y)) + (this.normal.z * value.z)) + (this.distance * value.w);
		}

		/// <summary>
		/// Get the dot product of a <see cref="Vector3"/> with
		/// the <see cref="normal"/> vector of this <see cref="Plane"/>
		/// plus the <see cref="distance"/> value of this <see cref="Plane"/>.
		/// </summary>
		/// <param name="value">The <see cref="Vector3"/> to calculate the dot product with.</param>
		/// <returns>
		/// The dot product of the specified <see cref="Vector3"/> and the normal of this <see cref="Plane"/>
		/// plus the <see cref="distance"/> value of this <see cref="Plane"/>.
		/// </returns>
		public double DotCoordinate(in Vector3 value)
		{
			return ((((this.normal.x * value.x) + (this.normal.y * value.y)) + (this.normal.z * value.z)) + this.distance);
		}

		/// <summary>
		/// Get the dot product of a <see cref="Vector3"/> with
		/// the <see cref="normal"/> vector of this <see cref="Plane"/>
		/// plus the <see cref="distance"/> value of this <see cref="Plane"/>.
		/// </summary>
		/// <param name="value">The <see cref="Vector3"/> to calculate the dot product with.</param>
		/// <param name="result">
		/// The dot product of the specified <see cref="Vector3"/> and the normal of this <see cref="Plane"/>
		/// plus the <see cref="distance"/> value of this <see cref="Plane"/>.
		/// </param>
		public void DotCoordinate(in Vector3 value, out double result)
		{
			result = (((this.normal.x * value.x) + (this.normal.y * value.y)) + (this.normal.z * value.z)) + this.distance;
		}

		/// <summary>
		/// Get the dot product of a <see cref="Vector3"/> with
		/// the <see cref="normal"/> vector of this <see cref="Plane"/>.
		/// </summary>
		/// <param name="value">The <see cref="Vector3"/> to calculate the dot product with.</param>
		/// <returns>
		/// The dot product of the specified <see cref="Vector3"/> and the normal of this <see cref="Plane"/>.
		/// </returns>
		public double DotNormal(in Vector3 value)
		{
			return (((this.normal.x * value.x) + (this.normal.y * value.y)) + (this.normal.z * value.z));
		}

		/// <summary>
		/// Get the dot product of a <see cref="Vector3"/> with
		/// the <see cref="normal"/> vector of this <see cref="Plane"/>.
		/// </summary>
		/// <param name="value">The <see cref="Vector3"/> to calculate the dot product with.</param>
		/// <param name="result">
		/// The dot product of the specified <see cref="Vector3"/> and the normal of this <see cref="Plane"/>.
		/// </param>
		public void DotNormal(in Vector3 value, out double result)
		{
			result = ((this.normal.x * value.x) + (this.normal.y * value.y)) + (this.normal.z * value.z);
		}

		/// <summary>
		/// Transforms a normalized plane by a matrix.
		/// </summary>
		/// <param name="plane">The normalized plane to transform.</param>
		/// <param name="matrix">The transformation matrix.</param>
		/// <returns>The transformed plane.</returns>
		public static Plane Transform(in Plane plane, in Matrix4x4 matrix)
		{
			Plane result;
			Transform(plane, matrix, out result);
			return result;
		}

		/// <summary>
		/// Transforms a normalized plane by a matrix.
		/// </summary>
		/// <param name="plane">The normalized plane to transform.</param>
		/// <param name="matrix">The transformation matrix.</param>
		/// <param name="result">The transformed plane.</param>
		public static void Transform(in Plane plane, in Matrix4x4 matrix, out Plane result)
		{
			// See "Transforming Normals" in http://www.glprogramming.com/red/appendixf.html
			// for an explanation of how this works.

			Matrix4x4 transformedMatrix;
			Matrix4x4.Invert(matrix, out transformedMatrix);
			Matrix4x4.Transpose(transformedMatrix, out transformedMatrix);

			var vector = new Vector4(plane.normal, plane.distance);

			Vector4 transformedVector;
			Vector4.Transform(vector, transformedMatrix, out transformedVector);

			result = new Plane(transformedVector);
		}

		/// <summary>
		/// Transforms a normalized plane by a quaternion rotation.
		/// </summary>
		/// <param name="plane">The normalized plane to transform.</param>
		/// <param name="rotation">The quaternion rotation.</param>
		/// <returns>The transformed plane.</returns>
		public static Plane Transform(in Plane plane, in Quaternion rotation)
		{
			Plane result;
			Transform(plane, rotation, out result);
			return result;
		}

		/// <summary>
		/// Transforms a normalized plane by a quaternion rotation.
		/// </summary>
		/// <param name="plane">The normalized plane to transform.</param>
		/// <param name="rotation">The quaternion rotation.</param>
		/// <param name="result">The transformed plane.</param>
		public static void Transform(in Plane plane, in Quaternion rotation, out Plane result)
		{
			Vector3.Transform(plane.normal, rotation, out result.normal);
			result.distance = plane.distance;
		}

		/// <summary>
		/// Normalize the normal vector of this plane.
		/// </summary>
		public void Normalize()
		{
			var length = normal.Magnitude;
			var factor = 1f / length;
			Vector3.Multiply(normal, factor, out normal);
			distance = distance * factor;
		}

		/// <summary>
		/// Get a normalized version of the specified plane.
		/// </summary>
		/// <param name="value">The <see cref="Plane"/> to normalize.</param>
		/// <returns>A normalized version of the specified <see cref="Plane"/>.</returns>
		public static Plane Normalize(in Plane value)
		{
			Plane ret;
			Normalize(value, out ret);
			return ret;
		}

		/// <summary>
		/// Get a normalized version of the specified plane.
		/// </summary>
		/// <param name="value">The <see cref="Plane"/> to normalize.</param>
		/// <param name="result">A normalized version of the specified <see cref="Plane"/>.</param>
		public static void Normalize(in Plane value, out Plane result)
		{
			var length = value.normal.Magnitude;
			var factor = 1f / length;
			Vector3.Multiply(value.normal, factor, out result.normal);
			result.distance = value.distance * factor;
		}

		/// <summary>
		/// Check if two planes are not equal.
		/// </summary>
		/// <param name="plane1">A <see cref="Plane"/> to check for inequality.</param>
		/// <param name="plane2">A <see cref="Plane"/> to check for inequality.</param>
		/// <returns><code>true</code> if the two planes are not equal, <code>false</code> if they are.</returns>
		public static bool operator !=(in Plane plane1, in Plane plane2)
		{
			return !plane1.Equals(plane2);
		}

		/// <summary>
		/// Check if two planes are equal.
		/// </summary>
		/// <param name="plane1">A <see cref="Plane"/> to check for equality.</param>
		/// <param name="plane2">A <see cref="Plane"/> to check for equality.</param>
		/// <returns><code>true</code> if the two planes are equal, <code>false</code> if they are not.</returns>
		public static bool operator ==(in Plane plane1, in Plane plane2)
		{
			return plane1.Equals(plane2);
		}

		/// <summary>
		/// Check if this <see cref="Plane"/> is equal to another <see cref="Plane"/>.
		/// </summary>
		/// <param name="other">An <see cref="Object"/> to check for equality with this <see cref="Plane"/>.</param>
		/// <returns>
		/// <code>true</code> if the specified <see cref="object"/> is equal to this <see cref="Plane"/>,
		/// <code>false</code> if it is not.
		/// </returns>
		public override bool Equals(object other)
		{
			return (other is Plane) ? this.Equals((Plane)other) : false;
		}

		/// <summary>
		/// Check if this <see cref="Plane"/> is equal to another <see cref="Plane"/>.
		/// </summary>
		/// <param name="other">A <see cref="Plane"/> to check for equality with this <see cref="Plane"/>.</param>
		/// <returns>
		/// <code>true</code> if the specified <see cref="Plane"/> is equal to this one,
		/// <code>false</code> if it is not.
		/// </returns>
		public bool Equals(Plane other)
		{
			return ((normal == other.normal) && (distance == other.distance));
		}

		/// <summary>
		/// Get a hash code for this <see cref="Plane"/>.
		/// </summary>
		/// <returns>A hash code for this <see cref="Plane"/>.</returns>
		public override int GetHashCode()
		{
			return normal.GetHashCode() ^ distance.GetHashCode();
		}

		/// <summary>
		/// Check if this <see cref="Plane"/> intersects a <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">The <see cref="BoundingBox"/> to test for intersection.</param>
		/// <returns>
		/// The type of intersection of this <see cref="Plane"/> with the specified <see cref="BoundingBox"/>.
		/// </returns>
		public PlaneIntersectionType Intersects(in BoundingBox box)
		{
			return box.Intersects(this);
		}

		/// <summary>
		/// Check if this <see cref="Plane"/> intersects a <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">The <see cref="BoundingBox"/> to test for intersection.</param>
		/// <param name="result">
		/// The type of intersection of this <see cref="Plane"/> with the specified <see cref="BoundingBox"/>.
		/// </param>
		public void Intersects(in BoundingBox box, out PlaneIntersectionType result)
		{
			box.Intersects(this, out result);
		}

		/// <summary>
		/// Check if this <see cref="Plane"/> intersects a <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="frustum">The <see cref="BoundingFrustum"/> to test for intersection.</param>
		/// <returns>
		/// The type of intersection of this <see cref="Plane"/> with the specified <see cref="BoundingFrustum"/>.
		/// </returns>
		public PlaneIntersectionType Intersects(in BoundingFrustum frustum)
		{
			return frustum.Intersects(this);
		}

		/// <summary>
		/// Check if this <see cref="Plane"/> intersects a <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingSphere"/> to test for intersection.</param>
		/// <returns>
		/// The type of intersection of this <see cref="Plane"/> with the specified <see cref="BoundingSphere"/>.
		/// </returns>
		public PlaneIntersectionType Intersects(in BoundingSphere sphere)
		{
			return sphere.Intersects(this);
		}

		/// <summary>
		/// Check if this <see cref="Plane"/> intersects a <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingSphere"/> to test for intersection.</param>
		/// <param name="result">
		/// The type of intersection of this <see cref="Plane"/> with the specified <see cref="BoundingSphere"/>.
		/// </param>
		public void Intersects(in BoundingSphere sphere, out PlaneIntersectionType result)
		{
			sphere.Intersects(this, out result);
		}

		internal PlaneIntersectionType Intersects(in Vector3 point)
		{
			double distance;
			DotCoordinate(point, out distance);

			if (distance > 0)
				return PlaneIntersectionType.Front;

			if (distance < 0)
				return PlaneIntersectionType.Back;

			return PlaneIntersectionType.Intersecting;
		}

		internal string DebugDisplayString
		{
			get
			{
				return string.Concat(
					 this.normal.DebugDisplayString, "  ",
					 this.distance.ToString()
					 );
			}
		}

		/// <summary>
		/// Get a <see cref="String"/> representation of this <see cref="Plane"/>.
		/// </summary>
		/// <returns>A <see cref="String"/> representation of this <see cref="Plane"/>.</returns>
		public override string ToString()
		{
			return "{Normal:" + normal + " D:" + distance + "}";
		}

		/// <summary>
		/// Deconstruction method for <see cref="Plane"/>.
		/// </summary>
		/// <param name="normal"></param>
		/// <param name="d"></param>
		public void Deconstruct(out Vector3 normal, out double d)
		{
			normal = this.normal;
			d = distance;
		}

		///// <summary>
		///// Returns a <see cref="System.Numerics.Plane"/>.
		///// </summary>
		//public System.Numerics.Plane ToNumerics()
		//{
		//	return new System.Numerics.Plane((float)this.Normal.X, (float)this.Normal.Y, (float)this.Normal.Z, (float)this.D);
		//}

		//#endregion

		//#region Operators

		///// <summary>
		///// Converts a <see cref="System.Numerics.Plane"/> to a <see cref="Plane"/>.
		///// </summary>
		///// <param name="value">The converted value.</param>
		//public static implicit operator Plane(System.Numerics.Plane value)
		//{
		//	return new Plane(value.Normal, value.D);
		//}

		#endregion
	}
#endif
}

