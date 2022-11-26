using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.SceneGraph
{
	public class ComponentUpdateSystem<T> : PooledComponentFactory<T>, ISystem where T : Component, new()
	{
		public ISystem NextSystem { get; set; }

		List<Component> _components = new List<Component>();

		public ComponentUpdateSystem()
		{
		}

		public void Update(float deltaTime)
		{
			int count = _components.Count;

			for (int x = 0; x < count; x++)
			{
				_components[x].Update(deltaTime);
			}
		}

		public override IComponent GetComponent()
		{
			var c = base.GetComponent();
			_components.Add((Component)c);
			return c;
		}

		public override void ReleaseComponent(IComponent component)
		{
			_components.Remove((Component)component);
			base.ReleaseComponent(component);
		}
	}
}
