﻿// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#if !UNITY_5_3_OR_NEWER

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

using Portland.Mathmatics.Geometry;

namespace Portland.Mathmatics
{
	/// <summary>
	/// Represents the right-handed 4x4 floating point matrix, which can store translation, scale and rotation information.
	/// </summary>
	[DataContract]
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public struct Matrix4x4 //: IEquatable<Matrix>
	{
		#region Public Constructors

		/// <summary>
		/// Constructs a matrix.
		/// </summary>
		/// <param name="m11">A first row and first column value.</param>
		/// <param name="m12">A first row and second column value.</param>
		/// <param name="m13">A first row and third column value.</param>
		/// <param name="m14">A first row and fourth column value.</param>
		/// <param name="m21">A second row and first column value.</param>
		/// <param name="m22">A second row and second column value.</param>
		/// <param name="m23">A second row and third column value.</param>
		/// <param name="m24">A second row and fourth column value.</param>
		/// <param name="m31">A third row and first column value.</param>
		/// <param name="m32">A third row and second column value.</param>
		/// <param name="m33">A third row and third column value.</param>
		/// <param name="m34">A third row and fourth column value.</param>
		/// <param name="m41">A fourth row and first column value.</param>
		/// <param name="m42">A fourth row and second column value.</param>
		/// <param name="m43">A fourth row and third column value.</param>
		/// <param name="m44">A fourth row and fourth column value.</param>

		public Matrix4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31,
						  float m32, float m33, float m34, float m41, float m42, float m43, float m44)
		{
			this.m11 = (float)m11;
			this.m12 = (float)m12;
			this.m13 = (float)m13;
			this.m14 = (float)m14;
			this.m21 = (float)m21;
			this.m22 = (float)m22;
			this.m23 = (float)m23;
			this.m24 = (float)m24;
			this.m31 = (float)m31;
			this.m32 = (float)m32;
			this.m33 = (float)m33;
			this.m34 = (float)m34;
			this.m41 = (float)m41;
			this.m42 = (float)m42;
			this.m43 = (float)m43;
			this.m44 = (float)m44;
		}

		public Matrix4x4(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31,
						  double m32, double m33, double m34, double m41, double m42, double m43, double m44)
		{
			this.m11 = (float)m11;
			this.m12 = (float)m12;
			this.m13 = (float)m13;
			this.m14 = (float)m14;
			this.m21 = (float)m21;
			this.m22 = (float)m22;
			this.m23 = (float)m23;
			this.m24 = (float)m24;
			this.m31 = (float)m31;
			this.m32 = (float)m32;
			this.m33 = (float)m33;
			this.m34 = (float)m34;
			this.m41 = (float)m41;
			this.m42 = (float)m42;
			this.m43 = (float)m43;
			this.m44 = (float)m44;
		}

		/// <summary>
		/// Constructs a matrix.
		/// </summary>
		/// <param name="row1">A first row of the created matrix.</param>
		/// <param name="row2">A second row of the created matrix.</param>
		/// <param name="row3">A third row of the created matrix.</param>
		/// <param name="row4">A fourth row of the created matrix.</param>
		public Matrix4x4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4)
		{
			this.m11 = (float)row1.x;
			this.m12 = (float)row1.y;
			this.m13 = (float)row1.z;
			this.m14 = (float)row1.w;
			this.m21 = (float)row2.x;
			this.m22 = (float)row2.y;
			this.m23 = (float)row2.z;
			this.m24 = (float)row2.w;
			this.m31 = (float)row3.x;
			this.m32 = (float)row3.y;
			this.m33 = (float)row3.z;
			this.m34 = (float)row3.w;
			this.m41 = (float)row4.x;
			this.m42 = (float)row4.y;
			this.m43 = (float)row4.z;
			this.m44 = (float)row4.w;
		}

		#endregion

		#region Public Fields

		/// <summary>
		/// A first row and first column value.
		/// </summary>
		[DataMember]
		public float m11;

		/// <summary>
		/// A first row and second column value.
		/// </summary>
		[DataMember]
		public float m12;

		/// <summary>
		/// A first row and third column value.
		/// </summary>
		[DataMember]
		public float m13;

		/// <summary>
		/// A first row and fourth column value.
		/// </summary>
		[DataMember]
		public float m14;

		/// <summary>
		/// A second row and first column value.
		/// </summary>
		[DataMember]
		public float m21;

		/// <summary>
		/// A second row and second column value.
		/// </summary>
		[DataMember]
		public float m22;

		/// <summary>
		/// A second row and third column value.
		/// </summary>
		[DataMember]
		public float m23;

		/// <summary>
		/// A second row and fourth column value.
		/// </summary>
		[DataMember]
		public float m24;

		/// <summary>
		/// A third row and first column value.
		/// </summary>
		[DataMember]
		public float m31;

		/// <summary>
		/// A third row and second column value.
		/// </summary>
		[DataMember]
		public float m32;

		/// <summary>
		/// A third row and third column value.
		/// </summary>
		[DataMember]
		public float m33;

		/// <summary>
		/// A third row and fourth column value.
		/// </summary>
		[DataMember]
		public float m34;

		/// <summary>
		/// A fourth row and first column value.
		/// </summary>
		[DataMember]
		public float m41;

		/// <summary>
		/// A fourth row and second column value.
		/// </summary>
		[DataMember]
		public float m42;

		/// <summary>
		/// A fourth row and third column value.
		/// </summary>
		[DataMember]
		public float m43;

		/// <summary>
		/// A fourth row and fourth column value.
		/// </summary>
		[DataMember]
		public float m44;

		#endregion

		#region Indexers

		/// <summary>
		/// Get or set the matrix element at the given index, indexed in row major order.
		/// </summary>
		/// <param name="index">The linearized, zero-based index of the matrix element.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If the index is less than <code>0</code> or larger than <code>15</code>.
		/// </exception>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m11;
					case 1: return m12;
					case 2: return m13;
					case 3: return m14;
					case 4: return m21;
					case 5: return m22;
					case 6: return m23;
					case 7: return m24;
					case 8: return m31;
					case 9: return m32;
					case 10: return m33;
					case 11: return m34;
					case 12: return m41;
					case 13: return m42;
					case 14: return m43;
					case 15: return m44;
				}
				throw new ArgumentOutOfRangeException();
			}

			set
			{
				switch (index)
				{
					case 0: m11 = value; break;
					case 1: m12 = value; break;
					case 2: m13 = value; break;
					case 3: m14 = value; break;
					case 4: m21 = value; break;
					case 5: m22 = value; break;
					case 6: m23 = value; break;
					case 7: m24 = value; break;
					case 8: m31 = value; break;
					case 9: m32 = value; break;
					case 10: m33 = value; break;
					case 11: m34 = value; break;
					case 12: m41 = value; break;
					case 13: m42 = value; break;
					case 14: m43 = value; break;
					case 15: m44 = value; break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Get or set the value at the specified row and column (indices are zero-based).
		/// </summary>
		/// <param name="row">The row of the element.</param>
		/// <param name="column">The column of the element.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// If the row or column is less than <code>0</code> or larger than <code>3</code>.
		/// </exception>
		public float this[int row, int column]
		{
			get
			{
				return this[(row * 4) + column];
			}

			set
			{
				this[(row * 4) + column] = value;
			}
		}

		#endregion

		#region Private Members
		private static Matrix4x4 identity = new Matrix4x4(1f, 0f, 0f, 0f,
																0f, 1f, 0f, 0f,
																0f, 0f, 1f, 0f,
																0f, 0f, 0f, 1f);
		#endregion

		#region Public Properties

		/// <summary>
		/// The backward vector formed from the third row M31, M32, M33 elements.
		/// </summary>
		public Vector3 Backward
		{
			get
			{
				return new Vector3(this.m31, this.m32, this.m33);
			}
			set
			{
				this.m31 = (float)value.x;
				this.m32 = (float)value.y;
				this.m33 = (float)value.z;
			}
		}

		/// <summary>
		/// The down vector formed from the second row -M21, -M22, -M23 elements.
		/// </summary>
		public Vector3 Down
		{
			get
			{
				return new Vector3(-this.m21, -this.m22, -this.m23);
			}
			set
			{
				this.m21 = (float)-value.x;
				this.m22 = (float)-value.y;
				this.m23 = (float)-value.z;
			}
		}

		/// <summary>
		/// The forward vector formed from the third row -M31, -M32, -M33 elements.
		/// </summary>
		public Vector3 Forward
		{
			get
			{
				return new Vector3(-this.m31, -this.m32, -this.m33);
			}
			set
			{
				this.m31 = (float)-value.x;
				this.m32 = (float)-value.y;
				this.m33 = (float)-value.z;
			}
		}

		/// <summary>
		/// Returns the identity matrix.
		/// </summary>
		public static Matrix4x4 Identity
		{
			get { return identity; }
		}

		/// <summary>
		/// The left vector formed from the first row -M11, -M12, -M13 elements.
		/// </summary>
		public Vector3 Left
		{
			get
			{
				return new Vector3(-this.m11, -this.m12, -this.m13);
			}
			set
			{
				this.m11 = (float)-value.x;
				this.m12 = (float)-value.y;
				this.m13 = (float)-value.z;
			}
		}

		/// <summary>
		/// The right vector formed from the first row M11, M12, M13 elements.
		/// </summary>
		public Vector3 Right
		{
			get
			{
				return new Vector3(this.m11, this.m12, this.m13);
			}
			set
			{
				this.m11 = (float)value.x;
				this.m12 = (float)value.y;
				this.m13 = (float)value.z;
			}
		}

		/// <summary>
		/// Position stored in this matrix.
		/// </summary>
		public Vector3 Translation
		{
			get
			{
				return new Vector3(this.m41, this.m42, this.m43);
			}
			set
			{
				this.m41 = (float)value.x;
				this.m42 = (float)value.y;
				this.m43 = (float)value.z;
			}
		}

		/// <summary>
		/// The upper vector formed from the second row M21, M22, M23 elements.
		/// </summary>
		public Vector3 Up
		{
			get
			{
				return new Vector3(this.m21, this.m22, this.m23);
			}
			set
			{
				this.m21 = (float)value.x;
				this.m22 = (float)value.y;
				this.m23 = (float)value.z;
			}
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> which contains sum of two matrixes.
		/// </summary>
		/// <param name="matrix1">The first matrix to add.</param>
		/// <param name="matrix2">The second matrix to add.</param>
		/// <returns>The result of the matrix addition.</returns>
		public static Matrix4x4 Add(Matrix4x4 matrix1, in Matrix4x4 matrix2)
		{
			matrix1.m11 += matrix2.m11;
			matrix1.m12 += matrix2.m12;
			matrix1.m13 += matrix2.m13;
			matrix1.m14 += matrix2.m14;
			matrix1.m21 += matrix2.m21;
			matrix1.m22 += matrix2.m22;
			matrix1.m23 += matrix2.m23;
			matrix1.m24 += matrix2.m24;
			matrix1.m31 += matrix2.m31;
			matrix1.m32 += matrix2.m32;
			matrix1.m33 += matrix2.m33;
			matrix1.m34 += matrix2.m34;
			matrix1.m41 += matrix2.m41;
			matrix1.m42 += matrix2.m42;
			matrix1.m43 += matrix2.m43;
			matrix1.m44 += matrix2.m44;
			return matrix1;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> which contains sum of two matrixes.
		/// </summary>
		/// <param name="matrix1">The first matrix to add.</param>
		/// <param name="matrix2">The second matrix to add.</param>
		/// <param name="result">The result of the matrix addition as an output parameter.</param>
		public static void Add(in Matrix4x4 matrix1, in Matrix4x4 matrix2, out Matrix4x4 result)
		{
			result.m11 = matrix1.m11 + matrix2.m11;
			result.m12 = matrix1.m12 + matrix2.m12;
			result.m13 = matrix1.m13 + matrix2.m13;
			result.m14 = matrix1.m14 + matrix2.m14;
			result.m21 = matrix1.m21 + matrix2.m21;
			result.m22 = matrix1.m22 + matrix2.m22;
			result.m23 = matrix1.m23 + matrix2.m23;
			result.m24 = matrix1.m24 + matrix2.m24;
			result.m31 = matrix1.m31 + matrix2.m31;
			result.m32 = matrix1.m32 + matrix2.m32;
			result.m33 = matrix1.m33 + matrix2.m33;
			result.m34 = matrix1.m34 + matrix2.m34;
			result.m41 = matrix1.m41 + matrix2.m41;
			result.m42 = matrix1.m42 + matrix2.m42;
			result.m43 = matrix1.m43 + matrix2.m43;
			result.m44 = matrix1.m44 + matrix2.m44;

		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> for spherical billboarding that rotates around specified object position.
		/// </summary>
		/// <param name="objectPosition">Position of billboard object. It will rotate around that vector.</param>
		/// <param name="cameraPosition">The camera position.</param>
		/// <param name="cameraUpVector">The camera up vector.</param>
		/// <param name="cameraForwardVector">Optional camera forward vector.</param>
		/// <returns>The <see cref="Matrix4x4"/> for spherical billboarding.</returns>
		public static Matrix4x4 CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition,
			 Vector3 cameraUpVector, Nullable<Vector3> cameraForwardVector)
		{
			Matrix4x4 result;

			// Delegate to the other overload of the function to do the work
			CreateBillboard(ref objectPosition, ref cameraPosition, ref cameraUpVector, cameraForwardVector, out result);

			return result;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> for spherical billboarding that rotates around specified object position.
		/// </summary>
		/// <param name="objectPosition">Position of billboard object. It will rotate around that vector.</param>
		/// <param name="cameraPosition">The camera position.</param>
		/// <param name="cameraUpVector">The camera up vector.</param>
		/// <param name="cameraForwardVector">Optional camera forward vector.</param>
		/// <param name="result">The <see cref="Matrix4x4"/> for spherical billboarding as an output parameter.</param>
		public static void CreateBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
			 ref Vector3 cameraUpVector, Vector3? cameraForwardVector, out Matrix4x4 result)
		{
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			vector.x = objectPosition.x - cameraPosition.x;
			vector.y = objectPosition.y - cameraPosition.y;
			vector.z = objectPosition.z - cameraPosition.z;
			float num = (float)vector.SqrMagnitude;
			if (num < 0.0001f)
			{
				vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.forward;
			}
			else
			{
				Vector3.Multiply(vector, 1f / MathF.Sqrt(num), out vector);
			}
			Vector3.Cross(cameraUpVector, vector, out vector3);
			vector3.Normalize();
			Vector3.Cross(vector, vector3, out vector2);
			result.m11 = (float)vector3.x;
			result.m12 = (float)vector3.y;
			result.m13 = (float)vector3.z;
			result.m14 = 0;
			result.m21 = (float)vector2.x;
			result.m22 = (float)vector2.y;
			result.m23 = (float)vector2.z;
			result.m24 = 0;
			result.m31 = (float)vector.x;
			result.m32 = (float)vector.y;
			result.m33 = (float)vector.z;
			result.m34 = 0;
			result.m41 = (float)objectPosition.x;
			result.m42 = (float)objectPosition.y;
			result.m43 = (float)objectPosition.z;
			result.m44 = 1;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> for cylindrical billboarding that rotates around specified axis.
		/// </summary>
		/// <param name="objectPosition">Object position the billboard will rotate around.</param>
		/// <param name="cameraPosition">Camera position.</param>
		/// <param name="rotateAxis">Axis of billboard for rotation.</param>
		/// <param name="cameraForwardVector">Optional camera forward vector.</param>
		/// <param name="objectForwardVector">Optional object forward vector.</param>
		/// <returns>The <see cref="Matrix4x4"/> for cylindrical billboarding.</returns>
		public static Matrix4x4 CreateConstrainedBillboard(Vector3 objectPosition, Vector3 cameraPosition,
			 Vector3 rotateAxis, Nullable<Vector3> cameraForwardVector, Nullable<Vector3> objectForwardVector)
		{
			Matrix4x4 result;
			CreateConstrainedBillboard(ref objectPosition, ref cameraPosition, ref rotateAxis,
				 cameraForwardVector, objectForwardVector, out result);
			return result;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> for cylindrical billboarding that rotates around specified axis.
		/// </summary>
		/// <param name="objectPosition">Object position the billboard will rotate around.</param>
		/// <param name="cameraPosition">Camera position.</param>
		/// <param name="rotateAxis">Axis of billboard for rotation.</param>
		/// <param name="cameraForwardVector">Optional camera forward vector.</param>
		/// <param name="objectForwardVector">Optional object forward vector.</param>
		/// <param name="result">The <see cref="Matrix4x4"/> for cylindrical billboarding as an output parameter.</param>
		public static void CreateConstrainedBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
			 ref Vector3 rotateAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector, out Matrix4x4 result)
		{
			float num;
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			vector2.x = objectPosition.x - cameraPosition.x;
			vector2.y = objectPosition.y - cameraPosition.y;
			vector2.z = objectPosition.z - cameraPosition.z;
			float num2 = vector2.SqrMagnitude;
			if (num2 < 0.0001f)
			{
				vector2 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.forward;
			}
			else
			{
				Vector3.Multiply(vector2, 1f / MathF.Sqrt(num2), out vector2);
			}
			Vector3 vector4 = rotateAxis;
			Vector3.Dot(rotateAxis, vector2, out num);
			if (MathF.Abs(num) > 0.9982547f)
			{
				if (objectForwardVector.HasValue)
				{
					vector = objectForwardVector.Value;
					Vector3.Dot(rotateAxis, vector, out num);
					if (MathF.Abs(num) > 0.9982547f)
					{
						num = (((rotateAxis.x * Vector3.forward.x) + (rotateAxis.y * Vector3.forward.y)) + (rotateAxis.z * Vector3.forward.z));
						vector = (MathF.Abs(num) > 0.9982547f) ? Vector3.right : Vector3.forward;
					}
				}
				else
				{
					num = ((rotateAxis.x * Vector3.forward.x) + (rotateAxis.y * Vector3.forward.y)) + (rotateAxis.z * Vector3.forward.z);
					vector = (MathF.Abs(num) > 0.9982547f) ? Vector3.right : Vector3.forward;
				}
				Vector3.Cross(rotateAxis, vector, out vector3);
				vector3.Normalize();
				Vector3.Cross(vector3, rotateAxis, out vector);
				vector.Normalize();
			}
			else
			{
				Vector3.Cross(rotateAxis, vector2, out vector3);
				vector3.Normalize();
				Vector3.Cross(vector3, vector4, out vector);
				vector.Normalize();
			}
			result.m11 = (float)vector3.x;
			result.m12 = (float)vector3.y;
			result.m13 = (float)vector3.z;
			result.m14 = 0;
			result.m21 = (float)vector4.x;
			result.m22 = (float)vector4.y;
			result.m23 = (float)vector4.z;
			result.m24 = 0;
			result.m31 = (float)vector.x;
			result.m32 = (float)vector.y;
			result.m33 = (float)vector.z;
			result.m34 = 0;
			result.m41 = (float)objectPosition.x;
			result.m42 = (float)objectPosition.y;
			result.m43 = (float)objectPosition.z;
			result.m44 = 1;

		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> which contains the rotation moment around specified axis.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle of rotation in radians.</param>
		/// <returns>The rotation <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateFromAxisAngle(Vector3 axis, float angle)
		{
			Matrix4x4 result;
			CreateFromAxisAngle(ref axis, angle, out result);
			return result;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> which contains the rotation moment around specified axis.
		/// </summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle of rotation in radians.</param>
		/// <param name="result">The rotation <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix4x4 result)
		{
			float x = (float)axis.x;
			float y = (float)axis.y;
			float z = (float)axis.z;
			float num2 = MathF.Sin(angle);
			float num = MathF.Cos(angle);
			float num11 = x * x;
			float num10 = y * y;
			float num9 = z * z;
			float num8 = x * y;
			float num7 = x * z;
			float num6 = y * z;
			result.m11 = num11 + (num * (1f - num11));
			result.m12 = (num8 - (num * num8)) + (num2 * z);
			result.m13 = (num7 - (num * num7)) - (num2 * y);
			result.m14 = 0;
			result.m21 = (num8 - (num * num8)) - (num2 * z);
			result.m22 = num10 + (num * (1f - num10));
			result.m23 = (num6 - (num * num6)) + (num2 * x);
			result.m24 = 0;
			result.m31 = (num7 - (num * num7)) + (num2 * y);
			result.m32 = (num6 - (num * num6)) - (num2 * x);
			result.m33 = num9 + (num * (1f - num9));
			result.m34 = 0;
			result.m41 = 0;
			result.m42 = 0;
			result.m43 = 0;
			result.m44 = 1;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> from a <see cref="Quaternion"/>.
		/// </summary>
		/// <param name="quaternion"><see cref="Quaternion"/> of rotation moment.</param>
		/// <returns>The rotation <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateFromQuaternion(in Quaternion quaternion)
		{
			Matrix4x4 result;
			CreateFromQuaternion(quaternion, out result);
			return result;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> from a <see cref="Quaternion"/>.
		/// </summary>
		/// <param name="quaternion"><see cref="Quaternion"/> of rotation moment.</param>
		/// <param name="result">The rotation <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateFromQuaternion(in Quaternion quaternion, out Matrix4x4 result)
		{
			float num9 = quaternion.x * quaternion.x;
			float num8 = quaternion.y * quaternion.y;
			float num7 = quaternion.z * quaternion.z;
			float num6 = quaternion.x * quaternion.y;
			float num5 = quaternion.z * quaternion.w;
			float num4 = quaternion.z * quaternion.x;
			float num3 = quaternion.y * quaternion.w;
			float num2 = quaternion.y * quaternion.z;
			float num = quaternion.x * quaternion.w;
			result.m11 = 1f - (2f * (num8 + num7));
			result.m12 = 2f * (num6 + num5);
			result.m13 = 2f * (num4 - num3);
			result.m14 = 0f;
			result.m21 = 2f * (num6 - num5);
			result.m22 = 1f - (2f * (num7 + num9));
			result.m23 = 2f * (num2 + num);
			result.m24 = 0f;
			result.m31 = 2f * (num4 + num3);
			result.m32 = 2f * (num2 - num);
			result.m33 = 1f - (2f * (num8 + num9));
			result.m34 = 0f;
			result.m41 = 0f;
			result.m42 = 0f;
			result.m43 = 0f;
			result.m44 = 1f;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> from the specified yaw, pitch and roll values.
		/// </summary>
		/// <param name="yaw">The yaw rotation value in radians.</param>
		/// <param name="pitch">The pitch rotation value in radians.</param>
		/// <param name="roll">The roll rotation value in radians.</param>
		/// <returns>The rotation <see cref="Matrix4x4"/>.</returns>
		/// <remarks>For more information about yaw, pitch and roll visit http://en.wikipedia.org/wiki/Euler_angles.
		/// </remarks>
		public static Matrix4x4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			Matrix4x4 matrix;
			CreateFromYawPitchRoll(yaw, pitch, roll, out matrix);
			return matrix;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> from the specified yaw, pitch and roll values.
		/// </summary>
		/// <param name="yaw">The yaw rotation value in radians.</param>
		/// <param name="pitch">The pitch rotation value in radians.</param>
		/// <param name="roll">The roll rotation value in radians.</param>
		/// <param name="result">The rotation <see cref="Matrix4x4"/> as an output parameter.</param>
		/// <remarks>For more information about yaw, pitch and roll visit http://en.wikipedia.org/wiki/Euler_angles.
		/// </remarks>
		public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Matrix4x4 result)
		{
			Quaternion quaternion;
			Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
			CreateFromQuaternion(quaternion, out result);
		}

		/// <summary>
		/// Creates a new viewing <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="cameraPosition">Position of the camera.</param>
		/// <param name="cameraTarget">Lookup vector of the camera.</param>
		/// <param name="cameraUpVector">The direction of the upper edge of the camera.</param>
		/// <returns>The viewing <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
		{
			Matrix4x4 matrix;
			CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out matrix);
			return matrix;
		}

		/// <summary>
		/// Creates a new viewing <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="cameraPosition">Position of the camera.</param>
		/// <param name="cameraTarget">Lookup vector of the camera.</param>
		/// <param name="cameraUpVector">The direction of the upper edge of the camera.</param>
		/// <param name="result">The viewing <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix4x4 result)
		{
			var vector = Vector3.Normalize(cameraPosition - cameraTarget);
			var vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
			var vector3 = Vector3.Cross(vector, vector2);
			result.m11 = (float)vector2.x;
			result.m12 = (float)vector3.x;
			result.m13 = (float)vector.x;
			result.m14 = 0f;
			result.m21 = (float)vector2.y;
			result.m22 = (float)vector3.y;
			result.m23 = (float)vector.y;
			result.m24 = 0f;
			result.m31 = (float)vector2.z;
			result.m32 = (float)vector3.z;
			result.m33 = (float)vector.z;
			result.m34 = 0f;
			result.m41 = (float)-Vector3.Dot(vector2, cameraPosition);
			result.m42 = (float)-Vector3.Dot(vector3, cameraPosition);
			result.m43 = (float)-Vector3.Dot(vector, cameraPosition);
			result.m44 = 1f;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for orthographic view.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="zNearPlane">Depth of the near plane.</param>
		/// <param name="zFarPlane">Depth of the far plane.</param>
		/// <returns>The new projection <see cref="Matrix4x4"/> for orthographic view.</returns>
		public static Matrix4x4 CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
		{
			Matrix4x4 matrix;
			CreateOrthographic(width, height, zNearPlane, zFarPlane, out matrix);
			return matrix;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for orthographic view.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="zNearPlane">Depth of the near plane.</param>
		/// <param name="zFarPlane">Depth of the far plane.</param>
		/// <param name="result">The new projection <see cref="Matrix4x4"/> for orthographic view as an output parameter.</param>
		public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix4x4 result)
		{
			result.m11 = 2f / width;
			result.m12 = result.m13 = result.m14 = 0f;
			result.m22 = 2f / height;
			result.m21 = result.m23 = result.m24 = 0f;
			result.m33 = 1f / (zNearPlane - zFarPlane);
			result.m31 = result.m32 = result.m34 = 0f;
			result.m41 = result.m42 = 0f;
			result.m43 = zNearPlane / (zNearPlane - zFarPlane);
			result.m44 = 1f;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for customized orthographic view.
		/// </summary>
		/// <param name="left">Lower x-value at the near plane.</param>
		/// <param name="right">Upper x-value at the near plane.</param>
		/// <param name="bottom">Lower y-coordinate at the near plane.</param>
		/// <param name="top">Upper y-value at the near plane.</param>
		/// <param name="zNearPlane">Depth of the near plane.</param>
		/// <param name="zFarPlane">Depth of the far plane.</param>
		/// <returns>The new projection <see cref="Matrix4x4"/> for customized orthographic view.</returns>
		public static Matrix4x4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			Matrix4x4 matrix;
			CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane, out matrix);
			return matrix;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for customized orthographic view.
		/// </summary>
		/// <param name="viewingVolume">The viewing volume.</param>
		/// <param name="zNearPlane">Depth of the near plane.</param>
		/// <param name="zFarPlane">Depth of the far plane.</param>
		/// <returns>The new projection <see cref="Matrix4x4"/> for customized orthographic view.</returns>
		public static Matrix4x4 CreateOrthographicOffCenter(Rectangle viewingVolume, float zNearPlane, float zFarPlane)
		{
			Matrix4x4 matrix;
			CreateOrthographicOffCenter(viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top, zNearPlane, zFarPlane, out matrix);
			return matrix;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for customized orthographic view.
		/// </summary>
		/// <param name="left">Lower x-value at the near plane.</param>
		/// <param name="right">Upper x-value at the near plane.</param>
		/// <param name="bottom">Lower y-coordinate at the near plane.</param>
		/// <param name="top">Upper y-value at the near plane.</param>
		/// <param name="zNearPlane">Depth of the near plane.</param>
		/// <param name="zFarPlane">Depth of the far plane.</param>
		/// <param name="result">The new projection <see cref="Matrix4x4"/> for customized orthographic view as an output parameter.</param>
		public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out Matrix4x4 result)
		{
			result.m11 = (float)(2.0 / ((float)right - (float)left));
			result.m12 = 0.0f;
			result.m13 = 0.0f;
			result.m14 = 0.0f;
			result.m21 = 0.0f;
			result.m22 = (float)(2.0 / ((float)top - (float)bottom));
			result.m23 = 0.0f;
			result.m24 = 0.0f;
			result.m31 = 0.0f;
			result.m32 = 0.0f;
			result.m33 = (float)(1.0 / ((float)zNearPlane - (float)zFarPlane));
			result.m34 = 0.0f;
			result.m41 = (float)(((float)left + (float)right) / ((float)left - (float)right));
			result.m42 = (float)(((float)top + (float)bottom) / ((float)bottom - (float)top));
			result.m43 = (float)((float)zNearPlane / ((float)zNearPlane - (float)zFarPlane));
			result.m44 = 1.0f;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for perspective view.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="nearPlaneDistance">Distance to the near plane.</param>
		/// <param name="farPlaneDistance">Distance to the far plane.</param>
		/// <returns>The new projection <see cref="Matrix4x4"/> for perspective view.</returns>
		public static Matrix4x4 CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix4x4 matrix;
			CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance, out matrix);
			return matrix;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for perspective view.
		/// </summary>
		/// <param name="width">Width of the viewing volume.</param>
		/// <param name="height">Height of the viewing volume.</param>
		/// <param name="nearPlaneDistance">Distance to the near plane.</param>
		/// <param name="farPlaneDistance">Distance to the far plane, or <see cref="float.PositiveInfinity"/>.</param>
		/// <param name="result">The new projection <see cref="Matrix4x4"/> for perspective view as an output parameter.</param>
		public static void CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance, out Matrix4x4 result)
		{
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentException("nearPlaneDistance <= 0");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentException("farPlaneDistance <= 0");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
			}

			var negFarRange = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

			result.m11 = (2.0f * nearPlaneDistance) / width;
			result.m12 = result.m13 = result.m14 = 0.0f;
			result.m22 = (2.0f * nearPlaneDistance) / height;
			result.m21 = result.m23 = result.m24 = 0.0f;
			result.m33 = negFarRange;
			result.m31 = result.m32 = 0.0f;
			result.m34 = -1.0f;
			result.m41 = result.m42 = result.m44 = 0.0f;
			result.m43 = nearPlaneDistance * negFarRange;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for perspective view with field of view.
		/// </summary>
		/// <param name="fieldOfView">Field of view in the y direction in radians.</param>
		/// <param name="aspectRatio">Width divided by height of the viewing volume.</param>
		/// <param name="nearPlaneDistance">Distance to the near plane.</param>
		/// <param name="farPlaneDistance">Distance to the far plane, or <see cref="float.PositiveInfinity"/>.</param>
		/// <returns>The new projection <see cref="Matrix4x4"/> for perspective view with FOV.</returns>
		public static Matrix4x4 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix4x4 result;
			CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out result);
			return result;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for perspective view with field of view.
		/// </summary>
		/// <param name="fieldOfView">Field of view in the y direction in radians.</param>
		/// <param name="aspectRatio">Width divided by height of the viewing volume.</param>
		/// <param name="nearPlaneDistance">Distance of the near plane.</param>
		/// <param name="farPlaneDistance">Distance of the far plane, or <see cref="float.PositiveInfinity"/>.</param>
		/// <param name="result">The new projection <see cref="Matrix4x4"/> for perspective view with FOV as an output parameter.</param>
		public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix4x4 result)
		{
			if ((fieldOfView <= 0f) || (fieldOfView >= 3.141593f))
			{
				throw new ArgumentException("fieldOfView <= 0 or >= PI");
			}
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentException("nearPlaneDistance <= 0");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentException("farPlaneDistance <= 0");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
			}

			var yScale = 1.0f / (float)Math.Tan((float)fieldOfView * 0.5f);
			var xScale = yScale / aspectRatio;
			var negFarRange = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

			result.m11 = xScale;
			result.m12 = result.m13 = result.m14 = 0.0f;
			result.m22 = yScale;
			result.m21 = result.m23 = result.m24 = 0.0f;
			result.m31 = result.m32 = 0.0f;
			result.m33 = negFarRange;
			result.m34 = -1.0f;
			result.m41 = result.m42 = result.m44 = 0.0f;
			result.m43 = nearPlaneDistance * negFarRange;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for customized perspective view.
		/// </summary>
		/// <param name="left">Lower x-value at the near plane.</param>
		/// <param name="right">Upper x-value at the near plane.</param>
		/// <param name="bottom">Lower y-coordinate at the near plane.</param>
		/// <param name="top">Upper y-value at the near plane.</param>
		/// <param name="nearPlaneDistance">Distance to the near plane.</param>
		/// <param name="farPlaneDistance">Distance to the far plane.</param>
		/// <returns>The new <see cref="Matrix4x4"/> for customized perspective view.</returns>
		public static Matrix4x4 CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix4x4 result;
			CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance, out result);
			return result;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for customized perspective view.
		/// </summary>
		/// <param name="viewingVolume">The viewing volume.</param>
		/// <param name="nearPlaneDistance">Distance to the near plane.</param>
		/// <param name="farPlaneDistance">Distance to the far plane.</param>
		/// <returns>The new <see cref="Matrix4x4"/> for customized perspective view.</returns>
		public static Matrix4x4 CreatePerspectiveOffCenter(Rectangle viewingVolume, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix4x4 result;
			CreatePerspectiveOffCenter(viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top, nearPlaneDistance, farPlaneDistance, out result);
			return result;
		}

		/// <summary>
		/// Creates a new projection <see cref="Matrix4x4"/> for customized perspective view.
		/// </summary>
		/// <param name="left">Lower x-value at the near plane.</param>
		/// <param name="right">Upper x-value at the near plane.</param>
		/// <param name="bottom">Lower y-coordinate at the near plane.</param>
		/// <param name="top">Upper y-value at the near plane.</param>
		/// <param name="nearPlaneDistance">Distance to the near plane.</param>
		/// <param name="farPlaneDistance">Distance to the far plane.</param>
		/// <param name="result">The new <see cref="Matrix4x4"/> for customized perspective view as an output parameter.</param>
		public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance, out Matrix4x4 result)
		{
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentException("nearPlaneDistance <= 0");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentException("farPlaneDistance <= 0");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
			}
			result.m11 = (2f * nearPlaneDistance) / (right - left);
			result.m12 = result.m13 = result.m14 = 0;
			result.m22 = (2f * nearPlaneDistance) / (top - bottom);
			result.m21 = result.m23 = result.m24 = 0;
			result.m31 = (left + right) / (right - left);
			result.m32 = (top + bottom) / (top - bottom);
			result.m33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			result.m34 = -1;
			result.m43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
			result.m41 = result.m42 = result.m44 = 0;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> around X axis.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <returns>The rotation <see cref="Matrix4x4"/> around X axis.</returns>
		public static Matrix4x4 CreateRotationX(float radians)
		{
			Matrix4x4 result;
			CreateRotationX(radians, out result);
			return result;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> around X axis.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <param name="result">The rotation <see cref="Matrix4x4"/> around X axis as an output parameter.</param>
		public static void CreateRotationX(float radians, out Matrix4x4 result)
		{
			result = Matrix4x4.Identity;

			var val1 = MathF.Cos(radians);
			var val2 = MathF.Sin(radians);

			result.m22 = val1;
			result.m23 = val2;
			result.m32 = -val2;
			result.m33 = val1;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> around Y axis.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <returns>The rotation <see cref="Matrix4x4"/> around Y axis.</returns>
		public static Matrix4x4 CreateRotationY(float radians)
		{
			Matrix4x4 result;
			CreateRotationY(radians, out result);
			return result;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> around Y axis.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <param name="result">The rotation <see cref="Matrix4x4"/> around Y axis as an output parameter.</param>
		public static void CreateRotationY(float radians, out Matrix4x4 result)
		{
			result = Matrix4x4.Identity;

			var val1 = MathF.Cos(radians);
			var val2 = MathF.Sin(radians);

			result.m11 = val1;
			result.m13 = -val2;
			result.m31 = val2;
			result.m33 = val1;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> around Z axis.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <returns>The rotation <see cref="Matrix4x4"/> around Z axis.</returns>
		public static Matrix4x4 CreateRotationZ(float radians)
		{
			Matrix4x4 result;
			CreateRotationZ(radians, out result);
			return result;
		}

		/// <summary>
		/// Creates a new rotation <see cref="Matrix4x4"/> around Z axis.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <param name="result">The rotation <see cref="Matrix4x4"/> around Z axis as an output parameter.</param>
		public static void CreateRotationZ(float radians, out Matrix4x4 result)
		{
			result = Matrix4x4.Identity;

			var val1 = MathF.Cos(radians);
			var val2 = MathF.Sin(radians);

			result.m11 = val1;
			result.m12 = val2;
			result.m21 = -val2;
			result.m22 = val1;
		}

		/// <summary>
		/// Creates a new scaling <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="scale">Scale value for all three axises.</param>
		/// <returns>The scaling <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateScale(float scale)
		{
			Matrix4x4 result;
			CreateScale(scale, scale, scale, out result);
			return result;
		}

		/// <summary>
		/// Creates a new scaling <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="scale">Scale value for all three axises.</param>
		/// <param name="result">The scaling <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateScale(float scale, out Matrix4x4 result)
		{
			CreateScale(scale, scale, scale, out result);
		}

		/// <summary>
		/// Creates a new scaling <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="xScale">Scale value for X axis.</param>
		/// <param name="yScale">Scale value for Y axis.</param>
		/// <param name="zScale">Scale value for Z axis.</param>
		/// <returns>The scaling <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateScale(float xScale, float yScale, float zScale)
		{
			Matrix4x4 result;
			CreateScale(xScale, yScale, zScale, out result);
			return result;
		}

		/// <summary>
		/// Creates a new scaling <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="xScale">Scale value for X axis.</param>
		/// <param name="yScale">Scale value for Y axis.</param>
		/// <param name="zScale">Scale value for Z axis.</param>
		/// <param name="result">The scaling <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateScale(float xScale, float yScale, float zScale, out Matrix4x4 result)
		{
			result.m11 = xScale;
			result.m12 = 0;
			result.m13 = 0;
			result.m14 = 0;
			result.m21 = 0;
			result.m22 = yScale;
			result.m23 = 0;
			result.m24 = 0;
			result.m31 = 0;
			result.m32 = 0;
			result.m33 = zScale;
			result.m34 = 0;
			result.m41 = 0;
			result.m42 = 0;
			result.m43 = 0;
			result.m44 = 1;
		}

		/// <summary>
		/// Creates a new scaling <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="scales"><see cref="Vector3"/> representing x,y and z scale values.</param>
		/// <returns>The scaling <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateScale(Vector3 scales)
		{
			Matrix4x4 result;
			CreateScale(ref scales, out result);
			return result;
		}

		/// <summary>
		/// Creates a new scaling <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="scales"><see cref="Vector3"/> representing x,y and z scale values.</param>
		/// <param name="result">The scaling <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateScale(ref Vector3 scales, out Matrix4x4 result)
		{
			result.m11 = (float)scales.x;
			result.m12 = 0;
			result.m13 = 0;
			result.m14 = 0;
			result.m21 = 0;
			result.m22 = (float)scales.y;
			result.m23 = 0;
			result.m24 = 0;
			result.m31 = 0;
			result.m32 = 0;
			result.m33 = (float)scales.z;
			result.m34 = 0;
			result.m41 = 0;
			result.m42 = 0;
			result.m43 = 0;
			result.m44 = 1;
		}


		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that flattens geometry into a specified <see cref="Plane"/> as if casting a shadow from a specified light source. 
		/// </summary>
		/// <param name="lightDirection">A vector specifying the direction from which the light that will cast the shadow is coming.</param>
		/// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
		/// <returns>A <see cref="Matrix4x4"/> that can be used to flatten geometry onto the specified plane from the specified direction. </returns>
		public static Matrix4x4 CreateShadow(Vector3 lightDirection, Plane plane)
		{
			Matrix4x4 result;
			CreateShadow(ref lightDirection, ref plane, out result);
			return result;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that flattens geometry into a specified <see cref="Plane"/> as if casting a shadow from a specified light source. 
		/// </summary>
		/// <param name="lightDirection">A vector specifying the direction from which the light that will cast the shadow is coming.</param>
		/// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
		/// <param name="result">A <see cref="Matrix4x4"/> that can be used to flatten geometry onto the specified plane from the specified direction as an output parameter.</param>
		public static void CreateShadow(ref Vector3 lightDirection, ref Plane plane, out Matrix4x4 result)
		{
			float dot = (float)((plane.normal.x * lightDirection.x) + (plane.normal.y * lightDirection.y) + (plane.normal.z * lightDirection.z));
			float x = (float)-plane.normal.x;
			float y = (float)-plane.normal.y;
			float z = (float)-plane.normal.z;
			float d = (float)-plane.distance;

			result.m11 = (float)((x * lightDirection.x) + dot);
			result.m12 = (float)(x * lightDirection.y);
			result.m13 = (float)(x * lightDirection.z);
			result.m14 = 0;
			result.m21 = (float)(y * lightDirection.x);
			result.m22 = (float)((y * lightDirection.y) + dot);
			result.m23 = (float)(y * lightDirection.z);
			result.m24 = 0;
			result.m31 = (float)(z * lightDirection.x);
			result.m32 = (float)(z * lightDirection.y);
			result.m33 = (float)((z * lightDirection.z) + dot);
			result.m34 = 0;
			result.m41 = (float)(d * lightDirection.x);
			result.m42 = (float)(d * lightDirection.y);
			result.m43 = (float)(d * lightDirection.z);
			result.m44 = (float)dot;
		}

		/// <summary>
		/// Creates a new translation <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="xPosition">X coordinate of translation.</param>
		/// <param name="yPosition">Y coordinate of translation.</param>
		/// <param name="zPosition">Z coordinate of translation.</param>
		/// <returns>The translation <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateTranslation(float xPosition, float yPosition, float zPosition)
		{
			Matrix4x4 result;
			CreateTranslation(xPosition, yPosition, zPosition, out result);
			return result;
		}

		/// <summary>
		/// Creates a new translation <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="position">X,Y and Z coordinates of translation.</param>
		/// <param name="result">The translation <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateTranslation(ref Vector3 position, out Matrix4x4 result)
		{
			result.m11 = 1;
			result.m12 = 0;
			result.m13 = 0;
			result.m14 = 0;
			result.m21 = 0;
			result.m22 = 1;
			result.m23 = 0;
			result.m24 = 0;
			result.m31 = 0;
			result.m32 = 0;
			result.m33 = 1;
			result.m34 = 0;
			result.m41 = (float)position.x;
			result.m42 = (float)position.y;
			result.m43 = (float)position.z;
			result.m44 = 1;
		}

		/// <summary>
		/// Creates a new translation <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="position">X,Y and Z coordinates of translation.</param>
		/// <returns>The translation <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateTranslation(Vector3 position)
		{
			Matrix4x4 result;
			CreateTranslation(ref position, out result);
			return result;
		}

		/// <summary>
		/// Creates a new translation <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="xPosition">X coordinate of translation.</param>
		/// <param name="yPosition">Y coordinate of translation.</param>
		/// <param name="zPosition">Z coordinate of translation.</param>
		/// <param name="result">The translation <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out Matrix4x4 result)
		{
			result.m11 = 1;
			result.m12 = 0;
			result.m13 = 0;
			result.m14 = 0;
			result.m21 = 0;
			result.m22 = 1;
			result.m23 = 0;
			result.m24 = 0;
			result.m31 = 0;
			result.m32 = 0;
			result.m33 = 1;
			result.m34 = 0;
			result.m41 = xPosition;
			result.m42 = yPosition;
			result.m43 = zPosition;
			result.m44 = 1;
		}

		/// <summary>
		/// Creates a new reflection <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="value">The plane that used for reflection calculation.</param>
		/// <returns>The reflection <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateReflection(Plane value)
		{
			Matrix4x4 result;
			CreateReflection(ref value, out result);
			return result;
		}

		/// <summary>
		/// Creates a new reflection <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="value">The plane that used for reflection calculation.</param>
		/// <param name="result">The reflection <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateReflection(ref Plane value, out Matrix4x4 result)
		{
			Plane plane;
			Plane.Normalize(value, out plane);
			float x = (float)plane.normal.x;
			float y = (float)plane.normal.y;
			float z = (float)plane.normal.z;
			float num3 = -2f * x;
			float num2 = -2f * y;
			float num = -2f * z;
			result.m11 = (num3 * x) + 1f;
			result.m12 = num2 * x;
			result.m13 = num * x;
			result.m14 = 0;
			result.m21 = num3 * y;
			result.m22 = (num2 * y) + 1;
			result.m23 = num * y;
			result.m24 = 0;
			result.m31 = num3 * z;
			result.m32 = num2 * z;
			result.m33 = (num * z) + 1;
			result.m34 = 0;
			result.m41 = (float)(num3 * plane.distance);
			result.m42 = (float)(num2 * plane.distance);
			result.m43 = (float)(num * plane.distance);
			result.m44 = 1;
		}

		/// <summary>
		/// Creates a new world <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="position">The position vector.</param>
		/// <param name="forward">The forward direction vector.</param>
		/// <param name="up">The upward direction vector. Usually <see cref="Vector3.up"/>.</param>
		/// <returns>The world <see cref="Matrix4x4"/>.</returns>
		public static Matrix4x4 CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
		{
			Matrix4x4 ret;
			CreateWorld(ref position, ref forward, ref up, out ret);
			return ret;
		}

		/// <summary>
		/// Creates a new world <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="position">The position vector.</param>
		/// <param name="forward">The forward direction vector.</param>
		/// <param name="up">The upward direction vector. Usually <see cref="Vector3.up"/>.</param>
		/// <param name="result">The world <see cref="Matrix4x4"/> as an output parameter.</param>
		public static void CreateWorld(ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix4x4 result)
		{
			Vector3 x, y, z;
			Vector3.Normalize(forward, out z);
			Vector3.Cross(forward, up, out x);
			Vector3.Cross(x, forward, out y);
			x.Normalize();
			y.Normalize();

			result = new Matrix4x4();
			result.Right = x;
			result.Up = y;
			result.Forward = z;
			result.Translation = position;
			result.m44 = 1f;
		}

		/// <summary>
		/// Decomposes this matrix to translation, rotation and scale elements. Returns <c>true</c> if matrix can be decomposed; <c>false</c> otherwise.
		/// </summary>
		/// <param name="scale">Scale vector as an output parameter.</param>
		/// <param name="rotation">Rotation quaternion as an output parameter.</param>
		/// <param name="translation">Translation vector as an output parameter.</param>
		/// <returns><c>true</c> if matrix can be decomposed; <c>false</c> otherwise.</returns>
		public bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
		{
			translation.x = this.m41;
			translation.y = this.m42;
			translation.z = this.m43;

			float xs = (MathF.Sign(m11 * m12 * m13 * m14) < 0) ? -1 : 1;
			float ys = (MathF.Sign(m21 * m22 * m23 * m24) < 0) ? -1 : 1;
			float zs = (MathF.Sign(m31 * m32 * m33 * m34) < 0) ? -1 : 1;

			scale.x = xs * MathF.Sqrt(this.m11 * this.m11 + this.m12 * this.m12 + this.m13 * this.m13);
			scale.y = ys * MathF.Sqrt(this.m21 * this.m21 + this.m22 * this.m22 + this.m23 * this.m23);
			scale.z = zs * MathF.Sqrt(this.m31 * this.m31 + this.m32 * this.m32 + this.m33 * this.m33);

			if (scale.x == 0.0 || scale.y == 0.0 || scale.z == 0.0)
			{
				rotation = Quaternion.Identity;
				return false;
			}

			Matrix4x4 m1 = new Matrix4x4(this.m11 / scale.x, m12 / scale.x, m13 / scale.x, 0,
										  this.m21 / scale.y, m22 / scale.y, m23 / scale.y, 0,
										  this.m31 / scale.z, m32 / scale.z, m33 / scale.z, 0,
										  0, 0, 0, 1);

			rotation = Quaternion.CreateFromRotationMatrix(m1);
			return true;
		}

		/// <summary>
		/// Returns a determinant of this <see cref="Matrix4x4"/>.
		/// </summary>
		/// <returns>Determinant of this <see cref="Matrix4x4"/></returns>
		/// <remarks>See more about determinant here - http://en.wikipedia.org/wiki/Determinant.
		/// </remarks>
		public float Determinant()
		{
			float num22 = this.m11;
			float num21 = this.m12;
			float num20 = this.m13;
			float num19 = this.m14;
			float num12 = this.m21;
			float num11 = this.m22;
			float num10 = this.m23;
			float num9 = this.m24;
			float num8 = this.m31;
			float num7 = this.m32;
			float num6 = this.m33;
			float num5 = this.m34;
			float num4 = this.m41;
			float num3 = this.m42;
			float num2 = this.m43;
			float num = this.m44;
			float num18 = (num6 * num) - (num5 * num2);
			float num17 = (num7 * num) - (num5 * num3);
			float num16 = (num7 * num2) - (num6 * num3);
			float num15 = (num8 * num) - (num5 * num4);
			float num14 = (num8 * num2) - (num6 * num4);
			float num13 = (num8 * num3) - (num7 * num4);
			return ((((num22 * (((num11 * num18) - (num10 * num17)) + (num9 * num16))) - (num21 * (((num12 * num18) - (num10 * num15)) + (num9 * num14)))) + (num20 * (((num12 * num17) - (num11 * num15)) + (num9 * num13)))) - (num19 * (((num12 * num16) - (num11 * num14)) + (num10 * num13))));
		}

		/// <summary>
		/// Divides the elements of a <see cref="Matrix4x4"/> by the elements of another matrix.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">Divisor <see cref="Matrix4x4"/>.</param>
		/// <returns>The result of dividing the matrix.</returns>
		public static Matrix4x4 Divide(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			matrix1.m11 = matrix1.m11 / matrix2.m11;
			matrix1.m12 = matrix1.m12 / matrix2.m12;
			matrix1.m13 = matrix1.m13 / matrix2.m13;
			matrix1.m14 = matrix1.m14 / matrix2.m14;
			matrix1.m21 = matrix1.m21 / matrix2.m21;
			matrix1.m22 = matrix1.m22 / matrix2.m22;
			matrix1.m23 = matrix1.m23 / matrix2.m23;
			matrix1.m24 = matrix1.m24 / matrix2.m24;
			matrix1.m31 = matrix1.m31 / matrix2.m31;
			matrix1.m32 = matrix1.m32 / matrix2.m32;
			matrix1.m33 = matrix1.m33 / matrix2.m33;
			matrix1.m34 = matrix1.m34 / matrix2.m34;
			matrix1.m41 = matrix1.m41 / matrix2.m41;
			matrix1.m42 = matrix1.m42 / matrix2.m42;
			matrix1.m43 = matrix1.m43 / matrix2.m43;
			matrix1.m44 = matrix1.m44 / matrix2.m44;
			return matrix1;
		}

		/// <summary>
		/// Divides the elements of a <see cref="Matrix4x4"/> by the elements of another matrix.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">Divisor <see cref="Matrix4x4"/>.</param>
		/// <param name="result">The result of dividing the matrix as an output parameter.</param>
		public static void Divide(in Matrix4x4 matrix1, in Matrix4x4 matrix2, out Matrix4x4 result)
		{
			result.m11 = matrix1.m11 / matrix2.m11;
			result.m12 = matrix1.m12 / matrix2.m12;
			result.m13 = matrix1.m13 / matrix2.m13;
			result.m14 = matrix1.m14 / matrix2.m14;
			result.m21 = matrix1.m21 / matrix2.m21;
			result.m22 = matrix1.m22 / matrix2.m22;
			result.m23 = matrix1.m23 / matrix2.m23;
			result.m24 = matrix1.m24 / matrix2.m24;
			result.m31 = matrix1.m31 / matrix2.m31;
			result.m32 = matrix1.m32 / matrix2.m32;
			result.m33 = matrix1.m33 / matrix2.m33;
			result.m34 = matrix1.m34 / matrix2.m34;
			result.m41 = matrix1.m41 / matrix2.m41;
			result.m42 = matrix1.m42 / matrix2.m42;
			result.m43 = matrix1.m43 / matrix2.m43;
			result.m44 = matrix1.m44 / matrix2.m44;
		}

		/// <summary>
		/// Divides the elements of a <see cref="Matrix4x4"/> by a scalar.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="divider">Divisor scalar.</param>
		/// <returns>The result of dividing a matrix by a scalar.</returns>
		public static Matrix4x4 Divide(Matrix4x4 matrix1, float divider)
		{
			float num = 1f / divider;
			matrix1.m11 = matrix1.m11 * num;
			matrix1.m12 = matrix1.m12 * num;
			matrix1.m13 = matrix1.m13 * num;
			matrix1.m14 = matrix1.m14 * num;
			matrix1.m21 = matrix1.m21 * num;
			matrix1.m22 = matrix1.m22 * num;
			matrix1.m23 = matrix1.m23 * num;
			matrix1.m24 = matrix1.m24 * num;
			matrix1.m31 = matrix1.m31 * num;
			matrix1.m32 = matrix1.m32 * num;
			matrix1.m33 = matrix1.m33 * num;
			matrix1.m34 = matrix1.m34 * num;
			matrix1.m41 = matrix1.m41 * num;
			matrix1.m42 = matrix1.m42 * num;
			matrix1.m43 = matrix1.m43 * num;
			matrix1.m44 = matrix1.m44 * num;
			return matrix1;
		}

		/// <summary>
		/// Divides the elements of a <see cref="Matrix4x4"/> by a scalar.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="divider">Divisor scalar.</param>
		/// <param name="result">The result of dividing a matrix by a scalar as an output parameter.</param>
		public static void Divide(in Matrix4x4 matrix1, float divider, out Matrix4x4 result)
		{
			float num = 1f / divider;
			result.m11 = matrix1.m11 * num;
			result.m12 = matrix1.m12 * num;
			result.m13 = matrix1.m13 * num;
			result.m14 = matrix1.m14 * num;
			result.m21 = matrix1.m21 * num;
			result.m22 = matrix1.m22 * num;
			result.m23 = matrix1.m23 * num;
			result.m24 = matrix1.m24 * num;
			result.m31 = matrix1.m31 * num;
			result.m32 = matrix1.m32 * num;
			result.m33 = matrix1.m33 * num;
			result.m34 = matrix1.m34 * num;
			result.m41 = matrix1.m41 * num;
			result.m42 = matrix1.m42 * num;
			result.m43 = matrix1.m43 * num;
			result.m44 = matrix1.m44 * num;
		}

		/// <summary>
		/// Compares whether current instance is equal to specified <see cref="Matrix4x4"/> without any tolerance.
		/// </summary>
		/// <param name="other">The <see cref="Matrix4x4"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public bool Equals(Matrix4x4 other)
		{
			return ((((((this.m11 == other.m11) && (this.m22 == other.m22)) && ((this.m33 == other.m33) && (this.m44 == other.m44))) && (((this.m12 == other.m12) && (this.m13 == other.m13)) && ((this.m14 == other.m14) && (this.m21 == other.m21)))) && ((((this.m23 == other.m23) && (this.m24 == other.m24)) && ((this.m31 == other.m31) && (this.m32 == other.m32))) && (((this.m34 == other.m34) && (this.m41 == other.m41)) && (this.m42 == other.m42)))) && (this.m43 == other.m43));
		}

		/// <summary>
		/// Compares whether current instance is equal to specified <see cref="Object"/> without any tolerance.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public override bool Equals(object obj)
		{
			bool flag = false;
			if (obj is Matrix4x4)
			{
				flag = this.Equals((Matrix4x4)obj);
			}
			return flag;
		}

		/// <summary>
		/// Gets the hash code of this <see cref="Matrix4x4"/>.
		/// </summary>
		/// <returns>Hash code of this <see cref="Matrix4x4"/>.</returns>
		public override int GetHashCode()
		{
			return (((((((((((((((this.m11.GetHashCode() + this.m12.GetHashCode()) + this.m13.GetHashCode()) + this.m14.GetHashCode()) + this.m21.GetHashCode()) + this.m22.GetHashCode()) + this.m23.GetHashCode()) + this.m24.GetHashCode()) + this.m31.GetHashCode()) + this.m32.GetHashCode()) + this.m33.GetHashCode()) + this.m34.GetHashCode()) + this.m41.GetHashCode()) + this.m42.GetHashCode()) + this.m43.GetHashCode()) + this.m44.GetHashCode());
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> which contains inversion of the specified matrix. 
		/// </summary>
		/// <param name="matrix">Source <see cref="Matrix4x4"/>.</param>
		/// <returns>The inverted matrix.</returns>
		public static Matrix4x4 Invert(in Matrix4x4 matrix)
		{
			Matrix4x4 result;
			Invert(matrix, out result);
			return result;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> which contains inversion of the specified matrix. 
		/// </summary>
		/// <param name="matrix">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="result">The inverted matrix as output parameter.</param>
		public static void Invert(in Matrix4x4 matrix, out Matrix4x4 result)
		{
			float num1 = matrix.m11;
			float num2 = matrix.m12;
			float num3 = matrix.m13;
			float num4 = matrix.m14;
			float num5 = matrix.m21;
			float num6 = matrix.m22;
			float num7 = matrix.m23;
			float num8 = matrix.m24;
			float num9 = matrix.m31;
			float num10 = matrix.m32;
			float num11 = matrix.m33;
			float num12 = matrix.m34;
			float num13 = matrix.m41;
			float num14 = matrix.m42;
			float num15 = matrix.m43;
			float num16 = matrix.m44;
			float num17 = (float)((float)num11 * (float)num16 - (float)num12 * (float)num15);
			float num18 = (float)((float)num10 * (float)num16 - (float)num12 * (float)num14);
			float num19 = (float)((float)num10 * (float)num15 - (float)num11 * (float)num14);
			float num20 = (float)((float)num9 * (float)num16 - (float)num12 * (float)num13);
			float num21 = (float)((float)num9 * (float)num15 - (float)num11 * (float)num13);
			float num22 = (float)((float)num9 * (float)num14 - (float)num10 * (float)num13);
			float num23 = (float)((float)num6 * (float)num17 - (float)num7 * (float)num18 + (float)num8 * (float)num19);
			float num24 = (float)-((float)num5 * (float)num17 - (float)num7 * (float)num20 + (float)num8 * (float)num21);
			float num25 = (float)((float)num5 * (float)num18 - (float)num6 * (float)num20 + (float)num8 * (float)num22);
			float num26 = (float)-((float)num5 * (float)num19 - (float)num6 * (float)num21 + (float)num7 * (float)num22);
			float num27 = (float)(1.0 / ((float)num1 * (float)num23 + (float)num2 * (float)num24 + (float)num3 * (float)num25 + (float)num4 * (float)num26));

			result.m11 = num23 * num27;
			result.m21 = num24 * num27;
			result.m31 = num25 * num27;
			result.m41 = num26 * num27;
			result.m12 = (float)-((float)num2 * (float)num17 - (float)num3 * (float)num18 + (float)num4 * (float)num19) * num27;
			result.m22 = (float)((float)num1 * (float)num17 - (float)num3 * (float)num20 + (float)num4 * (float)num21) * num27;
			result.m32 = (float)-((float)num1 * (float)num18 - (float)num2 * (float)num20 + (float)num4 * (float)num22) * num27;
			result.m42 = (float)((float)num1 * (float)num19 - (float)num2 * (float)num21 + (float)num3 * (float)num22) * num27;
			float num28 = (float)((float)num7 * (float)num16 - (float)num8 * (float)num15);
			float num29 = (float)((float)num6 * (float)num16 - (float)num8 * (float)num14);
			float num30 = (float)((float)num6 * (float)num15 - (float)num7 * (float)num14);
			float num31 = (float)((float)num5 * (float)num16 - (float)num8 * (float)num13);
			float num32 = (float)((float)num5 * (float)num15 - (float)num7 * (float)num13);
			float num33 = (float)((float)num5 * (float)num14 - (float)num6 * (float)num13);
			result.m13 = (float)((float)num2 * (float)num28 - (float)num3 * (float)num29 + (float)num4 * (float)num30) * num27;
			result.m23 = (float)-((float)num1 * (float)num28 - (float)num3 * (float)num31 + (float)num4 * (float)num32) * num27;
			result.m33 = (float)((float)num1 * (float)num29 - (float)num2 * (float)num31 + (float)num4 * (float)num33) * num27;
			result.m43 = (float)-((float)num1 * (float)num30 - (float)num2 * (float)num32 + (float)num3 * (float)num33) * num27;
			float num34 = (float)((float)num7 * (float)num12 - (float)num8 * (float)num11);
			float num35 = (float)((float)num6 * (float)num12 - (float)num8 * (float)num10);
			float num36 = (float)((float)num6 * (float)num11 - (float)num7 * (float)num10);
			float num37 = (float)((float)num5 * (float)num12 - (float)num8 * (float)num9);
			float num38 = (float)((float)num5 * (float)num11 - (float)num7 * (float)num9);
			float num39 = (float)((float)num5 * (float)num10 - (float)num6 * (float)num9);
			result.m14 = (float)-((float)num2 * (float)num34 - (float)num3 * (float)num35 + (float)num4 * (float)num36) * num27;
			result.m24 = (float)((float)num1 * (float)num34 - (float)num3 * (float)num37 + (float)num4 * (float)num38) * num27;
			result.m34 = (float)-((float)num1 * (float)num35 - (float)num2 * (float)num37 + (float)num4 * (float)num39) * num27;
			result.m44 = (float)((float)num1 * (float)num36 - (float)num2 * (float)num38 + (float)num3 * (float)num39) * num27;


			/*
			
			
            ///
            // Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
            // 
            // 1. Calculate the 2x2 determinants needed the 4x4 determinant based on the 2x2 determinants 
            // 3. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
            // 4. Divide adjugate matrix with the determinant to find the inverse
            
            float det1, det2, det3, det4, det5, det6, det7, det8, det9, det10, det11, det12;
            float detMatrix;
            FindDeterminants(ref matrix, out detMatrix, out det1, out det2, out det3, out det4, out det5, out det6, 
                             out det7, out det8, out det9, out det10, out det11, out det12);
            
            float invDetMatrix = 1f / detMatrix;
            
            Matrix ret; // Allow for matrix and result to point to the same structure
            
            ret.M11 = (matrix.M22*det12 - matrix.M23*det11 + matrix.M24*det10) * invDetMatrix;
            ret.M12 = (-matrix.M12*det12 + matrix.M13*det11 - matrix.M14*det10) * invDetMatrix;
            ret.M13 = (matrix.M42*det6 - matrix.M43*det5 + matrix.M44*det4) * invDetMatrix;
            ret.M14 = (-matrix.M32*det6 + matrix.M33*det5 - matrix.M34*det4) * invDetMatrix;
            ret.M21 = (-matrix.M21*det12 + matrix.M23*det9 - matrix.M24*det8) * invDetMatrix;
            ret.M22 = (matrix.M11*det12 - matrix.M13*det9 + matrix.M14*det8) * invDetMatrix;
            ret.M23 = (-matrix.M41*det6 + matrix.M43*det3 - matrix.M44*det2) * invDetMatrix;
            ret.M24 = (matrix.M31*det6 - matrix.M33*det3 + matrix.M34*det2) * invDetMatrix;
            ret.M31 = (matrix.M21*det11 - matrix.M22*det9 + matrix.M24*det7) * invDetMatrix;
            ret.M32 = (-matrix.M11*det11 + matrix.M12*det9 - matrix.M14*det7) * invDetMatrix;
            ret.M33 = (matrix.M41*det5 - matrix.M42*det3 + matrix.M44*det1) * invDetMatrix;
            ret.M34 = (-matrix.M31*det5 + matrix.M32*det3 - matrix.M34*det1) * invDetMatrix;
            ret.M41 = (-matrix.M21*det10 + matrix.M22*det8 - matrix.M23*det7) * invDetMatrix;
            ret.M42 = (matrix.M11*det10 - matrix.M12*det8 + matrix.M13*det7) * invDetMatrix;
            ret.M43 = (-matrix.M41*det4 + matrix.M42*det2 - matrix.M43*det1) * invDetMatrix;
            ret.M44 = (matrix.M31*det4 - matrix.M32*det2 + matrix.M33*det1) * invDetMatrix;
            
            result = ret;
            */
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains linear interpolation of the values in specified matrixes.
		/// </summary>
		/// <param name="matrix1">The first <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">The second <see cref="Vector2"/>.</param>
		/// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
		/// <returns>>The result of linear interpolation of the specified matrixes.</returns>
		public static Matrix4x4 Lerp(Matrix4x4 matrix1, Matrix4x4 matrix2, float amount)
		{
			matrix1.m11 = matrix1.m11 + ((matrix2.m11 - matrix1.m11) * amount);
			matrix1.m12 = matrix1.m12 + ((matrix2.m12 - matrix1.m12) * amount);
			matrix1.m13 = matrix1.m13 + ((matrix2.m13 - matrix1.m13) * amount);
			matrix1.m14 = matrix1.m14 + ((matrix2.m14 - matrix1.m14) * amount);
			matrix1.m21 = matrix1.m21 + ((matrix2.m21 - matrix1.m21) * amount);
			matrix1.m22 = matrix1.m22 + ((matrix2.m22 - matrix1.m22) * amount);
			matrix1.m23 = matrix1.m23 + ((matrix2.m23 - matrix1.m23) * amount);
			matrix1.m24 = matrix1.m24 + ((matrix2.m24 - matrix1.m24) * amount);
			matrix1.m31 = matrix1.m31 + ((matrix2.m31 - matrix1.m31) * amount);
			matrix1.m32 = matrix1.m32 + ((matrix2.m32 - matrix1.m32) * amount);
			matrix1.m33 = matrix1.m33 + ((matrix2.m33 - matrix1.m33) * amount);
			matrix1.m34 = matrix1.m34 + ((matrix2.m34 - matrix1.m34) * amount);
			matrix1.m41 = matrix1.m41 + ((matrix2.m41 - matrix1.m41) * amount);
			matrix1.m42 = matrix1.m42 + ((matrix2.m42 - matrix1.m42) * amount);
			matrix1.m43 = matrix1.m43 + ((matrix2.m43 - matrix1.m43) * amount);
			matrix1.m44 = matrix1.m44 + ((matrix2.m44 - matrix1.m44) * amount);
			return matrix1;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains linear interpolation of the values in specified matrixes.
		/// </summary>
		/// <param name="matrix1">The first <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">The second <see cref="Vector2"/>.</param>
		/// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
		/// <param name="result">The result of linear interpolation of the specified matrixes as an output parameter.</param>
		public static void Lerp(in Matrix4x4 matrix1, in Matrix4x4 matrix2, float amount, out Matrix4x4 result)
		{
			result.m11 = matrix1.m11 + ((matrix2.m11 - matrix1.m11) * amount);
			result.m12 = matrix1.m12 + ((matrix2.m12 - matrix1.m12) * amount);
			result.m13 = matrix1.m13 + ((matrix2.m13 - matrix1.m13) * amount);
			result.m14 = matrix1.m14 + ((matrix2.m14 - matrix1.m14) * amount);
			result.m21 = matrix1.m21 + ((matrix2.m21 - matrix1.m21) * amount);
			result.m22 = matrix1.m22 + ((matrix2.m22 - matrix1.m22) * amount);
			result.m23 = matrix1.m23 + ((matrix2.m23 - matrix1.m23) * amount);
			result.m24 = matrix1.m24 + ((matrix2.m24 - matrix1.m24) * amount);
			result.m31 = matrix1.m31 + ((matrix2.m31 - matrix1.m31) * amount);
			result.m32 = matrix1.m32 + ((matrix2.m32 - matrix1.m32) * amount);
			result.m33 = matrix1.m33 + ((matrix2.m33 - matrix1.m33) * amount);
			result.m34 = matrix1.m34 + ((matrix2.m34 - matrix1.m34) * amount);
			result.m41 = matrix1.m41 + ((matrix2.m41 - matrix1.m41) * amount);
			result.m42 = matrix1.m42 + ((matrix2.m42 - matrix1.m42) * amount);
			result.m43 = matrix1.m43 + ((matrix2.m43 - matrix1.m43) * amount);
			result.m44 = matrix1.m44 + ((matrix2.m44 - matrix1.m44) * amount);
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains a multiplication of two matrix.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">Source <see cref="Matrix4x4"/>.</param>
		/// <returns>Result of the matrix multiplication.</returns>
		public static Matrix4x4 Multiply(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			var m11 = (((matrix1.m11 * matrix2.m11) + (matrix1.m12 * matrix2.m21)) + (matrix1.m13 * matrix2.m31)) + (matrix1.m14 * matrix2.m41);
			var m12 = (((matrix1.m11 * matrix2.m12) + (matrix1.m12 * matrix2.m22)) + (matrix1.m13 * matrix2.m32)) + (matrix1.m14 * matrix2.m42);
			var m13 = (((matrix1.m11 * matrix2.m13) + (matrix1.m12 * matrix2.m23)) + (matrix1.m13 * matrix2.m33)) + (matrix1.m14 * matrix2.m43);
			var m14 = (((matrix1.m11 * matrix2.m14) + (matrix1.m12 * matrix2.m24)) + (matrix1.m13 * matrix2.m34)) + (matrix1.m14 * matrix2.m44);
			var m21 = (((matrix1.m21 * matrix2.m11) + (matrix1.m22 * matrix2.m21)) + (matrix1.m23 * matrix2.m31)) + (matrix1.m24 * matrix2.m41);
			var m22 = (((matrix1.m21 * matrix2.m12) + (matrix1.m22 * matrix2.m22)) + (matrix1.m23 * matrix2.m32)) + (matrix1.m24 * matrix2.m42);
			var m23 = (((matrix1.m21 * matrix2.m13) + (matrix1.m22 * matrix2.m23)) + (matrix1.m23 * matrix2.m33)) + (matrix1.m24 * matrix2.m43);
			var m24 = (((matrix1.m21 * matrix2.m14) + (matrix1.m22 * matrix2.m24)) + (matrix1.m23 * matrix2.m34)) + (matrix1.m24 * matrix2.m44);
			var m31 = (((matrix1.m31 * matrix2.m11) + (matrix1.m32 * matrix2.m21)) + (matrix1.m33 * matrix2.m31)) + (matrix1.m34 * matrix2.m41);
			var m32 = (((matrix1.m31 * matrix2.m12) + (matrix1.m32 * matrix2.m22)) + (matrix1.m33 * matrix2.m32)) + (matrix1.m34 * matrix2.m42);
			var m33 = (((matrix1.m31 * matrix2.m13) + (matrix1.m32 * matrix2.m23)) + (matrix1.m33 * matrix2.m33)) + (matrix1.m34 * matrix2.m43);
			var m34 = (((matrix1.m31 * matrix2.m14) + (matrix1.m32 * matrix2.m24)) + (matrix1.m33 * matrix2.m34)) + (matrix1.m34 * matrix2.m44);
			var m41 = (((matrix1.m41 * matrix2.m11) + (matrix1.m42 * matrix2.m21)) + (matrix1.m43 * matrix2.m31)) + (matrix1.m44 * matrix2.m41);
			var m42 = (((matrix1.m41 * matrix2.m12) + (matrix1.m42 * matrix2.m22)) + (matrix1.m43 * matrix2.m32)) + (matrix1.m44 * matrix2.m42);
			var m43 = (((matrix1.m41 * matrix2.m13) + (matrix1.m42 * matrix2.m23)) + (matrix1.m43 * matrix2.m33)) + (matrix1.m44 * matrix2.m43);
			var m44 = (((matrix1.m41 * matrix2.m14) + (matrix1.m42 * matrix2.m24)) + (matrix1.m43 * matrix2.m34)) + (matrix1.m44 * matrix2.m44);
			matrix1.m11 = m11;
			matrix1.m12 = m12;
			matrix1.m13 = m13;
			matrix1.m14 = m14;
			matrix1.m21 = m21;
			matrix1.m22 = m22;
			matrix1.m23 = m23;
			matrix1.m24 = m24;
			matrix1.m31 = m31;
			matrix1.m32 = m32;
			matrix1.m33 = m33;
			matrix1.m34 = m34;
			matrix1.m41 = m41;
			matrix1.m42 = m42;
			matrix1.m43 = m43;
			matrix1.m44 = m44;
			return matrix1;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains a multiplication of two matrix.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="result">Result of the matrix multiplication as an output parameter.</param>
		public static void Multiply(in Matrix4x4 matrix1, in Matrix4x4 matrix2, out Matrix4x4 result)
		{
			var m11 = (((matrix1.m11 * matrix2.m11) + (matrix1.m12 * matrix2.m21)) + (matrix1.m13 * matrix2.m31)) + (matrix1.m14 * matrix2.m41);
			var m12 = (((matrix1.m11 * matrix2.m12) + (matrix1.m12 * matrix2.m22)) + (matrix1.m13 * matrix2.m32)) + (matrix1.m14 * matrix2.m42);
			var m13 = (((matrix1.m11 * matrix2.m13) + (matrix1.m12 * matrix2.m23)) + (matrix1.m13 * matrix2.m33)) + (matrix1.m14 * matrix2.m43);
			var m14 = (((matrix1.m11 * matrix2.m14) + (matrix1.m12 * matrix2.m24)) + (matrix1.m13 * matrix2.m34)) + (matrix1.m14 * matrix2.m44);
			var m21 = (((matrix1.m21 * matrix2.m11) + (matrix1.m22 * matrix2.m21)) + (matrix1.m23 * matrix2.m31)) + (matrix1.m24 * matrix2.m41);
			var m22 = (((matrix1.m21 * matrix2.m12) + (matrix1.m22 * matrix2.m22)) + (matrix1.m23 * matrix2.m32)) + (matrix1.m24 * matrix2.m42);
			var m23 = (((matrix1.m21 * matrix2.m13) + (matrix1.m22 * matrix2.m23)) + (matrix1.m23 * matrix2.m33)) + (matrix1.m24 * matrix2.m43);
			var m24 = (((matrix1.m21 * matrix2.m14) + (matrix1.m22 * matrix2.m24)) + (matrix1.m23 * matrix2.m34)) + (matrix1.m24 * matrix2.m44);
			var m31 = (((matrix1.m31 * matrix2.m11) + (matrix1.m32 * matrix2.m21)) + (matrix1.m33 * matrix2.m31)) + (matrix1.m34 * matrix2.m41);
			var m32 = (((matrix1.m31 * matrix2.m12) + (matrix1.m32 * matrix2.m22)) + (matrix1.m33 * matrix2.m32)) + (matrix1.m34 * matrix2.m42);
			var m33 = (((matrix1.m31 * matrix2.m13) + (matrix1.m32 * matrix2.m23)) + (matrix1.m33 * matrix2.m33)) + (matrix1.m34 * matrix2.m43);
			var m34 = (((matrix1.m31 * matrix2.m14) + (matrix1.m32 * matrix2.m24)) + (matrix1.m33 * matrix2.m34)) + (matrix1.m34 * matrix2.m44);
			var m41 = (((matrix1.m41 * matrix2.m11) + (matrix1.m42 * matrix2.m21)) + (matrix1.m43 * matrix2.m31)) + (matrix1.m44 * matrix2.m41);
			var m42 = (((matrix1.m41 * matrix2.m12) + (matrix1.m42 * matrix2.m22)) + (matrix1.m43 * matrix2.m32)) + (matrix1.m44 * matrix2.m42);
			var m43 = (((matrix1.m41 * matrix2.m13) + (matrix1.m42 * matrix2.m23)) + (matrix1.m43 * matrix2.m33)) + (matrix1.m44 * matrix2.m43);
			var m44 = (((matrix1.m41 * matrix2.m14) + (matrix1.m42 * matrix2.m24)) + (matrix1.m43 * matrix2.m34)) + (matrix1.m44 * matrix2.m44);
			result.m11 = m11;
			result.m12 = m12;
			result.m13 = m13;
			result.m14 = m14;
			result.m21 = m21;
			result.m22 = m22;
			result.m23 = m23;
			result.m24 = m24;
			result.m31 = m31;
			result.m32 = m32;
			result.m33 = m33;
			result.m34 = m34;
			result.m41 = m41;
			result.m42 = m42;
			result.m43 = m43;
			result.m44 = m44;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains a multiplication of <see cref="Matrix4x4"/> and a scalar.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <returns>Result of the matrix multiplication with a scalar.</returns>
		public static Matrix4x4 Multiply(Matrix4x4 matrix1, float scaleFactor)
		{
			matrix1.m11 *= scaleFactor;
			matrix1.m12 *= scaleFactor;
			matrix1.m13 *= scaleFactor;
			matrix1.m14 *= scaleFactor;
			matrix1.m21 *= scaleFactor;
			matrix1.m22 *= scaleFactor;
			matrix1.m23 *= scaleFactor;
			matrix1.m24 *= scaleFactor;
			matrix1.m31 *= scaleFactor;
			matrix1.m32 *= scaleFactor;
			matrix1.m33 *= scaleFactor;
			matrix1.m34 *= scaleFactor;
			matrix1.m41 *= scaleFactor;
			matrix1.m42 *= scaleFactor;
			matrix1.m43 *= scaleFactor;
			matrix1.m44 *= scaleFactor;
			return matrix1;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains a multiplication of <see cref="Matrix4x4"/> and a scalar.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <param name="result">Result of the matrix multiplication with a scalar as an output parameter.</param>
		public static void Multiply(in Matrix4x4 matrix1, float scaleFactor, out Matrix4x4 result)
		{
			result.m11 = matrix1.m11 * scaleFactor;
			result.m12 = matrix1.m12 * scaleFactor;
			result.m13 = matrix1.m13 * scaleFactor;
			result.m14 = matrix1.m14 * scaleFactor;
			result.m21 = matrix1.m21 * scaleFactor;
			result.m22 = matrix1.m22 * scaleFactor;
			result.m23 = matrix1.m23 * scaleFactor;
			result.m24 = matrix1.m24 * scaleFactor;
			result.m31 = matrix1.m31 * scaleFactor;
			result.m32 = matrix1.m32 * scaleFactor;
			result.m33 = matrix1.m33 * scaleFactor;
			result.m34 = matrix1.m34 * scaleFactor;
			result.m41 = matrix1.m41 * scaleFactor;
			result.m42 = matrix1.m42 * scaleFactor;
			result.m43 = matrix1.m43 * scaleFactor;
			result.m44 = matrix1.m44 * scaleFactor;

		}

		/// <summary>
		/// Copy the values of specified <see cref="Matrix4x4"/> to the float array.
		/// </summary>
		/// <param name="matrix">The source <see cref="Matrix4x4"/>.</param>
		/// <returns>The array which matrix values will be stored.</returns>
		/// <remarks>
		/// Required for OpenGL 2.0 projection matrix stuff.
		/// </remarks>
		public static float[] ToFloatArray(Matrix4x4 matrix)
		{
			float[] matarray = {
									matrix.m11, matrix.m12, matrix.m13, matrix.m14,
									matrix.m21, matrix.m22, matrix.m23, matrix.m24,
									matrix.m31, matrix.m32, matrix.m33, matrix.m34,
									matrix.m41, matrix.m42, matrix.m43, matrix.m44
								};
			return matarray;
		}

		/// <summary>
		/// Returns a matrix with the all values negated.
		/// </summary>
		/// <param name="matrix">Source <see cref="Matrix4x4"/>.</param>
		/// <returns>Result of the matrix negation.</returns>
		public static Matrix4x4 Negate(Matrix4x4 matrix)
		{
			matrix.m11 = -matrix.m11;
			matrix.m12 = -matrix.m12;
			matrix.m13 = -matrix.m13;
			matrix.m14 = -matrix.m14;
			matrix.m21 = -matrix.m21;
			matrix.m22 = -matrix.m22;
			matrix.m23 = -matrix.m23;
			matrix.m24 = -matrix.m24;
			matrix.m31 = -matrix.m31;
			matrix.m32 = -matrix.m32;
			matrix.m33 = -matrix.m33;
			matrix.m34 = -matrix.m34;
			matrix.m41 = -matrix.m41;
			matrix.m42 = -matrix.m42;
			matrix.m43 = -matrix.m43;
			matrix.m44 = -matrix.m44;
			return matrix;
		}

		/// <summary>
		/// Returns a matrix with the all values negated.
		/// </summary>
		/// <param name="matrix">Source <see cref="Matrix4x4"/>.</param>
		/// <param name="result">Result of the matrix negation as an output parameter.</param>
		public static void Negate(in Matrix4x4 matrix, out Matrix4x4 result)
		{
			result.m11 = -matrix.m11;
			result.m12 = -matrix.m12;
			result.m13 = -matrix.m13;
			result.m14 = -matrix.m14;
			result.m21 = -matrix.m21;
			result.m22 = -matrix.m22;
			result.m23 = -matrix.m23;
			result.m24 = -matrix.m24;
			result.m31 = -matrix.m31;
			result.m32 = -matrix.m32;
			result.m33 = -matrix.m33;
			result.m34 = -matrix.m34;
			result.m41 = -matrix.m41;
			result.m42 = -matrix.m42;
			result.m43 = -matrix.m43;
			result.m44 = -matrix.m44;
		}

		///// <summary>
		///// Converts a <see cref="System.Numerics.Matrix4x4"/> to a <see cref="Matrix"/>.
		///// </summary>
		///// <param name="value">The converted value.</param>
		//public static implicit operator Matrix(System.Numerics.Matrix4x4 value)
		//{
		//	return new Matrix(
		//		 value.M11, value.M12, value.M13, value.M14,
		//		 value.M21, value.M22, value.M23, value.M24,
		//		 value.M31, value.M32, value.M33, value.M34,
		//		 value.M41, value.M42, value.M43, value.M44);
		//}

		/// <summary>
		/// Adds two matrixes.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix"/> on the left of the add sign.</param>
		/// <param name="matrix2">Source <see cref="Matrix"/> on the right of the add sign.</param>
		/// <returns>Sum of the matrixes.</returns>
		public static Matrix4x4 operator +(Matrix4x4 matrix1, in Matrix4x4 matrix2)
		{
			matrix1.m11 = matrix1.m11 + matrix2.m11;
			matrix1.m12 = matrix1.m12 + matrix2.m12;
			matrix1.m13 = matrix1.m13 + matrix2.m13;
			matrix1.m14 = matrix1.m14 + matrix2.m14;
			matrix1.m21 = matrix1.m21 + matrix2.m21;
			matrix1.m22 = matrix1.m22 + matrix2.m22;
			matrix1.m23 = matrix1.m23 + matrix2.m23;
			matrix1.m24 = matrix1.m24 + matrix2.m24;
			matrix1.m31 = matrix1.m31 + matrix2.m31;
			matrix1.m32 = matrix1.m32 + matrix2.m32;
			matrix1.m33 = matrix1.m33 + matrix2.m33;
			matrix1.m34 = matrix1.m34 + matrix2.m34;
			matrix1.m41 = matrix1.m41 + matrix2.m41;
			matrix1.m42 = matrix1.m42 + matrix2.m42;
			matrix1.m43 = matrix1.m43 + matrix2.m43;
			matrix1.m44 = matrix1.m44 + matrix2.m44;
			return matrix1;
		}

		/// <summary>
		/// Divides the elements of a <see cref="Matrix4x4"/> by the elements of another <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/> on the left of the div sign.</param>
		/// <param name="matrix2">Divisor <see cref="Matrix4x4"/> on the right of the div sign.</param>
		/// <returns>The result of dividing the matrixes.</returns>
		public static Matrix4x4 operator /(Matrix4x4 matrix1, in Matrix4x4 matrix2)
		{
			matrix1.m11 = matrix1.m11 / matrix2.m11;
			matrix1.m12 = matrix1.m12 / matrix2.m12;
			matrix1.m13 = matrix1.m13 / matrix2.m13;
			matrix1.m14 = matrix1.m14 / matrix2.m14;
			matrix1.m21 = matrix1.m21 / matrix2.m21;
			matrix1.m22 = matrix1.m22 / matrix2.m22;
			matrix1.m23 = matrix1.m23 / matrix2.m23;
			matrix1.m24 = matrix1.m24 / matrix2.m24;
			matrix1.m31 = matrix1.m31 / matrix2.m31;
			matrix1.m32 = matrix1.m32 / matrix2.m32;
			matrix1.m33 = matrix1.m33 / matrix2.m33;
			matrix1.m34 = matrix1.m34 / matrix2.m34;
			matrix1.m41 = matrix1.m41 / matrix2.m41;
			matrix1.m42 = matrix1.m42 / matrix2.m42;
			matrix1.m43 = matrix1.m43 / matrix2.m43;
			matrix1.m44 = matrix1.m44 / matrix2.m44;
			return matrix1;
		}

		/// <summary>
		/// Divides the elements of a <see cref="Matrix4x4"/> by a scalar.
		/// </summary>
		/// <param name="matrix">Source <see cref="Matrix4x4"/> on the left of the div sign.</param>
		/// <param name="divider">Divisor scalar on the right of the div sign.</param>
		/// <returns>The result of dividing a matrix by a scalar.</returns>
		public static Matrix4x4 operator /(Matrix4x4 matrix, float divider)
		{
			float num = 1f / divider;
			matrix.m11 = matrix.m11 * num;
			matrix.m12 = matrix.m12 * num;
			matrix.m13 = matrix.m13 * num;
			matrix.m14 = matrix.m14 * num;
			matrix.m21 = matrix.m21 * num;
			matrix.m22 = matrix.m22 * num;
			matrix.m23 = matrix.m23 * num;
			matrix.m24 = matrix.m24 * num;
			matrix.m31 = matrix.m31 * num;
			matrix.m32 = matrix.m32 * num;
			matrix.m33 = matrix.m33 * num;
			matrix.m34 = matrix.m34 * num;
			matrix.m41 = matrix.m41 * num;
			matrix.m42 = matrix.m42 * num;
			matrix.m43 = matrix.m43 * num;
			matrix.m44 = matrix.m44 * num;
			return matrix;
		}

		/// <summary>
		/// Compares whether two <see cref="Matrix4x4"/> instances are equal without any tolerance.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/> on the left of the equal sign.</param>
		/// <param name="matrix2">Source <see cref="Matrix4x4"/> on the right of the equal sign.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public static bool operator ==(in Matrix4x4 matrix1, in Matrix4x4 matrix2)
		{
			return (
				 matrix1.m11 == matrix2.m11 &&
				 matrix1.m12 == matrix2.m12 &&
				 matrix1.m13 == matrix2.m13 &&
				 matrix1.m14 == matrix2.m14 &&
				 matrix1.m21 == matrix2.m21 &&
				 matrix1.m22 == matrix2.m22 &&
				 matrix1.m23 == matrix2.m23 &&
				 matrix1.m24 == matrix2.m24 &&
				 matrix1.m31 == matrix2.m31 &&
				 matrix1.m32 == matrix2.m32 &&
				 matrix1.m33 == matrix2.m33 &&
				 matrix1.m34 == matrix2.m34 &&
				 matrix1.m41 == matrix2.m41 &&
				 matrix1.m42 == matrix2.m42 &&
				 matrix1.m43 == matrix2.m43 &&
				 matrix1.m44 == matrix2.m44
				 );
		}

		/// <summary>
		/// Compares whether two <see cref="Matrix4x4"/> instances are not equal without any tolerance.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/> on the left of the not equal sign.</param>
		/// <param name="matrix2">Source <see cref="Matrix4x4"/> on the right of the not equal sign.</param>
		/// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
		public static bool operator !=(in Matrix4x4 matrix1, in Matrix4x4 matrix2)
		{
			return (
				 matrix1.m11 != matrix2.m11 ||
				 matrix1.m12 != matrix2.m12 ||
				 matrix1.m13 != matrix2.m13 ||
				 matrix1.m14 != matrix2.m14 ||
				 matrix1.m21 != matrix2.m21 ||
				 matrix1.m22 != matrix2.m22 ||
				 matrix1.m23 != matrix2.m23 ||
				 matrix1.m24 != matrix2.m24 ||
				 matrix1.m31 != matrix2.m31 ||
				 matrix1.m32 != matrix2.m32 ||
				 matrix1.m33 != matrix2.m33 ||
				 matrix1.m34 != matrix2.m34 ||
				 matrix1.m41 != matrix2.m41 ||
				 matrix1.m42 != matrix2.m42 ||
				 matrix1.m43 != matrix2.m43 ||
				 matrix1.m44 != matrix2.m44
				 );
		}

		/// <summary>
		/// Multiplies two matrixes.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/> on the left of the mul sign.</param>
		/// <param name="matrix2">Source <see cref="Matrix4x4"/> on the right of the mul sign.</param>
		/// <returns>Result of the matrix multiplication.</returns>
		/// <remarks>
		/// Using matrix multiplication algorithm - see http://en.wikipedia.org/wiki/Matrix_multiplication.
		/// </remarks>
		public static Matrix4x4 operator *(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			var m11 = (((matrix1.m11 * matrix2.m11) + (matrix1.m12 * matrix2.m21)) + (matrix1.m13 * matrix2.m31)) + (matrix1.m14 * matrix2.m41);
			var m12 = (((matrix1.m11 * matrix2.m12) + (matrix1.m12 * matrix2.m22)) + (matrix1.m13 * matrix2.m32)) + (matrix1.m14 * matrix2.m42);
			var m13 = (((matrix1.m11 * matrix2.m13) + (matrix1.m12 * matrix2.m23)) + (matrix1.m13 * matrix2.m33)) + (matrix1.m14 * matrix2.m43);
			var m14 = (((matrix1.m11 * matrix2.m14) + (matrix1.m12 * matrix2.m24)) + (matrix1.m13 * matrix2.m34)) + (matrix1.m14 * matrix2.m44);
			var m21 = (((matrix1.m21 * matrix2.m11) + (matrix1.m22 * matrix2.m21)) + (matrix1.m23 * matrix2.m31)) + (matrix1.m24 * matrix2.m41);
			var m22 = (((matrix1.m21 * matrix2.m12) + (matrix1.m22 * matrix2.m22)) + (matrix1.m23 * matrix2.m32)) + (matrix1.m24 * matrix2.m42);
			var m23 = (((matrix1.m21 * matrix2.m13) + (matrix1.m22 * matrix2.m23)) + (matrix1.m23 * matrix2.m33)) + (matrix1.m24 * matrix2.m43);
			var m24 = (((matrix1.m21 * matrix2.m14) + (matrix1.m22 * matrix2.m24)) + (matrix1.m23 * matrix2.m34)) + (matrix1.m24 * matrix2.m44);
			var m31 = (((matrix1.m31 * matrix2.m11) + (matrix1.m32 * matrix2.m21)) + (matrix1.m33 * matrix2.m31)) + (matrix1.m34 * matrix2.m41);
			var m32 = (((matrix1.m31 * matrix2.m12) + (matrix1.m32 * matrix2.m22)) + (matrix1.m33 * matrix2.m32)) + (matrix1.m34 * matrix2.m42);
			var m33 = (((matrix1.m31 * matrix2.m13) + (matrix1.m32 * matrix2.m23)) + (matrix1.m33 * matrix2.m33)) + (matrix1.m34 * matrix2.m43);
			var m34 = (((matrix1.m31 * matrix2.m14) + (matrix1.m32 * matrix2.m24)) + (matrix1.m33 * matrix2.m34)) + (matrix1.m34 * matrix2.m44);
			var m41 = (((matrix1.m41 * matrix2.m11) + (matrix1.m42 * matrix2.m21)) + (matrix1.m43 * matrix2.m31)) + (matrix1.m44 * matrix2.m41);
			var m42 = (((matrix1.m41 * matrix2.m12) + (matrix1.m42 * matrix2.m22)) + (matrix1.m43 * matrix2.m32)) + (matrix1.m44 * matrix2.m42);
			var m43 = (((matrix1.m41 * matrix2.m13) + (matrix1.m42 * matrix2.m23)) + (matrix1.m43 * matrix2.m33)) + (matrix1.m44 * matrix2.m43);
			var m44 = (((matrix1.m41 * matrix2.m14) + (matrix1.m42 * matrix2.m24)) + (matrix1.m43 * matrix2.m34)) + (matrix1.m44 * matrix2.m44);
			matrix1.m11 = m11;
			matrix1.m12 = m12;
			matrix1.m13 = m13;
			matrix1.m14 = m14;
			matrix1.m21 = m21;
			matrix1.m22 = m22;
			matrix1.m23 = m23;
			matrix1.m24 = m24;
			matrix1.m31 = m31;
			matrix1.m32 = m32;
			matrix1.m33 = m33;
			matrix1.m34 = m34;
			matrix1.m41 = m41;
			matrix1.m42 = m42;
			matrix1.m43 = m43;
			matrix1.m44 = m44;
			return matrix1;
		}

		/// <summary>
		/// Multiplies the elements of matrix by a scalar.
		/// </summary>
		/// <param name="matrix">Source <see cref="Matrix4x4"/> on the left of the mul sign.</param>
		/// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
		/// <returns>Result of the matrix multiplication with a scalar.</returns>
		public static Matrix4x4 operator *(Matrix4x4 matrix, float scaleFactor)
		{
			matrix.m11 = matrix.m11 * scaleFactor;
			matrix.m12 = matrix.m12 * scaleFactor;
			matrix.m13 = matrix.m13 * scaleFactor;
			matrix.m14 = matrix.m14 * scaleFactor;
			matrix.m21 = matrix.m21 * scaleFactor;
			matrix.m22 = matrix.m22 * scaleFactor;
			matrix.m23 = matrix.m23 * scaleFactor;
			matrix.m24 = matrix.m24 * scaleFactor;
			matrix.m31 = matrix.m31 * scaleFactor;
			matrix.m32 = matrix.m32 * scaleFactor;
			matrix.m33 = matrix.m33 * scaleFactor;
			matrix.m34 = matrix.m34 * scaleFactor;
			matrix.m41 = matrix.m41 * scaleFactor;
			matrix.m42 = matrix.m42 * scaleFactor;
			matrix.m43 = matrix.m43 * scaleFactor;
			matrix.m44 = matrix.m44 * scaleFactor;
			return matrix;
		}

		/// <summary>
		/// Subtracts the values of one <see cref="Matrix4x4"/> from another <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="matrix1">Source <see cref="Matrix4x4"/> on the left of the sub sign.</param>
		/// <param name="matrix2">Source <see cref="Matrix4x4"/> on the right of the sub sign.</param>
		/// <returns>Result of the matrix subtraction.</returns>
		public static Matrix4x4 operator -(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			matrix1.m11 = matrix1.m11 - matrix2.m11;
			matrix1.m12 = matrix1.m12 - matrix2.m12;
			matrix1.m13 = matrix1.m13 - matrix2.m13;
			matrix1.m14 = matrix1.m14 - matrix2.m14;
			matrix1.m21 = matrix1.m21 - matrix2.m21;
			matrix1.m22 = matrix1.m22 - matrix2.m22;
			matrix1.m23 = matrix1.m23 - matrix2.m23;
			matrix1.m24 = matrix1.m24 - matrix2.m24;
			matrix1.m31 = matrix1.m31 - matrix2.m31;
			matrix1.m32 = matrix1.m32 - matrix2.m32;
			matrix1.m33 = matrix1.m33 - matrix2.m33;
			matrix1.m34 = matrix1.m34 - matrix2.m34;
			matrix1.m41 = matrix1.m41 - matrix2.m41;
			matrix1.m42 = matrix1.m42 - matrix2.m42;
			matrix1.m43 = matrix1.m43 - matrix2.m43;
			matrix1.m44 = matrix1.m44 - matrix2.m44;
			return matrix1;
		}

		/// <summary>
		/// Inverts values in the specified <see cref="Matrix4x4"/>.
		/// </summary>
		/// <param name="matrix">Source <see cref="Matrix4x4"/> on the right of the sub sign.</param>
		/// <returns>Result of the inversion.</returns>
		public static Matrix4x4 operator -(Matrix4x4 matrix)
		{
			matrix.m11 = -matrix.m11;
			matrix.m12 = -matrix.m12;
			matrix.m13 = -matrix.m13;
			matrix.m14 = -matrix.m14;
			matrix.m21 = -matrix.m21;
			matrix.m22 = -matrix.m22;
			matrix.m23 = -matrix.m23;
			matrix.m24 = -matrix.m24;
			matrix.m31 = -matrix.m31;
			matrix.m32 = -matrix.m32;
			matrix.m33 = -matrix.m33;
			matrix.m34 = -matrix.m34;
			matrix.m41 = -matrix.m41;
			matrix.m42 = -matrix.m42;
			matrix.m43 = -matrix.m43;
			matrix.m44 = -matrix.m44;
			return matrix;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains subtraction of one matrix from another.
		/// </summary>
		/// <param name="matrix1">The first <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">The second <see cref="Matrix4x4"/>.</param>
		/// <returns>The result of the matrix subtraction.</returns>
		public static Matrix4x4 Subtract(Matrix4x4 matrix1, in Matrix4x4 matrix2)
		{
			matrix1.m11 = matrix1.m11 - matrix2.m11;
			matrix1.m12 = matrix1.m12 - matrix2.m12;
			matrix1.m13 = matrix1.m13 - matrix2.m13;
			matrix1.m14 = matrix1.m14 - matrix2.m14;
			matrix1.m21 = matrix1.m21 - matrix2.m21;
			matrix1.m22 = matrix1.m22 - matrix2.m22;
			matrix1.m23 = matrix1.m23 - matrix2.m23;
			matrix1.m24 = matrix1.m24 - matrix2.m24;
			matrix1.m31 = matrix1.m31 - matrix2.m31;
			matrix1.m32 = matrix1.m32 - matrix2.m32;
			matrix1.m33 = matrix1.m33 - matrix2.m33;
			matrix1.m34 = matrix1.m34 - matrix2.m34;
			matrix1.m41 = matrix1.m41 - matrix2.m41;
			matrix1.m42 = matrix1.m42 - matrix2.m42;
			matrix1.m43 = matrix1.m43 - matrix2.m43;
			matrix1.m44 = matrix1.m44 - matrix2.m44;
			return matrix1;
		}

		/// <summary>
		/// Creates a new <see cref="Matrix4x4"/> that contains subtraction of one matrix from another.
		/// </summary>
		/// <param name="matrix1">The first <see cref="Matrix4x4"/>.</param>
		/// <param name="matrix2">The second <see cref="Matrix4x4"/>.</param>
		/// <param name="result">The result of the matrix subtraction as an output parameter.</param>
		public static void Subtract(in Matrix4x4 matrix1, in Matrix4x4 matrix2, out Matrix4x4 result)
		{
			result.m11 = matrix1.m11 - matrix2.m11;
			result.m12 = matrix1.m12 - matrix2.m12;
			result.m13 = matrix1.m13 - matrix2.m13;
			result.m14 = matrix1.m14 - matrix2.m14;
			result.m21 = matrix1.m21 - matrix2.m21;
			result.m22 = matrix1.m22 - matrix2.m22;
			result.m23 = matrix1.m23 - matrix2.m23;
			result.m24 = matrix1.m24 - matrix2.m24;
			result.m31 = matrix1.m31 - matrix2.m31;
			result.m32 = matrix1.m32 - matrix2.m32;
			result.m33 = matrix1.m33 - matrix2.m33;
			result.m34 = matrix1.m34 - matrix2.m34;
			result.m41 = matrix1.m41 - matrix2.m41;
			result.m42 = matrix1.m42 - matrix2.m42;
			result.m43 = matrix1.m43 - matrix2.m43;
			result.m44 = matrix1.m44 - matrix2.m44;
		}

		internal string DebugDisplayString
		{
			get
			{
				if (this == Identity)
				{
					return "Identity";
				}

				return string.Concat(
					  "( ", this.m11.ToString(), "  ", this.m12.ToString(), "  ", this.m13.ToString(), "  ", this.m14.ToString(), " )  \r\n",
					  "( ", this.m21.ToString(), "  ", this.m22.ToString(), "  ", this.m23.ToString(), "  ", this.m24.ToString(), " )  \r\n",
					  "( ", this.m31.ToString(), "  ", this.m32.ToString(), "  ", this.m33.ToString(), "  ", this.m34.ToString(), " )  \r\n",
					  "( ", this.m41.ToString(), "  ", this.m42.ToString(), "  ", this.m43.ToString(), "  ", this.m44.ToString(), " )");
			}
		}

		/// <summary>
		/// Returns a <see cref="String"/> representation of this <see cref="Matrix4x4"/> in the format:
		/// {M11:[<see cref="m11"/>] M12:[<see cref="m12"/>] M13:[<see cref="m13"/>] M14:[<see cref="m14"/>]}
		/// {M21:[<see cref="m21"/>] M12:[<see cref="m22"/>] M13:[<see cref="m23"/>] M14:[<see cref="m24"/>]}
		/// {M31:[<see cref="m31"/>] M32:[<see cref="m32"/>] M33:[<see cref="m33"/>] M34:[<see cref="m34"/>]}
		/// {M41:[<see cref="m41"/>] M42:[<see cref="m42"/>] M43:[<see cref="m43"/>] M44:[<see cref="m44"/>]}
		/// </summary>
		/// <returns>A <see cref="String"/> representation of this <see cref="Matrix4x4"/>.</returns>
		public override string ToString()
		{
			return "{M11:" + m11 + " M12:" + m12 + " M13:" + m13 + " M14:" + m14 + "}"
				 + " {M21:" + m21 + " M22:" + m22 + " M23:" + m23 + " M24:" + m24 + "}"
				 + " {M31:" + m31 + " M32:" + m32 + " M33:" + m33 + " M34:" + m34 + "}"
				 + " {M41:" + m41 + " M42:" + m42 + " M43:" + m43 + " M44:" + m44 + "}";
		}

		/// <summary>
		/// Swap the matrix rows and columns.
		/// </summary>
		/// <param name="matrix">The matrix for transposing operation.</param>
		/// <returns>The new <see cref="Matrix4x4"/> which contains the transposing result.</returns>
		public static Matrix4x4 Transpose(Matrix4x4 matrix)
		{
			Matrix4x4 ret;
			Transpose(matrix, out ret);
			return ret;
		}

		/// <summary>
		/// Swap the matrix rows and columns.
		/// </summary>
		/// <param name="matrix">The matrix for transposing operation.</param>
		/// <param name="result">The new <see cref="Matrix4x4"/> which contains the transposing result as an output parameter.</param>
		public static void Transpose(in Matrix4x4 matrix, out Matrix4x4 result)
		{
			Matrix4x4 ret;

			ret.m11 = matrix.m11;
			ret.m12 = matrix.m21;
			ret.m13 = matrix.m31;
			ret.m14 = matrix.m41;

			ret.m21 = matrix.m12;
			ret.m22 = matrix.m22;
			ret.m23 = matrix.m32;
			ret.m24 = matrix.m42;

			ret.m31 = matrix.m13;
			ret.m32 = matrix.m23;
			ret.m33 = matrix.m33;
			ret.m34 = matrix.m43;

			ret.m41 = matrix.m14;
			ret.m42 = matrix.m24;
			ret.m43 = matrix.m34;
			ret.m44 = matrix.m44;

			result = ret;
		}

		///// <summary>
		///// Returns a <see cref="System.Numerics.Matrix4x4"/>.
		///// </summary>
		//public System.Numerics.Matrix4x4 ToNumerics()
		//{
		//	return new System.Numerics.Matrix4x4(
		//		 this.M11, this.M12, this.M13, this.M14,
		//		 this.M21, this.M22, this.M23, this.M24,
		//		 this.M31, this.M32, this.M33, this.M34,
		//		 this.M41, this.M42, this.M43, this.M44);
		//}

		#endregion

		#region Private Static Methods

		/// <summary>
		/// Helper method for using the Laplace expansion theorem using two rows expansions to calculate major and 
		/// minor determinants of a 4x4 matrix. This method is used for inverting a matrix.
		/// </summary>
		private static void FindDeterminants(ref Matrix4x4 matrix, out float major,
														 out float minor1, out float minor2, out float minor3, out float minor4, out float minor5, out float minor6,
														 out float minor7, out float minor8, out float minor9, out float minor10, out float minor11, out float minor12)
		{
			float det1 = (float)matrix.m11 * (float)matrix.m22 - (float)matrix.m12 * (float)matrix.m21;
			float det2 = (float)matrix.m11 * (float)matrix.m23 - (float)matrix.m13 * (float)matrix.m21;
			float det3 = (float)matrix.m11 * (float)matrix.m24 - (float)matrix.m14 * (float)matrix.m21;
			float det4 = (float)matrix.m12 * (float)matrix.m23 - (float)matrix.m13 * (float)matrix.m22;
			float det5 = (float)matrix.m12 * (float)matrix.m24 - (float)matrix.m14 * (float)matrix.m22;
			float det6 = (float)matrix.m13 * (float)matrix.m24 - (float)matrix.m14 * (float)matrix.m23;
			float det7 = (float)matrix.m31 * (float)matrix.m42 - (float)matrix.m32 * (float)matrix.m41;
			float det8 = (float)matrix.m31 * (float)matrix.m43 - (float)matrix.m33 * (float)matrix.m41;
			float det9 = (float)matrix.m31 * (float)matrix.m44 - (float)matrix.m34 * (float)matrix.m41;
			float det10 = (float)matrix.m32 * (float)matrix.m43 - (float)matrix.m33 * (float)matrix.m42;
			float det11 = (float)matrix.m32 * (float)matrix.m44 - (float)matrix.m34 * (float)matrix.m42;
			float det12 = (float)matrix.m33 * (float)matrix.m44 - (float)matrix.m34 * (float)matrix.m43;

			major = (float)(det1 * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7);
			minor1 = (float)det1;
			minor2 = (float)det2;
			minor3 = (float)det3;
			minor4 = (float)det4;
			minor5 = (float)det5;
			minor6 = (float)det6;
			minor7 = (float)det7;
			minor8 = (float)det8;
			minor9 = (float)det9;
			minor10 = (float)det10;
			minor11 = (float)det11;
			minor12 = (float)det12;
		}

		#endregion
	}
}

#endif
