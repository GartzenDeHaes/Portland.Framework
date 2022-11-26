using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Mathmatics;

namespace Portland.AI.Semantics
{
	public class SemanticDataSource
	{
		private Dictionary<string, Thing> _classes = new Dictionary<string, Thing>();
		private Dictionary<Int32Guid, ThingInstance> _instances = new Dictionary<Int32Guid, ThingInstance>();

		public void DefineClass(string name, ThingAttribute attribs = ThingAttribute.NONE)
		{
			if (_classes.ContainsKey(name))
			{
				return;
			}

			Thing it = new Thing() { ClassName = name };
			_classes.Add(name, it);
		}

		public void DefineClass(string name, string baseClass, ThingAttribute attribs = ThingAttribute.NONE)
		{
			DefineClass(name, attribs);

			var it = _classes[name];
			it.Super = _classes[baseClass];
		}

		public void DefineSlot(string className, string slotName)
		{
			var it = _classes[className];
			if (! it.Slots.ContainsKey(slotName))
			{
				it.Slots.Add(slotName, new Slot() { Name = slotName });
			}
		}

		public void DefineSlotGuard(string className, string slotName, ISlotGuard guard)
		{
			var it = _classes[className];
			var slot = it.Slots[slotName];
			slot.LinkValidators.Add(guard);
		}

		//public void DefineValue(string className, string valueName, SimpleVariant.VariantType type)
		//{
		//	DefineValue(className, valueName, type);
		//}

		public void DefineValue(string className, string valueName, VariantType type, float value)
		{
			DefineValue(className, valueName, type, value);
		}

		public void DefineValue(string className, string valueName, VariantType type, float start = 0f, bool useRange = false, bool randStart = false, float min = Single.MinValue, float max = Single.MaxValue)
		{
			var it = _classes[className];

			if (it.Values.ContainsKey(valueName))
			{
				return;
			}

			it.Values.Add(valueName, new ValueSlot() { Name = valueName, DataType = type, Start = start, UseMinMaxRange = useRange, StartRandomize = randStart, Min = min, Max = max });
		}

		public ThingInstance Create(string className)
		{
			var it = _classes[className];

			var inst = new ThingInstance() { MyClass = it };

			InitInstance(inst, it);

			_instances.Add(inst.InstanceId, inst);

			return inst;
		}

		private void InitInstance(ThingInstance inst, Thing cls)
		{
			foreach (var slot in cls.Slots.Values)
			{
				if (inst.Slots.ContainsKey(slot.Name))
				{
					continue;
				}

				inst.Slots.Add(slot.Name, new SlotInstance() { Holder = inst, SlotClass = slot });
			}

			foreach (var val in cls.Values.Values)
			{
				if (inst.ValueSlots.ContainsKey(val.Name))
				{
					continue;
				}

				inst.ValueSlots.Add(val.Name, new ValueSlotInstance(val));
			}

			if (cls.Super != null)
			{
				InitInstance(inst, cls.Super);
			}
		}

		public ThingInstance Select(Int32Guid id)
		{
			return _instances[id];
		}

		public IEnumerable Query(DataCommand cmd)
		{
			return cmd.Query(_instances.Values);
		}
	}
}
