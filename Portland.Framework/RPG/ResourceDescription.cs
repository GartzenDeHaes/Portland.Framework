using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public enum ResourceType
	{
		None = 0,
		Sprite = 1,
		Text = 2,
		Audio = 3,
		Prefab = 4,
	}

	[Serializable]
	public struct ResourceDescription
	{
		public string ResourceKey;
		public ResourceType ContentType;
	}
}
