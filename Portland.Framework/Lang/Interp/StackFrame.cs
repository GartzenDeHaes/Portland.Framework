using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Interp
{
	public struct StackFrame
	{
		Variant Data;

		//public Variant ReturnValue { get { return Data; } }

		public Variant this[int index]
		{
			get { return Data[index];}
		}

		public Variant this[string key]
		{
			get { return Data[key]; }
		}

		public void SetProp(string key, in Variant value)
		{
			Data.SetProp(key, value);
		}

		public bool HasProp(string name)
		{
			return Data.HasProp(name);
		}

		public bool TryGetProp(string name, out Variant value)
		{
			return Data.TryGetProp(name, out value);
		}

		public void SetReturnValue(float val)
		{
			Data.Set(val);
		}

		public void SetReturnValue(int val)
		{
			Data.Set(val);
		}

		public void SetReturnValue(string val)
		{
			Data.Set(val);
		}

		public void SetReturnValue(in Variant val)
		{
			Data.Set(val);
		}

		public Variant GetReturnValue()
		{
			return Data;
		}

		public void SetPropArray(string name, string index, in Variant value)
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
