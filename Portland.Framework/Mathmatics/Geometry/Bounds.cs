using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public struct Bounds : IEquatable<Bounds>
	{
		public Vector3 Center;
		public Vector3 Extents;

		// Creates new Bounds with a given /center/ and total /size/. Bound ::ref::extents will be half the given size.
		public Bounds(Vector3 center, Vector3 size)
		{
			Center = center;
			Extents = size * 0.5F;
		}

		// used to allow Bounds to be used as keys in hash tables
		public override int GetHashCode()
		{
			return Center.GetHashCode() ^ (Extents.GetHashCode() << 2);
		}

		// also required for being able to use Vector4s as keys in hash tables
		public override bool Equals(object other)
		{
			if (!(other is Bounds)) return false;

			return Equals((Bounds)other);
		}

		public bool Equals(Bounds other)
		{
			return Center.Equals(other.Center) && Extents.Equals(other.Extents);
		}

		// The center of the bounding box.
		//public Vector3 Center { get { return _center; } set { _center = value; } }

		// The total size of the box. This is always twice as large as the ::ref::extents.
		public Vector3 Size { get { return Extents * 2.0F; } set { Extents = value * 0.5F; } }

		// The extents of the box. This is always half of the ::ref::size.
		//public Vector3 Extents { get { return _extents; } set { _extents = value; } }

		// The minimal point of the box. This is always equal to ''center-extents''.
		public Vector3 Min { get { return Center - Extents; } set { SetMinMax(value, Max); } }

		// The maximal point of the box. This is always equal to ''center+extents''.
		public Vector3 Max { get { return Center + Extents; } set { SetMinMax(Min, value); } }

		//*undoc*
		public static bool operator ==(Bounds lhs, Bounds rhs)
		{
			// Returns false in the presence of NaN values.
			return (lhs.Center == rhs.Center && lhs.Extents == rhs.Extents);
		}

		//*undoc*
		public static bool operator !=(Bounds lhs, Bounds rhs)
		{
			// Returns true in the presence of NaN values.
			return !(lhs == rhs);
		}

		// Sets the bounds to the /min/ and /max/ value of the box.
		public void SetMinMax(Vector3 min, Vector3 max)
		{
			Extents = (max - min) * 0.5F;
			Center = min + Center;
		}

		// Grows the Bounds to include the /point/.
		public void Encapsulate(Vector3 point)
		{
			SetMinMax(Vector3.Min(Min, point), Vector3.Max(Max, point));
		}

		// Grows the Bounds to include the /Bounds/.
		public void Encapsulate(Bounds bounds)
		{
			Encapsulate(bounds.Center - bounds.Extents);
			Encapsulate(bounds.Center + bounds.Extents);
		}

		// Expand the bounds by increasing its /size/ by /amount/ along each side.
		public void Expand(float amount)
		{
			amount *= .5f;
			Extents += new Vector3(amount, amount, amount);
		}

		// Expand the bounds by increasing its /size/ by /amount/ along each side.
		public void Expand(Vector3 amount)
		{
			Extents += amount * .5f;
		}

		// Does another bounding box intersect with this bounding box?
		public bool Intersects(Bounds bounds)
		{
			return (Min.x <= bounds.Max.x) && (Max.x >= bounds.Min.x) &&
				 (Min.y <= bounds.Max.y) && (Max.y >= bounds.Min.y) &&
				 (Min.z <= bounds.Max.z) && (Max.z >= bounds.Min.z);
		}

		public bool IntersectRay(Ray ray) 
		{ 
			double dist; 
			return IntersectRayAABB(ray, out dist); 
		}
		
		public bool IntersectRay(Ray ray, out double distance) 
		{ 
			return IntersectRayAABB(ray, out distance); 
		}

		/// *listonly*
		override public string ToString()
		{
			return String.Format("Center: {0}, Extents: {1}", Center.ToString(), Extents.ToString());
		}

		private bool IntersectRayAABB(Ray ray, out double q)
		{
			q = Double.PositiveInfinity;
			BoundingBox bb = new BoundingBox(Min, Max);
			var dist = bb.Intersects(ray);

			if (dist == null)
			{
				return false;
			}

			q = dist.Value;
			return true;
		}
	}
}
