using System;
using System.Data;

namespace Portland.Interp
{
	public interface IVariant : IEquatable<IVariant>, IComparable<IVariant>
	{
		IVariant this[int index] { get; set; }
		IVariant this[string index] { get; set; }

		//int Count { get; }
		VtType DataType { get; }
		int Length { get; }

		int AsInt();
		DateTime AsDateTime();
		float AsFloat();
		void Clear();
		void ClearProp(string name);
		void ClearPropArray(string name, int size);
		void CopyTo(IVariant v);
		void Dispose();
		IVariant Dup();
		bool Equals(object obj);
		bool Equals(string s);
		int GetHashCode();
		bool HasProp(string name);
		bool IsArray();
		bool IsBool();
		bool IsDateTime();
		bool IsError();
		bool IsNull();
		bool IsNullOrError();
		bool IsNumeric();
		bool IsReal();
		bool IsString();
		bool IsWholeNumber();
		void Set(bool i);
		void Set(float d);
		void Set(in DateTime dtm);
		void Set(in IVariant v);
		void Set(int i);
		void Set(string s);
		void SetError();
		void SetNull();
		void SetProp(IVariant name, in IVariant value);
		void SetPropArray(string name, IVariant index, in IVariant value);
		string ToString();
		int ToInt();
		float ToFloat();
		bool ToBool();
		bool TryGetProp(IVariant name, out IVariant value);

		IVariant MathAdd(IVariant rhs);
		IVariant MathSub(IVariant rhs);
		IVariant MathMul(IVariant rhs);
		IVariant MathDiv(IVariant rhs);
		IVariant MathMod(IVariant rhs);
		IVariant MathNeg();
	}

	public class NativeVariantBase : IVariant
	{
		public virtual IVariant this[int index] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
		public virtual IVariant this[string index] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		public virtual VtType DataType { get; set; }
		public virtual int Length { get; set; }

		public virtual DateTime AsDateTime()
		{
			throw new NotImplementedException();
		}

		public virtual float AsFloat()
		{
			throw new NotImplementedException();
		}

		public virtual int AsInt()
		{
			throw new NotImplementedException();
		}

		public virtual void Clear()
		{
		}

		public virtual void ClearProp(string name)
		{
			throw new NotImplementedException();
		}

		public virtual void ClearPropArray(string name, int size)
		{
			throw new NotImplementedException();
		}

		public virtual int CompareTo(IVariant other)
		{
			throw new NotImplementedException();
		}

		public virtual void CopyTo(IVariant v)
		{
			throw new NotImplementedException();
		}

		public virtual void Dispose()
		{
		}

		public virtual IVariant Dup()
		{
			throw new NotImplementedException();
		}

		public virtual bool Equals(string s)
		{
			return false;
		}

		public virtual bool Equals(IVariant other)
		{
			return false;
		}

		public virtual bool HasProp(string name)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsArray()
		{
			return false;
		}

		public virtual bool IsBool()
		{
			return false;
		}

		public virtual bool IsDateTime()
		{
			return false;
		}

		public virtual bool IsError()
		{
			return false;
		}

		public virtual bool IsNull()
		{
			return false;
		}

		public virtual bool IsNullOrError()
		{
			return false;
		}

		public virtual bool IsNumeric()
		{
			return false;
		}

		public virtual bool IsReal()
		{
			return false;
		}

		public virtual bool IsString()
		{
			return false;
		}

		public virtual bool IsWholeNumber()
		{
			return false;
		}

		public virtual IVariant MathAdd(IVariant rhs)
		{
			throw new NotImplementedException();
		}

		public virtual IVariant MathDiv(IVariant rhs)
		{
			throw new NotImplementedException();
		}

		public virtual IVariant MathMod(IVariant rhs)
		{
			throw new NotImplementedException();
		}

		public virtual IVariant MathMul(IVariant rhs)
		{
			throw new NotImplementedException();
		}

		public virtual IVariant MathNeg()
		{
			throw new NotImplementedException();
		}

		public virtual IVariant MathSub(IVariant rhs)
		{
			throw new NotImplementedException();
		}

		public virtual void Set(bool i)
		{
			throw new NotImplementedException();
		}

		public virtual void Set(float d)
		{
			throw new NotImplementedException();
		}

		public virtual void Set(in DateTime dtm)
		{
			throw new NotImplementedException();
		}

		public virtual void Set(in IVariant v)
		{
			throw new NotImplementedException();
		}

		public virtual void Set(int i)
		{
			throw new NotImplementedException();
		}

		public virtual void Set(string s)
		{
			throw new NotImplementedException();
		}

		public virtual void SetError()
		{
			throw new NotImplementedException();
		}

		public virtual void SetNull()
		{
			throw new NotImplementedException();
		}

		public virtual void SetProp(IVariant name, in IVariant value)
		{
			throw new NotImplementedException();
		}

		public virtual void SetPropArray(string name, IVariant index, in IVariant value)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return "<<NATIVE>>";
		}

		public virtual bool ToBool()
		{
			throw new NotImplementedException();
		}

		public virtual float ToFloat()
		{
			throw new NotImplementedException();
		}

		public virtual int ToInt()
		{
			throw new NotImplementedException();
		}

		public virtual bool TryGetProp(IVariant name, out IVariant value)
		{
			value = null;
			return false;
		}
	}
}