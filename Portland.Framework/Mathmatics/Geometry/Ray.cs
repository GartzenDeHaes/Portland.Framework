#region License
/*
MIT License
Copyright Â© 2006 The Mono.Xna Team


All rights reserved.


Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:


The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.


THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	//	[Serializable]
	//	public struct Ray : IEquatable<Ray>
	//	{
	//		#region Public Fields

	//		public Vector3 Direction;
	//		public Vector3 Origin;

	//		#endregion

	//		#region Public Constructors

	//		public Ray(in Vector3 position, in Vector3 direction)
	//		{
	//			this.Origin = position;
	//			this.Direction = direction;
	//		}

	//		#endregion

	//		#region Public Methods

	//		public override bool Equals(object obj)
	//		{
	//			return (obj is Ray) ? this.Equals((Ray)obj) : false;
	//		}

	//		public bool Equals(Ray other)
	//		{
	//			return this.Origin.Equals(other.Origin) && this.Direction.Equals(other.Direction);
	//		}

	//		public override int GetHashCode()
	//		{
	//			return Origin.GetHashCode() ^ Direction.GetHashCode();
	//		}

	//		public float? Intersects(in BoundingBox box)
	//		{
	//			//first test if start in box
	//			if (Origin.x >= box.Min.x
	//				 && Origin.x <= box.Max.x
	//				 && Origin.y >= box.Min.y
	//				 && Origin.y <= box.Max.y
	//				 && Origin.z >= box.Min.z
	//				 && Origin.z <= box.Max.z)
	//				return 0.0f;// here we concidere cube is full and origine is in cube so intersect at origine

	//			//Second we check each face
	//			Vector3 maxT = new Vector3(-1.0f);
	//			//Vector3 minT = new Vector3(-1.0f);
	//			//calcul intersection with each faces
	//			if (Origin.x < box.Min.x && Direction.x != 0.0f)
	//				maxT.x = (box.Min.x - Origin.x) / Direction.x;
	//			else if (Origin.x > box.Max.x && Direction.x != 0.0f)
	//				maxT.x = (box.Max.x - Origin.x) / Direction.x;
	//			if (Origin.y < box.Min.y && Direction.y != 0.0f)
	//				maxT.y = (box.Min.y - Origin.y) / Direction.y;
	//			else if (Origin.y > box.Max.y && Direction.y != 0.0f)
	//				maxT.y = (box.Max.y - Origin.y) / Direction.y;
	//			if (Origin.z < box.Min.z && Direction.z != 0.0f)
	//				maxT.z = (box.Min.z - Origin.z) / Direction.z;
	//			else if (Origin.z > box.Max.z && Direction.z != 0.0f)
	//				maxT.z = (box.Max.z - Origin.z) / Direction.z;

	//			//get the maximum maxT
	//			if (maxT.x > maxT.y && maxT.x > maxT.z)
	//			{
	//				if (maxT.x < 0.0f)
	//					return null;// ray go on opposite of face
	//									//coordonate of hit point of face of cube
	//				float coord = Origin.z + maxT.x * Direction.z;
	//				// if hit point coord ( intersect face with ray) is out of other plane coord it miss 
	//				if (coord < box.Min.z || coord > box.Max.z)
	//					return null;
	//				coord = Origin.y + maxT.x * Direction.y;
	//				if (coord < box.Min.y || coord > box.Max.y)
	//					return null;
	//				return maxT.x;
	//			}
	//			if (maxT.y > maxT.x && maxT.y > maxT.z)
	//			{
	//				if (maxT.y < 0.0f)
	//					return null;// ray go on opposite of face
	//									//coordonate of hit point of face of cube
	//				float coord = Origin.z + maxT.y * Direction.z;
	//				// if hit point coord ( intersect face with ray) is out of other plane coord it miss 
	//				if (coord < box.Min.z || coord > box.Max.z)
	//					return null;
	//				coord = Origin.x + maxT.y * Direction.x;
	//				if (coord < box.Min.x || coord > box.Max.x)
	//					return null;
	//				return maxT.y;
	//			}
	//			else //Z
	//			{
	//				if (maxT.z < 0.0f)
	//					return null;// ray go on opposite of face
	//									//coordonate of hit point of face of cube
	//				float coord = Origin.x + maxT.z * Direction.x;
	//				// if hit point coord ( intersect face with ray) is out of other plane coord it miss 
	//				if (coord < box.Min.x || coord > box.Max.x)
	//					return null;
	//				coord = Origin.y + maxT.z * Direction.y;
	//				if (coord < box.Min.y || coord > box.Max.y)
	//					return null;
	//				return maxT.z;
	//			}
	//		}

	//		public void Intersects(in BoundingBox box, out float? result)
	//		{
	//			result = Intersects(box);
	//		}

	//		public float? Intersects(in BoundingFrustum frustum)
	//		{
	//			if (frustum == null)
	//			{
	//				throw new ArgumentNullException("frustum");
	//			}

	//			return frustum.Intersects(this);
	//		}

	//		public float? Intersects(in BoundingSphere sphere)
	//		{
	//			float? result;
	//			Intersects(sphere, out result);
	//			return result;
	//		}

	//		public float? Intersects(in Plane plane)
	//		{
	//			float? result;
	//			Intersects(plane, out result);
	//			return result;
	//		}

	//		public void Intersects(in Plane plane, out float? result)
	//		{
	//			var den = Vector3.Dot(Direction, plane.Normal);
	//			if (MathF.Abs(den) < 0.00001f)
	//			{
	//				result = null;
	//				return;
	//			}

	//			result = (-plane.D - Vector3.Dot(plane.Normal, Origin)) / den;

	//			if (result < 0.0f)
	//			{
	//				if (result < -0.00001f)
	//				{
	//					result = null;
	//					return;
	//				}

	//				result = 0.0f;
	//			}
	//		}

	//		public void Intersects(in BoundingSphere sphere, out float? result)
	//		{
	//			// Find the vector between where the ray starts the the sphere's centre
	//			Vector3 difference = sphere.Center - this.Origin;

	//			float differenceLengthSquared = difference.SqrMagnitude;
	//			float sphereRadiusSquared = sphere.Radius * sphere.Radius;

	//			float distanceAlongRay;

	//			// If the distance between the ray start and the sphere's centre is less than
	//			// the radius of the sphere, it means we've intersected. N.B. checking the LengthSquared is faster.
	//			if (differenceLengthSquared < sphereRadiusSquared)
	//			{
	//				result = 0.0f;
	//				return;
	//			}

	//			Vector3.Dot(this.Direction, difference, out distanceAlongRay);
	//			// If the ray is pointing away from the sphere then we don't ever intersect
	//			if (distanceAlongRay < 0)
	//			{
	//				result = null;
	//				return;
	//			}

	//			// Next we kinda use Pythagoras to check if we are within the bounds of the sphere
	//			// if x = radius of sphere
	//			// if y = distance between ray position and sphere centre
	//			// if z = the distance we've travelled along the ray
	//			// if x^2 + z^2 - y^2 < 0, we do not intersect
	//			float dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;

	//			result = (dist < 0) ? null : distanceAlongRay - (float?)MathF.Sqrt(dist);
	//		}

	//		public static bool operator !=(in Ray a, in Ray b)
	//		{
	//			return !a.Equals(b);
	//		}

	//		public static bool operator ==(in Ray a, in Ray b)
	//		{
	//			return a.Equals(b);
	//		}

	//		public override string ToString()
	//		{
	//			return string.Format("{{Origin:{0} Direction:{1}}}", Origin.ToString(), Direction.ToString());
	//		}

	//		#endregion

	//		/// <summary>
	//		/// Returns a point at <paramref name="distance"/> units from origin along the line
	//		/// </summary>
	//		public Vector3 GetPoint(float distance)
	//		{
	//			return Origin + Direction * distance;
	//		}
	//	}

	[Serializable]
	public struct Ray2 : IEquatable<Ray2>
	{
		#region Public Fields

		public Vector2 Direction;
		public Vector2 Origin;

		#endregion

		#region Public Constructors

		public Ray2(Vector2 position, Vector2 direction)
		{
			this.Origin = position;
			this.Direction = direction;
		}

		#endregion

		#region Public Methods

		public override bool Equals(object obj)
		{
			return (obj is Ray2) ? this.Equals((Ray2)obj) : false;
		}

		public bool Equals(Ray2 other)
		{
			return this.Origin.Equals(other.Origin) && this.Direction.Equals(other.Direction);
		}

		public override int GetHashCode()
		{
			return Origin.GetHashCode() ^ Direction.GetHashCode();
		}

		public float? Intersects(BoundingBox box)
		{
			//first test if start in box
			if (Origin.X >= box.Min.X
				 && Origin.X <= box.Max.X
				 && Origin.Y >= box.Min.Y
				 && Origin.Y <= box.Max.Y)
				return 0.0f;// here we concidere cube is full and origine is in cube so intersect at origine

			//Second we check each face
			Vector2 maxT = new Vector2(-1.0f);
			//Vector3 minT = new Vector3(-1.0f);
			//calcul intersection with each faces
			if (Origin.X < box.Min.X && Direction.X != 0.0f)
				maxT.X = (box.Min.X - Origin.X) / Direction.X;
			else if (Origin.X > box.Max.X && Direction.X != 0.0f)
				maxT.X = (box.Max.X - Origin.X) / Direction.X;
			if (Origin.Y < box.Min.Y && Direction.Y != 0.0f)
				maxT.Y = (box.Min.Y - Origin.Y) / Direction.Y;
			else if (Origin.Y > box.Max.Y && Direction.Y != 0.0f)
				maxT.Y = (box.Max.Y - Origin.Y) / Direction.Y;

			//get the maximum maxT
			if (maxT.X > maxT.Y)
			{
				if (maxT.X < 0.0f)
					return null;// ray go on opposite of face
									//coordonate of hit point of face of cube
				float coord = maxT.X;
				// if hit point coord ( intersect face with ray) is out of other plane coord it miss 
				if (coord < box.Min.Z || coord > box.Max.Z)
					return null;
				coord = Origin.Y + maxT.X * Direction.Y;
				if (coord < box.Min.Y || coord > box.Max.Y)
					return null;
				return maxT.X;
			}
			if (maxT.Y > maxT.X)
			{
				if (maxT.Y < 0.0f)
					return null;// ray go on opposite of face
									//coordonate of hit point of face of cube
				float coord = maxT.Y;
				// if hit point coord ( intersect face with ray) is out of other plane coord it miss 
				if (coord < box.Min.Z || coord > box.Max.Z)
					return null;
				coord = Origin.X + maxT.Y * Direction.X;
				if (coord < box.Min.X || coord > box.Max.X)
					return null;
				return maxT.Y;
			}
			return null;
		}

		public void Intersects(ref BoundingBox box, out float? result)
		{
			result = Intersects(box);
		}

		public static bool operator !=(Ray2 a, Ray2 b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(Ray2 a, Ray2 b)
		{
			return a.Equals(b);
		}

		public override string ToString()
		{
			return string.Format("{{Origin:{0} Direction:{1}}}", Origin.ToString(), Direction.ToString());
		}

		#endregion

		/// <summary>
		/// Returns a point at <paramref name="distance"/> units from origin along the line
		/// </summary>
		public Vector2 GetPoint(float distance)
		{
			return Origin + Direction * distance;
		}
	}
}
