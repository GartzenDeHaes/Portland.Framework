using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Mathmatics;
using Portland.Text;

namespace Portland.AI.Semantics
{
	[Serializable]
	public class SemanticTag
	{
		public Int32Guid Id;
		public string Tag;		// class
		public string Name;     // instance name
		public ThematicRoleFlags Roles;

		public SemanticTag(string cls, string name)
		{
			Id = new Int32Guid();
			Tag = cls;
			Name = name;
			Roles = new ThematicRoleFlags();
		}

		public SemanticTag(string cls, String8 name)
		{
			Id = new Int32Guid();
			Tag = cls;
			Name = name;
			Roles = new ThematicRoleFlags();
		}
	}
}
