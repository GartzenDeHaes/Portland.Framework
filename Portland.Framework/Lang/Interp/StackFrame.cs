using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Interp
{
	public struct StackFrame
	{
		public Variant Data;

		//public Variant ReturnValue { get { return Data; } }

		public IVariant this[int index]
		{
			get { return Data[index];}
		}

		public IVariant this[string key]
		{
			get { return Data[key]; }
		}

		public void SetProp(string key, in IVariant value)
		{
			Data.SetProp(new Variant(key), value);
		}

		public bool HasProp(string name)
		{
			return Data.HasProp(name);
		}

		public bool TryGetProp(string name, out IVariant value)
		{
			return Data.TryGetProp(new Variant(name), out value);
		}

		public void SetPropArray(string name, IVariant index, in IVariant value)
		{
			Data.SetPropArray(name, index, value);
		}

		public void ClearPropArray(string name, int size)
		{
			Data.ClearPropArray(name, size);
		}

		public void ClearProp(string name)
		{
			Data.ClearProp(name);
		}

		public static StackFrame Create()
		{
			return new StackFrame { Data = new Variant() };
		}
	}
}
