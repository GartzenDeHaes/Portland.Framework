using System;
using System.Collections.Generic;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

using NUnit.Framework;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

namespace ProceduralToolkit.Tests
{
	public class TestBase
	{
		private const float Epsilon = MathHelper.Epsilonf * 10000f;
		private const string vector2Format = "actual: {0} expected: {1}\ndelta: {2:F8}\n{3}";
		private const string floatFormat = "actual: {0:G9} expected: {1:G9}\ndelta: {2:F8}\n{3}";

		private List<Vector2> _directionPoints2;
		protected List<Vector2> directionPoints2
		{
			get { return _directionPoints2 ?? (_directionPoints2 = Geometry.PointsOnCircle2(1, 24)); }
		}
		private List<Vector2> _originPoints2;
		protected List<Vector2> originPoints2
		{
			get { return _originPoints2 ?? (_originPoints2 = Geometry.PointsOnCircle2(10, 24)); }
		}

		private List<Vector3> _directionPoints3;
		protected List<Vector3> directionPoints3
		{
			get { return _directionPoints3 ?? (_directionPoints3 = Geometry.PointsOnSphere(1.0f, 100)); }
		}
		private List<Vector3> _originPoints3;
		protected List<Vector3> originPoints3
		{
			get { return _originPoints3 ?? (_originPoints3 = Geometry.PointsOnSphere(10, 100)); }
		}

		protected static void AreEqual(Vector2 actual, Vector2 expected, string message = null)
		{
			float delta = (float)((actual - expected).Magnitude);
			Assert.True(delta < Epsilon, vector2Format, actual.ToString(), expected.ToString(), delta, message);
		}

		protected static void AreEqual(Vector3 actual, Vector3 expected, string message = null)
		{
			float delta = (float)((actual - expected).Magnitude);
			Assert.True(delta < Epsilon, vector2Format, actual.ToString(), expected.ToString(), delta, message);
		}

		protected static void AreEqual(float actual, float expected, string message = null)
		{
			float delta = MathF.Abs(actual - expected);
			Assert.True(delta < Epsilon, floatFormat, actual, expected, delta, message);
		}

		protected static void AreEqual(double actual, double expected, string message = null)
		{
			var delta = Math.Abs(actual - expected);
			Assert.True(delta < Epsilon, floatFormat, actual, expected, delta, message);
		}

		protected static Vector3d GetTangent(Vector3 direction)
		{
			// direction.Normalize();
			// var t1 = Vector3d.Cross(direction, Vector3d.Forward);
			// var t2 = Vector3d.Cross(direction, Vector3d.Up);
			// if (t1.SqrMagnitude > t2.SqrMagnitude)
			// {
			// 	return t1;
			// }
			// else
			// {
			// 	return t2;
			// }

			Vector3d d2 = direction;
			Vector3d tangent = d2;
			Vector3d.OrthoNormalize(ref d2, ref tangent);
			return tangent;
		}

		protected static Vector3d GetTangent_ShouldBeCorrectOne(Vector3 direction)
		{
			var t1 = Vector3d.Cross(direction, Vector3d.Forward);
			var t2 = Vector3d.Cross(direction, Vector3d.Up);
			if (t1.SqrMagnitude > t2.SqrMagnitude)
			{
				return t1;
			}
			else
			{
				return t2;
			}
		}
	}
}
