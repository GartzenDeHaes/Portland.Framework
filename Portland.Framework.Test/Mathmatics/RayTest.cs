﻿
using NUnit.Framework;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace MonoGame.Tests.Framework
{
	class RayTest
	{
		[Test]
		public void BoundingBoxIntersects()
		{
			// Our test box.
			BoundingBox box;
			box.Min = new Vector3(-10, -20, -30);
			box.Max = new Vector3(10, 20, 30);
			var center = (box.Max + box.Min) * 0.5f;

			// Test misses.
			Assert.IsNull(new Ray(center - Vector3.UnitX * 40, -Vector3.UnitX).Intersects(box));
			Assert.IsNull(new Ray(center + Vector3.UnitX * 40, Vector3.UnitX).Intersects(box));
			Assert.IsNull(new Ray(center - Vector3.UnitY * 40, -Vector3.UnitY).Intersects(box));
			Assert.IsNull(new Ray(center + Vector3.UnitY * 40, Vector3.UnitY).Intersects(box));
			Assert.IsNull(new Ray(center - Vector3.UnitZ * 40, -Vector3.UnitZ).Intersects(box));
			Assert.IsNull(new Ray(center + Vector3.UnitZ * 40, Vector3.UnitZ).Intersects(box));

			// Test middle of each face.
			Assert.AreEqual(30.0f, new Ray(center - Vector3.UnitX * 40, Vector3.UnitX).Intersects(box));
			Assert.AreEqual(30.0f, new Ray(center + Vector3.UnitX * 40, -Vector3.UnitX).Intersects(box));
			Assert.AreEqual(20.0f, new Ray(center - Vector3.UnitY * 40, Vector3.UnitY).Intersects(box));
			Assert.AreEqual(20.0f, new Ray(center + Vector3.UnitY * 40, -Vector3.UnitY).Intersects(box));
			Assert.AreEqual(10.0f, new Ray(center - Vector3.UnitZ * 40, Vector3.UnitZ).Intersects(box));
			Assert.AreEqual(10.0f, new Ray(center + Vector3.UnitZ * 40, -Vector3.UnitZ).Intersects(box));

			// Test the corners along each axis.
			Assert.AreEqual(10.0f, new Ray(box.Min - Vector3.UnitX * 10, Vector3.UnitX).Intersects(box));
			Assert.AreEqual(10.0f, new Ray(box.Min - Vector3.UnitY * 10, Vector3.UnitY).Intersects(box));
			Assert.AreEqual(10.0f, new Ray(box.Min - Vector3.UnitZ * 10, Vector3.UnitZ).Intersects(box));
			Assert.AreEqual(10.0f, new Ray(box.Max + Vector3.UnitX * 10, -Vector3.UnitX).Intersects(box));
			Assert.AreEqual(10.0f, new Ray(box.Max + Vector3.UnitY * 10, -Vector3.UnitY).Intersects(box));
			Assert.AreEqual(10.0f, new Ray(box.Max + Vector3.UnitZ * 10, -Vector3.UnitZ).Intersects(box));

			// Test inside out.
			Assert.AreEqual(0.0f, new Ray(center, Vector3.UnitX).Intersects(box));
			Assert.AreEqual(0.0f, new Ray(center, -Vector3.UnitX).Intersects(box));
			Assert.AreEqual(0.0f, new Ray(center, Vector3.UnitY).Intersects(box));
			Assert.AreEqual(0.0f, new Ray(center, -Vector3.UnitY).Intersects(box));
			Assert.AreEqual(0.0f, new Ray(center, Vector3.UnitZ).Intersects(box));
			Assert.AreEqual(0.0f, new Ray(center, -Vector3.UnitZ).Intersects(box));
		}

#if !XNA
		[Test]
		public void Deconstruct()
		{
			Ray ray = new Ray(Vector3.Backward, Vector3.right);

			Vector3 position = ray.Origin, direction = ray.Direction;

			Assert.AreEqual(position, ray.Origin);
			Assert.AreEqual(direction, ray.Direction);
		}
#endif
	}
}
