using System;
using System.Runtime.InteropServices;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics
{
	/// <summary>
	/// Represents the location of an object in 3D space.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct Vector3d //: IEquatable<Vector3d>
	{
		/// <summary>
		/// The X component of this vector.
		/// </summary>
		[FieldOffset(0)]
		public double X;

		/// <summary>
		/// The Y component of this vector.
		/// </summary>
		[FieldOffset(8)]
		public double Y;

		/// <summary>
		/// The Z component of this vector.
		/// </summary>
		[FieldOffset(16)]
		public double Z;

		public double x { get { return X; } }
		public double y { get { return Y; } }
		public double z { get { return Z; } }

		/// <summary>
		/// Creates a new vector from the specified value.
		/// </summary>
		/// <param name="value">The value for the components of the vector.</param>
		public Vector3d(double value)
		{
			X = Y = Z = value;
		}

		/// <summary>
		/// Creates a new vector from the specified values.
		/// </summary>
		/// <param name="x">The X component of the vector.</param>
		/// <param name="y">The Y component of the vector.</param>
		/// <param name="z">The Z component of the vector.</param>
		public Vector3d(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Creates a new vector from copying another.
		/// </summary>
		/// <param name="v">The vector to copy.</param>
		public Vector3d(in Vector3d v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		/// <summary>
		/// Converts this Vector3d to a string in the format &lt;x,y,z&gt;.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("<{0},{1},{2}>", X, Y, Z);
		}

		#region Math

		/// <summary>
		/// Truncates the decimal component of each part of this Vector3d.
		/// </summary>
		public void Floor()
		{
			X = Math.Floor(X);
			Y = Math.Floor(Y);
			Z = Math.Floor(Z);
		}

		/// <summary>
		/// Rounds the decimal component of each part of this Vector3d.
		/// </summary>
		public void Round()
		{
			X = Math.Round((double)this.X);
			Y = Math.Round((double)this.Y);
			Z = Math.Round((double)this.Z);
		}

		/// <summary>
		/// Clamps the vector to within the specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		public void Clamp(double value)
		{
			if (Math.Abs(X) > value)
				X = value * (X < 0 ? -1 : 1);
			if (Math.Abs(Y) > value)
				Y = value * (Y < 0 ? -1 : 1);
			if (Math.Abs(Z) > value)
				Z = value * (Z < 0 ? -1 : 1);
		}

		/// <summary>
		/// Calculates the distance between two Vector3d objects.
		/// </summary>
		public double DistanceTo(in Vector3d other)
		{
			return Math.Sqrt((other.X - X) * (other.X - X) +
								  (other.Y - Y) * (other.Y - Y) +
								  (other.Z - Z) * (other.Z - Z));
		}

		public Vector3d Transform(in Matrix matrix)
		{
			var x = (X * matrix.M11) + (Y * matrix.M21) + (Z * matrix.M31) + matrix.M41;
			var y = (X * matrix.M12) + (Y * matrix.M22) + (Z * matrix.M32) + matrix.M42;
			var z = (X * matrix.M13) + (Y * matrix.M23) + (Z * matrix.M33) + matrix.M43;
			return new Vector3d(x, y, z);
		}

		/// <summary>
		/// Finds the distance of this vector from Vector3d.Zero
		/// </summary>
		public double Magnitude
		{
			get { return Math.Sqrt(SqrMagnitude); }
		}

		public double SqrMagnitude
		{
			get { return X * X + Y * Y + Z * Z; }
		}

		/// <summary>
		/// Returns the component-wise minumum of two vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <returns></returns>
		public static Vector3d Min(in Vector3d value1, in Vector3d value2)
		{
			return new Vector3d(
				 Math.Min((double)value1.X, (double)value2.X),
				 Math.Min((double)value1.Y, (double)value2.Y),
				 Math.Min((double)value1.Z, (double)value2.Z)
				 );
		}

		/// <summary>
		/// Returns the component-wise maximum of two vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <returns></returns>
		public static Vector3d Max(in Vector3d value1, in Vector3d value2)
		{
			return new Vector3d(
				 Math.Max((double)value1.X, (double)value2.X),
				 Math.Max((double)value1.Y, (double)value2.Y),
				 Math.Max((double)value1.Z, (double)value2.Z)
				 );
		}

		/// <summary>
		/// Calculates the dot product between two vectors.
		/// </summary>
		public static double Dot(in Vector3d value1, in Vector3d value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
		}

		/// <summary>
		/// Computes the cross product of two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector.</param>
		/// <returns>The cross product of two vectors.</returns>
		public static Vector3d Cross(in Vector3d vector1, in Vector3d vector2)
		{
			Cross(vector1, in vector2, out var ret);
			return ret;
		}

		/// <summary>
		/// Computes the cross product of two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector.</param>
		/// <param name="result">The cross product of two vectors as an output parameter.</param>
		public static void Cross(in Vector3d vector1, in Vector3d vector2, out Vector3d result)
		{
			var x = vector1.Y * vector2.Z - vector2.Y * vector1.Z;
			var y = -(vector1.X * vector2.Z - vector2.X * vector1.Z);
			var z = vector1.X * vector2.Y - vector2.X * vector1.Y;
			result.X = x;
			result.Y = y;
			result.Z = z;
		}

		#endregion

#if UNITY_2019_4_OR_NEWER
		public static implicit operator UnityEngine.Vector3(in Vector3d v)
		{
			return new UnityEngine.Vector3((float)v.X, (float)v.Y, (float)v.Z);
		}

		public static implicit operator Vector3d(in UnityEngine.Vector3 v)
		{
			return new Vector3d(v.x, v.y, v.z);
		}
#else
		public static implicit operator Vector3(in Vector3d v)
		{
			return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
		}

		public static implicit operator Vector3d(in Vector3 v)
		{
			return new Vector3d(v.x, v.y, v.z);
		}
#endif

		#region Operators

		public static bool operator !=(in Vector3d a, in Vector3d b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(in Vector3d a, in Vector3d b)
		{
			return a.Equals(b);
		}

		public static Vector3d operator +(in Vector3d a, in Vector3d b)
		{
			return new Vector3d(
				 a.X + b.X,
				 a.Y + b.Y,
				 a.Z + b.Z);
		}

		public static Vector3d operator +(in Vector3d l, in Vector3i r)
		{
			return new Vector3d(
				 l.X + r.X,
				 l.Y + r.Y,
				 l.Z + r.Z);
		}

		public static Vector3d operator +(in Vector3i l, in Vector3d r)
		{
			return r + l;
		}

		public static Vector3d operator -(in Vector3d a, in Vector3d b)
		{
			return new Vector3d(
				 a.X - b.X,
				 a.Y - b.Y,
				 a.Z - b.Z);
		}

		public static Vector3d operator +(in Vector3d a, in Size b)
		{
			return new Vector3d(
				 a.X + b.Width,
				 a.Y + b.Height,
				 a.Z + b.Depth);
		}

		public static Vector3d operator -(in Vector3d a, in Size b)
		{
			return new Vector3d(
				 a.X - b.Width,
				 a.Y - b.Height,
				 a.Z - b.Depth);
		}

		public static Vector3d operator -(in Vector3d a)
		{
			return new Vector3d(
				 -a.X,
				 -a.Y,
				 -a.Z);
		}

		public static Vector3d operator *(in Vector3d a, in Vector3d b)
		{
			return new Vector3d(
				 a.X * b.X,
				 a.Y * b.Y,
				 a.Z * b.Z);
		}

		public static Vector3d operator /(in Vector3d a, in Vector3d b)
		{
			return new Vector3d(
				 a.X / b.X,
				 a.Y / b.Y,
				 a.Z / b.Z);
		}

		public static Vector3d operator %(in Vector3d a, in Vector3d b)
		{
			return new Vector3d(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
		}

		public static Vector3d operator +(in Vector3d a, double b)
		{
			return new Vector3d(
				 a.X + b,
				 a.Y + b,
				 a.Z + b);
		}

		public static Vector3d operator -(in Vector3d a, double b)
		{
			return new Vector3d(
				 a.X - b,
				 a.Y - b,
				 a.Z - b);
		}

		public static Vector3d operator *(in Vector3d a, double b)
		{
			return new Vector3d(
				 a.X * b,
				 a.Y * b,
				 a.Z * b);
		}

		public static Vector3d operator /(in Vector3d a, double b)
		{
			return new Vector3d(
				 a.X / b,
				 a.Y / b,
				 a.Z / b);
		}

		public static Vector3d operator %(in Vector3d a, double b)
		{
			return new Vector3d(a.X % b, a.Y % b, a.Y % b);
		}

		public static Vector3d operator +(double a, in Vector3d b)
		{
			return new Vector3d(
				 a + b.X,
				 a + b.Y,
				 a + b.Z);
		}

		public static Vector3d operator -(double a, in Vector3d b)
		{
			return new Vector3d(
				 a - b.X,
				 a - b.Y,
				 a - b.Z);
		}

		public static Vector3d operator *(double a, in Vector3d b)
		{
			return new Vector3d(
				 a * b.X,
				 a * b.Y,
				 a * b.Z);
		}

		public static Vector3d operator /(double a, in Vector3d b)
		{
			return new Vector3d(
				 a / b.X,
				 a / b.Y,
				 a / b.Z);
		}

		public static Vector3d operator %(double a, in Vector3d b)
		{
			return new Vector3d(a % b.X, a % b.Y, a % b.Y);
		}

		#endregion

		#region Constants

		/// <summary>
		/// A vector with its components set to 0.0.
		/// </summary>
		public static readonly Vector3d Zero = new Vector3d((double)0);

		/// <summary>
		/// A vector with its components set to 1.0.
		/// </summary>
		public static readonly Vector3d One = new Vector3d((double)1);


		/// <summary>
		/// A vector that points upward.
		/// </summary>
		public static readonly Vector3d Up = new Vector3d(0, 1, (double)0);

		/// <summary>
		/// A vector that points downward.
		/// </summary>
		public static readonly Vector3d Down = new Vector3d(0, -1, (double)0);

		/// <summary>
		/// A vector that points to the left.
		/// </summary>
		public static readonly Vector3d Left = new Vector3d(-1, 0, (double)0);

		/// <summary>
		/// A vector that points to the right.
		/// </summary>
		public static readonly Vector3d Right = new Vector3d(1, 0, (double)0);

		/// <summary>
		/// A vector that points backward.
		/// </summary>
		public static readonly Vector3d Backward = new Vector3d(0, 0, (double)-1);

		/// <summary>
		/// A vector that points forward.
		/// </summary>
		public static readonly Vector3d Forward = new Vector3d(0, 0, (double)1);

		public static readonly Vector3d East = new Vector3d(1, 0, (double)0);
		public static readonly Vector3d West = new Vector3d(-1, 0, (double)0);
		public static readonly Vector3d North = new Vector3d(0, 0, (double)-1);
		public static readonly Vector3d South = new Vector3d(0, 0, (double)1);

		public static readonly Vector3d UnitX = new Vector3d(1f, 0f, 0f);
		public static readonly Vector3d UnitY = new Vector3d(0f, 1f, 0f);
		public static readonly Vector3d UnitZ = new Vector3d(0f, 0f, 1f);

		#endregion

		/// <summary>
		/// Returns the distance between two vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		public static double Distance(Vector3d value1, Vector3d value2)
		{
			double result;
			DistanceSquared(ref value1, ref value2, out result);
			return Math.Sqrt(result);
		}

		/// <summary>
		/// Returns the distance between two vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <param name="result">The distance between two vectors as an output parameter.</param>
		public static void Distance(ref Vector3d value1, ref Vector3d value2, out double result)
		{
			DistanceSquared(ref value1, ref value2, out result);
			result = Math.Sqrt(result);
		}

		/// <summary>
		/// Returns the squared distance between two vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <returns>The squared distance between two vectors.</returns>
		public static double DistanceSquared(Vector3d value1, Vector3d value2)
		{
			return (value1.X - value2.X) * (value1.X - value2.X) +
					  (value1.Y - value2.Y) * (value1.Y - value2.Y) +
					  (value1.Z - value2.Z) * (value1.Z - value2.Z);
		}

		/// <summary>
		/// Returns the squared distance between two vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <param name="result">The squared distance between two vectors as an output parameter.</param>
		public static void DistanceSquared(ref Vector3d value1, ref Vector3d value2, out double result)
		{
			result = (value1.X - value2.X) * (value1.X - value2.X) +
						(value1.Y - value2.Y) * (value1.Y - value2.Y) +
						(value1.Z - value2.Z) * (value1.Z - value2.Z);
		}

		public static Vector3d Subtract(Vector3d value1, Vector3d value2)
		{
			value1.X -= value2.X;
			value1.Y -= value2.Y;
			value1.Z -= value2.Z;
			return value1;
		}

		/// <summary>
		/// Determines whether this and another vector are equal.
		/// </summary>
		/// <param name="other">The other vector.</param>
		/// <returns></returns>
		public bool Equals(Vector3d other)
		{
			return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z);
		}

		/// <summary>
		/// Determines whether this and another object are equal.
		/// </summary>
		/// <param name="obj">The other object.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return obj is Vector3d && Equals((Vector3d)obj);
		}

		/// <summary>
		/// Gets the hash code for this vector.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int result = X.GetHashCode();
				result = (result * 397) ^ Y.GetHashCode();
				result = (result * 397) ^ Z.GetHashCode();
				return result;
			}
		}

		internal string DebugDisplayString
		{
			get
			{
				return string.Concat(
					 this.X.ToString(), "  ",
					 this.Y.ToString(), "  ",
					 this.Z.ToString()
				);
			}
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a transformation of 3d-vector by the specified <see cref="Matrix"/>.
		/// </summary>
		/// <param name="position">Source <see cref="Vector3d"/>.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <returns>Transformed <see cref="Vector3d"/>.</returns>
		public static Vector3d Transform(Vector3d position, Matrix matrix)
		{
			Transform(ref position, ref matrix, out position);
			return position;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a transformation of 3d-vector by the specified <see cref="Matrix"/>.
		/// </summary>
		/// <param name="position">Source <see cref="Vector3d"/>.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <param name="result">Transformed <see cref="Vector3d"/> as an output parameter.</param>
		public static void Transform(ref Vector3d position, ref Matrix matrix, out Vector3d result)
		{
			var x = (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41;
			var y = (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42;
			var z = (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43;
			result.X = x;
			result.Y = y;
			result.Z = z;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a transformation of 3d-vector by the specified <see cref="Quaternion"/>, representing the rotation.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3d"/>.</param>
		/// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
		/// <returns>Transformed <see cref="Vector3d"/>.</returns>
		public static Vector3d Transform(Vector3d value, Quaternion rotation)
		{
			Vector3d result;
			Transform(ref value, ref rotation, out result);
			return result;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a transformation of 3d-vector by the specified <see cref="Quaternion"/>, representing the rotation.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3d"/>.</param>
		/// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
		/// <param name="result">Transformed <see cref="Vector3d"/> as an output parameter.</param>
		public static void Transform(ref Vector3d value, ref Quaternion rotation, out Vector3d result)
		{
			double x = 2 * (rotation.y * value.Z - rotation.z * value.Y);
			double y = 2 * (rotation.z * value.X - rotation.x * value.Z);
			double z = 2 * (rotation.x * value.Y - rotation.y * value.X);

			result.X = value.X + x * rotation.w + (rotation.y * z - rotation.z * y);
			result.Y = value.Y + y * rotation.w + (rotation.z * x - rotation.x * z);
			result.Z = value.Z + z * rotation.w + (rotation.x * y - rotation.y * x);
		}

		/// <summary>
		/// Apply transformation on vectors within array of <see cref="Vector3d"/> by the specified <see cref="Matrix"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="sourceIndex">The starting index of transformation in the source array.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <param name="destinationArray">Destination array.</param>
		/// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="Vector3d"/> should be written.</param>
		/// <param name="length">The number of vectors to be transformed.</param>
		public static void Transform(Vector3d[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3d[] destinationArray, int destinationIndex, int length)
		{
			if (sourceArray == null)
				throw new ArgumentNullException("sourceArray");
			if (destinationArray == null)
				throw new ArgumentNullException("destinationArray");
			if (sourceArray.Length < sourceIndex + length)
				throw new ArgumentException("Source array length is lesser than sourceIndex + length");
			if (destinationArray.Length < destinationIndex + length)
				throw new ArgumentException("Destination array length is lesser than destinationIndex + length");

			// TODO: Are there options on some platforms to implement a vectorized version of this?

			for (var i = 0; i < length; i++)
			{
				var position = sourceArray[sourceIndex + i];
				destinationArray[destinationIndex + i] =
					 new Vector3d(
						  (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
						  (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
						  (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
			}
		}

		/// <summary>
		/// Apply transformation on vectors within array of <see cref="Vector3d"/> by the specified <see cref="Quaternion"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="sourceIndex">The starting index of transformation in the source array.</param>
		/// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
		/// <param name="destinationArray">Destination array.</param>
		/// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="Vector3d"/> should be written.</param>
		/// <param name="length">The number of vectors to be transformed.</param>
		public static void Transform(Vector3d[] sourceArray, int sourceIndex, ref Quaterniond rotation, Vector3d[] destinationArray, int destinationIndex, int length)
		{
			if (sourceArray == null)
				throw new ArgumentNullException("sourceArray");
			if (destinationArray == null)
				throw new ArgumentNullException("destinationArray");
			if (sourceArray.Length < sourceIndex + length)
				throw new ArgumentException("Source array length is lesser than sourceIndex + length");
			if (destinationArray.Length < destinationIndex + length)
				throw new ArgumentException("Destination array length is lesser than destinationIndex + length");

			// TODO: Are there options on some platforms to implement a vectorized version of this?

			for (var i = 0; i < length; i++)
			{
				var position = sourceArray[sourceIndex + i];

				double x = 2 * (rotation.Y * position.Z - rotation.Z * position.Y);
				double y = 2 * (rotation.Z * position.X - rotation.X * position.Z);
				double z = 2 * (rotation.X * position.Y - rotation.Y * position.X);

				destinationArray[destinationIndex + i] =
					 new Vector3d(
						  position.X + x * rotation.W + (rotation.Y * z - rotation.Z * y),
						  position.Y + y * rotation.W + (rotation.Z * x - rotation.X * z),
						  position.Z + z * rotation.W + (rotation.X * y - rotation.Y * x));
			}
		}

		/// <summary>
		/// Apply transformation on all vectors within array of <see cref="Vector3d"/> by the specified <see cref="Matrix"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <param name="destinationArray">Destination array.</param>
		public static void Transform(Vector3d[] sourceArray, ref Matrix matrix, Vector3d[] destinationArray)
		{
			if (sourceArray == null)
				throw new ArgumentNullException("sourceArray");
			if (destinationArray == null)
				throw new ArgumentNullException("destinationArray");
			if (destinationArray.Length < sourceArray.Length)
				throw new ArgumentException("Destination array length is lesser than source array length");

			// TODO: Are there options on some platforms to implement a vectorized version of this?

			for (var i = 0; i < sourceArray.Length; i++)
			{
				var position = sourceArray[i];
				destinationArray[i] =
					 new Vector3d(
						  (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
						  (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
						  (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
			}
		}

		/// <summary>
		/// Apply transformation on all vectors within array of <see cref="Vector3d"/> by the specified <see cref="Quaternion"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
		/// <param name="destinationArray">Destination array.</param>
		public static void Transform(Vector3d[] sourceArray, ref Quaterniond rotation, Vector3d[] destinationArray)
		{
			if (sourceArray == null)
				throw new ArgumentNullException("sourceArray");
			if (destinationArray == null)
				throw new ArgumentNullException("destinationArray");
			if (destinationArray.Length < sourceArray.Length)
				throw new ArgumentException("Destination array length is lesser than source array length");

			// TODO: Are there options on some platforms to implement a vectorized version of this?

			for (var i = 0; i < sourceArray.Length; i++)
			{
				var position = sourceArray[i];

				double x = 2 * (rotation.Y * position.Z - rotation.Z * position.Y);
				double y = 2 * (rotation.Z * position.X - rotation.X * position.Z);
				double z = 2 * (rotation.X * position.Y - rotation.Y * position.X);

				destinationArray[i] =
					 new Vector3d(
						  position.X + x * rotation.W + (rotation.Y * z - rotation.Z * y),
						  position.Y + y * rotation.W + (rotation.Z * x - rotation.X * z),
						  position.Z + z * rotation.W + (rotation.X * y - rotation.Y * x));
			}
		}

		/// <summary>
		/// Turns this <see cref="Vector3d"/> to a unit vector with the same direction.
		/// </summary>
		public void Normalize()
		{
			double factor = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
			factor = 1f / factor;
			X *= factor;
			Y *= factor;
			Z *= factor;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a normalized values from another vector.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3d"/>.</param>
		/// <returns>Unit vector.</returns>
		public static Vector3d Normalize(in Vector3d value)
		{
			var sqrt = value.SqrMagnitude;
			if (sqrt == 0)
			{
				return Zero;
			}
			double factor = Math.Sqrt(sqrt);
			factor = 1f / factor;
			return new Vector3d(value.X * factor, value.Y * factor, value.Z * factor);
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a normalized values from another vector.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3d"/>.</param>
		/// <param name="result">Unit vector as an output parameter.</param>
		public static void Normalize(in Vector3d value, out Vector3d result)
		{
			var sqrt = value.SqrMagnitude;
			if (sqrt == 0)
			{
				result = Zero;
			}
			else
			{
				double factor = Math.Sqrt(sqrt);
				factor = 1f / factor;
				result.X = value.X * factor;
				result.Y = value.Y * factor;
				result.Z = value.Z * factor;
			}
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a transformation of the specified normal by the specified <see cref="Matrix"/>.
		/// </summary>
		/// <param name="normal">Source <see cref="Vector3d"/> which represents a normal vector.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <returns>Transformed normal.</returns>
		public static Vector3d TransformNormal(Vector3d normal, Matrix matrix)
		{
			TransformNormal(normal, matrix, out normal);
			return normal;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a transformation of the specified normal by the specified <see cref="Matrix"/>.
		/// </summary>
		/// <param name="normal">Source <see cref="Vector3d"/> which represents a normal vector.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <param name="result">Transformed normal as an output parameter.</param>
		public static void TransformNormal(in Vector3d normal, in Matrix matrix, out Vector3d result)
		{
			var x = (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31);
			var y = (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32);
			var z = (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33);
			result.X = x;
			result.Y = y;
			result.Z = z;
		}

		/// <summary>
		/// Apply transformation on normals within array of <see cref="Vector3d"/> by the specified <see cref="Matrix"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="sourceIndex">The starting index of transformation in the source array.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <param name="destinationArray">Destination array.</param>
		/// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="Vector3d"/> should be written.</param>
		/// <param name="length">The number of normals to be transformed.</param>
		public static void TransformNormal(Vector3d[] sourceArray,
		 int sourceIndex,
		 ref Matrix matrix,
		 Vector3d[] destinationArray,
		 int destinationIndex,
		 int length)
		{
			if (sourceArray == null)
				throw new ArgumentNullException("sourceArray");
			if (destinationArray == null)
				throw new ArgumentNullException("destinationArray");
			if (sourceArray.Length < sourceIndex + length)
				throw new ArgumentException("Source array length is lesser than sourceIndex + length");
			if (destinationArray.Length < destinationIndex + length)
				throw new ArgumentException("Destination array length is lesser than destinationIndex + length");

			for (int x = 0; x < length; x++)
			{
				var normal = sourceArray[sourceIndex + x];

				destinationArray[destinationIndex + x] =
					  new Vector3d(
						  (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
						  (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
						  (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
			}
		}

		/// <summary>
		/// Apply transformation on all normals within array of <see cref="Vector3d"/> by the specified <see cref="Matrix"/> and places the results in an another array.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
		/// <param name="destinationArray">Destination array.</param>
		public static void TransformNormal(Vector3d[] sourceArray, ref Matrix matrix, Vector3d[] destinationArray)
		{
			if (sourceArray == null)
				throw new ArgumentNullException("sourceArray");
			if (destinationArray == null)
				throw new ArgumentNullException("destinationArray");
			if (destinationArray.Length < sourceArray.Length)
				throw new ArgumentException("Destination array length is lesser than source array length");

			for (var i = 0; i < sourceArray.Length; i++)
			{
				var normal = sourceArray[i];

				destinationArray[i] =
					 new Vector3d(
						  (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
						  (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
						  (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
			}
		}

		/// <summary>
		/// Creates a new <see cref="Vector3df"/> that contains linear interpolation of the specified vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
		/// <returns>The result of linear interpolation of the specified vectors.</returns>
		public static Vector3d Lerp(in Vector3d value1, in Vector3d value2, float amount)
		{
			return new Vector3d(
				 MathHelper.Lerp(value1.X, value2.X, amount),
				 MathHelper.Lerp(value1.Y, value2.Y, amount),
				 MathHelper.Lerp(value1.Z, value2.Z, amount));
		}

		/// <summary>
		/// Creates a new <see cref="Vector3df"/> that contains linear interpolation of the specified vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
		/// <param name="result">The result of linear interpolation of the specified vectors as an output parameter.</param>
		public static void Lerp(ref Vector3d value1, ref Vector3d value2, float amount, out Vector3d result)
		{
			result.X = MathHelper.Lerp(value1.X, value2.X, amount);
			result.Y = MathHelper.Lerp(value1.Y, value2.Y, amount);
			result.Z = MathHelper.Lerp(value1.Z, value2.Z, amount);
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a multiplication of two vectors.
		/// </summary>
		/// <param name="value1">Source <see cref="Vector3d"/>.</param>
		/// <param name="value2">Source <see cref="Vector3d"/>.</param>
		/// <returns>The result of the vector multiplication.</returns>
		public static Vector3d Multiply(Vector3d value1, Vector3d value2)
		{
			value1.X *= value2.X;
			value1.Y *= value2.Y;
			value1.Z *= value2.Z;
			return value1;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a multiplication of <see cref="Vector3d"/> and a scalar.
		/// </summary>
		/// <param name="value1">Source <see cref="Vector3d"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <returns>The result of the vector multiplication with a scalar.</returns>
		public static Vector3d Multiply(Vector3d value1, float scaleFactor)
		{
			value1.X *= scaleFactor;
			value1.Y *= scaleFactor;
			value1.Z *= scaleFactor;
			return value1;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a multiplication of <see cref="Vector3d"/> and a scalar.
		/// </summary>
		/// <param name="value1">Source <see cref="Vector3d"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <param name="result">The result of the multiplication with a scalar as an output parameter.</param>
		public static void Multiply(ref Vector3d value1, double scaleFactor, out Vector3d result)
		{
			result.X = value1.X * scaleFactor;
			result.Y = value1.Y * scaleFactor;
			result.Z = value1.Z * scaleFactor;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3d"/> that contains a multiplication of two vectors.
		/// </summary>
		/// <param name="value1">Source <see cref="Vector3d"/>.</param>
		/// <param name="value2">Source <see cref="Vector3d"/>.</param>
		/// <param name="result">The result of the vector multiplication as an output parameter.</param>
		public static void Multiply(ref Vector3d value1, ref Vector3d value2, out Vector3d result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
		}

		/// <summary>
		/// Returns a dot product of two vectors.
		/// </summary>
		/// <param name="value1">The first vector.</param>
		/// <param name="value2">The second vector.</param>
		/// <param name="result">The dot product of two vectors as an output parameter.</param>
		public static void Dot(in Vector3d value1, in Vector3d value2, out double result)
		{
			result = value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
		}

		// Access the x, y, z components using [0], [1], [2] respectively.
		public double this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
					default:
						throw new IndexOutOfRangeException("Invalid Vector3d index!");
				}
			}

			set
			{
				switch (index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					case 2: Z = value; break;
					default:
						throw new IndexOutOfRangeException("Invalid Vector3d index!");
				}
			}
		}

		public static void OrthoNormalize(ref Vector3d normal, ref Vector3d tangent) 
		{ 
			normal.Normalize(); 
			tangent.Normalize(); 
			tangent = Cross(tangent, normal); 
		}

		public Vector2 ToVector2()
		{
			return new Vector2((float)X, (float)Y);
		}

		/// <summary>
		/// Convert two linearly distributed numbers between 0 and 1 to a point on a unit sphere (radius = 1)
		/// </summary>
		/// <param name="random1">Linearly distributed random number between 0 and 1</param>
		/// <param name="random2">Linearly distributed random number between 0 and 1</param>
		/// <returns>A cartesian point on the unit sphere</returns>
		public static Vector3d OnUnitSphere(float random1, float random2)
		{
			var theta = random1 * 2 * Math.PI;
			var phi = Math.Acos((2 * random2) - 1);

			// Convert from spherical coordinates to Cartesian
			var sinPhi = Math.Sin(phi);

			var x = sinPhi * Math.Cos(theta);
			var y = sinPhi * Math.Sin(theta);
			var z = Math.Cos(phi);

			return new Vector3d(x, y, z);
		}

		public static Vector3d InsideUnitSphere()
		{
			var theta = 2f * Math.PI * MathHelper.RandomNextFloat();
			var phi = Math.Acos(1f - 2f * MathHelper.RandomNextFloat());
			Vector3d v;
			v.X = Math.Sin(phi) * Math.Cos(theta);
			v.Y = Math.Sin(phi) * Math.Sin(theta);
			v.Z = Math.Cos(phi);

			return v;
		}

		public static Vector3d LerpUnclamped(in Vector3d value1, in Vector3d value2, float amount)
		{
			return new Vector3d(
				 MathHelper.LerpUnclamped(value1.X, value2.X, amount),
				 MathHelper.LerpUnclamped(value1.Y, value2.Y, amount),
				 MathHelper.LerpUnclamped(value1.Z, value2.Z, amount));
		}

		public void Deconstruct(out double x, out double y, out double z)
		{
			x = X;
			y = Y;
			z = Z;
		}

		/// <summary>
		/// Round the members of this <see cref="Vector3df /> towards positive infinity.
		/// </summary>
		public void Ceiling()
		{
			X = Math.Ceiling(X);
			Y = Math.Ceiling(Y);
			Z = Math.Ceiling(Z);
		}

		public static Vector3d Ceiling(Vector3d value)
		{
			value.X = Math.Ceiling(value.X);
			value.Y = Math.Ceiling(value.Y);
			value.Z = Math.Ceiling(value.Z);
			return value;
		}

		public static void Ceiling(ref Vector3d value, out Vector3d result)
		{
			result.X = Math.Ceiling(value.X);
			result.Y = Math.Ceiling(value.Y);
			result.Z = Math.Ceiling(value.Z);
		}

		/// <summary>
		/// Creates a new <see cref="Vector3df /> that contains members from another vector rounded towards negative infinity.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3df />.</param>
		/// <returns>The rounded <see cref="Vector3df />.</returns>
		public static Vector3d Floor(Vector3d value)
		{
			value.X = Math.Floor(value.X);
			value.Y = Math.Floor(value.Y);
			value.Z = Math.Floor(value.Z);
			return value;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3df /> that contains members from another vector rounded towards negative infinity.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3df />.</param>
		/// <param name="result">The rounded <see cref="Vector3df />.</param>
		public static void Floor(ref Vector3d value, out Vector3d result)
		{
			result.X = Math.Floor(value.X);
			result.Y = Math.Floor(value.Y);
			result.Z = Math.Floor(value.Z);
		}

		/// <summary>
		/// Creates a new <see cref="Vector3df /> that contains members from another vector rounded to the nearest integer value.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3df />.</param>
		/// <returns>The rounded <see cref="Vector3df />.</returns>
		public static Vector3d Round(Vector3d value)
		{
			value.X = Math.Round(value.X);
			value.Y = Math.Round(value.Y);
			value.Z = Math.Round(value.Z);
			return value;
		}

		/// <summary>
		/// Creates a new <see cref="Vector3df /> that contains members from another vector rounded to the nearest integer value.
		/// </summary>
		/// <param name="value">Source <see cref="Vector3df />.</param>
		/// <param name="result">The rounded <see cref="Vector3df />.</param>
		public static void Round(ref Vector3d value, out Vector3d result)
		{
			result.X = Math.Round(value.X);
			result.Y = Math.Round(value.Y);
			result.Z = Math.Round(value.Z);
		}

		public const double Epsilon = MathHelper.Epsilond;
	}
}
