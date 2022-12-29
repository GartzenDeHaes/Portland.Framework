﻿#region File Description
//-----------------------------------------------------------------------------
// A class containing all the basic transformations a renderable object can have.
// This include: Translation, Rotation, and Scale.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

using Portland.Mathmatics;

namespace Maximum.SceneGraph
{
	///// <summary>
	///// How to apply rotation (euler vs quaternion).
	///// </summary>
	//public enum RotationType
	//{
	//	/// <summary>
	//	/// Euler rotation.
	//	/// </summary>
	//	Euler,

	//	/// <summary>
	//	/// Quaternion rotation.
	//	/// </summary>
	//	Quaternion,
	//}

	///// <summary>
	///// Different way to build matrix from transformations.
	///// </summary>
	//public enum TransformOrder
	//{
	//	/// <summary>
	//	/// Apply position, then rotation, then scale.
	//	/// </summary>
	//	PositionRotationScale,

	//	/// <summary>
	//	/// Apply position, then scale, then rotation.
	//	/// </summary>
	//	PositionScaleRotation,

	//	/// <summary>
	//	/// Apply scale, then position, then rotation.
	//	/// </summary>
	//	ScalePositionRotation,

	//	/// <summary>
	//	/// Apply scale, then rotation, then position.
	//	/// </summary>
	//	ScaleRotationPosition,

	//	/// <summary>
	//	/// Apply rotation, then scale, then position.
	//	/// </summary>
	//	RotationScalePosition,

	//	/// <summary>
	//	/// Apply rotation, then position, then scale.
	//	/// </summary>
	//	RotationPositionScale,
	//}

	///// <summary>
	///// Different ways to apply rotation (order in which we rotate the different axis).
	///// </summary>
	//public enum RotationOrder
	//{
	//	/// <summary>
	//	/// Rotate by axis order X, Y, Z.
	//	/// </summary>
	//	RotateXYZ,

	//	/// <summary>
	//	/// Rotate by axis order X, Z, Y.
	//	/// </summary>
	//	RotateXZY,

	//	/// <summary>
	//	/// Rotate by axis order Y, X, Z.
	//	/// </summary>
	//	RotateYXZ,

	//	/// <summary>
	//	/// Rotate by axis order Y, Z, X.
	//	/// </summary>
	//	RotateYZX,

	//	/// <summary>
	//	/// Rotate by axis order Z, X, Y.
	//	/// </summary>
	//	RotateZXY,

	//	/// <summary>
	//	/// Rotate by axis order Z, Y, X.
	//	/// </summary>
	//	RotateZYX,
	//}

	/// <summary>
	/// Contain all the possible node transformations.
	/// </summary>
	public sealed class Transformations
	{
		/// <summary>
		/// Node position / translation.
		/// </summary>
		public Vector3 Position;

		/// <summary>
		/// Node rotation.
		/// </summary>
		public Quaternion Rotation;

		/// <summary>
		/// Node scale.
		/// </summary>
		public Vector3 Scale;

		///// <summary>
		///// Order to apply different transformations to create the final matrix.
		///// </summary>
		//private TransformOrder TransformOrder = TransformOrder.ScaleRotationPosition;

		///// <summary>
		///// Axis order to apply rotation.
		///// </summary>
		//private RotationOrder RotationOrder = RotationOrder.RotateYXZ;

		///// <summary>
		///// What type of rotation to use.
		///// </summary>
		//private RotationType RotationType = RotationType.Quaternion;

		/// <summary>
		/// Create new default transformations.
		/// </summary>
		public Transformations()
		{
			Position = Vector3.zero;
			Rotation = Quaternion.Identity;
			Scale = Vector3.one;
		}

		/// <summary>
		/// Clone transformations.
		/// </summary>
		public Transformations(Transformations other)
		{
			Position = other.Position;
			Rotation = other.Rotation;
			Scale = other.Scale;
			//TransformOrder = other.TransformOrder;
			//RotationOrder = other.RotationOrder;
			//RotationType = other.RotationType;
		}

		/// <summary>
		/// Clone transformations.
		/// </summary>
		/// <returns>Copy of this transformations.</returns>
		public Transformations Clone()
		{
			return new Transformations(this);
		}

		/// <summary>
		/// Build and return just the rotation matrix for this treansformations.
		/// </summary>
		/// <returns>Rotation matrix.</returns>
		private Matrix4x4 BuildRotationMatrix()
		{
			//// handle euler rotation
			//if (RotationType == RotationType.Euler)
			//{
			//	switch (RotationOrder)
			//	{
			//		case RotationOrder.RotateXYZ:
			//			return Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z);

			//		case RotationOrder.RotateXZY:
			//			return Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationY(Rotation.Y);

			//		case RotationOrder.RotateYXZ:
			//			return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);

			//		case RotationOrder.RotateYZX:
			//			return Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationX(Rotation.X);

			//		case RotationOrder.RotateZXY:
			//			return Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);

			//		case RotationOrder.RotateZYX:
			//			return Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationX(Rotation.X);

			//		default:
			//			throw new System.Exception("Unknown rotation order!");
			//	}
			//}
			//// handle quaternion rotation
			//else if (RotationType == RotationType.Quaternion)
			//{
			// quaternion to use
			//Quaternion quat;

			// build quaternion based on rotation order
			//switch (RotationOrder)
			//{
			//case RotationOrder.RotateXYZ:
			//quat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z);
			//break;

			//case RotationOrder.RotateXZY:
			//	quat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y);
			//	break;

			//case RotationOrder.RotateYXZ:
			//	quat = Quaternion.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
			//	break;

			//case RotationOrder.RotateYZX:
			//	quat = Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X);
			//	break;

			//case RotationOrder.RotateZXY:
			//	quat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y);
			//	break;

			//case RotationOrder.RotateZYX:
			//	quat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X);
			//	break;

			//default:
			//	throw new System.Exception("Unknown rotation order!");
			//}

			// convert to a matrix and return
			return Matrix4x4.CreateFromQuaternion(Rotation);
			//}
			//// should never happen.
			//else
			//{
			//	throw new System.Exception("Unknown rotation type!");
			//}
		}

		/// <summary>
		/// Build and return a matrix from current transformations.
		/// </summary>
		/// <returns>Matrix with all transformations applied.</returns>
		public Matrix4x4 BuildMatrix()
		{
			// create the matrix parts
			Matrix4x4 pos = Matrix4x4.CreateTranslation(Position * Scale);
			Matrix4x4 rot = BuildRotationMatrix();
			//Matrix scale = Matrix.CreateScale(Scale);

			// build and return matrix based on order
			//switch (TransformOrder)
			//{
			//	case TransformOrder.PositionRotationScale:
			//		return pos * rot * scale;

			//	case TransformOrder.PositionScaleRotation:
			//		return pos * scale * rot;

			//	case TransformOrder.ScalePositionRotation:
			//		return scale * pos * rot;

			//case TransformOrder.ScaleRotationPosition:
			return /*scale * */ rot * pos;

			//	case TransformOrder.RotationScalePosition:
			//		return rot * scale * pos;

			//	case TransformOrder.RotationPositionScale:
			//		return rot * pos * scale;

			//	default:
			//		throw new System.Exception("Unknown build matrix order!");
			//}
		}

		public void SetScale(float x, float y, float z)
		{
			Scale.x = x;
			Scale.y = y;
			Scale.z = z;
		}

		public void ScaleBy(float factor)
		{
			Scale *= factor;
		}

		public void Translate(in Vector3 amt)
		{
			Position += amt;
		}

		public void SetPosition(in Vector3 newPos)
		{
			Position = newPos;
		}

		public void SetRotation(float x, float y, float z, float w)
		{
			Rotation = new Quaternion(x, y, z, w);
		}

		public void SetRotation(float x, float y, float z)
		{
			Quaternion.CreateFromYawPitchRoll(z, y, x, out Rotation);
		}

		public void RotateBy(float x, float y, float z)
		{
			Rotation *= Quaternion.Euler(x, y, z);
		}
	}
}
