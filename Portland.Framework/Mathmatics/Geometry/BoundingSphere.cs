//// MIT License - Copyright (C) The Mono.Xna Team
//// This file is subject to the terms and conditions defined in
//// file 'LICENSE.txt', which is part of this source code package.

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Runtime.Serialization;

//using Portland.Mathmatics.Geometry;
//using Portland.Mathmatics;

//namespace Portland.Mathmatics.Geometry
//{
//	/// <summary>
//	/// Describes a sphere in 3D-space for bounding operations.
//	/// </summary>
//	[DataContract]
//	[DebuggerDisplay("{DebugDisplayString,nq}")]
//	public struct BoundingSphere : IEquatable<BoundingSphere>
//	{
//		#region Public Fields

//		/// <summary>
//		/// The sphere center.
//		/// </summary>
//		[DataMember]
//		public Vector3 Center;

//		/// <summary>
//		/// The sphere radius.
//		/// </summary>
//		[DataMember]
//		public float Radius;

//		#endregion

//		#region Internal Properties

//		internal string DebugDisplayString
//		{
//			get
//			{
//				return string.Concat(
//					 "Center( ", this.Center.DebugDisplayString, " )  \r\n",
//					 "Radius( ", this.Radius.ToString(), " )"
//					 );
//			}
//		}

//		#endregion

//		#region Constructors

//		/// <summary>
//		/// Constructs a bounding sphere with the specified center and radius.  
//		/// </summary>
//		/// <param name="center">The sphere center.</param>
//		/// <param name="radius">The sphere radius.</param>
//		public BoundingSphere(in Vector3 center, float radius)
//		{
//			this.Center = center;
//			this.Radius = radius;
//		}

//		#endregion

//		#region Public Methods

//		#region Contains

//		/// <summary>
//		/// Test if a bounding box is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="box">The box for testing.</param>
//		/// <returns>The containment type.</returns>
//		public ContainmentType Contains(in BoundingBox box)
//		{
//			//check if all corner is in sphere
//			bool inside = true;
//			foreach (Vector3 corner in box.GetCorners())
//			{
//				if (this.Contains(corner) == ContainmentType.Disjoint)
//				{
//					inside = false;
//					break;
//				}
//			}

//			if (inside)
//				return ContainmentType.Contains;

//			//check if the distance from sphere center to cube face < radius
//			double dmin = 0;

//			if (Center.x < box.Min.x)
//				dmin += (Center.x - box.Min.x) * (Center.x - box.Min.x);

//			else if (Center.x > box.Max.x)
//				dmin += (Center.x - box.Max.x) * (Center.x - box.Max.x);

//			if (Center.y < box.Min.y)
//				dmin += (Center.y - box.Min.y) * (Center.y - box.Min.y);

//			else if (Center.y > box.Max.y)
//				dmin += (Center.y - box.Max.y) * (Center.y - box.Max.y);

//			if (Center.z < box.Min.z)
//				dmin += (Center.z - box.Min.z) * (Center.z - box.Min.z);

//			else if (Center.z > box.Max.z)
//				dmin += (Center.z - box.Max.z) * (Center.z - box.Max.z);

//			if (dmin <= Radius * Radius)
//				return ContainmentType.Intersects;

//			//else disjoint
//			return ContainmentType.Disjoint;
//		}

//		/// <summary>
//		/// Test if a bounding box is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="box">The box for testing.</param>
//		/// <param name="result">The containment type as an output parameter.</param>
//		public void Contains(in BoundingBox box, out ContainmentType result)
//		{
//			result = this.Contains(box);
//		}

//		/// <summary>
//		/// Test if a frustum is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="frustum">The frustum for testing.</param>
//		/// <returns>The containment type.</returns>
//		public ContainmentType Contains(in BoundingFrustum frustum)
//		{
//			//check if all corner is in sphere
//			bool inside = true;

//			Vector3[] corners = frustum.GetCorners();
//			foreach (Vector3 corner in corners)
//			{
//				if (this.Contains(corner) == ContainmentType.Disjoint)
//				{
//					inside = false;
//					break;
//				}
//			}
//			if (inside)
//				return ContainmentType.Contains;

//			//check if the distance from sphere center to frustrum face < radius
//			double dmin = 0;
//			//TODO : calcul dmin

//			if (dmin <= Radius * Radius)
//				return ContainmentType.Intersects;

//			//else disjoint
//			return ContainmentType.Disjoint;
//		}

//		/// <summary>
//		/// Test if a frustum is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="frustum">The frustum for testing.</param>
//		/// <param name="result">The containment type as an output parameter.</param>
//		public void Contains(ref BoundingFrustum frustum, out ContainmentType result)
//		{
//			result = this.Contains(frustum);
//		}

//		/// <summary>
//		/// Test if a sphere is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="sphere">The other sphere for testing.</param>
//		/// <returns>The containment type.</returns>
//		public ContainmentType Contains(in BoundingSphere sphere)
//		{
//			ContainmentType result;
//			Contains(sphere, out result);
//			return result;
//		}

//		/// <summary>
//		/// Test if a sphere is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="sphere">The other sphere for testing.</param>
//		/// <param name="result">The containment type as an output parameter.</param>
//		public void Contains(in BoundingSphere sphere, out ContainmentType result)
//		{
//			float sqDistance;
//			Vector3.DistanceSquared(sphere.Center, Center, out sqDistance);

//			if (sqDistance > (sphere.Radius + Radius) * (sphere.Radius + Radius))
//				result = ContainmentType.Disjoint;

//			else if (sqDistance <= (Radius - sphere.Radius) * (Radius - sphere.Radius))
//				result = ContainmentType.Contains;

//			else
//				result = ContainmentType.Intersects;
//		}

//		/// <summary>
//		/// Test if a point is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="point">The vector in 3D-space for testing.</param>
//		/// <returns>The containment type.</returns>
//		public ContainmentType Contains(in Vector3 point)
//		{
//			ContainmentType result;
//			Contains(point, out result);
//			return result;
//		}

//		/// <summary>
//		/// Test if a point is fully inside, outside, or just intersecting the sphere.
//		/// </summary>
//		/// <param name="point">The vector in 3D-space for testing.</param>
//		/// <param name="result">The containment type as an output parameter.</param>
//		public void Contains(in Vector3 point, out ContainmentType result)
//		{
//			var sqRadius = Radius * Radius;
//			float sqDistance;
//			Vector3.DistanceSquared(point, Center, out sqDistance);

//			if (sqDistance > sqRadius)
//				result = ContainmentType.Disjoint;

//			else if (sqDistance < sqRadius)
//				result = ContainmentType.Contains;

//			else
//				result = ContainmentType.Intersects;
//		}

//		#endregion

//		#region CreateFromBoundingBox

//		/// <summary>
//		/// Creates the smallest <see cref="BoundingSphere"/> that can contain a specified <see cref="BoundingBox"/>.
//		/// </summary>
//		/// <param name="box">The box to create the sphere from.</param>
//		/// <returns>The new <see cref="BoundingSphere"/>.</returns>
//		public static BoundingSphere CreateFromBoundingBox(in BoundingBox box)
//		{
//			BoundingSphere result;
//			CreateFromBoundingBox(box, out result);
//			return result;
//		}

//		/// <summary>
//		/// Creates the smallest <see cref="BoundingSphere"/> that can contain a specified <see cref="BoundingBox"/>.
//		/// </summary>
//		/// <param name="box">The box to create the sphere from.</param>
//		/// <param name="result">The new <see cref="BoundingSphere"/> as an output parameter.</param>
//		public static void CreateFromBoundingBox(in BoundingBox box, out BoundingSphere result)
//		{
//			// Find the center of the box.
//			Vector3 center = new Vector3((box.Min.x + box.Max.x) / 2.0f,
//												  (box.Min.y + box.Max.y) / 2.0f,
//												  (box.Min.z + box.Max.z) / 2.0f);

//			// Find the distance between the center and one of the corners of the box.
//			var radius = Vector3.Distance(center, box.Max);

//			result = new BoundingSphere(center, radius);
//		}

//		#endregion

//		/// <summary>
//		/// Creates the smallest <see cref="BoundingSphere"/> that can contain a specified <see cref="BoundingFrustum"/>.
//		/// </summary>
//		/// <param name="frustum">The frustum to create the sphere from.</param>
//		/// <returns>The new <see cref="BoundingSphere"/>.</returns>
//		public static BoundingSphere CreateFromFrustum(BoundingFrustum frustum)
//		{
//			return CreateFromPoints(frustum.GetCorners());
//		}

//		/// <summary>
//		/// Creates the smallest <see cref="BoundingSphere"/> that can contain a specified list of points in 3D-space. 
//		/// </summary>
//		/// <param name="points">List of point to create the sphere from.</param>
//		/// <returns>The new <see cref="BoundingSphere"/>.</returns>
//		public static BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
//		{
//			if (points == null)
//				throw new ArgumentNullException("points");

//			// From "Real-Time Collision Detection" (Page 89)

//			var minx = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
//			var maxx = -minx;
//			var miny = minx;
//			var maxy = -minx;
//			var minz = minx;
//			var maxz = -minx;

//			// Find the most extreme points along the principle axis.
//			var numPoints = 0;
//			foreach (var pt in points)
//			{
//				++numPoints;

//				if (pt.x < minx.x)
//					minx = pt;
//				if (pt.x > maxx.x)
//					maxx = pt;
//				if (pt.y < miny.y)
//					miny = pt;
//				if (pt.y > maxy.y)
//					maxy = pt;
//				if (pt.z < minz.z)
//					minz = pt;
//				if (pt.z > maxz.z)
//					maxz = pt;
//			}

//			if (numPoints == 0)
//				throw new ArgumentException("You should have at least one point in points.");

//			var sqDistX = Vector3.DistanceSquared(maxx, minx);
//			var sqDistY = Vector3.DistanceSquared(maxy, miny);
//			var sqDistZ = Vector3.DistanceSquared(maxz, minz);

//			// Pick the pair of most distant points.
//			var min = minx;
//			var max = maxx;
//			if (sqDistY > sqDistX && sqDistY > sqDistZ)
//			{
//				max = maxy;
//				min = miny;
//			}
//			if (sqDistZ > sqDistX && sqDistZ > sqDistY)
//			{
//				max = maxz;
//				min = minz;
//			}

//			var center = (min + max) * 0.5f;
//			var radius = Vector3.Distance(max, center);

//			// Test every point and expand the sphere.
//			// The current bounding sphere is just a good approximation and may not enclose all points.            
//			// From: Mathematics for 3D Game Programming and Computer Graphics, Eric Lengyel, Third Edition.
//			// Page 218
//			float sqRadius = radius * radius;
//			foreach (var pt in points)
//			{
//				Vector3 diff = (pt - center);
//				float sqDist = diff.SqrMagnitude;
//				if (sqDist > sqRadius)
//				{
//					float distance = MathF.Sqrt(sqDist); // equal to diff.Length();
//					Vector3 direction = diff / distance;
//					Vector3 G = center - radius * direction;
//					center = (G + pt) / 2;
//					radius = Vector3.Distance(pt, center);
//					sqRadius = radius * radius;
//				}
//			}

//			return new BoundingSphere(center, radius);
//		}

//		/// <summary>
//		/// Creates the smallest <see cref="BoundingSphere"/> that can contain two spheres.
//		/// </summary>
//		/// <param name="original">First sphere.</param>
//		/// <param name="additional">Second sphere.</param>
//		/// <returns>The new <see cref="BoundingSphere"/>.</returns>
//		public static BoundingSphere CreateMerged(in BoundingSphere original, in BoundingSphere additional)
//		{
//			BoundingSphere result;
//			CreateMerged(original, additional, out result);
//			return result;
//		}

//		/// <summary>
//		/// Creates the smallest <see cref="BoundingSphere"/> that can contain two spheres.
//		/// </summary>
//		/// <param name="original">First sphere.</param>
//		/// <param name="additional">Second sphere.</param>
//		/// <param name="result">The new <see cref="BoundingSphere"/> as an output parameter.</param>
//		public static void CreateMerged(in BoundingSphere original, in BoundingSphere additional, out BoundingSphere result)
//		{
//			Vector3 ocenterToaCenter = Vector3.Subtract(additional.Center, original.Center);
//			var distance = ocenterToaCenter.Magnitude;
//			if (distance <= original.Radius + additional.Radius)//intersect
//			{
//				if (distance <= original.Radius - additional.Radius)//original contain additional
//				{
//					result = original;
//					return;
//				}
//				if (distance <= additional.Radius - original.Radius)//additional contain original
//				{
//					result = additional;
//					return;
//				}
//			}
//			//else find center of new sphere and radius
//			var leftRadius = MathF.Max(original.Radius - distance, additional.Radius);
//			var Rightradius = MathF.Max(original.Radius + distance, additional.Radius);
//			ocenterToaCenter = ocenterToaCenter + (((leftRadius - Rightradius) / (2 * ocenterToaCenter.Magnitude)) * ocenterToaCenter);//oCenterToResultCenter

//			result = new BoundingSphere();
//			result.Center = original.Center + ocenterToaCenter;
//			result.Radius = (leftRadius + Rightradius) / 2;
//		}

//		/// <summary>
//		/// Compares whether current instance is equal to specified <see cref="BoundingSphere"/>.
//		/// </summary>
//		/// <param name="other">The <see cref="BoundingSphere"/> to compare.</param>
//		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
//		public bool Equals(BoundingSphere other)
//		{
//			return this.Center == other.Center && this.Radius == other.Radius;
//		}

//		/// <summary>
//		/// Compares whether current instance is equal to specified <see cref="Object"/>.
//		/// </summary>
//		/// <param name="obj">The <see cref="Object"/> to compare.</param>
//		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
//		public override bool Equals(object obj)
//		{
//			if (obj is BoundingSphere)
//				return this.Equals((BoundingSphere)obj);

//			return false;
//		}

//		/// <summary>
//		/// Gets the hash code of this <see cref="BoundingSphere"/>.
//		/// </summary>
//		/// <returns>Hash code of this <see cref="BoundingSphere"/>.</returns>
//		public override int GetHashCode()
//		{
//			return this.Center.GetHashCode() + this.Radius.GetHashCode();
//		}

//		#region Intersects

//		/// <summary>
//		/// Gets whether or not a specified <see cref="BoundingBox"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="box">The box for testing.</param>
//		/// <returns><c>true</c> if <see cref="BoundingBox"/> intersects with this sphere; <c>false</c> otherwise.</returns>
//		public bool Intersects(in BoundingBox box)
//		{
//			return box.Intersects(this);
//		}

//		/// <summary>
//		/// Gets whether or not a specified <see cref="BoundingBox"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="box">The box for testing.</param>
//		/// <param name="result"><c>true</c> if <see cref="BoundingBox"/> intersects with this sphere; <c>false</c> otherwise. As an output parameter.</param>
//		public void Intersects(in BoundingBox box, out bool result)
//		{
//			box.Intersects(this, out result);
//		}

//		/*
//		TODO : Make the public bool Intersects(BoundingFrustum frustum) overload

//		public bool Intersects(BoundingFrustum frustum)
//		{
//			 if (frustum == null)
//				  throw new NullReferenceException();

//			 throw new NotImplementedException();
//		}

//		*/

//		/// <summary>
//		/// Gets whether or not the other <see cref="BoundingSphere"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="sphere">The other sphere for testing.</param>
//		/// <returns><c>true</c> if other <see cref="BoundingSphere"/> intersects with this sphere; <c>false</c> otherwise.</returns>
//		public bool Intersects(in BoundingSphere sphere)
//		{
//			bool result;
//			Intersects(sphere, out result);
//			return result;
//		}

//		/// <summary>
//		/// Gets whether or not the other <see cref="BoundingSphere"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="sphere">The other sphere for testing.</param>
//		/// <param name="result"><c>true</c> if other <see cref="BoundingSphere"/> intersects with this sphere; <c>false</c> otherwise. As an output parameter.</param>
//		public void Intersects(in BoundingSphere sphere, out bool result)
//		{
//			float sqDistance;
//			Vector3.DistanceSquared(sphere.Center, Center, out sqDistance);

//			if (sqDistance > (sphere.Radius + Radius) * (sphere.Radius + Radius))
//				result = false;
//			else
//				result = true;
//		}

//		/// <summary>
//		/// Gets whether or not a specified <see cref="Plane"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="plane">The plane for testing.</param>
//		/// <returns>Type of intersection.</returns>
//		public PlaneIntersectionType Intersects(in Plane plane)
//		{
//			// TODO: we might want to inline this for performance reasons
//			this.Intersects(plane, out var result);
//			return result;
//		}

//		/// <summary>
//		/// Gets whether or not a specified <see cref="Plane"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="plane">The plane for testing.</param>
//		/// <param name="result">Type of intersection as an output parameter.</param>
//		public void Intersects(in Plane plane, out PlaneIntersectionType result)
//		{
//			float distance;
//			// TODO: we might want to inline this for performance reasons
//			Vector3.Dot(plane.Normal, this.Center, out distance);
//			distance += plane.D;
//			if (distance > this.Radius)
//				result = PlaneIntersectionType.Front;
//			else if (distance < -this.Radius)
//				result = PlaneIntersectionType.Back;
//			else
//				result = PlaneIntersectionType.Intersecting;
//		}

//		/// <summary>
//		/// Gets whether or not a specified <see cref="Ray"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="ray">The ray for testing.</param>
//		/// <returns>Distance of ray intersection or <c>null</c> if there is no intersection.</returns>
//		public double? Intersects(in Ray ray)
//		{
//			return ray.Intersects(this);
//		}

//		/// <summary>
//		/// Gets whether or not a specified <see cref="Ray"/> intersects with this sphere.
//		/// </summary>
//		/// <param name="ray">The ray for testing.</param>
//		/// <param name="result">Distance of ray intersection or <c>null</c> if there is no intersection as an output parameter.</param>
//		public void Intersects(in Ray ray, out float? result)
//		{
//			ray.Intersects(this, out result);
//		}

//		#endregion

//		/// <summary>
//		/// Returns a <see cref="String"/> representation of this <see cref="BoundingSphere"/> in the format:
//		/// {Center:[<see cref="Center"/>] Radius:[<see cref="Radius"/>]}
//		/// </summary>
//		/// <returns>A <see cref="String"/> representation of this <see cref="BoundingSphere"/>.</returns>
//		public override string ToString()
//		{
//			return "{Center:" + this.Center + " Radius:" + this.Radius + "}";
//		}

//		#region Transform

//		/// <summary>
//		/// Creates a new <see cref="BoundingSphere"/> that contains a transformation of translation and scale from this sphere by the specified <see cref="Matrix"/>.
//		/// </summary>
//		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
//		/// <returns>Transformed <see cref="BoundingSphere"/>.</returns>
//		public BoundingSphere Transform(in Matrix matrix)
//		{
//			BoundingSphere sphere = new BoundingSphere();
//			sphere.Center = Vector3.Transform(this.Center, matrix);
//			sphere.Radius = this.Radius * MathF.Sqrt(MathF.Max(((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12)) + (matrix.M13 * matrix.M13), MathF.Max(((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22)) + (matrix.M23 * matrix.M23), ((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32)) + (matrix.M33 * matrix.M33))));
//			return sphere;
//		}

//		/// <summary>
//		/// Creates a new <see cref="BoundingSphere"/> that contains a transformation of translation and scale from this sphere by the specified <see cref="Matrix"/>.
//		/// </summary>
//		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
//		/// <param name="result">Transformed <see cref="BoundingSphere"/> as an output parameter.</param>
//		public void Transform(ref Matrix matrix, out BoundingSphere result)
//		{
//			result.Center = Vector3.Transform(this.Center, matrix);
//			result.Radius = this.Radius * MathF.Sqrt(MathF.Max(((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12)) + (matrix.M13 * matrix.M13), MathF.Max(((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22)) + (matrix.M23 * matrix.M23), ((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32)) + (matrix.M33 * matrix.M33))));
//		}

//		#endregion

//		/// <summary>
//		/// Deconstruction method for <see cref="BoundingSphere"/>.
//		/// </summary>
//		/// <param name="center"></param>
//		/// <param name="radius"></param>
//		public void Deconstruct(out Vector3 center, out double radius)
//		{
//			center = Center;
//			radius = Radius;
//		}

//		#endregion

//		#region Operators

//		/// <summary>
//		/// Compares whether two <see cref="BoundingSphere"/> instances are equal.
//		/// </summary>
//		/// <param name="a"><see cref="BoundingSphere"/> instance on the left of the equal sign.</param>
//		/// <param name="b"><see cref="BoundingSphere"/> instance on the right of the equal sign.</param>
//		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
//		public static bool operator ==(in BoundingSphere a, in BoundingSphere b)
//		{
//			return a.Equals(b);
//		}

//		/// <summary>
//		/// Compares whether two <see cref="BoundingSphere"/> instances are not equal.
//		/// </summary>
//		/// <param name="a"><see cref="BoundingSphere"/> instance on the left of the not equal sign.</param>
//		/// <param name="b"><see cref="BoundingSphere"/> instance on the right of the not equal sign.</param>
//		/// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
//		public static bool operator !=(BoundingSphere a, BoundingSphere b)
//		{
//			return !a.Equals(b);
//		}

//		#endregion
//	}
//}
