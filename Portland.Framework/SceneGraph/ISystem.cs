using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.SceneGraph
{
	public interface ISystem
	{
		ISystem NextSystem { get; set; }
		void Update(float deltaTime);
	}
}
