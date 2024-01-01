using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using Portland.Collections;
using Portland.Mathmatics;
using Portland.Text;

namespace Portland.Interp
{
	public enum VtType : short
	{
		VT_EMPTY,
		VT_ERROR,
		VT_INT,
		VT_REAL,
		VT_STRING,
		//VT_IIDENTITY,
		VT_KEYVALUE,
		VT_DATETIME,
		//VT_FVEC,
		VT_BOOL,
		VT_ARRAY
	}

	//[Serializable]
	public sealed class Variant : IPoolable, IDisposable, IVariant
	//public struct Variant : IEquatable<Variant>, IComparable<Variant>//, IPoolable, IDisposable
	{
		//private static SimplePool<float> _vecpool = new SimplePool<float>();

		//private Dictionary<string, Variant> _proplst;

		private VtType _type;
		private IntFloat _ix;
		//private int _i;
		//private float _x,
		//private float _y, _z;
		private object _o = null;

		public VtType DataType
		{
			get
			{
				return _type;
			}
		}

		//public string Key
		//{
		//	get
		//	{
		//		if (_type == VtType.VT_KEYVALUE)
		//		{

		//			return ((Dictionary<string, Variant>)_o).Keys.First();
		//		}

		//		return ToString();
		//	}
		//}

		public int Length
		{
			get
			{
				switch (_type)
				{
					case VtType.VT_BOOL:
					case VtType.VT_INT:
					case VtType.VT_REAL:
						return 4;
					case VtType.VT_STRING:
						return ((string)_o).Length;
					case VtType.VT_ERROR:
						return 0;
					//case VtType.VT_IIDENTITY:
					case VtType.VT_DATETIME:
						return 8;
					case VtType.VT_ARRAY:
						return ((Vector<IVariant>)_o).Count;
					//case VtType.VT_KEYVALUE:
					//	return _proplst.Count;
					//case VtType.VT_FVEC:
					//	return 12;
					default:
						if (_o is Dictionary<string, IVariant> props)
						{
							return props.Count;
						}
						return 0;
				}
			}
		}

		public Variant()
		{
			_ix = default(IntFloat);
			_type = VtType.VT_EMPTY;
		}

		public Variant(int i)
		{
			_ix = default(IntFloat);
			_ix.IntValue = i;
			_type = VtType.VT_INT;
		}

		public Variant(bool i)
		{
			_ix = default(IntFloat);
			_ix.IntValue = i ? 1 : 0;
			_type = VtType.VT_BOOL;
		}

		public Variant(float d)
		{
			_ix = default(IntFloat);
			_ix.FloatValue = d;
			_type = VtType.VT_REAL;
		}

		public Variant(string s)
		{
			_ix = default(IntFloat);
			_o = s;
			_type = VtType.VT_STRING;
		}

		public static Variant CreateArray(int size)
		{
			var v = new Variant();
			v._type = VtType.VT_ARRAY;
			var vec = new Vector<IVariant>(size);
			for (int i = 0; i < size; ++i)
			{
				vec.Add(new Variant());
			}
			
			v._o = vec;
			return v;
		}

		//public Variant(IIdentity uid)
		//{
		//	_o = uid;
		//	_type = VtType.VT_IIDENTITY;
		//}

		public static Variant GetError()
		{
			Variant er = new Variant();
			er.SetError();
			return er;
		}

		private void EnsureProps()
		{
			if (_type == VtType.VT_ARRAY)
			{
				throw new ArrayTypeMismatchException("Cannot convert map to arrray");
			}
			if (_o == null || _o is String)
			{
				_o = new Dictionary<string, IVariant>();
				_type = VtType.VT_KEYVALUE;
			}
		}

		public bool HasProp(string name)
		{
			if (_o == null)
			{
				return false;
			}

			return ((Dictionary<string, IVariant>)_o).ContainsKey(name);
		}

		public bool TryGetProp(IVariant name, out IVariant value)
		{
			if (_o == null)
			{
				value = default(Variant);
				return false;
			}
			if (_type == VtType.VT_ARRAY)
			{
				value = ((Vector<IVariant>)_o).ElementAt(name.ToInt());
				return true;
			}
			return ((Dictionary<string, IVariant>)_o).TryGetValue(name.ToString(), out value);
		}

		public void SetProp(IVariant vname, in IVariant value)
		{
			//Debug.Assert(! ReferenceEquals(value, this));

			if (_type == VtType.VT_ARRAY)
			{
				((Vector<IVariant>)_o).SetElementAt(vname.ToInt(), value);
				return;
			}

			EnsureProps();

			var name = vname.ToString();
			var o = (Dictionary<string, IVariant>)_o;

			if (o.ContainsKey(name))
			{
				o[name] = value;
			}
			else
			{
				o.Add(name, value);
			}
		}

		public void ClearProp(string name)
		{
			EnsureProps();

			var o = (Dictionary<string, IVariant>)_o;

			if (o.ContainsKey(name))
			{
				o[name].Clear();
			}
			else
			{
				o.Add(name, new Variant());
			}
		}

		public void SetPropArray(string name, IVariant index, in IVariant value)
		{
			//Debug.Assert(!ReferenceEquals(value, this));

			EnsureProps();

			var o = (Dictionary<string, IVariant>)_o;

			if (!o.ContainsKey(name))
			{
				o.Add(name, new Variant());
			}

			((Dictionary<string, IVariant>)_o)[name].SetProp(index, value);
		}

		public void ClearPropArray(string name, int size)
		{
			EnsureProps();

			var proplst = ((Dictionary<string, IVariant>)_o);

			if (!proplst.ContainsKey(name))
			{
				proplst.Add(name, Variant.CreateArray(size));
			}
			else
			{
				proplst[name] = Variant.CreateArray(size);
			}

			//for (int i = 0; i < size; i++)
			//{
			//	proplst[name].SetProp(new Variant(i), new Variant());
			//}
		}

		public IVariant this[string index]
		{
			get
			{
				EnsureProps();

				var o = (Dictionary<string, IVariant>)_o;
				if (!o.ContainsKey(index))
				{
					o.Add(index, new Variant());
				}
				return o[index];
			}
			set
			{
				EnsureProps();

				var o = (Dictionary<string, IVariant>)_o;

				if (o.ContainsKey(index))
				{
					o[index] = value;
				}
				else
				{
					o.Add(index, value);
				}
			}
		}

		public IVariant this[int index]
		{
			get
			{
				if (_o is Dictionary<string, IVariant> dict)
				{
					return this[index.ToString()];
				}
				if (_type != VtType.VT_ARRAY)
				{
					_type = VtType.VT_ARRAY;
					_ix.IntValue = 0;
					_o = new Vector<IVariant>();
				}
				return ((Vector<IVariant>)_o).ElementAt(index);
			}
			set 
			{
				if (_o is Dictionary<string, IVariant> dict)
				{
					this[index.ToString()] = value;
				}
				if (_type != VtType.VT_ARRAY)
				{
					_type = VtType.VT_ARRAY;
					_ix.IntValue = 0;
					_o = new Vector<IVariant>();
				}
				((Vector<IVariant>)_o).SetElementAt(index, value);
			}
		}

		//public int Count
		//{
		//	get
		//	{
		//		if (_o is String s)
		//		{
		//			return s.Length;
		//		}
		//		if (_o is Dictionary<string, IVariant> dict)
		//		{
		//			return dict.Count;
		//		}
		//		else if (_type == VtType.VT_STRING)
		//		{
		//			return ((string)_o).Length;
		//		}
		//		return 0;
		//	}
		//}

		public void Set(bool i)
		{
			Clear();
			_ix.IntValue = i ? 1 : 0;
			_type = VtType.VT_BOOL;
		}

		public void Set(int i)
		{
			Clear();
			_ix.IntValue = i;
			_type = VtType.VT_INT;
		}

		public void Set(in IVariant v)
		{
			//v.CopyTo(ref this);
			v.CopyTo(this);
		}

		public void Set(float d)
		{
			Clear();
			_ix.FloatValue = d;
			_type = VtType.VT_REAL;
		}

		public void Set(string s)
		{
			Clear();
			_o = s;
			_type = VtType.VT_STRING;
		}

		public void Set(in DateTime dtm)
		{
			Clear();
			_type = VtType.VT_DATETIME;
			_o = dtm;
		}

		public void SetError()
		{
			Clear();
			_type = VtType.VT_ERROR;
		}

		public void SetNull()
		{
			Clear();
			_o = null;
			_type = VtType.VT_EMPTY;
		}

		//public void Set(IIdentity uid)
		//{
		//	Clear();
		//	_type = VtType.VT_IIDENTITY;
		//	_o = uid;
		//}

		//public void Set(string key, string value)
		//{
		//	Clear();
		//	_type = VtType.VT_KEYVALUE;
		//	SetProp(key, value);
		//}

		//public void Set(float x, float y, float z)
		//{
		//	Clear();
		//	_type = VtType.VT_FVEC;
		//	_ix.FloatValue = x;
		//	_y = y;
		//	_z = z;
		//}

		public void Clear()
		{
			_type = VtType.VT_EMPTY;
			_o = null;
			_ix.IntValue = 0;
			//_y = _z = 0f;

			//if (_proplst != null)
			//{
			//	_proplst.Clear();
			//}
		}

		public bool IsNull()
		{
			return _type == VtType.VT_EMPTY;
		}

		public bool IsError()
		{
			return _type == VtType.VT_ERROR;
		}

		public bool IsNullOrError()
		{
			return _type == VtType.VT_EMPTY || _type == VtType.VT_ERROR;
		}

		public bool IsNumeric()
		{
			return _type == VtType.VT_INT || _type == VtType.VT_REAL;
		}

		public bool IsString()
		{
			return _type == VtType.VT_STRING;
		}

		public bool IsWholeNumber()
		{
			return _type == VtType.VT_INT;
		}

		public bool IsReal()
		{
			return _type == VtType.VT_REAL;
		}

		//public bool IsIdentity()
		//{
		//	return _type == VtType.VT_IIDENTITY;
		//}

		//public bool IsKeyValue()
		//{
		//	return _type == VtType.VT_KEYVALUE;
		//}

		public bool IsDateTime()
		{
			return _type == VtType.VT_DATETIME;
		}

		public bool IsArray()
		{
			return _o is Dictionary<string, Variant>;
		}

		public bool IsBool()
		{
			return _type == VtType.VT_BOOL;
		}

		public void CopyTo(IVariant v)
		{
			v.Clear();

			switch (_type)
			{
				case VtType.VT_EMPTY:
					break;
				case VtType.VT_ERROR:
					v.SetError();
					break;
				case VtType.VT_INT:
					v.Set(_ix.IntValue);
					break;
				case VtType.VT_REAL:
					v.Set(_ix.FloatValue);
					break;
				case VtType.VT_STRING:
					v.Set((string)_o);
					break;
				case VtType.VT_DATETIME:
					v.Set((DateTime)_o);
					break;
				case VtType.VT_BOOL:
					v.Set(_ix.IntValue != 0);
					break;
				case VtType.VT_ARRAY:
					for (int i = 0; i < ((Vector<IVariant>)_o).Count; ++i)
					{
						v[i] = this[i];
					}
					return;
			}

			if (_o is Dictionary<string, Variant> dict)
			{
				foreach (var prop in dict.Keys)
				{
					v[prop] = dict[prop].Dup();
				}
			}
		}

		// for new
		public IVariant Dup()
		{
			Variant v = new Variant();
			//CopyTo(ref v);
			CopyTo(v);
			return v;
		}

		public int AsInt()
		{
			return (int)this;
		}

		public DateTime AsDateTime()
		{
			return (DateTime)_o;
		}
		
		public float AsFloat()
		{
			return _ix.FloatValue;
		}

		//public IIdentity AsIdentity()
		//{
		//	return _o as IIdentity;
		//}

		//public void AsFloatVector(out float x, out float y, out float z)
		//{
		//	if (_type == VtType.VT_FVEC)
		//	{
		//		x = _ix.FloatValue;
		//		y = _y;
		//		z = _z;
		//		return;
		//	}

		//	x = 0f;
		//	y = 0f;
		//	z = 0f;
		//}

		//public void AsFloatVector(out float x, out float y)
		//{
		//	float z;
		//	AsFloatVector(out x, out y, out z);
		//}

		#region Conversions

		//public static implicit operator Variant(string s)
		//{
		//	return new Variant(s);
		//}
		//public static implicit operator string(in Variant v)
		//{
		//	return v.ToString();
		//}

		public static implicit operator Variant(in DateTime v)
		{
			Variant ret = new Variant();
			ret.Set(v.ToString());
			return ret;
		}
		public static implicit operator Variant(DateTime? v)
		{
			Variant ret = new Variant();
			if (!v.HasValue)
			{
				return ret;
			}

			ret.Set(v.Value);
			return ret;
		}
		public static implicit operator DateTime(in Variant v)
		{
			if (v._type == VtType.VT_DATETIME)
			{
				return (DateTime)v._o;
			}
			if (v._type == VtType.VT_STRING)
			{
				DateTime dtm;
				if (DateTime.TryParse((string)v._o, out dtm))
				{
					return dtm;
				}
			}

			return DateTime.MinValue;
		}
		public static implicit operator DateTime?(in Variant v)
		{
			if (v._type == VtType.VT_DATETIME)
			{
				return (DateTime)v._o;
			}
			if (v._type == VtType.VT_STRING)
			{
				DateTime o;
				if (DateTime.TryParse(v.ToString(), out o))
					return o;
				return null;
			}

			return null;
		}

		public static implicit operator Variant(int? v)
		{
			Variant ret = new Variant();

			if (v.HasValue)
			{
				ret.Set((int)v);
			}
			return ret;
		}
		public static implicit operator int(in Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return v._ix.IntValue;
			}
			if (v._type == VtType.VT_REAL)
			{
				return (int)v._ix.FloatValue;
			}
			if (v._type == VtType.VT_STRING)
			{
				string val = (string)v._o;
				if (StringHelper.IsInt(val))
				{
					return Int32.Parse(val);
				}
				if (StringHelper.IsNumeric(val))
				{
					return (int)Double.Parse(val);
				}
			}

			return 0;
		}
		public static implicit operator int?(in Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return v._ix.IntValue;
			}
			if (v._type == VtType.VT_REAL)
			{
				return (int)v._ix.FloatValue;
			}
			if (v._type == VtType.VT_STRING)
			{
				string val = (string)v._o;
				if (StringHelper.IsInt(val))
				{
					return Int32.Parse(val);
				}
				if (StringHelper.IsNumeric(val))
				{
					return (int)Double.Parse(val);
				}
			}

			return null;
		}

		public static implicit operator Variant(float? v)
		{
			Variant ret = new Variant();
			if (v.HasValue)
			{
				ret.Set((float)v);
			}
			return ret;
		}
		public static implicit operator float(in Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return (float)v._ix.IntValue;
			}
			if (v._type == VtType.VT_REAL)
			{
				return v._ix.FloatValue;
			}
			if (v._type == VtType.VT_STRING)
			{
				string val = (string)v._o;
				if (StringHelper.IsInt(val))
				{
					return (float)Int32.Parse(val);
				}
				if (StringHelper.IsNumeric(val))
				{
					return Single.Parse(val);
				}
			}

			return Single.NaN;
		}
		public static implicit operator float?(in Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return (float)v._ix.IntValue;
			}
			if (v._type == VtType.VT_REAL)
			{
				return v._ix.FloatValue;
			}
			if (v._type == VtType.VT_STRING)
			{
				string val = (string)v._o;
				if (StringHelper.IsInt(val))
				{
					return (float)Int32.Parse(val);
				}
				if (StringHelper.IsNumeric(val))
				{
					return Single.Parse(val);
				}
			}

			return null;
		}

		public static implicit operator Variant(bool? v)
		{
			Variant ret = new Variant();
			ret.Set(v.GetValueOrDefault() ? true : false);
			return ret;
		}
		public static implicit operator bool(Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return v._ix.IntValue != 0;
			}
			if (v._type == VtType.VT_REAL)
			{
				return v._ix.FloatValue != 0f;
			}
			if (v._type == VtType.VT_STRING)
			{
				return v._o.Equals("TRUE") || v._o.Equals("true");
			}

			return false;
		}

		public static implicit operator Variant(List<string> v)
		{
			Variant ret = new Variant();
			int count = 0;

			foreach (var s in v)
			{
				ret[count++] = new Variant(s);
			}

			return ret;
		}

		public static implicit operator List<string>(in Variant v)
		{
			var lst = new List<string>();

			if (v._o is Dictionary<string, Variant> proplst)
			{
				foreach (var key in proplst.Keys)
				{
					if (StringHelper.IsInt(key))
					{
						lst.Add(proplst[key].ToString());
					}
				}
			}

			return lst;
		}

		#endregion

		public static Variant Parse(string value)
		{
			Variant ret = new Variant();

			if (value == null)
			{
				return ret;
			}

			if (StringHelper.IsInt(value) || StringHelper.IsHexNum(value))
			{
				ret.Set(Int32.Parse(value));
				return ret;
			}
			else if (StringHelper.IsNumeric(value))
			{
				ret.Set(Single.Parse(value));
				return ret;
			}
			else if (value.IndexOf('-') > -1 || value.IndexOf('/') > -1)
			{
				DateTime dtm;
				if (DateTime.TryParse(value, out dtm))
				{
					ret.Set(dtm);
					return ret;
				}
			}

			if (value.Length == 4 && (value.Equals("null") || value.Equals("NULL")))
			{
				return ret;
			}
			if (value.Length == 4 && (value.Equals("true") || value.Equals("TRUE")))
			{
				ret.Set(true);
				return ret;
			}
			if (value.Length == 5 && (value.Equals("false") || value.Equals("FALSE")))
			{
				ret.Set(false);
				return ret;
			}

			if (value.Length > 2 && value[0] == '{' && value.EndsWith("]}"))
			{
				// JSON
				Debug.WriteLine("TODO: Variant JSON in Parse()");
			}

			if (value.Length > 6 && value[0] == '[' && value.EndsWith("]") && StringHelper.CountOccurancesOf(value, ',') == 3)
			{
				Debug.WriteLine("TODO: Variant Vector in Parse()");
			}

			if (StringHelper.CountOccurancesOf(value, '=') == 1)
			{
				var parts = value.Split('=');
				ret[parts[0]] = new Variant(parts[1]);
				return ret;
			}

			ret.Set(value);

			return ret;
		}

		public override string ToString()
		{
			switch (_type)
			{
				case VtType.VT_BOOL:
					return _ix.IntValue == 0 ? "FALSE" : "TRUE";
				case VtType.VT_INT:
					return _ix.IntValue.ToString();
				case VtType.VT_REAL:
					return _ix.FloatValue.ToString();
				case VtType.VT_STRING:
					return (string)_o;
				case VtType.VT_ERROR:
					return "UNDEFINED";
				//case VtType.VT_IIDENTITY:
				case VtType.VT_DATETIME:
					return _o.ToString();

					//case VtType.VT_KEYVALUE:
					//{
					//	var key = _proplst.Keys.First();
					//	var val = _proplst[key];
					//	return key + "=" + val;
					//}
					//case VtType.VT_FVEC:
					//	return "[" + _ix.FloatValue + "," + _y + "," + _z + "]";
			}

			if (_type == VtType.VT_KEYVALUE)
			{
				var proplst = (Dictionary<string, IVariant>)_o;
				StringBuilder buf = new StringBuilder();
				bool addComma = false;

				buf.Append('[');

				foreach (var key in proplst.Keys)
				{
					if (addComma)
					{
						buf.Append(',');
					}
					else
					{
						addComma = true;
					}
					buf.Append(key);
					buf.Append("=\"");
					buf.Append(proplst[key].ToString());
					buf.Append('"');
				}

				buf.Append(']');

				return buf.ToString();
			}
			else if (_type == VtType.VT_ARRAY)
			{
				StringBuilder buf = new StringBuilder();
				bool addComma = false;

				buf.Append('[');

				var vec = (Vector<IVariant>)_o;
				for(int i = 0; i < vec.Count; ++i)
				{
					var val = vec[i];
					if (addComma)
					{
						buf.Append(',');
					}
					else
					{
						addComma = true;
					}
					buf.Append(i);
					buf.Append("=\"");
					buf.Append(val.ToString());
					buf.Append('"');
				}

				buf.Append(']');

				return buf.ToString();
			}

			return "NULL";
		}

		public int ToInt()
		{
			switch (_type)
			{
				case VtType.VT_EMPTY:
					return 0;
				case VtType.VT_ERROR:
					return -1;
				case VtType.VT_INT:
				case VtType.VT_BOOL:
					return _ix.IntValue;
				case VtType.VT_REAL:
					return (int)_ix.FloatValue;
				case VtType.VT_STRING:
					if (StringHelper.IsInt((string)_o))
					{
						return Int32.Parse((string)_o);
					}
					return -1;
				case VtType.VT_DATETIME:
					unchecked
					{
						return (int)((DateTime)_o).Ticks;
					}
			}
			return -1;
		}

		public float ToFloat()
		{
			switch (_type)
			{
				case VtType.VT_EMPTY:
					return 0;
				case VtType.VT_ERROR:
					return -1;
				case VtType.VT_INT:
				case VtType.VT_BOOL:
					return _ix.IntValue;
				case VtType.VT_REAL:
					return _ix.FloatValue;
				case VtType.VT_STRING:
					if (StringHelper.IsNumeric((string)_o))
					{
						return Single.Parse((string)_o);
					}
					return -1;
				case VtType.VT_DATETIME:
					unchecked
					{
						return (float)((DateTime)_o).Ticks;
					}
			}
			return -1;
		}

		public bool ToBool()
		{
			switch (_type)
			{
				case VtType.VT_EMPTY:
					return false;
				case VtType.VT_ERROR:
					return false;
				case VtType.VT_INT:
				case VtType.VT_BOOL:
					return _ix.IntValue != 0;
				case VtType.VT_REAL:
					return _ix.FloatValue != 0;
				default:
					return _o != null;
			}
		}

		//public string ToJSON()
		//{
		//	StringBuilder buf = new StringBuilder();
		//	buf.Append('{');
		//	buf.Append('"');
		//	buf.Append(ToString());
		//	buf.Append('"');
		//	buf.Append(":[{");

		//	bool comma = false;

		//	if (_proplst != null)
		//	{
		//		foreach(var key in _proplst.Keys)
		//		{
		//			if (comma)
		//			{
		//				buf.Append(',');
		//			}
		//			else
		//			{
		//				comma = true;
		//			}

		//			buf.Append('"');
		//			buf.Append(key);
		//			buf.Append('"');
		//			buf.Append(':');
		//			if (_proplst[key].Count > 0)
		//			{
		//				buf.Append(_proplst[key].ToJSON());
		//			}
		//			else
		//			{
		//				buf.Append('"');
		//				buf.Append(_proplst[key].ToString());
		//				buf.Append('"');
		//			}
		//		}
		//	}

		//	buf.Append("}]}");

		//	return buf.ToString();
		//}

		//public static Variant ParseJSON(string json)
		//{
		//	JSONLex lex = new JSONLex(json);
		//	lex.Next();
		//	return ParseJSON(lex);
		//}

		//private static void Match(JSONLex lex, JSONLexToken token)
		//{
		//	if (lex.Token != token)
		//	{
		//		throw new FormatException("Expected JSON " + token);
		//	}

		//	lex.Next();
		//}

		//static Variant ParseJSON(JSONLex lex)
		//{
		//	Match(lex, JSONLexToken.LBRACE);

		//	Variant v = Variant.Parse(lex.Lexum);

		//	Match(lex, JSONLexToken.STRING);

		//	Match(lex, JSONLexToken.COLON);
		//	Match(lex, JSONLexToken.LBRACK);
		//	Match(lex, JSONLexToken.LBRACE);

		//	while (lex.Token != JSONLexToken.RBRACE)
		//	{
		//		string key = lex.Lexum;

		//		Match(lex, JSONLexToken.STRING);
		//		Match(lex, JSONLexToken.COLON);

		//		if (lex.Token == JSONLexToken.STRING)
		//		{
		//			if (StringHelper.IsInt(key))
		//			{
		//				v[Int32.Parse(key)] = Variant.Parse(lex.Lexum);
		//			}
		//			else
		//			{
		//				v.SetProp(key, Variant.Parse(lex.Lexum));
		//			}

		//			Match(lex, JSONLexToken.STRING);
		//		}
		//		else
		//		{
		//			if (StringHelper.IsInt(key))
		//			{
		//				v[Int32.Parse(key)] = ParseJSON(lex);
		//			}
		//			else
		//			{
		//				v.SetProp(key, ParseJSON(lex));
		//			}
		//		}

		//		if (lex.Token == JSONLexToken.COMMA)
		//		{
		//			lex.Next();
		//		}
		//	}

		//	Match(lex, JSONLexToken.RBRACE);
		//	Match(lex, JSONLexToken.RBRACK);
		//	Match(lex, JSONLexToken.RBRACE);

		//	return v;
		//}

		//public void ToBin(byte[] data, ref int pos)
		//{
		//	data[pos++] = (byte)_type;

		//	switch (_type)
		//	{
		//		case VtType.VT_BOOL:
		//			BitConverterNoAlloc.GetBytes(_ix.IntValue, data, ref pos);
		//			break;
		//		case VtType.VT_INT:
		//			BitConverterNoAlloc.GetBytes(_ix.IntValue, data, ref pos);
		//			break;
		//		case VtType.VT_REAL:
		//			BitConverterNoAlloc.GetBytes(_z, data, ref pos);
		//			break;
		//		case VtType.VT_EMPTY:
		//			break;
		//		case VtType.VT_FVEC:
		//			BitConverterNoAlloc.GetBytes(_ix.FloatValue, data, ref pos);
		//			BitConverterNoAlloc.GetBytes(_y, data, ref pos);
		//			BitConverterNoAlloc.GetBytes(_z, data, ref pos);
		//			break;
		//		case VtType.VT_KEYVALUE:
		//			break;
		//		case VtType.VT_ERROR:
		//			break;
		//		case VtType.VT_STRING:
		//			BitConverterNoAlloc.GetBytes((string)_o, data, ref pos);
		//			break;
		//		case VtType.VT_IIDENTITY:
		//			throw new InvalidOperationException("Not serializing Identity");
		//		case VtType.VT_DATETIME:
		//			BitConverterNoAlloc.GetBytes(((DateTime)_o).Ticks, data, ref pos);
		//			break;
		//	}

		//	int count = 0;
		//	if (_proplst != null)
		//	{
		//		count = _proplst.Count;
		//	}

		//	BitConverterNoAlloc.GetBytes((short)count, data, ref pos);

		//	if (count == 0)
		//	{
		//		return;
		//	}

		//	foreach (var key in _proplst.Keys)
		//	{
		//		BitConverterNoAlloc.GetBytes(key, data, ref pos);
		//		_proplst[key].ToBin(data, ref pos);
		//	}
		//}

		//public static Variant ParseBin(byte[] data, ref int pos)
		//{
		//	return ParseBin(data, ref pos, new Variant());
		//}

		//public static Variant ParseBin(byte[] data, ref int pos, Variant v)
		//{
		//	StringBuilder sbuf = null;
		//	v._type = (VtType)data[pos++];

		//	switch (v._type)
		//	{
		//		case VtType.VT_BOOL:
		//			v._ix.IntValue = BitConverter.ToInt32(data, pos);
		//			pos += 4;
		//			break;
		//		case VtType.VT_INT:
		//			v._ix.IntValue = BitConverter.ToInt32(data, pos);
		//			pos += 4;
		//			break;
		//		case VtType.VT_REAL:
		//			v._z = BitConverter.ToSingle(data, pos);
		//			pos += 8;
		//			break;
		//		case VtType.VT_EMPTY:
		//			break;
		//		case VtType.VT_FVEC:
		//			v._ix.FloatValue = BitConverter.ToSingle(data, pos);
		//			pos += 4;
		//			v._y = BitConverter.ToSingle(data, pos);
		//			pos += 4;
		//			v._z = BitConverter.ToSingle(data, pos);
		//			pos += 8;
		//			break;
		//		case VtType.VT_KEYVALUE:
		//			break;
		//		case VtType.VT_ERROR:
		//			break;
		//		case VtType.VT_STRING:
		//			sbuf = new StringBuilder();
		//			v._o = BitConverterNoAlloc.ToString(data, ref pos, sbuf);
		//			break;
		//		case VtType.VT_IIDENTITY:
		//			throw new InvalidOperationException("Not serializing Identity");
		//		case VtType.VT_DATETIME:
		//			v._o = new DateTime(BitConverter.ToInt64(data, pos));
		//			pos += 8;
		//			break;
		//	}

		//	int count = BitConverter.ToInt16(data, pos);
		//	pos += 2;

		//	if (count == 0)
		//	{
		//		return v;
		//	}

		//	if (sbuf == null)
		//	{
		//		sbuf = new StringBuilder();
		//	}

		//	v._proplst = new Dictionary<string, Variant>();

		//	for (int x = 0; x < count; x++)
		//	{
		//		string key = BitConverterNoAlloc.ToString(data, ref pos, sbuf);
		//		v._proplst.Add(key, Variant.ParseBin(data, ref pos));
		//	}

		//	return v;
		//}

		public void Dispose()
		{
			Clear();
			//_proplst = null;
		}

		#region Compare

		public static bool operator ==(in Variant a, in Variant b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(in Variant a, in Variant b)
		{
			return !(a == b);
		}

		public bool Equals(string s)
		{
			bool ret = false;
			switch (_type)
			{
				case VtType.VT_EMPTY:
					break;
				case VtType.VT_ERROR:
					break;
				case VtType.VT_INT:
					break;
				case VtType.VT_REAL:
					break;
				case VtType.VT_STRING:
					ret = s.Equals((string)_o);
					break;
				//case VtType.VT_IIDENTITY:
				//	break;
				//case VtType.VT_KEYVALUE:
				//	break;
				case VtType.VT_DATETIME:
					break;
				//case VtType.VT_FVEC:
				//	break;
				case VtType.VT_BOOL:
					break;
			}
			return ret;
		}

		public bool Equals(IVariant obj)
		{
			if (_type != obj.DataType || AsInt() != obj.AsInt())
			{
				return false;
			}

			if (Length != obj.Length)
			{
				return false;
			}

			switch (_type)
			{
				//case VtType.VT_EMPTY:
				case VtType.VT_ERROR:
				case VtType.VT_INT:
				case VtType.VT_REAL:
				case VtType.VT_BOOL:
					return true;
				case VtType.VT_STRING:
					return _o.Equals(obj.ToString());
				case VtType.VT_DATETIME:
					return ((DateTime)_o).Equals(obj.AsDateTime());
				case VtType.VT_ARRAY:
					var v = (Vector<IVariant>)_o;
					for (int i = 0; i < v.Count; ++i)
					{
						if (! v[i].Equals(obj[i]))
						{
							return false;
						}
					}
					break;
			}

			if (_o is Dictionary<string, IVariant> d1)
			{
				foreach (var prop in d1.Keys)
				{
					if (d1[prop] != obj[prop])
					{
						return false;
					}
				}
			}

			return true;
		}

		public override bool Equals(object obj)
		{
			if (null == obj)
			{
				return false;
			}
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			if (_o != null)
			{
				return _o.GetHashCode();
			}

			switch (_type)
			{
				case VtType.VT_BOOL:
				case VtType.VT_INT:
					return _ix.IntValue.GetHashCode();
				case VtType.VT_REAL:
					return _ix.FloatValue.GetHashCode();
				case VtType.VT_EMPTY:
					return 0;
				//case VtType.VT_FVEC:
				//	return _ix.FloatValue.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
				//case VtType.VT_KEYVALUE:
				//	return Key.GetHashCode() ^ this[Key].GetHashCode();
				case VtType.VT_ERROR:
					return -1;
				case VtType.VT_STRING:
					return _o.GetHashCode();
				//case VtType.VT_IIDENTITY:
				//	return _o.GetHashCode();
				case VtType.VT_DATETIME:
					return _o.GetHashCode();
			}

			return 424242424;
		}

		public static bool operator <=(Variant a, Variant b)
		{
			return !(a > b);
		}

		public static bool operator >=(Variant a, Variant b)
		{
			return !(a < b);
		}

		public static bool operator >(Variant a, Variant b)
		{
			return !(a < b || a == b);
		}

		public static bool operator <(Variant a, Variant b)
		{
			if (a.IsNullOrError() || b.IsNullOrError())
			{
				return false;
			}

			if (a._type == VtType.VT_STRING || b._type == VtType.VT_STRING)
			{
				return a.CompareTo(b) < 0;
			}
			if (a._type == VtType.VT_REAL || b._type == VtType.VT_REAL)
			{
				return ((double)a) < ((double)b);
			}
			if (a._type == VtType.VT_INT || b._type == VtType.VT_INT)
			{
				return ((int)a) < ((int)b);
			}
			if (a._type == VtType.VT_DATETIME && b._type == VtType.VT_DATETIME)
			{
				return ((DateTime)a) < ((DateTime)b);
			}

			return false;
		}

		public IVariant MathAdd(IVariant rhs)
		{
			if (_type == VtType.VT_REAL || rhs.DataType == VtType.VT_REAL)
			{
				return new Variant(ToFloat() + rhs.ToFloat());
			}
			if (_type == VtType.VT_INT || rhs.DataType == VtType.VT_INT)
			{
				return new Variant(ToInt() + rhs.ToInt());
			}
			if (_type == VtType.VT_STRING || rhs.DataType == VtType.VT_STRING)
			{
				return new Variant(ToString() + rhs.ToString());
			}

			return GetError();
		}

		public IVariant MathSub(IVariant rhs)
		{
			if (_type == VtType.VT_REAL || rhs.DataType == VtType.VT_REAL)
			{
				return new Variant(ToFloat() - rhs.ToFloat());
			}
			if (_type == VtType.VT_INT || rhs.DataType == VtType.VT_INT)
			{
				return new Variant(ToInt() - rhs.ToInt());
			}

			return GetError();
		}

		public IVariant MathMul(IVariant rhs)
		{
			if (_type == VtType.VT_REAL || rhs.DataType == VtType.VT_REAL)
			{
				return new Variant(ToFloat() * rhs.ToFloat());
			}
			if (_type == VtType.VT_INT || rhs.DataType == VtType.VT_INT)
			{
				return new Variant(ToInt() * rhs.ToInt());
			}

			return GetError();
		}

		public IVariant MathDiv(IVariant rhs)
		{
			if (_type == VtType.VT_REAL || rhs.DataType == VtType.VT_REAL)
			{
				var rhsv = rhs.ToFloat();

				if (rhsv == 0)
				{
					return GetError();
				}
				return new Variant(ToFloat() / rhsv);
			}
			if (_type == VtType.VT_INT || rhs.DataType == VtType.VT_INT)
			{
				var rhsv = rhs.ToInt();

				if (rhsv == 0)
				{
					return GetError();
				}
				return new Variant(ToInt() / rhsv);
			}

			return GetError();
		}

		public IVariant MathMod(IVariant rhs)
		{
			if (_type == VtType.VT_REAL || rhs.DataType == VtType.VT_REAL)
			{
				return new Variant(ToFloat() % rhs.ToFloat());
			}
			if (_type == VtType.VT_INT || rhs.DataType == VtType.VT_INT)
			{
				return new Variant(ToInt() % rhs.ToInt());
			}

			return GetError();
		}

		public IVariant MathNeg()
		{
			if (_type == VtType.VT_REAL)
			{
				return new Variant(-_ix.FloatValue);
			}
			if (_type == VtType.VT_INT)
			{
				return new Variant(-_ix.IntValue);
			}

			return GetError();
		}

		public static Variant operator +(Variant lhs, Variant rhs)
		{
			if (lhs._type == VtType.VT_STRING || rhs._type == VtType.VT_STRING)
			{
				return new Variant(lhs.ToString() + rhs.ToString());
			}

			if (lhs.IsNullOrError() || rhs.IsNullOrError())
			{
				return GetError();
			}

			if (lhs._type == VtType.VT_REAL || rhs._type == VtType.VT_REAL)
			{
				return ((float)lhs) + ((float)rhs);
			}
			if (lhs._type == VtType.VT_INT || rhs._type == VtType.VT_INT)
			{
				return ((int)lhs) + ((int)rhs);
			}

			return GetError();
		}

		public static Variant operator -(Variant lhs, Variant rhs)
		{
			if (lhs._type == VtType.VT_STRING || rhs._type == VtType.VT_STRING)
			{
				return GetError();
			}

			if (lhs.IsNullOrError() || rhs.IsNullOrError())
			{
				return GetError();
			}

			if (lhs._type == VtType.VT_REAL || rhs._type == VtType.VT_REAL)
			{
				return ((float)lhs) - ((float)rhs);
			}
			if (lhs._type == VtType.VT_INT || rhs._type == VtType.VT_INT)
			{
				return ((int)lhs) - ((int)rhs);
			}

			return GetError();
		}

		public static Variant operator *(Variant lhs, Variant rhs)
		{
			if (lhs._type == VtType.VT_STRING || rhs._type == VtType.VT_STRING)
			{
				return GetError();
			}

			if (lhs.IsNullOrError() || rhs.IsNullOrError())
			{
				return GetError();
			}

			if (lhs._type == VtType.VT_REAL || rhs._type == VtType.VT_REAL)
			{
				return ((float)lhs) * ((float)rhs);
			}
			if (lhs._type == VtType.VT_INT || rhs._type == VtType.VT_INT)
			{
				return ((int)lhs) * ((int)rhs);
			}

			return GetError();

		}

		public static Variant operator /(Variant lhs, Variant rhs)
		{
			if (lhs._type == VtType.VT_STRING || rhs._type == VtType.VT_STRING)
			{
				return GetError();
			}

			if (lhs.IsNullOrError() || rhs.IsNullOrError())
			{
				return GetError();
			}

			if (lhs._type == VtType.VT_REAL || rhs._type == VtType.VT_REAL)
			{
				return ((float)lhs) / ((float)rhs);
			}
			if (lhs._type == VtType.VT_INT || rhs._type == VtType.VT_INT)
			{
				return ((int)lhs) / ((int)rhs);
			}

			return GetError();
		}

		public static Variant operator %(Variant lhs, Variant rhs)
		{
			if (lhs._type == VtType.VT_STRING || rhs._type == VtType.VT_STRING)
			{
				return GetError();
			}

			if (lhs.IsNullOrError() || rhs.IsNullOrError())
			{
				return GetError();
			}

			if (lhs._type == VtType.VT_REAL || rhs._type == VtType.VT_REAL)
			{
				return ((float)lhs) % ((float)rhs);
			}
			if (lhs._type == VtType.VT_INT || rhs._type == VtType.VT_INT)
			{
				return ((int)lhs) % ((int)rhs);
			}

			return GetError();
		}

		public int CompareTo(IVariant b)
		{
			if (IsError() || b.IsError())
			{
				return -1;
			}
			if (IsNull() && b.IsNull())
			{
				return 0;
			}
			if (_type == VtType.VT_STRING || b.DataType == VtType.VT_STRING)
			{
				return ToString().CompareTo(b);
			}
			if (_type == VtType.VT_REAL || b.DataType == VtType.VT_REAL)
			{
				var f = ToFloat() - b.ToFloat();
				return f > 0 ? 1 : (f == 0 ? 0 : -1);
			}
			if (_type == VtType.VT_INT || b.DataType == VtType.VT_INT)
			{
				var f = ToInt() - b.ToInt();
				return f > 0 ? 1 : (f == 0 ? 0 : -1);
			}

			return ToString().CompareTo(b.ToString());

			//if (_type != other.DataType)
			//{
			//	return _type.CompareTo(other.DataType);
			//}

			//if (_type == VtType.VT_STRING)
			//{
			//	return ((string)_o).CompareTo(other.ToString());
			//}
			//if (_type == VtType.VT_INT || _type == VtType.VT_BOOL)
			//{
			//	return _ix.IntValue.CompareTo(other.AsInt());
			//}
			//if (_type == VtType.VT_REAL)
			//{
			//	return _ix.FloatValue.CompareTo(other.AsFloat());
			//}
			////if (_type == VtType.VT_FVEC)
			////{
			////	return (int)((MathF.Abs(_ix.FloatValue) + Math.Abs(_y) + MathF.Abs(_z)) - (MathF.Abs(other._ix.FloatValue) + Math.Abs(other._y) + MathF.Abs(other._z)));
			////}

			//return 0;
		}

		public void Pool_Activate()
		{
		}

		public void Pool_Deactivate()
		{
			Clear();
		}

		#endregion
	}
}
