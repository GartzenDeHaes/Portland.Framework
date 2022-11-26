using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.SceneGraph
{
	[TestFixture]
	public class SceneGraphTest
	{
		public class TestComponent : Component
		{
			//public int UpdateCount;
			public int BBCount;

			public BoundingBox Box = new BoundingBox(Vector3.Down, Vector3.Up);

			public override void OnAdded(IEntity entity)
			{
				base.OnAdded(entity);
				entity.IncludeChildNodesInBoundingBox = true;
			}

			//public override void Update(float deltaTime)
			//{
			//	UpdateCount++;
			//}

			public override BoundingBox GetBoundingBox()
			{
				BBCount++;
				return Box;
			}
		}

		public class TestSystem : ISystem, IComponentFactory
		{
			public ISystem NextSystem { get; set; }
			public Type ManagedComponent { get { return typeof(TestComponent); } }

			public int CreateCount;
			public int UpdateCount;
			public int ReleaseCount;

			public IComponent GetComponent()
			{
				CreateCount++;
				return new TestComponent();
			}

			public void ReleaseComponent(IComponent component)
			{
				ReleaseCount++;
			}

			public void Update(float deltaTime)
			{
				UpdateCount++;
			}
		}

		[Test]
		public void A_Basic_Setup()
		{
			var scene = new Scene("test");
			var tsys = new TestSystem();

			scene.AddSystem(tsys);
			var e1 = scene.CreateEnitity();
			Assert.False(e1.HasComponents);
			Assert.That(tsys.CreateCount, Is.EqualTo(0));
			Assert.True(scene.SceneGraph.Transform.HasChildNodes);
			Assert.That(e1.Transform.Parent, Is.EqualTo(scene.SceneGraph.Transform));
			Assert.False(e1.IsBoundingBoxDirty);

			scene.Update(0.2f);
			Assert.That(tsys.UpdateCount, Is.EqualTo(1));

			scene.AddComponentTo<TestComponent>(e1);
			var c1 = e1.GetComponent<TestComponent>();
			Assert.True(e1.HasComponents);
			Assert.That(c1.Transform.Entity, Is.EqualTo(e1));
			Assert.True(e1.IsBoundingBoxDirty);

			scene.Update(0.2f);
			Assert.That(tsys.UpdateCount, Is.EqualTo(2));
			//Assert.That(c1.UpdateCount, Is.EqualTo(1));

			Assert.That(c1.BBCount, Is.EqualTo(1));
			Assert.That(c1.Box, Is.EqualTo(e1.BoundingBox));

			scene.DestroyEntity(e1);
			Assert.That(tsys.ReleaseCount, Is.EqualTo(1));
			Assert.False(scene.SceneGraph.Transform.HasChildNodes);
		}
	}
}
