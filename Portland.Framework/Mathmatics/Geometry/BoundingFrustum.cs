// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Defines a viewing frustum for intersection operations.
	/// </summary>
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public class BoundingFrustum : IEquatable<BoundingFrustum>
	{
		#region Private Fields

		private Matrix4x4 _matrix;
		private readonly Vector3[] _corners = new Vector3[CornerCount];
		private readonly Plane[] _planes = new Plane[PlaneCount];

		#endregion

		#region Public Fields

		/// <summary>
		/// The number of planes in the frustum.
		/// </summary>
		public const int PlaneCount = 6;

		/// <summary>
		/// The number of corner points in the frustum.
		/// </summary>
		public const int CornerCount = 8;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the <see cref="Matrix"/> of the frustum.
		/// </summary>
		public Matrix4x4 Matrix
		{
			get { return this._matrix; }
			set
			{
				this._matrix = value;
				this.CreatePlanes();    // FIXME: The odds are the planes will be used a lot more often than the matrix
				this.CreateCorners();   // is updated, so this should help performance. I hope ;)
			}
		}

		/// <summary>
		/// Gets the near plane of the frustum.
		/// </summary>
		public Plane Near
		{
			get { return this._planes[0]; }
		}

		/// <summary>
		/// Gets the far plane of the frustum.
		/// </summary>
		public Plane Far
		{
			get { return this._planes[1]; }
		}

		/// <summary>
		/// Gets the left plane of the frustum.
		/// </summary>
		public Plane Left
		{
			get { return this._planes[2]; }
		}

		/// <summary>
		/// Gets the right plane of the frustum.
		/// </summary>
		public Plane Right
		{
			get { return this._planes[3]; }
		}

		/// <summary>
		/// Gets the top plane of the frustum.
		/// </summary>
		public Plane Top
		{
			get { return this._planes[4]; }
		}

		/// <summary>
		/// Gets the bottom plane of the frustum.
		/// </summary>
		public Plane Bottom
		{
			get { return this._planes[5]; }
		}

		#endregion

		#region Internal Properties

		internal string DebugDisplayString
		{
			get
			{
#if UNITY_5_3_OR_NEWER
				return string.Concat(
					 "Near( ", this._planes[0].ToString(), " )  \r\n",
					 "Far( ", this._planes[1].ToString(), " )  \r\n",
					 "Left( ", this._planes[2].ToString(), " )  \r\n",
					 "Right( ", this._planes[3].ToString(), " )  \r\n",
					 "Top( ", this._planes[4].ToString(), " )  \r\n",
					 "Bottom( ", this._planes[5].ToString(), " )  "
					 );
#else
				return string.Concat(
					 "Near( ", this._planes[0].DebugDisplayString, " )  \r\n",
					 "Far( ", this._planes[1].DebugDisplayString, " )  \r\n",
					 "Left( ", this._planes[2].DebugDisplayString, " )  \r\n",
					 "Right( ", this._planes[3].DebugDisplayString, " )  \r\n",
					 "Top( ", this._planes[4].DebugDisplayString, " )  \r\n",
					 "Bottom( ", this._planes[5].DebugDisplayString, " )  "
					 );
#endif
			}
		}

#endregion

#region Constructors

		/// <summary>
		/// Constructs the frustum by extracting the view planes from a matrix.
		/// </summary>
		/// <param name="value">Combined matrix which usually is (View * Projection).</param>
		public BoundingFrustum(Matrix4x4 value)
		{
			this._matrix = value;
			this.CreatePlanes();
			this.CreateCorners();
		}

#endregion

#region Operators

		/// <summary>
		/// Compares whether two <see cref="BoundingFrustum"/> instances are equal.
		/// </summary>
		/// <param name="a"><see cref="BoundingFrustum"/> instance on the left of the equal sign.</param>
		/// <param name="b"><see cref="BoundingFrustum"/> instance on the right of the equal sign.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public static bool operator ==(BoundingFrustum a, BoundingFrustum b)
		{
			if (Equals(a, null))
				return (Equals(b, null));

			if (Equals(b, null))
				return (Equals(a, null));

			return a._matrix == (b._matrix);
		}

		/// <summary>
		/// Compares whether two <see cref="BoundingFrustum"/> instances are not equal.
		/// </summary>
		/// <param name="a"><see cref="BoundingFrustum"/> instance on the left of the not equal sign.</param>
		/// <param name="b"><see cref="BoundingFrustum"/> instance on the right of the not equal sign.</param>
		/// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
		public static bool operator !=(BoundingFrustum a, BoundingFrustum b)
		{
			return !(a == b);
		}

#endregion

#region Public Methods

#region Contains

		/// <summary>
		/// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">A <see cref="BoundingBox"/> for testing.</param>
		/// <returns>Result of testing for containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingBox"/>.</returns>
		public ContainmentType Contains(BoundingBox box)
		{
			var result = default(ContainmentType);
			this.Contains(ref box, out result);
			return result;
		}

		/// <summary>
		/// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="box">A <see cref="BoundingBox"/> for testing.</param>
		/// <param name="result">Result of testing for containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingBox"/> as an output parameter.</param>
		public void Contains(ref BoundingBox box, out ContainmentType result)
		{
			var intersects = false;
			for (var i = 0; i < PlaneCount; ++i)
			{
				var planeIntersectionType = default(PlaneIntersectionType);
				box.Intersects(this._planes[i], out planeIntersectionType);
				switch (planeIntersectionType)
				{
					case PlaneIntersectionType.Front:
						result = ContainmentType.Disjoint;
						return;
					case PlaneIntersectionType.Intersecting:
						intersects = true;
						break;
				}
			}
			result = intersects ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		/// <summary>
		/// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="frustum">A <see cref="BoundingFrustum"/> for testing.</param>
		/// <returns>Result of testing for containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingFrustum"/>.</returns>
		public ContainmentType Contains(BoundingFrustum frustum)
		{
			if (this == frustum)                // We check to see if the two frustums are equal
				return ContainmentType.Contains;// If they are, there's no need to go any further.

			var intersects = false;
			for (var i = 0; i < PlaneCount; ++i)
			{
				PlaneIntersectionType planeIntersectionType;
				frustum.Intersects(ref _planes[i], out planeIntersectionType);
				switch (planeIntersectionType)
				{
					case PlaneIntersectionType.Front:
						return ContainmentType.Disjoint;
					case PlaneIntersectionType.Intersecting:
						intersects = true;
						break;
				}
			}
			return intersects ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		/// <summary>
		/// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">A <see cref="BoundingSphere"/> for testing.</param>
		/// <returns>Result of testing for containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingSphere"/>.</returns>
		public ContainmentType Contains(BoundingSphere sphere)
		{
			var result = default(ContainmentType);
			this.Contains(ref sphere, out result);
			return result;
		}

		/// <summary>
		/// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingSphere"/>.
		/// </summary>
		/// <param name="sphere">A <see cref="BoundingSphere"/> for testing.</param>
		/// <param name="result">Result of testing for containment between this <see cref="BoundingFrustum"/> and specified <see cref="BoundingSphere"/> as an output parameter.</param>
		public void Contains(ref BoundingSphere sphere, out ContainmentType result)
		{
			var intersects = false;
			for (var i = 0; i < PlaneCount; ++i)
			{
				var planeIntersectionType = default(PlaneIntersectionType);

				// TODO: we might want to inline this for performance reasons
				sphere.Intersects(this._planes[i], out planeIntersectionType);
				switch (planeIntersectionType)
				{
					case PlaneIntersectionType.Front:
						result = ContainmentType.Disjoint;
						return;
					case PlaneIntersectionType.Intersecting:
						intersects = true;
						break;
				}
			}
			result = intersects ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		/// <summary>
		/// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="Vector3"/>.
		/// </summary>
		/// <param name="point">A <see cref="Vector3"/> for testing.</param>
		/// <returns>Result of testing for containment between this <see cref="BoundingFrustum"/> and specified <see cref="Vector3"/>.</returns>
		public ContainmentType Contains(Vector3 point)
		{
			var result = default(ContainmentType);
			this.Contains(point, out result);
			return result;
		}

		/// <summary>
		/// Containment test between this <see cref="BoundingFrustum"/> and specified <see cref="Vector3"/>.
		/// </summary>
		/// <param name="point">A <see cref="Vector3"/> for testing.</param>
		/// <param name="result">Result of testing for containment between this <see cref="BoundingFrustum"/> and specified <see cref="Vector3"/> as an output parameter.</param>
		public void Contains(in Vector3 point, out ContainmentType result)
		{
			for (var i = 0; i < PlaneCount; ++i)
			{
				// TODO: we might want to inline this for performance reasons
				if (PlaneHelper.ClassifyPoint(point, this._planes[i]) > 0)
				{
					result = ContainmentType.Disjoint;
					return;
				}
			}
			result = ContainmentType.Contains;
		}

#endregion

		/// <summary>
		/// Compares whether current instance is equal to specified <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="other">The <see cref="BoundingFrustum"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public bool Equals(BoundingFrustum other)
		{
			return (this == other);
		}

		/// <summary>
		/// Compares whether current instance is equal to specified <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public override bool Equals(object obj)
		{
			return (obj is BoundingFrustum) && this == ((BoundingFrustum)obj);
		}

		/// <summary>
		/// Returns a copy of internal corners array.
		/// </summary>
		/// <returns>The array of corners.</returns>
		public Vector3[] GetCorners()
		{
			return (Vector3[])this._corners.Clone();
		}

		/// <summary>
		/// Returns a copy of internal corners array.
		/// </summary>
		/// <param name="corners">The array which values will be replaced to corner values of this instance. It must have size of <see cref="BoundingFrustum.CornerCount"/>.</param>
		public void GetCorners(Vector3[] corners)
		{
			if (corners == null) throw new ArgumentNullException("corners");
			if (corners.Length < CornerCount) throw new ArgumentOutOfRangeException("corners");

			this._corners.CopyTo(corners, 0);
		}

		/// <summary>
		/// Gets the hash code of this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <returns>Hash code of this <see cref="BoundingFrustum"/>.</returns>
		public override int GetHashCode()
		{
			return this._matrix.GetHashCode();
		}

		/// <summary>
		/// Gets whether or not a specified <see cref="BoundingBox"/> intersects with this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="box">A <see cref="BoundingBox"/> for intersection test.</param>
		/// <returns><c>true</c> if specified <see cref="BoundingBox"/> intersects with this <see cref="BoundingFrustum"/>; <c>false</c> otherwise.</returns>
		public bool Intersects(BoundingBox box)
		{
			var result = false;
			this.Intersects(ref box, out result);
			return result;
		}

		/// <summary>
		/// Gets whether or not a specified <see cref="BoundingBox"/> intersects with this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="box">A <see cref="BoundingBox"/> for intersection test.</param>
		/// <param name="result"><c>true</c> if specified <see cref="BoundingBox"/> intersects with this <see cref="BoundingFrustum"/>; <c>false</c> otherwise as an output parameter.</param>
		public void Intersects(ref BoundingBox box, out bool result)
		{
			var containment = default(ContainmentType);
			this.Contains(ref box, out containment);
			result = containment != ContainmentType.Disjoint;
		}

		/// <summary>
		/// Gets whether or not a specified <see cref="BoundingFrustum"/> intersects with this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="frustum">An other <see cref="BoundingFrustum"/> for intersection test.</param>
		/// <returns><c>true</c> if other <see cref="BoundingFrustum"/> intersects with this <see cref="BoundingFrustum"/>; <c>false</c> otherwise.</returns>
		public bool Intersects(BoundingFrustum frustum)
		{
			return Contains(frustum) != ContainmentType.Disjoint;
		}

		/// <summary>
		/// Gets whether or not a specified <see cref="BoundingSphere"/> intersects with this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="sphere">A <see cref="BoundingSphere"/> for intersection test.</param>
		/// <returns><c>true</c> if specified <see cref="BoundingSphere"/> intersects with this <see cref="BoundingFrustum"/>; <c>false</c> otherwise.</returns>
		public bool Intersects(BoundingSphere sphere)
		{
			var result = default(bool);
			this.Intersects(ref sphere, out result);
			return result;
		}

		/// <summary>
		/// Gets whether or not a specified <see cref="BoundingSphere"/> intersects with this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="sphere">A <see cref="BoundingSphere"/> for intersection test.</param>
		/// <param name="result"><c>true</c> if specified <see cref="BoundingSphere"/> intersects with this <see cref="BoundingFrustum"/>; <c>false</c> otherwise as an output parameter.</param>
		public void Intersects(ref BoundingSphere sphere, out bool result)
		{
			var containment = default(ContainmentType);
			this.Contains(ref sphere, out containment);
			result = containment != ContainmentType.Disjoint;
		}

		/// <summary>
		/// Gets type of intersection between specified <see cref="Plane"/> and this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="plane">A <see cref="Plane"/> for intersection test.</param>
		/// <returns>A plane intersection type.</returns>
		public PlaneIntersectionType Intersects(Plane plane)
		{
			PlaneIntersectionType result;
			Intersects(ref plane, out result);
			return result;
		}

		/// <summary>
		/// Gets type of intersection between specified <see cref="Plane"/> and this <see cref="BoundingFrustum"/>.
		/// </summary>
		/// <param name="plane">A <see cref="Plane"/> for intersection test.</param>
		/// <param name="result">A plane intersection type as an output parameter.</param>
		public void Intersects(ref Plane plane, out PlaneIntersectionType result)
		{
			result = plane.Intersects(_corners[0]);
			for (int i = 1; i < _corners.Length; i++)
				if (plane.Intersects(_corners[i]) != result)
					result = PlaneIntersectionType.Intersecting;
		}

		/// <summary>
		/// Gets the distance of intersection of <see cref="Ray"/> and this <see cref="BoundingFrustum"/> or null if no intersection happens.
		/// </summary>
		/// <param name="ray">A <see cref="Ray"/> for intersection test.</param>
		/// <returns>Distance at which ray intersects with this <see cref="BoundingFrustum"/> or null if no intersection happens.</returns>
		public float? Intersects(Ray ray)
		{
			float? result;
			Intersects(ref ray, out result);
			return result;
		}

		/// <summary>
		/// Gets the distance of intersection of <see cref="Ray"/> and this <see cref="BoundingFrustum"/> or null if no intersection happens.
		/// </summary>
		/// <param name="ray">A <see cref="Ray"/> for intersection test.</param>
		/// <param name="result">Distance at which ray intersects with this <see cref="BoundingFrustum"/> or null if no intersection happens as an output parameter.</param>
		public void Intersects(ref Ray ray, out float? result)
		{
			ContainmentType ctype;
			this.Contains(ray.Origin, out ctype);

			switch (ctype)
			{
				case ContainmentType.Disjoint:
					result = null;
					return;
				case ContainmentType.Contains:
					result = 0.0f;
					return;
				case ContainmentType.Intersects:
					throw new NotImplementedException();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Returns a <see cref="String"/> representation of this <see cref="BoundingFrustum"/> in the format:
		/// {Near:[nearPlane] Far:[farPlane] Left:[leftPlane] Right:[rightPlane] Top:[topPlane] Bottom:[bottomPlane]}
		/// </summary>
		/// <returns><see cref="String"/> representation of this <see cref="BoundingFrustum"/>.</returns>
		public override string ToString()
		{
			return "{Near: " + this._planes[0] +
					 " Far:" + this._planes[1] +
					 " Left:" + this._planes[2] +
					 " Right:" + this._planes[3] +
					 " Top:" + this._planes[4] +
					 " Bottom:" + this._planes[5] +
					 "}";
		}

#endregion

#region Private Methods

		private void CreateCorners()
		{
			IntersectionPoint(ref this._planes[0], ref this._planes[2], ref this._planes[4], out this._corners[0]);
			IntersectionPoint(ref this._planes[0], ref this._planes[3], ref this._planes[4], out this._corners[1]);
			IntersectionPoint(ref this._planes[0], ref this._planes[3], ref this._planes[5], out this._corners[2]);
			IntersectionPoint(ref this._planes[0], ref this._planes[2], ref this._planes[5], out this._corners[3]);
			IntersectionPoint(ref this._planes[1], ref this._planes[2], ref this._planes[4], out this._corners[4]);
			IntersectionPoint(ref this._planes[1], ref this._planes[3], ref this._planes[4], out this._corners[5]);
			IntersectionPoint(ref this._planes[1], ref this._planes[3], ref this._planes[5], out this._corners[6]);
			IntersectionPoint(ref this._planes[1], ref this._planes[2], ref this._planes[5], out this._corners[7]);
		}

		private void CreatePlanes()
		{
			this._planes[0] = new Plane(-this._matrix.m13, -this._matrix.m23, -this._matrix.m33, -this._matrix.m43);
			this._planes[1] = new Plane(this._matrix.m13 - this._matrix.m14, this._matrix.m23 - this._matrix.m24, this._matrix.m33 - this._matrix.m34, this._matrix.m43 - this._matrix.m44);
			this._planes[2] = new Plane(-this._matrix.m14 - this._matrix.m11, -this._matrix.m24 - this._matrix.m21, -this._matrix.m34 - this._matrix.m31, -this._matrix.m44 - this._matrix.m41);
			this._planes[3] = new Plane(this._matrix.m11 - this._matrix.m14, this._matrix.m21 - this._matrix.m24, this._matrix.m31 - this._matrix.m34, this._matrix.m41 - this._matrix.m44);
			this._planes[4] = new Plane(this._matrix.m12 - this._matrix.m14, this._matrix.m22 - this._matrix.m24, this._matrix.m32 - this._matrix.m34, this._matrix.m42 - this._matrix.m44);
			this._planes[5] = new Plane(-this._matrix.m14 - this._matrix.m12, -this._matrix.m24 - this._matrix.m22, -this._matrix.m34 - this._matrix.m32, -this._matrix.m44 - this._matrix.m42);

			this.NormalizePlane(ref this._planes[0]);
			this.NormalizePlane(ref this._planes[1]);
			this.NormalizePlane(ref this._planes[2]);
			this.NormalizePlane(ref this._planes[3]);
			this.NormalizePlane(ref this._planes[4]);
			this.NormalizePlane(ref this._planes[5]);
		}

		private static void IntersectionPoint(ref Plane a, ref Plane b, ref Plane c, out Vector3 result)
		{
			// Formula used
			//                d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )
			//P =   -------------------------------------------------------------------------
			//                             N1 . ( N2 * N3 )
			//
			// Note: N refers to the normal, d refers to the displacement. '.' means dot product. '*' means cross product

			Vector3 v1, v2, v3;
			Vector3 cross;

			Vector3.Cross(b.normal, c.normal, out cross);

			float f;
			Vector3.Dot(a.normal, cross, out f);
			f *= -1.0f;

			Vector3.Cross(b.normal, c.normal, out cross);
			Vector3.Multiply(cross, a.distance, out v1);
			//v1 = (a.D * (Vector3.Cross(b.Normal, c.Normal)));


			Vector3.Cross(c.normal, a.normal, out cross);
			Vector3.Multiply(cross, b.distance, out v2);
			//v2 = (b.D * (Vector3.Cross(c.Normal, a.Normal)));


			Vector3.Cross(a.normal, b.normal, out cross);
			Vector3.Multiply(cross, c.distance, out v3);
			//v3 = (c.D * (Vector3.Cross(a.Normal, b.Normal)));

			result.x = (v1.x + v2.x + v3.x) / f;
			result.y = (v1.y + v2.y + v3.y) / f;
			result.z = (v1.z + v2.z + v3.z) / f;
		}

		private void NormalizePlane(ref Plane p)
		{
			float factor = 1f / p.normal.Magnitude;
			p.normal.x *= factor;
			p.normal.y *= factor;
			p.normal.z *= factor;
			p.distance *= factor;
		}

#endregion
	}
}

