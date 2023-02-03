using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.RPG
{
	public struct PropertyInstanceKey
	{
		public short Index;
		public short DefinitonIndex;
		public AsciiId4 PropertyId;
	}

	public struct PropertySetKeys
	{
		public PropertyInstanceKey[] Properties;
		public String8 SetId;
	}
}
