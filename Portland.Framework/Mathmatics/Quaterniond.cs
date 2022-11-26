// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics
{
	/// <summary>
	/// An efficient mathematical representation for three dimensional rotations.
	/// </summary>
	[DataContract]
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public struct Quaterniond //: IEquatable<Quaterniond>
	{
		#region Private Fields

		private static readonly Quaterniond _identity = new Quaterniond(0, 0, 0, 1);

		#endregion

		#region Public Fields

		/// <summary>
		/// The x coordinate of this <see cref="Quaterniond"/>.
		/// </summary>
		[DataMember]
		public double X;

		/// <summary>
		/// The y coordinate of this <see cref="Quaterniond"/>.
		/// </summary>
		[DataMember]
		public double Y;

		/// <summary>
		/// The z coordinate of this <see cref="Quaterniond"/>.
		/// </summary>
		[DataMember]
		public double Z;

		/// <summary>
		/// The rotation component of this <see cref="Quaterniond"/>.
		/// </summary>
		[DataMember]
		public double W;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a quaternion with X, Y, Z and W from four values.
		/// </summary>
		/// <param name="x">The x coordinate in 3d-space.</param>
		/// <param name="y">The y coordinate in 3d-space.</param>
		/// <param name="z">The z coordinate in 3d-space.</param>
		/// <param name="w">The rotation component.</param>
		public Quaterniond(double x, double y, double z, double w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		/// <summary>
		/// Constructs a quaternion with X, Y, Z from <see cref="Vector3"/> and rotation component from a scalar.
		/// </summary>
		/// <param name="value">The x, y, z coordinates in 3d-space.</param>
		/// <param name="w">The rotation component.</param>
		public Quaterniond(Vector3d value, double w)
		{
			this.X = value.X;
			this.Y = value.Y;
			this.Z = value.Z;
			this.W = w;
		}

		/// <summary>
		/// Constructs a quaternion from <see cref="Vector4"/>.
		/// </summary>
		/// <param name="value">The x, y, z coordinates in 3d-space and the rotation component.</param>
		public Quaterniond(Vector4 value)
		{
			this.X = (double)value.X;
			this.Y = (double)value.Y;
			this.Z = (double)value.Z;
			this.W = (double)value.W;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Returns a quaternion representing no rotation.
		/// </summary>
		public static Quaterniond Identity
		{
			get { return _identity; }
		}

		#endregion

		#region Internal Properties

		internal string DebugDisplayString
		{
			get
			{
				if (this == Quaterniond._identity)
				{
					return "Identity";
				}

				return string.Concat(
					 this.X.ToString(), " ",
					 this.Y.ToString(), " ",
					 this.Z.ToString(), " ",
					 this.W.ToString()
				);
			}
		}

		#endregion

		#region Public Methods

		#region Add

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains the sum of two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <returns>The result of the quaternion addition.</returns>
		public static Quaterniond Add(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			quaternion.X = quaternion1.X + quaternion2.X;
			quaternion.Y = quaternion1.Y + quaternion2.Y;
			quaternion.Z = quaternion1.Z + quaternion2.Z;
			quaternion.W = quaternion1.W + quaternion2.W;
			return quaternion;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains the sum of two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <param name="result">The result of the quaternion addition as an output parameter.</param>
		public static void Add(ref Quaterniond quaternion1, ref Quaterniond quaternion2, out Quaterniond result)
		{
			result.X = quaternion1.X + quaternion2.X;
			result.Y = quaternion1.Y + quaternion2.Y;
			result.Z = quaternion1.Z + quaternion2.Z;
			result.W = quaternion1.W + quaternion2.W;
		}

		#endregion

		#region Concatenate

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains concatenation between two quaternion.
		/// </summary>
		/// <param name="value1">The first <see cref="Quaterniond"/> to concatenate.</param>
		/// <param name="value2">The second <see cref="Quaterniond"/> to concatenate.</param>
		/// <returns>The result of rotation of <paramref name="value1"/> followed by <paramref name="value2"/> rotation.</returns>
		public static Quaterniond Concatenate(Quaterniond value1, Quaterniond value2)
		{
			Quaterniond quaternion;

			double x1 = value1.X;
			double y1 = value1.Y;
			double z1 = value1.Z;
			double w1 = value1.W;

			double x2 = value2.X;
			double y2 = value2.Y;
			double z2 = value2.Z;
			double w2 = value2.W;

			quaternion.X = ((x2 * w1) + (x1 * w2)) + ((y2 * z1) - (z2 * y1));
			quaternion.Y = ((y2 * w1) + (y1 * w2)) + ((z2 * x1) - (x2 * z1));
			quaternion.Z = ((z2 * w1) + (z1 * w2)) + ((x2 * y1) - (y2 * x1));
			quaternion.W = (w2 * w1) - (((x2 * x1) + (y2 * y1)) + (z2 * z1));

			return quaternion;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains concatenation between two quaternion.
		/// </summary>
		/// <param name="value1">The first <see cref="Quaterniond"/> to concatenate.</param>
		/// <param name="value2">The second <see cref="Quaterniond"/> to concatenate.</param>
		/// <param name="result">The result of rotation of <paramref name="value1"/> followed by <paramref name="value2"/> rotation as an output parameter.</param>
		public static void Concatenate(ref Quaterniond value1, ref Quaterniond value2, out Quaterniond result)
		{
			double x1 = value1.X;
			double y1 = value1.Y;
			double z1 = value1.Z;
			double w1 = value1.W;

			double x2 = value2.X;
			double y2 = value2.Y;
			double z2 = value2.Z;
			double w2 = value2.W;

			result.X = ((x2 * w1) + (x1 * w2)) + ((y2 * z1) - (z2 * y1));
			result.Y = ((y2 * w1) + (y1 * w2)) + ((z2 * x1) - (x2 * z1));
			result.Z = ((z2 * w1) + (z1 * w2)) + ((x2 * y1) - (y2 * x1));
			result.W = (w2 * w1) - (((x2 * x1) + (y2 * y1)) + (z2 * z1));
		}

		#endregion

		#region Conjugate

		/// <summary>
		/// Transforms this quaternion into its conjugated version.
		/// </summary>
		public void Conjugate()
		{
			X = -X;
			Y = -Y;
			Z = -Z;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains conjugated version of the specified quaternion.
		/// </summary>
		/// <param name="value">The quaternion which values will be used to create the conjugated version.</param>
		/// <returns>The conjugate version of the specified quaternion.</returns>
		public static Quaterniond Conjugate(Quaterniond value)
		{
			return new Quaterniond(-value.X, -value.Y, -value.Z, value.W);
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains conjugated version of the specified quaternion.
		/// </summary>
		/// <param name="value">The quaternion which values will be used to create the conjugated version.</param>
		/// <param name="result">The conjugated version of the specified quaternion as an output parameter.</param>
		public static void Conjugate(ref Quaterniond value, out Quaterniond result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			result.W = value.W;
		}

		#endregion

		#region CreateFromAxisAngle

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified axis and angle.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle in radians.</param>
		/// <returns>The new quaternion builded from axis and angle.</returns>
		public static Quaterniond CreateFromAxisAngle(Vector3d axis, double angle)
		{
			double half = angle * 0.5f;
			double sin = Math.Sin(half);
			double cos = Math.Cos(half);
			return new Quaterniond(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified axis and angle.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle in radians.</param>
		/// <param name="result">The new quaternion builded from axis and angle as an output parameter.</param>
		public static void CreateFromAxisAngle(ref Vector3d axis, double angle, out Quaterniond result)
		{
			double half = angle * 0.5f;
			double sin = Math.Sin(half);
			double cos = Math.Cos(half);
			result.X = axis.X * sin;
			result.Y = axis.Y * sin;
			result.Z = axis.Z * sin;
			result.W = cos;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified axis and angle.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle in radians.</param>
		/// <returns>The new quaternion builded from axis and angle.</returns>
		public static Quaterniond CreateFromAxisAngle(Vector3 axis, double angle)
		{
			double half = angle * 0.5f;
			double sin = Math.Sin(half);
			double cos = Math.Cos(half);
			return new Quaterniond((double)(axis.X * sin), (double)(axis.Y * sin), (double)(axis.Z * sin), cos);
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified axis and angle.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle in radians.</param>
		/// <param name="result">The new quaternion builded from axis and angle as an output parameter.</param>
		public static void CreateFromAxisAngle(ref Vector3 axis, double angle, out Quaterniond result)
		{
			double half = angle * 0.5f;
			double sin = Math.Sin(half);
			double cos = Math.Cos(half);
			result.X = (double)(axis.X * sin);
			result.Y = (double)(axis.Y * sin);
			result.Z = (double)(axis.Z * sin);
			result.W = cos;
		}

		public static Quaterniond AngleAxis(float angle, Vector3 axis)
		{
			var half = angle * 0.5f;
			var sin = Math.Sin(half);
			var cos = Math.Cos(half);
			Quaterniond result;
			result.X = axis.X * sin;
			result.Y = axis.Y * sin;
			result.Z = axis.Z * sin;
			result.W = cos;
			return result;
		}

		#endregion

		#region CreateFromRotationMatrix

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified <see cref="Matrix"/>.
		/// </summary>
		/// <param name="matrix">The rotation matrix.</param>
		/// <returns>A quaternion composed from the rotation part of the matrix.</returns>
		public static Quaterniond CreateFromRotationMatrix(Matrixd matrix)
		{
			Quaterniond quaternion;
			double sqrt;
			double half;
			double scale = matrix.M11 + matrix.M22 + matrix.M33;

			if (scale > 0.0f)
			{
				sqrt = Math.Sqrt(scale + 1.0f);
				quaternion.W = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;

				quaternion.X = (matrix.M23 - matrix.M32) * sqrt;
				quaternion.Y = (matrix.M31 - matrix.M13) * sqrt;
				quaternion.Z = (matrix.M12 - matrix.M21) * sqrt;

				return quaternion;
			}
			if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
			{
				sqrt = Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
				half = 0.5f / sqrt;

				quaternion.X = 0.5f * sqrt;
				quaternion.Y = (matrix.M12 + matrix.M21) * half;
				quaternion.Z = (matrix.M13 + matrix.M31) * half;
				quaternion.W = (matrix.M23 - matrix.M32) * half;

				return quaternion;
			}
			if (matrix.M22 > matrix.M33)
			{
				sqrt = Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
				half = 0.5f / sqrt;

				quaternion.X = (matrix.M21 + matrix.M12) * half;
				quaternion.Y = 0.5f * sqrt;
				quaternion.Z = (matrix.M32 + matrix.M23) * half;
				quaternion.W = (matrix.M31 - matrix.M13) * half;

				return quaternion;
			}
			sqrt = Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
			half = 0.5f / sqrt;

			quaternion.X = (matrix.M31 + matrix.M13) * half;
			quaternion.Y = (matrix.M32 + matrix.M23) * half;
			quaternion.Z = 0.5f * sqrt;
			quaternion.W = (matrix.M12 - matrix.M21) * half;

			return quaternion;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified <see cref="Matrix"/>.
		/// </summary>
		/// <param name="matrix">The rotation matrix.</param>
		/// <param name="result">A quaternion composed from the rotation part of the matrix as an output parameter.</param>
		public static void CreateFromRotationMatrix(ref Matrixd matrix, out Quaterniond result)
		{
			double sqrt;
			double half;
			double scale = matrix.M11 + matrix.M22 + matrix.M33;

			if (scale > 0.0f)
			{
				sqrt = Math.Sqrt(scale + 1.0f);
				result.W = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;

				result.X = (matrix.M23 - matrix.M32) * sqrt;
				result.Y = (matrix.M31 - matrix.M13) * sqrt;
				result.Z = (matrix.M12 - matrix.M21) * sqrt;
			}
			else
			if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
			{
				sqrt = Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
				half = 0.5f / sqrt;

				result.X = 0.5f * sqrt;
				result.Y = (matrix.M12 + matrix.M21) * half;
				result.Z = (matrix.M13 + matrix.M31) * half;
				result.W = (matrix.M23 - matrix.M32) * half;
			}
			else if (matrix.M22 > matrix.M33)
			{
				sqrt = Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
				half = 0.5f / sqrt;

				result.X = (matrix.M21 + matrix.M12) * half;
				result.Y = 0.5f * sqrt;
				result.Z = (matrix.M32 + matrix.M23) * half;
				result.W = (matrix.M31 - matrix.M13) * half;
			}
			else
			{
				sqrt = Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
				half = 0.5f / sqrt;

				result.X = (matrix.M31 + matrix.M13) * half;
				result.Y = (matrix.M32 + matrix.M23) * half;
				result.Z = 0.5f * sqrt;
				result.W = (matrix.M12 - matrix.M21) * half;
			}
		}

		#endregion

		#region CreateFromYawPitchRoll

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified yaw, pitch and roll angles.
		/// </summary>
		/// <param name="yaw">Yaw around the y axis in radians.</param>
		/// <param name="pitch">Pitch around the x axis in radians.</param>
		/// <param name="roll">Roll around the z axis in radians.</param>
		/// <returns>A new quaternion from the concatenated yaw, pitch, and roll angles.</returns>
		public static Quaterniond CreateFromYawPitchRoll(double yaw, double pitch, double roll)
		{
			double halfRoll = roll * 0.5f;
			double halfPitch = pitch * 0.5f;
			double halfYaw = yaw * 0.5f;

			double sinRoll = Math.Sin(halfRoll);
			double cosRoll = Math.Cos(halfRoll);
			double sinPitch = Math.Sin(halfPitch);
			double cosPitch = Math.Cos(halfPitch);
			double sinYaw = Math.Sin(halfYaw);
			double cosYaw = Math.Cos(halfYaw);

			return new Quaterniond((cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll),
										 (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll),
										 (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll),
										 (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll));
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> from the specified yaw, pitch and roll angles.
		/// </summary>
		/// <param name="yaw">Yaw around the y axis in radians.</param>
		/// <param name="pitch">Pitch around the x axis in radians.</param>
		/// <param name="roll">Roll around the z axis in radians.</param>
		/// <param name="result">A new quaternion from the concatenated yaw, pitch, and roll angles as an output parameter.</param>
		public static void CreateFromYawPitchRoll(double yaw, double pitch, double roll, out Quaterniond result)
		{
			double halfRoll = roll * 0.5f;
			double halfPitch = pitch * 0.5f;
			double halfYaw = yaw * 0.5f;

			double sinRoll = Math.Sin(halfRoll);
			double cosRoll = Math.Cos(halfRoll);
			double sinPitch = Math.Sin(halfPitch);
			double cosPitch = Math.Cos(halfPitch);
			double sinYaw = Math.Sin(halfYaw);
			double cosYaw = Math.Cos(halfYaw);

			result.X = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
			result.Y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
			result.Z = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
			result.W = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
		}

		#endregion

		#region Divide

		/// <summary>
		/// Divides a <see cref="Quaterniond"/> by the other <see cref="Quaterniond"/>.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Divisor <see cref="Quaterniond"/>.</param>
		/// <returns>The result of dividing the quaternions.</returns>
		public static Quaterniond Divide(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			double x = quaternion1.X;
			double y = quaternion1.Y;
			double z = quaternion1.Z;
			double w = quaternion1.W;
			double num14 = (((quaternion2.X * quaternion2.X) + (quaternion2.Y * quaternion2.Y)) + (quaternion2.Z * quaternion2.Z)) + (quaternion2.W * quaternion2.W);
			double num5 = 1f / num14;
			double num4 = -quaternion2.X * num5;
			double num3 = -quaternion2.Y * num5;
			double num2 = -quaternion2.Z * num5;
			double num = quaternion2.W * num5;
			double num13 = (y * num2) - (z * num3);
			double num12 = (z * num4) - (x * num2);
			double num11 = (x * num3) - (y * num4);
			double num10 = ((x * num4) + (y * num3)) + (z * num2);
			quaternion.X = ((x * num) + (num4 * w)) + num13;
			quaternion.Y = ((y * num) + (num3 * w)) + num12;
			quaternion.Z = ((z * num) + (num2 * w)) + num11;
			quaternion.W = (w * num) - num10;
			return quaternion;
		}

		/// <summary>
		/// Divides a <see cref="Quaterniond"/> by the other <see cref="Quaterniond"/>.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Divisor <see cref="Quaterniond"/>.</param>
		/// <param name="result">The result of dividing the quaternions as an output parameter.</param>
		public static void Divide(ref Quaterniond quaternion1, ref Quaterniond quaternion2, out Quaterniond result)
		{
			double x = quaternion1.X;
			double y = quaternion1.Y;
			double z = quaternion1.Z;
			double w = quaternion1.W;
			double num14 = (((quaternion2.X * quaternion2.X) + (quaternion2.Y * quaternion2.Y)) + (quaternion2.Z * quaternion2.Z)) + (quaternion2.W * quaternion2.W);
			double num5 = 1f / num14;
			double num4 = -quaternion2.X * num5;
			double num3 = -quaternion2.Y * num5;
			double num2 = -quaternion2.Z * num5;
			double num = quaternion2.W * num5;
			double num13 = (y * num2) - (z * num3);
			double num12 = (z * num4) - (x * num2);
			double num11 = (x * num3) - (y * num4);
			double num10 = ((x * num4) + (y * num3)) + (z * num2);
			result.X = ((x * num) + (num4 * w)) + num13;
			result.Y = ((y * num) + (num3 * w)) + num12;
			result.Z = ((z * num) + (num2 * w)) + num11;
			result.W = (w * num) - num10;
		}

		#endregion

		#region Dot

		/// <summary>
		/// Returns a dot product of two quaternions.
		/// </summary>
		/// <param name="quaternion1">The first quaternion.</param>
		/// <param name="quaternion2">The second quaternion.</param>
		/// <returns>The dot product of two quaternions.</returns>
		public static double Dot(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			return ((((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W));
		}

		/// <summary>
		/// Returns a dot product of two quaternions.
		/// </summary>
		/// <param name="quaternion1">The first quaternion.</param>
		/// <param name="quaternion2">The second quaternion.</param>
		/// <param name="result">The dot product of two quaternions as an output parameter.</param>
		public static void Dot(ref Quaterniond quaternion1, ref Quaterniond quaternion2, out double result)
		{
			result = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
		}

		#endregion

		#region Equals

		/// <summary>
		/// Compares whether current instance is equal to specified <see cref="Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Quaterniond)
				return Equals((Quaterniond)obj);
			return false;
		}

		/// <summary>
		/// Compares whether current instance is equal to specified <see cref="Quaterniond"/>.
		/// </summary>
		/// <param name="other">The <see cref="Quaterniond"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public bool Equals(Quaterniond other)
		{
			return X == other.X &&
						 Y == other.Y &&
						 Z == other.Z &&
						 W == other.W;
		}

		#endregion

		/// <summary>
		/// Gets the hash code of this <see cref="Quaterniond"/>.
		/// </summary>
		/// <returns>Hash code of this <see cref="Quaterniond"/>.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
		}

		#region Inverse

		/// <summary>
		/// Returns the inverse quaternion which represents the opposite rotation.
		/// </summary>
		/// <param name="quaternion">Source <see cref="Quaterniond"/>.</param>
		/// <returns>The inverse quaternion.</returns>
		public static Quaterniond Inverse(Quaterniond quaternion)
		{
			Quaterniond quaternion2;
			double num2 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
			double num = 1f / num2;
			quaternion2.X = -quaternion.X * num;
			quaternion2.Y = -quaternion.Y * num;
			quaternion2.Z = -quaternion.Z * num;
			quaternion2.W = quaternion.W * num;
			return quaternion2;
		}

		/// <summary>
		/// Returns the inverse quaternion which represents the opposite rotation.
		/// </summary>
		/// <param name="quaternion">Source <see cref="Quaterniond"/>.</param>
		/// <param name="result">The inverse quaternion as an output parameter.</param>
		public static void Inverse(ref Quaterniond quaternion, out Quaterniond result)
		{
			double num2 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
			double num = 1f / num2;
			result.X = -quaternion.X * num;
			result.Y = -quaternion.Y * num;
			result.Z = -quaternion.Z * num;
			result.W = quaternion.W * num;
		}

		#endregion

		/// <summary>
		/// Returns the magnitude of the quaternion components.
		/// </summary>
		/// <returns>The magnitude of the quaternion components.</returns>
		public double Length()
		{
			return Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
		}

		/// <summary>
		/// Returns the squared magnitude of the quaternion components.
		/// </summary>
		/// <returns>The squared magnitude of the quaternion components.</returns>
		public double LengthSquared()
		{
			return (X * X) + (Y * Y) + (Z * Z) + (W * W);
		}

		#region Lerp

		/// <summary>
		/// Performs a linear blend between two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <returns>The result of linear blending between two quaternions.</returns>
		public static Quaterniond Lerp(Quaterniond quaternion1, Quaterniond quaternion2, double amount)
		{
			double num = amount;
			double num2 = 1f - num;
			Quaterniond quaternion = new Quaterniond();
			double num5 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
			if (num5 >= 0f)
			{
				quaternion.X = (num2 * quaternion1.X) + (num * quaternion2.X);
				quaternion.Y = (num2 * quaternion1.Y) + (num * quaternion2.Y);
				quaternion.Z = (num2 * quaternion1.Z) + (num * quaternion2.Z);
				quaternion.W = (num2 * quaternion1.W) + (num * quaternion2.W);
			}
			else
			{
				quaternion.X = (num2 * quaternion1.X) - (num * quaternion2.X);
				quaternion.Y = (num2 * quaternion1.Y) - (num * quaternion2.Y);
				quaternion.Z = (num2 * quaternion1.Z) - (num * quaternion2.Z);
				quaternion.W = (num2 * quaternion1.W) - (num * quaternion2.W);
			}
			double num4 = (((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y)) + (quaternion.Z * quaternion.Z)) + (quaternion.W * quaternion.W);
			double num3 = 1f / Math.Sqrt(num4);
			quaternion.X *= num3;
			quaternion.Y *= num3;
			quaternion.Z *= num3;
			quaternion.W *= num3;
			return quaternion;
		}

		/// <summary>
		/// Performs a linear blend between two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <param name="result">The result of linear blending between two quaternions as an output parameter.</param>
		public static void Lerp(ref Quaterniond quaternion1, ref Quaterniond quaternion2, double amount, out Quaterniond result)
		{
			double num = amount;
			double num2 = 1f - num;
			double num5 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
			if (num5 >= 0f)
			{
				result.X = (num2 * quaternion1.X) + (num * quaternion2.X);
				result.Y = (num2 * quaternion1.Y) + (num * quaternion2.Y);
				result.Z = (num2 * quaternion1.Z) + (num * quaternion2.Z);
				result.W = (num2 * quaternion1.W) + (num * quaternion2.W);
			}
			else
			{
				result.X = (num2 * quaternion1.X) - (num * quaternion2.X);
				result.Y = (num2 * quaternion1.Y) - (num * quaternion2.Y);
				result.Z = (num2 * quaternion1.Z) - (num * quaternion2.Z);
				result.W = (num2 * quaternion1.W) - (num * quaternion2.W);
			}
			double num4 = (((result.X * result.X) + (result.Y * result.Y)) + (result.Z * result.Z)) + (result.W * result.W);
			double num3 = 1f / Math.Sqrt(num4);
			result.X *= num3;
			result.Y *= num3;
			result.Z *= num3;
			result.W *= num3;

		}

		#endregion

		#region Slerp

		/// <summary>
		/// Performs a spherical linear blend between two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <returns>The result of spherical linear blending between two quaternions.</returns>
		public static Quaterniond Slerp(Quaterniond quaternion1, Quaterniond quaternion2, double amount)
		{
			double num2;
			double num3;
			Quaterniond quaternion;
			double num = amount;
			double num4 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
			bool flag = false;
			if (num4 < 0f)
			{
				flag = true;
				num4 = -num4;
			}
			if (num4 > 0.999999f)
			{
				num3 = 1f - num;
				num2 = flag ? -num : num;
			}
			else
			{
				double num5 = Math.Acos(num4);
				double num6 = (double)(1.0 / Math.Sin((double)num5));
				num3 = Math.Sin((1f - num) * num5) * num6;
				num2 = flag ? (-Math.Sin(num * num5) * num6) : (Math.Sin(num * num5) * num6);
			}
			quaternion.X = (num3 * quaternion1.X) + (num2 * quaternion2.X);
			quaternion.Y = (num3 * quaternion1.Y) + (num2 * quaternion2.Y);
			quaternion.Z = (num3 * quaternion1.Z) + (num2 * quaternion2.Z);
			quaternion.W = (num3 * quaternion1.W) + (num2 * quaternion2.W);
			return quaternion;
		}

		/// <summary>
		/// Performs a spherical linear blend between two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <param name="result">The result of spherical linear blending between two quaternions as an output parameter.</param>
		public static void Slerp(ref Quaterniond quaternion1, ref Quaterniond quaternion2, double amount, out Quaterniond result)
		{
			double num2;
			double num3;
			double num = amount;
			double num4 = (((quaternion1.X * quaternion2.X) + (quaternion1.Y * quaternion2.Y)) + (quaternion1.Z * quaternion2.Z)) + (quaternion1.W * quaternion2.W);
			bool flag = false;
			if (num4 < 0f)
			{
				flag = true;
				num4 = -num4;
			}
			if (num4 > 0.999999f)
			{
				num3 = 1f - num;
				num2 = flag ? -num : num;
			}
			else
			{
				double num5 = Math.Acos(num4);
				double num6 = (double)(1.0 / Math.Sin((double)num5));
				num3 = Math.Sin((1f - num) * num5) * num6;
				num2 = flag ? (-Math.Sin(num * num5) * num6) : (Math.Sin(num * num5) * num6);
			}
			result.X = (num3 * quaternion1.X) + (num2 * quaternion2.X);
			result.Y = (num3 * quaternion1.Y) + (num2 * quaternion2.Y);
			result.Z = (num3 * quaternion1.Z) + (num2 * quaternion2.Z);
			result.W = (num3 * quaternion1.W) + (num2 * quaternion2.W);
		}

		#endregion

		#region Subtract

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains subtraction of one <see cref="Quaterniond"/> from another.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <returns>The result of the quaternion subtraction.</returns>
		public static Quaterniond Subtract(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			quaternion.X = quaternion1.X - quaternion2.X;
			quaternion.Y = quaternion1.Y - quaternion2.Y;
			quaternion.Z = quaternion1.Z - quaternion2.Z;
			quaternion.W = quaternion1.W - quaternion2.W;
			return quaternion;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains subtraction of one <see cref="Quaterniond"/> from another.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <param name="result">The result of the quaternion subtraction as an output parameter.</param>
		public static void Subtract(ref Quaterniond quaternion1, ref Quaterniond quaternion2, out Quaterniond result)
		{
			result.X = quaternion1.X - quaternion2.X;
			result.Y = quaternion1.Y - quaternion2.Y;
			result.Z = quaternion1.Z - quaternion2.Z;
			result.W = quaternion1.W - quaternion2.W;
		}

		#endregion

		#region Multiply

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains a multiplication of two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <returns>The result of the quaternion multiplication.</returns>
		public static Quaterniond Multiply(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			double x = quaternion1.X;
			double y = quaternion1.Y;
			double z = quaternion1.Z;
			double w = quaternion1.W;
			double num4 = quaternion2.X;
			double num3 = quaternion2.Y;
			double num2 = quaternion2.Z;
			double num = quaternion2.W;
			double num12 = (y * num2) - (z * num3);
			double num11 = (z * num4) - (x * num2);
			double num10 = (x * num3) - (y * num4);
			double num9 = ((x * num4) + (y * num3)) + (z * num2);
			quaternion.X = ((x * num) + (num4 * w)) + num12;
			quaternion.Y = ((y * num) + (num3 * w)) + num11;
			quaternion.Z = ((z * num) + (num2 * w)) + num10;
			quaternion.W = (w * num) - num9;
			return quaternion;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains a multiplication of <see cref="Quaterniond"/> and a scalar.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <returns>The result of the quaternion multiplication with a scalar.</returns>
		public static Quaterniond Multiply(Quaterniond quaternion1, double scaleFactor)
		{
			Quaterniond quaternion;
			quaternion.X = quaternion1.X * scaleFactor;
			quaternion.Y = quaternion1.Y * scaleFactor;
			quaternion.Z = quaternion1.Z * scaleFactor;
			quaternion.W = quaternion1.W * scaleFactor;
			return quaternion;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains a multiplication of <see cref="Quaterniond"/> and a scalar.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <param name="result">The result of the quaternion multiplication with a scalar as an output parameter.</param>
		public static void Multiply(ref Quaterniond quaternion1, double scaleFactor, out Quaterniond result)
		{
			result.X = quaternion1.X * scaleFactor;
			result.Y = quaternion1.Y * scaleFactor;
			result.Z = quaternion1.Z * scaleFactor;
			result.W = quaternion1.W * scaleFactor;
		}

		/// <summary>
		/// Creates a new <see cref="Quaterniond"/> that contains a multiplication of two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/>.</param>
		/// <param name="result">The result of the quaternion multiplication as an output parameter.</param>
		public static void Multiply(ref Quaterniond quaternion1, ref Quaterniond quaternion2, out Quaterniond result)
		{
			double x = quaternion1.X;
			double y = quaternion1.Y;
			double z = quaternion1.Z;
			double w = quaternion1.W;
			double num4 = quaternion2.X;
			double num3 = quaternion2.Y;
			double num2 = quaternion2.Z;
			double num = quaternion2.W;
			double num12 = (y * num2) - (z * num3);
			double num11 = (z * num4) - (x * num2);
			double num10 = (x * num3) - (y * num4);
			double num9 = ((x * num4) + (y * num3)) + (z * num2);
			result.X = ((x * num) + (num4 * w)) + num12;
			result.Y = ((y * num) + (num3 * w)) + num11;
			result.Z = ((z * num) + (num2 * w)) + num10;
			result.W = (w * num) - num9;
		}

		#endregion

		#region Negate

		/// <summary>
		/// Flips the sign of the all the quaternion components.
		/// </summary>
		/// <param name="quaternion">Source <see cref="Quaterniond"/>.</param>
		/// <returns>The result of the quaternion negation.</returns>
		public static Quaterniond Negate(Quaterniond quaternion)
		{
			return new Quaterniond(-quaternion.X, -quaternion.Y, -quaternion.Z, -quaternion.W);
		}

		/// <summary>
		/// Flips the sign of the all the quaternion components.
		/// </summary>
		/// <param name="quaternion">Source <see cref="Quaterniond"/>.</param>
		/// <param name="result">The result of the quaternion negation as an output parameter.</param>
		public static void Negate(ref Quaterniond quaternion, out Quaterniond result)
		{
			result.X = -quaternion.X;
			result.Y = -quaternion.Y;
			result.Z = -quaternion.Z;
			result.W = -quaternion.W;
		}

		#endregion

		#region Normalize

		/// <summary>
		/// Scales the quaternion magnitude to unit length.
		/// </summary>
		public void Normalize()
		{
			double num = 1f / Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
			X *= num;
			Y *= num;
			Z *= num;
			W *= num;
		}

		/// <summary>
		/// Scales the quaternion magnitude to unit length.
		/// </summary>
		/// <param name="quaternion">Source <see cref="Quaterniond"/>.</param>
		/// <returns>The unit length quaternion.</returns>
		public static Quaterniond Normalize(Quaterniond quaternion)
		{
			Quaterniond result;
			double num = 1f / Math.Sqrt((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y) + (quaternion.Z * quaternion.Z) + (quaternion.W * quaternion.W));
			result.X = quaternion.X * num;
			result.Y = quaternion.Y * num;
			result.Z = quaternion.Z * num;
			result.W = quaternion.W * num;
			return result;
		}

		/// <summary>
		/// Scales the quaternion magnitude to unit length.
		/// </summary>
		/// <param name="quaternion">Source <see cref="Quaterniond"/>.</param>
		/// <param name="result">The unit length quaternion an output parameter.</param>
		public static void Normalize(ref Quaterniond quaternion, out Quaterniond result)
		{
			double num = 1f / Math.Sqrt((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y) + (quaternion.Z * quaternion.Z) + (quaternion.W * quaternion.W));
			result.X = quaternion.X * num;
			result.Y = quaternion.Y * num;
			result.Z = quaternion.Z * num;
			result.W = quaternion.W * num;
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="String"/> representation of this <see cref="Quaterniond"/> in the format:
		/// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>] W:[<see cref="W"/>]}
		/// </summary>
		/// <returns>A <see cref="String"/> representation of this <see cref="Quaterniond"/>.</returns>
		public override string ToString()
		{
			return "{X:" + X + " Y:" + Y + " Z:" + Z + " W:" + W + "}";
		}

		/// <summary>
		/// Gets a <see cref="Vector4"/> representation for this object.
		/// </summary>
		/// <returns>A <see cref="Vector4"/> representation for this object.</returns>
		public Vector4 ToVector4()
		{
			return new Vector4((float)X, (float)Y, (float)Z, (float)W);
		}

		public void Deconstruct(out double x, out double y, out double z, out double w)
		{
			x = X;
			y = Y;
			z = Z;
			w = W;
		}

		///// <summary>
		///// Returns a <see cref="System.Numerics.Quaterniond"/>.
		///// </summary>
		//public System.Numerics.Quaterniond ToNumerics()
		//{
		//	return new System.Numerics.Quaterniond(this.X, this.Y, this.Z, this.W);
		//}

		#endregion

		#region Operators

		///// <summary>
		///// Converts a <see cref="System.Numerics.Quaterniond"/> to a <see cref="Quaterniond"/>.
		///// </summary>
		///// <param name="value">The converted value.</param>
		//public static implicit operator Quaterniond(System.Numerics.Quaterniond value)
		//{
		//	return new Quaterniond(value.X, value.Y, value.Z, value.W);
		//}

		/// <summary>
		/// Adds two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/> on the left of the add sign.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/> on the right of the add sign.</param>
		/// <returns>Sum of the vectors.</returns>
		public static Quaterniond operator +(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			quaternion.X = quaternion1.X + quaternion2.X;
			quaternion.Y = quaternion1.Y + quaternion2.Y;
			quaternion.Z = quaternion1.Z + quaternion2.Z;
			quaternion.W = quaternion1.W + quaternion2.W;
			return quaternion;
		}

		/// <summary>
		/// Divides a <see cref="Quaterniond"/> by the other <see cref="Quaterniond"/>.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/> on the left of the div sign.</param>
		/// <param name="quaternion2">Divisor <see cref="Quaterniond"/> on the right of the div sign.</param>
		/// <returns>The result of dividing the quaternions.</returns>
		public static Quaterniond operator /(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			double x = quaternion1.X;
			double y = quaternion1.Y;
			double z = quaternion1.Z;
			double w = quaternion1.W;
			double num14 = (((quaternion2.X * quaternion2.X) + (quaternion2.Y * quaternion2.Y)) + (quaternion2.Z * quaternion2.Z)) + (quaternion2.W * quaternion2.W);
			double num5 = 1f / num14;
			double num4 = -quaternion2.X * num5;
			double num3 = -quaternion2.Y * num5;
			double num2 = -quaternion2.Z * num5;
			double num = quaternion2.W * num5;
			double num13 = (y * num2) - (z * num3);
			double num12 = (z * num4) - (x * num2);
			double num11 = (x * num3) - (y * num4);
			double num10 = ((x * num4) + (y * num3)) + (z * num2);
			quaternion.X = ((x * num) + (num4 * w)) + num13;
			quaternion.Y = ((y * num) + (num3 * w)) + num12;
			quaternion.Z = ((z * num) + (num2 * w)) + num11;
			quaternion.W = (w * num) - num10;
			return quaternion;
		}

		/// <summary>
		/// Compares whether two <see cref="Quaterniond"/> instances are equal.
		/// </summary>
		/// <param name="quaternion1"><see cref="Quaterniond"/> instance on the left of the equal sign.</param>
		/// <param name="quaternion2"><see cref="Quaterniond"/> instance on the right of the equal sign.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public static bool operator ==(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			return ((((quaternion1.X == quaternion2.X) && (quaternion1.Y == quaternion2.Y)) && (quaternion1.Z == quaternion2.Z)) && (quaternion1.W == quaternion2.W));
		}

		/// <summary>
		/// Compares whether two <see cref="Quaterniond"/> instances are not equal.
		/// </summary>
		/// <param name="quaternion1"><see cref="Quaterniond"/> instance on the left of the not equal sign.</param>
		/// <param name="quaternion2"><see cref="Quaterniond"/> instance on the right of the not equal sign.</param>
		/// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
		public static bool operator !=(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			if (((quaternion1.X == quaternion2.X) && (quaternion1.Y == quaternion2.Y)) && (quaternion1.Z == quaternion2.Z))
			{
				return (quaternion1.W != quaternion2.W);
			}
			return true;
		}

		/// <summary>
		/// Multiplies two quaternions.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Quaterniond"/> on the left of the mul sign.</param>
		/// <param name="quaternion2">Source <see cref="Quaterniond"/> on the right of the mul sign.</param>
		/// <returns>Result of the quaternions multiplication.</returns>
		public static Quaterniond operator *(Quaterniond quaternion1, Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			double x = quaternion1.X;
			double y = quaternion1.Y;
			double z = quaternion1.Z;
			double w = quaternion1.W;
			double num4 = quaternion2.X;
			double num3 = quaternion2.Y;
			double num2 = quaternion2.Z;
			double num = quaternion2.W;
			double num12 = (y * num2) - (z * num3);
			double num11 = (z * num4) - (x * num2);
			double num10 = (x * num3) - (y * num4);
			double num9 = ((x * num4) + (y * num3)) + (z * num2);
			quaternion.X = ((x * num) + (num4 * w)) + num12;
			quaternion.Y = ((y * num) + (num3 * w)) + num11;
			quaternion.Z = ((z * num) + (num2 * w)) + num10;
			quaternion.W = (w * num) - num9;
			return quaternion;
		}

		/// <summary>
		/// Multiplies the components of quaternion by a scalar.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Vector3"/> on the left of the mul sign.</param>
		/// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
		/// <returns>Result of the quaternion multiplication with a scalar.</returns>
		public static Quaterniond operator *(in Quaterniond quaternion1, double scaleFactor)
		{
			Quaterniond quaternion;
			quaternion.X = quaternion1.X * scaleFactor;
			quaternion.Y = quaternion1.Y * scaleFactor;
			quaternion.Z = quaternion1.Z * scaleFactor;
			quaternion.W = quaternion1.W * scaleFactor;
			return quaternion;
		}

		/// <summary>
		/// Subtracts a <see cref="Quaterniond"/> from a <see cref="Quaterniond"/>.
		/// </summary>
		/// <param name="quaternion1">Source <see cref="Vector3"/> on the left of the sub sign.</param>
		/// <param name="quaternion2">Source <see cref="Vector3"/> on the right of the sub sign.</param>
		/// <returns>Result of the quaternion subtraction.</returns>
		public static Quaterniond operator -(in Quaterniond quaternion1, in Quaterniond quaternion2)
		{
			Quaterniond quaternion;
			quaternion.X = quaternion1.X - quaternion2.X;
			quaternion.Y = quaternion1.Y - quaternion2.Y;
			quaternion.Z = quaternion1.Z - quaternion2.Z;
			quaternion.W = quaternion1.W - quaternion2.W;
			return quaternion;

		}

		/// <summary>
		/// Flips the sign of the all the quaternion components.
		/// </summary>
		/// <param name="quaternion">Source <see cref="Quaterniond"/> on the right of the sub sign.</param>
		/// <returns>The result of the quaternion negation.</returns>
		public static Quaterniond operator -(in Quaterniond quaternion)
		{
			Quaterniond quaternion2;
			quaternion2.X = -quaternion.X;
			quaternion2.Y = -quaternion.Y;
			quaternion2.Z = -quaternion.Z;
			quaternion2.W = -quaternion.W;
			return quaternion2;
		}

		#endregion

		public static Quaterniond Euler(double pitch, double yaw, double roll)
		{
			yaw *= 0.5f;
			pitch *= 0.5f;
			roll *= 0.5f;

			double c1 = (double)Math.Cos(yaw);
			double c2 = (double)Math.Cos(pitch);
			double c3 = (double)Math.Cos(roll);
			double s1 = (double)Math.Sin(yaw);
			double s2 = (double)Math.Sin(pitch);
			double s3 = (double)Math.Sin(roll);

			Quaterniond q;
			q.X = s1 * s2 * c3 + c1 * c2 * s3;
			q.Y = s1 * c2 * c3 + c1 * s2 * s3;
			q.Z = c1 * s2 * c3 - s1 * c2 * s3;
			q.W = c1 * c2 * c3 - s1 * s2 * s3;

			return q;
		}

		// Rotates the point /point/ with /rotation/.
		public static Vector3d operator *(in Quaterniond rotation, in Vector3d point)
		{
			double x = rotation.X * 2d;
			double y = rotation.Y * 2d;
			double z = rotation.Z * 2d;
			double xx = rotation.X * x;
			double yy = rotation.Y * y;
			double zz = rotation.Z * z;
			double xy = rotation.X * y;
			double xz = rotation.X * z;
			double yz = rotation.Y * z;
			double wx = rotation.W * x;
			double wy = rotation.W * y;
			double wz = rotation.W * z;

			Vector3d res;
			res.X = (double)((1d - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z);
			res.Y = (double)((xy + wz) * point.X + (1F - (xx + zz)) * point.Y + (yz - wx) * point.Z);
			res.Z = (double)((xz - wy) * point.X + (yz + wx) * point.Y + (1F - (xx + yy)) * point.Z);
			return res;
		}
	}
}
