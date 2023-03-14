using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Types
{
	public class PropertyDefinitionSet
	{
		public string SetId;
		public PropertyDefinition[] Properties;
		public string OnLevelScript = String.Empty;
		public string OnInventoryScript = String.Empty;
		public string OnEffectScript = String.Empty;
	}
}
