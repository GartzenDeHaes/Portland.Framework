using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Portland.Collections;

namespace Portland.SceneGraph
{
	public sealed class Scene : IScene
	{
		IEntity _sceneGraph;

		ISystem _systems;
		ISystem _systemsTail;

		Dictionary<Type, IComponentFactory> _componentFactories = new Dictionary<Type, IComponentFactory>();
		//public String[] Layers;

		ConcurrentObjectPoolSem<Entity> _entityFactory = new ConcurrentObjectPoolSem<Entity>();

		public void Update(float deltaTime)
		{
			var sys = _systems;
			while (sys != null)
			{
				sys.Update(deltaTime);
				sys = _systems.NextSystem;
			}

			_sceneGraph.Update(deltaTime);
		}

		public Scene(string name)
		{
			SceneName = name;
			_sceneGraph = new Entity("root");
		}

		public IEntity SceneGraph
		{
			get { return _sceneGraph; }
		}

		public string SceneName
		{
			get; private set;
		}

		public IEntity CreateEnitity(IEntity parent = null)
		{
			var e = _entityFactory.Get();
			e.Scene = this;
			if (parent == null)
			{
				_sceneGraph.Transform.AddChildNode(e.Transform);
			}
			else
			{
				parent.Transform.AddChildNode(e.Transform);
			}
			return e;
		}

		public void DestroyEntity(IEntity entity)
		{
			if (entity is Entity e)
			{
				_entityFactory.Release(e);
			}
			else
			{
				entity.RemoveFromParent();
				entity.RemoveAllComponents();
				entity.Transform.RemoveAllChildren();
			}
		}

		public void AddComponentTo<T>(IEntity ent) where T : IComponent
		{
			var constr = _componentFactories[typeof(T)];
			var comp = constr.GetComponent();
			ent.AddComponent(comp);
		}

		public void RemoveComponentFrom<T>(IEntity ent) where T : IComponent
		{
			var t = ent.RemoveComponent<T>();
			if (t != null)
			{
				_componentFactories[typeof(T)].ReleaseComponent(t);
			}
		}

		public void ReleaseComponent(IComponent c)
		{
			Debug.Assert(c != null);
			_componentFactories[c.GetType()].ReleaseComponent(c);
		}

		public void AddSystem(ISystem sys)
		{
			if (_systemsTail == null)
			{
				_systems = sys;
				_systemsTail = sys;
			}
			else
			{
				_systemsTail.NextSystem = sys;
				_systemsTail = sys;
			}

			if (sys is IComponentFactory factr)
			{
				_componentFactories.Add(factr.ManagedComponent, factr);
			}
		}

		public void AddComponentFactory(IComponentFactory fact)
		{
			_componentFactories.Add(fact.ManagedComponent, fact);
		}
	}
}
