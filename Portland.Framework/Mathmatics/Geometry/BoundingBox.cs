// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Represents an axis-aligned bounding box (AABB) in 3D space.
	/// </summary>
	[DataContract]
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public struct BoundingBox : IEquatable<BoundingBox>
	{
		#region Public Fields

		/// <summary>
		///   The minimum extent of this <see cref="BoundingBox"/>.
		/// </summary>
		[DataMember]
		public Vector3 Min;

		/// <summary>
		///   The maximum extent of this <see cref="BoundingBox"/>.
		/// </summary>
		[DataMember]
		public Vector3 Max;

		public bool IsEmpty
		{
			get { return Min == Max; }
		}

		/// <summary>
		///   The number of corners in a <see cref="BoundingBox"/>. This is equal to 8.
		/// </summary>
		public const int CornerCount = 8;

		#endregion Public Fields

		#region Public Constructors

		/// <summary>
		///   Create a <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="min">The minimum extent of the <see cref="BoundingBox"/>.</param>
		/// <param name="max">The maximum extent of the <see cref="BoundingBox"/>.</param>
		public BoundingBox(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		#endregion Public Constructors

		#region Public Methods

		/// <summary>
		/// Gets the height of this bounding box.
		/// </summary>
		public double Height
		{
			get { return Max.y - Min.y; }
		}

		/// <summary>
		/// Gets the width of this bounding box.
		/// </summary>
		public double Width
		{
			get { return Max.x - Min.x; }
		}

		/// <summary>
		/// Gets the depth of this bounding box.
		/// </summary>
		public double Depth
		{
			get { return Max.z - Min.z; }
		}

		/// <summary>
		/// Gets the center of this bounding box.
		/// </summary>
		public Vector3 Center
		{
			get
			{
				return (Min + Max) / 2f;
			}
		}

		public double Volume
		{
			get
			{
				return Width * Height * Depth;
			}
		}

		public BoundingBox OffsetBy(in Vector3 Offset)
		{
			return new BoundingBox(Min + Offset, Max + Offset);
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> contains another <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">The <see cref="BoundingBox"/> to test for overlap.</param>
		/// <returns>
		///   A value indicating if this <see cref="BoundingBox"/> contains,
		///   intersects with or is disjoint with <paramref name="box"/>.
		/// </returns>
		public ContainmentType Contains(in BoundingBox box)
		{
			//test if all corner is in the same side of a face by just checking min and max
			if (box.Max.x < Min.x
				 || box.Min.x > Max.x
				 || box.Max.y < Min.y
				 || box.Min.y > Max.y
				 || box.Max.z < Min.z
				 || box.Min.z > Max.z)
				return ContainmentType.Disjoint;


			if (box.Min.x >= Min.x
				 && box.Max.x <= Max.x
				 && box.Min.y >= Min.y
				 && box.Max.y <= Max.y
				 && box.Min.z >= Min.z
				 && box.Max.z <= Max.z)
				return ContainmentType.Contains;

			return ContainmentType.Intersects;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> contains another <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">The <see cref="BoundingBox"/> to test for overlap.</param>
		/// <param name="result">
		///   A value indicating if this <see cref="BoundingBox"/> contains,
		///   intersects with or is disjoint with <paramref name="box"/>.
		/// </param>
		public void Contains(ref BoundingBox box, out ContainmentType result)
		{
			result = Contains(box);
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> contains a <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="frustum">The <see cref="BoundingFrustum"/> to test for overlap.</param>
		/// <returns>
		///   A value indicating if this <see cref="BoundingBox"/> contains,
		///   intersects with or is disjoint with <paramref name="frustum"/>.
		/// </returns>
		public ContainmentType Contains(in BoundingFrustum frustum)
		{
			//TODO: bad done here need a fix. 
			//Because question is not frustum contain box but reverse and this is not the same
			int i;
			ContainmentType contained;
			Vector3[] corners = frustum.GetCorners();

			// First we check if frustum is in box
			for (i = 0; i < corners.Length; i++)
			{
				this.Contains(corners[i], out contained);
				if (contained == ContainmentType.Disjoint)
					break;
			}

			if (i == corners.Length) // This means we checked all the corners and they were all contain or instersect
				return ContainmentType.Contains;

			if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
				return ContainmentType.Intersects;


			// If we get here, it means the first (and only) point we checked was actually contained in the frustum.
			// So we assume that all other points will also be contained. If one of the points is disjoint, we can
			// exit immediately saying that the result is Intersects
			i++;
			for (; i < corners.Length; i++)
			{
				this.Contains(corners[i], out contained);
				if (contained != ContainmentType.Contains)
					return ContainmentType.Intersects;

			}

			// If we get here, then we know all the points were actually contained, therefore result is Contains
			return ContainmentType.Contains;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> contains a <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingSphere"/> to test for overlap.</param>
		/// <returns>
		///   A value indicating if this <see cref="BoundingBox"/> contains,
		///   intersects with or is disjoint with <paramref name="sphere"/>.
		/// </returns>
		public ContainmentType Contains(in BoundingSphere sphere)
		{
			if (sphere.Center.x - Min.x >= sphere.Radius
				 && sphere.Center.y - Min.y >= sphere.Radius
				 && sphere.Center.z - Min.z >= sphere.Radius
				 && Max.x - sphere.Center.x >= sphere.Radius
				 && Max.y - sphere.Center.y >= sphere.Radius
				 && Max.z - sphere.Center.z >= sphere.Radius)
				return ContainmentType.Contains;

			double dmin = 0;

			double e = sphere.Center.x - Min.x;
			if (e < 0)
			{
				if (e < -sphere.Radius)
				{
					return ContainmentType.Disjoint;
				}
				dmin += e * e;
			}
			else
			{
				e = sphere.Center.x - Max.x;
				if (e > 0)
				{
					if (e > sphere.Radius)
					{
						return ContainmentType.Disjoint;
					}
					dmin += e * e;
				}
			}

			e = sphere.Center.y - Min.y;
			if (e < 0)
			{
				if (e < -sphere.Radius)
				{
					return ContainmentType.Disjoint;
				}
				dmin += e * e;
			}
			else
			{
				e = sphere.Center.y - Max.y;
				if (e > 0)
				{
					if (e > sphere.Radius)
					{
						return ContainmentType.Disjoint;
					}
					dmin += e * e;
				}
			}

			e = sphere.Center.z - Min.z;
			if (e < 0)
			{
				if (e < -sphere.Radius)
				{
					return ContainmentType.Disjoint;
				}
				dmin += e * e;
			}
			else
			{
				e = sphere.Center.z - Max.z;
				if (e > 0)
				{
					if (e > sphere.Radius)
					{
						return ContainmentType.Disjoint;
					}
					dmin += e * e;
				}
			}

			if (dmin <= sphere.Radius * sphere.Radius)
				return ContainmentType.Intersects;

			return ContainmentType.Disjoint;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> contains a <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingSphere"/> to test for overlap.</param>
		/// <param name="result">
		///   A value indicating if this <see cref="BoundingBox"/> contains,
		///   intersects with or is disjoint with <paramref name="sphere"/>.
		/// </param>
		public void Contains(in BoundingSphere sphere, out ContainmentType result)
		{
			result = this.Contains(sphere);
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> contains a point.
		/// </summary>
		/// <param name="point">The <see cref="Vector3"/> to test.</param>
		/// <returns>
		///   <see cref="ContainmentType.Contains"/> if this <see cref="BoundingBox"/> contains
		///   <paramref name="point"/> or <see cref="ContainmentType.Disjoint"/> if it does not.
		/// </returns>
		public ContainmentType Contains(in Vector3 point)
		{
			ContainmentType result;
			this.Contains(point, out result);
			return result;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> contains a point.
		/// </summary>
		/// <param name="point">The <see cref="Vector3"/> to test.</param>
		/// <param name="result">
		///   <see cref="ContainmentType.Contains"/> if this <see cref="BoundingBox"/> contains
		///   <paramref name="point"/> or <see cref="ContainmentType.Disjoint"/> if it does not.
		/// </param>
		public void Contains(in Vector3 point, out ContainmentType result)
		{
			//first we get if point is out of box
			if (point.x < this.Min.x
				 || point.x > this.Max.x
				 || point.y < this.Min.y
				 || point.y > this.Max.y
				 || point.z < this.Min.z
				 || point.z > this.Max.z)
			{
				result = ContainmentType.Disjoint;
			}
			else
			{
				result = ContainmentType.Contains;
			}
		}

		private static readonly Vector3 MaxVector3 = new Vector3(Single.MaxValue);
		private static readonly Vector3 MinVector3 = new Vector3(Single.MinValue);

		/// <summary>
		/// Create a bounding box from the given list of points.
		/// </summary>
		/// <param name="points">The array of Vector3 instances defining the point cloud to bound</param>
		/// <param name="index">The base index to start iterating from</param>
		/// <param name="count">The number of points to iterate</param>
		/// <returns>A bounding box that encapsulates the given point cloud.</returns>
		/// <exception cref="System.ArgumentException">Thrown if the given array is null or has no points.</exception>
		public static BoundingBox CreateFromPoints(Vector3[] points, int index = 0, int count = -1)
		{
			if (points == null || points.Length == 0)
				throw new ArgumentException();

			if (count == -1)
				count = points.Length;

			var minVec = MaxVector3;
			var maxVec = MinVector3;
			for (int i = index; i < count; i++)
			{
				minVec.x = (minVec.x < points[i].x) ? minVec.x : points[i].x;
				minVec.y = (minVec.y < points[i].y) ? minVec.y : points[i].y;
				minVec.z = (minVec.z < points[i].z) ? minVec.z : points[i].z;

				maxVec.x = (maxVec.x > points[i].x) ? maxVec.x : points[i].x;
				maxVec.y = (maxVec.y > points[i].y) ? maxVec.y : points[i].y;
				maxVec.z = (maxVec.z > points[i].z) ? maxVec.z : points[i].z;
			}

			return new BoundingBox(minVec, maxVec);
		}


		/// <summary>
		/// Create a bounding box from the given list of points.
		/// </summary>
		/// <param name="points">The list of Vector3 instances defining the point cloud to bound</param>
		/// <param name="index">The base index to start iterating from</param>
		/// <param name="count">The number of points to iterate</param>
		/// <returns>A bounding box that encapsulates the given point cloud.</returns>
		/// <exception cref="System.ArgumentException">Thrown if the given list is null or has no points.</exception>
		public static BoundingBox CreateFromPoints(List<Vector3> points, int index = 0, int count = -1)
		{
			if (points == null || points.Count == 0)
				throw new ArgumentException();

			if (count == -1)
				count = points.Count;

			var minVec = MaxVector3;
			var maxVec = MinVector3;
			for (int i = index; i < count; i++)
			{
				minVec.x = (minVec.x < points[i].x) ? minVec.x : points[i].x;
				minVec.y = (minVec.y < points[i].y) ? minVec.y : points[i].y;
				minVec.z = (minVec.z < points[i].z) ? minVec.z : points[i].z;

				maxVec.x = (maxVec.x > points[i].x) ? maxVec.x : points[i].x;
				maxVec.y = (maxVec.y > points[i].y) ? maxVec.y : points[i].y;
				maxVec.z = (maxVec.z > points[i].z) ? maxVec.z : points[i].z;
			}

			return new BoundingBox(minVec, maxVec);
		}

		/// <summary>
		///   Create the enclosing <see cref="BoundingBox"/> from the given list of points.
		/// </summary>
		/// <param name="points">The list of <see cref="Vector3"/> instances defining the point cloud to bound.</param>
		/// <returns>A <see cref="BoundingBox"/> that encloses the given point cloud.</returns>
		/// <exception cref="System.ArgumentException">Thrown if the given list has no points.</exception>
		public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
		{
			if (points == null)
				throw new ArgumentNullException();

			var empty = true;
			var minVec = MaxVector3;
			var maxVec = MinVector3;
			foreach (var ptVector in points)
			{
				minVec.x = (minVec.x < ptVector.x) ? minVec.x : ptVector.x;
				minVec.y = (minVec.y < ptVector.y) ? minVec.y : ptVector.y;
				minVec.z = (minVec.z < ptVector.z) ? minVec.z : ptVector.z;

				maxVec.x = (maxVec.x > ptVector.x) ? maxVec.x : ptVector.x;
				maxVec.y = (maxVec.y > ptVector.y) ? maxVec.y : ptVector.y;
				maxVec.z = (maxVec.z > ptVector.z) ? maxVec.z : ptVector.z;

				empty = false;
			}
			if (empty)
				throw new ArgumentException();

			return new BoundingBox(minVec, maxVec);
		}

		/// <summary>
		///   Create the enclosing <see cref="BoundingBox"/> of a <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingSphere"/> to enclose.</param>
		/// <returns>A <see cref="BoundingBox"/> enclosing <paramref name="sphere"/>.</returns>
		public static BoundingBox CreateFromSphere(in BoundingSphere sphere)
		{
			BoundingBox result;
			CreateFromSphere(sphere, out result);
			return result;
		}

		/// <summary>
		///   Create the enclosing <see cref="BoundingBox"/> of a <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingSphere"/> to enclose.</param>
		/// <param name="result">A <see cref="BoundingBox"/> enclosing <paramref name="sphere"/>.</param>
		public static void CreateFromSphere(in BoundingSphere sphere, out BoundingBox result)
		{
			var corner = new Vector3(sphere.Radius);
			result.Min = sphere.Center - corner;
			result.Max = sphere.Center + corner;
		}

		/// <summary>
		///   Create the <see cref="BoundingBox"/> enclosing two other <see cref="BoundingBox"/> instances.
		/// </summary>
		/// <param name="original">A <see cref="BoundingBox"/> to enclose.</param>
		/// <param name="additional">A <see cref="BoundingBox"/> to enclose.</param>
		/// <returns>
		///   The <see cref="BoundingBox"/> enclosing <paramref name="original"/> and <paramref name="additional"/>.
		/// </returns>
		public static BoundingBox CreateMerged(in BoundingBox original, in BoundingBox additional)
		{
			BoundingBox result;
			CreateMerged(original, additional, out result);
			return result;
		}

		/// <summary>
		///   Create the <see cref="BoundingBox"/> enclosing two other <see cref="BoundingBox"/> instances.
		/// </summary>
		/// <param name="original">A <see cref="BoundingBox"/> to enclose.</param>
		/// <param name="additional">A <see cref="BoundingBox"/> to enclose.</param>
		/// <param name="result">
		///   The <see cref="BoundingBox"/> enclosing <paramref name="original"/> and <paramref name="additional"/>.
		/// </param>
		public static void CreateMerged(in BoundingBox original, in BoundingBox additional, out BoundingBox result)
		{
			result.Min.x = Math.Min(original.Min.x, additional.Min.x);
			result.Min.y = Math.Min(original.Min.y, additional.Min.y);
			result.Min.z = Math.Min(original.Min.z, additional.Min.z);
			result.Max.x = Math.Max(original.Max.x, additional.Max.x);
			result.Max.y = Math.Max(original.Max.y, additional.Max.y);
			result.Max.z = Math.Max(original.Max.z, additional.Max.z);
		}

		/// <summary>
		///   Check if two <see cref="BoundingBox"/> instances are equal.
		/// </summary>
		/// <param name="other">The <see cref="BoundingBox"/> to compare with this <see cref="BoundingBox"/>.</param>
		/// <returns>
		///   <code>true</code> if <see cref="other"/> is equal to this <see cref="BoundingBox"/>,
		///   <code>false</code> if it is not.
		/// </returns>
		public bool Equals(BoundingBox other)
		{
			return (this.Min == other.Min) && (this.Max == other.Max);
		}

		/// <summary>
		///   Check if two <see cref="BoundingBox"/> instances are equal.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with this <see cref="BoundingBox"/>.</param>
		/// <returns>
		///   <code>true</code> if <see cref="obj"/> is equal to this <see cref="BoundingBox"/>,
		///   <code>false</code> if it is not.
		/// </returns>
		public override bool Equals(object obj)
		{
			return (obj is BoundingBox) && this.Equals((BoundingBox)obj);
		}

		/// <summary>
		///   Get an array of <see cref="Vector3"/> containing the corners of this <see cref="BoundingBox"/>.
		/// </summary>
		/// <returns>An array of <see cref="Vector3"/> containing the corners of this <see cref="BoundingBox"/>.</returns>
		public Vector3[] GetCorners()
		{
			return new Vector3[] {
					 new Vector3(this.Min.x, this.Max.y, this.Max.z),
					 new Vector3(this.Max.x, this.Max.y, this.Max.z),
					 new Vector3(this.Max.x, this.Min.y, this.Max.z),
					 new Vector3(this.Min.x, this.Min.y, this.Max.z),
					 new Vector3(this.Min.x, this.Max.y, this.Min.z),
					 new Vector3(this.Max.x, this.Max.y, this.Min.z),
					 new Vector3(this.Max.x, this.Min.y, this.Min.z),
					 new Vector3(this.Min.x, this.Min.y, this.Min.z)
				};
		}

		/// <summary>
		///   Fill the first 8 places of an array of <see cref="Vector3"/>
		///   with the corners of this <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="corners">The array to fill.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="corners"/> is <code>null</code>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///   If <paramref name="corners"/> has a length of less than 8.
		/// </exception>
		public void GetCorners(Vector3[] corners)
		{
			if (corners == null)
			{
				throw new ArgumentNullException("corners");
			}
			if (corners.Length < 8)
			{
				throw new ArgumentOutOfRangeException("corners", "Not Enought Corners");
			}
			corners[0].x = this.Min.x;
			corners[0].y = this.Max.y;
			corners[0].z = this.Max.z;
			corners[1].x = this.Max.x;
			corners[1].y = this.Max.y;
			corners[1].z = this.Max.z;
			corners[2].x = this.Max.x;
			corners[2].y = this.Min.y;
			corners[2].z = this.Max.z;
			corners[3].x = this.Min.x;
			corners[3].y = this.Min.y;
			corners[3].z = this.Max.z;
			corners[4].x = this.Min.x;
			corners[4].y = this.Max.y;
			corners[4].z = this.Min.z;
			corners[5].x = this.Max.x;
			corners[5].y = this.Max.y;
			corners[5].z = this.Min.z;
			corners[6].x = this.Max.x;
			corners[6].y = this.Min.y;
			corners[6].z = this.Min.z;
			corners[7].x = this.Min.x;
			corners[7].y = this.Min.y;
			corners[7].z = this.Min.z;
		}

		/// <summary>
		///   Get the hash code for this <see cref="BoundingBox"/>.
		/// </summary>
		/// <returns>A hash code for this <see cref="BoundingBox"/>.</returns>
		public override int GetHashCode()
		{
			return this.Min.GetHashCode() + this.Max.GetHashCode();
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects another <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">The <see cref="BoundingBox"/> to test for intersection.</param>
		/// <returns>
		///   <code>true</code> if this <see cref="BoundingBox"/> intersects <paramref name="box"/>,
		///   <code>false</code> if it does not.
		/// </returns>
		public bool Intersects(in BoundingBox box)
		{
			bool result;
			Intersects(box, out result);
			return result;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects another <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">The <see cref="BoundingBox"/> to test for intersection.</param>
		/// <param name="result">
		///   <code>true</code> if this <see cref="BoundingBox"/> intersects <paramref name="box"/>,
		///   <code>false</code> if it does not.
		/// </param>
		public void Intersects(in BoundingBox box, out bool result)
		{
			if ((this.Max.x >= box.Min.x) && (this.Min.x <= box.Max.x))
			{
				if ((this.Max.y < box.Min.y) || (this.Min.y > box.Max.y))
				{
					result = false;
					return;
				}

				result = (this.Max.z >= box.Min.z) && (this.Min.z <= box.Max.z);
				return;
			}

			result = false;
			return;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects a <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="frustum">The <see cref="BoundingFrustum"/> to test for intersection.</param>
		/// <returns>
		///   <code>true</code> if this <see cref="BoundingBox"/> intersects <paramref name="frustum"/>,
		///   <code>false</code> if it does not.
		/// </returns>
		public bool Intersects(in BoundingFrustum frustum)
		{
			return frustum.Intersects(this);
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects a <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingFrustum"/> to test for intersection.</param>
		/// <returns>
		///   <code>true</code> if this <see cref="BoundingBox"/> intersects <paramref name="sphere"/>,
		///   <code>false</code> if it does not.
		/// </returns>
		public bool Intersects(in BoundingSphere sphere)
		{
			bool result;
			Intersects(sphere, out result);
			return result;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects a <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="sphere">The <see cref="BoundingFrustum"/> to test for intersection.</param>
		/// <param name="result">
		///   <code>true</code> if this <see cref="BoundingBox"/> intersects <paramref name="sphere"/>,
		///   <code>false</code> if it does not.
		/// </param>
		public void Intersects(in BoundingSphere sphere, out bool result)
		{
			var squareDistance = 0.0;
			var point = sphere.Center;
			if (point.x < Min.x) squareDistance += (Min.x - point.x) * (Min.x - point.x);
			if (point.x > Max.x) squareDistance += (point.x - Max.x) * (point.x - Max.x);
			if (point.y < Min.y) squareDistance += (Min.y - point.y) * (Min.y - point.y);
			if (point.y > Max.y) squareDistance += (point.y - Max.y) * (point.y - Max.y);
			if (point.z < Min.z) squareDistance += (Min.z - point.z) * (Min.z - point.z);
			if (point.z > Max.z) squareDistance += (point.z - Max.z) * (point.z - Max.z);
			result = squareDistance <= sphere.Radius * sphere.Radius;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects a <see cref="Plane"/>.
		/// </summary>
		/// <param name="plane">The <see cref="Plane"/> to test for intersection.</param>
		/// <returns>
		///   <code>true</code> if this <see cref="BoundingBox"/> intersects <paramref name="plane"/>,
		///   <code>false</code> if it does not.
		/// </returns>
		public PlaneIntersectionType Intersects(in Plane plane)
		{
			PlaneIntersectionType result;
			Intersects(plane, out result);
			return result;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects a <see cref="Plane"/>.
		/// </summary>
		/// <param name="plane">The <see cref="Plane"/> to test for intersection.</param>
		/// <param name="result">
		///   <code>true</code> if this <see cref="BoundingBox"/> intersects <paramref name="plane"/>,
		///   <code>false</code> if it does not.
		/// </param>
		public void Intersects(in Plane plane, out PlaneIntersectionType result)
		{
			// See http://zach.in.tu-clausthal.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html

			Vector3 positiveVertex;
			Vector3 negativeVertex;

			if (plane.Normal.x >= 0)
			{
				positiveVertex.x = Max.x;
				negativeVertex.x = Min.x;
			}
			else
			{
				positiveVertex.x = Min.x;
				negativeVertex.x = Max.x;
			}

			if (plane.Normal.y >= 0)
			{
				positiveVertex.y = Max.y;
				negativeVertex.y = Min.y;
			}
			else
			{
				positiveVertex.y = Min.y;
				negativeVertex.y = Max.y;
			}

			if (plane.Normal.z >= 0)
			{
				positiveVertex.z = Max.z;
				negativeVertex.z = Min.z;
			}
			else
			{
				positiveVertex.z = Min.z;
				negativeVertex.z = Max.z;
			}

			// Inline Vector3.Dot(plane.Normal, negativeVertex) + plane.D;
			var distance = plane.Normal.x * negativeVertex.x + plane.Normal.y * negativeVertex.y + plane.Normal.z * negativeVertex.z + plane.D;
			if (distance > 0)
			{
				result = PlaneIntersectionType.Front;
				return;
			}

			// Inline Vector3.Dot(plane.Normal, positiveVertex) + plane.D;
			distance = plane.Normal.x * positiveVertex.x + plane.Normal.y * positiveVertex.y + plane.Normal.z * positiveVertex.z + plane.D;
			if (distance < 0)
			{
				result = PlaneIntersectionType.Back;
				return;
			}

			result = PlaneIntersectionType.Intersecting;
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects a <see cref="Ray"/>.
		/// </summary>
		/// <param name="ray">The <see cref="Ray"/> to test for intersection.</param>
		/// <returns>
		///   The distance along the <see cref="Ray"/> to the intersection point or
		///   <code>null</code> if the <see cref="Ray"/> does not intesect this <see cref="BoundingBox"/>.
		/// </returns>
		public Nullable<double> Intersects(in Ray ray)
		{
			return ray.Intersects(this);
		}

		/// <summary>
		///   Check if this <see cref="BoundingBox"/> intersects a <see cref="Ray"/>.
		/// </summary>
		/// <param name="ray">The <see cref="Ray"/> to test for intersection.</param>
		/// <param name="result">
		///   The distance along the <see cref="Ray"/> to the intersection point or
		///   <code>null</code> if the <see cref="Ray"/> does not intesect this <see cref="BoundingBox"/>.
		/// </param>
		public void Intersects(in Ray ray, out Nullable<double> result)
		{
			result = Intersects(ray);
		}

		/// <summary>
		///   Check if two <see cref="BoundingBox"/> instances are equal.
		/// </summary>
		/// <param name="a">A <see cref="BoundingBox"/> to compare the other.</param>
		/// <param name="b">A <see cref="BoundingBox"/> to compare the other.</param>
		/// <returns>
		///   <code>true</code> if <see cref="a"/> is equal to this <see cref="b"/>,
		///   <code>false</code> if it is not.
		/// </returns>
		public static bool operator ==(in BoundingBox a, in BoundingBox b)
		{
			return a.Equals(b);
		}

		/// <summary>
		///   Check if two <see cref="BoundingBox"/> instances are not equal.
		/// </summary>
		/// <param name="a">A <see cref="BoundingBox"/> to compare the other.</param>
		/// <param name="b">A <see cref="BoundingBox"/> to compare the other.</param>
		/// <returns>
		///   <code>true</code> if <see cref="a"/> is not equal to this <see cref="b"/>,
		///   <code>false</code> if it is.
		/// </returns>
		public static bool operator !=(in BoundingBox a, in BoundingBox b)
		{
			return !a.Equals(b);
		}

		internal string DebugDisplayString
		{
			get
			{
				return string.Concat(
					 "Min( ", Min.x, ",", Min.y, " )  \r\n",
					 "Max( ", Max.x, ",", Max.y, " )"
					 );
			}
		}

		/// <summary>
		/// Get a <see cref="String"/> representation of this <see cref="BoundingBox"/>.
		/// </summary>
		/// <returns>A <see cref="String"/> representation of this <see cref="BoundingBox"/>.</returns>
		public override string ToString()
		{
			return "{{Min:" + this.Min.ToString() + " Max:" + this.Max.ToString() + "}}";
		}

		/// <summary>
		/// Deconstruction method for <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public void Deconstruct(out Vector3 min, out Vector3 max)
		{
			min = Min;
			max = Max;
		}

		#endregion Public Methods

		public static readonly BoundingBox One = new BoundingBox(Vector3.Zero, Vector3.One);
	}
}
