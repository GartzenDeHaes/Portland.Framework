using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Mathmatics;
using Portland.Text;

namespace Portland.AI
{
	/// <summary>
	/// TODO
	/// </summary>
	public sealed class PropertyManager
	{
		public enum PropertyChangeSemantic
		{
			/// <summary>Starts at zero and accumulates such as hunger, thirst, tiredness</summary>
			Accumulates,
			/// <summary>Starts at 100 and depletes such as health, stamina, fuel</summary>
			Depletes,
		}

		public class PropertyDefinition
		{
			public AsciiId4 Id;
			public string Name;
			public PropertyChangeSemantic ChangeSemantic;
			/// <summary>Allow changes outside probability range, probably zero in most cases</summary>
			public float Miniumum;
			/// <summary>Allow changes outside probability range</summary>
			public float Maxiumum;
			public float ChangePerSecond;
			public int ValueIndex;

			public float DefaultValue { get { return ChangeSemantic == PropertyChangeSemantic.Accumulates ? Miniumum : Maxiumum; } }
		}

		public struct PropertyDefSet
		{
			public AsciiId4 SetName;
			public PropertyDefinition[] Properties;
		}

		public struct PropertyInstanceKey
		{
			public int Index;
			public AsciiId4 Id;
		}

		public struct PropertySetInstance
		{
			public PropertyInstanceKey[] Properties;
			public AsciiId4 SetId;
		}

		Vector<float>[] _values;
		PropertyDefinition[] _definitions;
		Vector<PropertyDefSet> _sets = new Vector<PropertyDefSet>();
		//Vector<PropertySetInstance> _pool = new Vector<PropertySetInstance>();

		public void Update(float deltaTimeInSeconds)
		{
			float changePerSecond;
			Vector<float> values;

			for (int i = 0; i < _definitions.Length; i++)
			{
				changePerSecond = _definitions[i].ChangePerSecond;
				values = _values[i];

				for (int j = 0; j < values.Count; j++)
				{
					values[i] += deltaTimeInSeconds * changePerSecond;
				}
			}
		}

		public PropertyDefinition DefineProperty(AsciiId4 id, string name)
		{
			var def = new PropertyDefinition { Id = id, Name = name, Miniumum = 0, Maxiumum = 100 };
			for (int i = 0; i < _definitions.Length; i++)
			{
				if (_definitions[i] == null)
				{
					_definitions[i] = def;
					def.ValueIndex= i;
					return def;
				}
			}

			throw new Exception($"Out of space for property definition {name}");
		}

		bool TryGetDefinition(AsciiId4 id, out PropertyDefinition def)
		{
			for (int i = 0; i < _definitions.Length; i++)
			{
				def = _definitions[i];
				if (def.Id == id)
				{
					return true;
				}
			}

			def = null;
			return false;
		}

		public void DefinePropertySet(AsciiId4 setName, AsciiId4[] propIds)
		{
			var set = new PropertyDefSet { SetName = setName, Properties = new PropertyDefinition[propIds.Length] };

			for (int i = 0; i < propIds.Length; i++)
			{
				if (TryGetDefinition(propIds[i], out var def))
				{
					set.Properties[i] = def;
				}
				else
				{
					throw new Exception($"Property definition {propIds[i]} not found");
				}
			}

			_sets.Add(set);
		}

		bool TryGetSetDef(AsciiId4 setId, out PropertyDefSet setDef)
		{
			for(int i = 0; i < _sets.Count; i++)
			{
				setDef = _sets[i];
				if (setDef.SetName == setId)
				{
					return true;
				}
			}

			setDef = default(PropertyDefSet);
			return false;
		}

		public PropertySetInstance CreateSetInstance(AsciiId4 setId)
		{
			if (! TryGetSetDef(setId, out var setDef))
			{
				throw new Exception($"Definition for set {setId} not found");
			}

			var inst = new PropertySetInstance { SetId = setId, Properties = new PropertyInstanceKey[setDef.Properties.Length] };
			PropertyDefinition def;

			for (int i = 0; i < setDef.Properties.Length; i++)
			{
				def = setDef.Properties[i];
				inst.Properties[i] = new PropertyInstanceKey { 
					Id = def.Id,
					Index = _values[def.ValueIndex].AddElement(def.DefaultValue) 
				};
			}

			return inst;
		}

		public PropertyManager(int maxNumPropDefs = 8)
		{
			_definitions = new PropertyDefinition[maxNumPropDefs];
			_values = new Vector<float>[maxNumPropDefs];

			for (int i = 0; i < maxNumPropDefs; i++)
			{
				_values[i] = new Vector<float>();
			}
		}
	}
}
