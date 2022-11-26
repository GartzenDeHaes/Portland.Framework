using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.SceneGraph
{
	public interface IComponentFactory
	{
		Type ManagedComponent { get; }
		IComponent GetComponent();
		void ReleaseComponent(IComponent component);
	}
}
