using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using Portland.Collections;
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
		VT_IIDENTITY,
		VT_KEYVALUE,
		VT_DATETIME,
		VT_FVEC,
		VT_BOOL
	}

	[Serializable]
	public sealed class Variant : IEquatable<Variant>, IComparable<Variant>, IPoolable, IDisposable
	{
		//private static SimplePool<float> _vecpool = new SimplePool<float>();

		private Dictionary<string, Variant> _proplst;

		private VtType _type;
		private int _i;
		private float _x, _y, _z;
		private object _o = null;

		public VtType DataType
		{
			get
			{
				return _type;
			}
		}

		public string Key
		{
			get
			{
				if (_type == VtType.VT_KEYVALUE)
				{
					
					return _proplst.Keys.First();
				}

				return ToString();
			}
		}

		public Variant()
		{
			_type = VtType.VT_EMPTY;
		}

		public Variant(int i)
		{
			_i = i;
			_type = VtType.VT_INT;
		}

		public Variant(bool i)
		{
			_i = i ? 1 : 0;
			_type = VtType.VT_BOOL;
		}

		public Variant(float d)
		{
			_z = d;
			_type = VtType.VT_REAL;
		}

		public Variant(string s)
		{
			_o = s;
			_type = VtType.VT_STRING;
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
			if (_proplst == null)
			{
				_proplst = new Dictionary<string, Variant>();
			}
		}

		public bool HasProp(string name)
		{
			if (_proplst == null)
			{
				return false;
			}

			return _proplst.ContainsKey(name);
		}

		public void SetProp(string name, Variant value)
		{
			Debug.Assert(! ReferenceEquals(value, this));

			EnsureProps();

			if (_proplst.ContainsKey(name))
			{
				_proplst[name] = value;
			}
			else
			{
				_proplst.Add(name, value);
			}
		}

		public void ClearProp(string name)
		{
			EnsureProps();

			if (_proplst.ContainsKey(name))
			{
				_proplst[name].Clear();
			}
		}

		public void SetPropArray(string name, string index, Variant value)
		{
			Debug.Assert(!ReferenceEquals(value, this));

			EnsureProps();

			if (!_proplst.ContainsKey(name))
			{
				_proplst.Add(name, new Variant());
			}

			_proplst[name].SetProp(index, value);
		}

		public void ClearPropArray(string name, int size)
		{
			EnsureProps();

			if (_proplst.ContainsKey(name))
			{
				_proplst[name].Clear();
				for (int i = 0; i < size; i++)
				{
					_proplst[name].SetProp(i.ToString(), new Variant());
				}
			}
		}

		public Variant this[string index]
		{
			get
			{
				EnsureProps();

				if (!_proplst.ContainsKey(index))
				{
					_proplst.Add(index, new Variant());
				}
				return _proplst[index];
			}
			set
			{
				EnsureProps();

				if (_proplst.ContainsKey(index))
				{
					_proplst[index] = value;
				}
				else
				{
					_proplst.Add(index, value);
				}
			}
		}

		public Variant this[int index]
		{
			get { return this[index.ToString()]; }
			set { this[index.ToString()] = value; }
		}

		public int Count
		{
			get
			{
				if (_proplst != null)
				{
					return _proplst.Count;
				}
				else if (_type == VtType.VT_STRING)
				{
					return ((string)_o).Length;
				}
				return 0;
			}
		}

		public void Set(bool i)
		{
			Clear();
			_i = i ? 1 : 0;
			_type = VtType.VT_BOOL;
		}

		public void Set(int i)
		{
			Clear();
			_i = i;
			_type = VtType.VT_INT;
		}

		public void Set(Variant v)
		{
			v.CopyTo(this);
		}

		public void Set(float d)
		{
			Clear();
			_z = d;
			_type = VtType.VT_REAL;
		}

		public void Set(string s)
		{
			Clear();
			_o = s;
			_type = VtType.VT_STRING;
		}

		public void Set(DateTime dtm)
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
			_proplst = null;
			_type = VtType.VT_EMPTY;
		}

		//public void Set(IIdentity uid)
		//{
		//	Clear();
		//	_type = VtType.VT_IIDENTITY;
		//	_o = uid;
		//}

		public void Set(string key, string value)
		{
			Clear();
			_type = VtType.VT_KEYVALUE;
			SetProp(key, value);
		}

		public void Set(float x, float y, float z)
		{
			Clear();
			_type = VtType.VT_FVEC;
			_x = x;
			_y = y;
			_z = z;
		}

		public void Clear()
		{
			_type = VtType.VT_EMPTY;
			_o = null;
			_i = 0;
			_x = _y = _z = 0f;

			if (_proplst != null)
			{
				_proplst.Clear();
			}
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

		public bool IsIdentity()
		{
			return _type == VtType.VT_IIDENTITY;
		}

		public bool IsKeyValue()
		{
			return _type == VtType.VT_KEYVALUE;
		}

		public bool IsDateTime()
		{
			return _type == VtType.VT_DATETIME;
		}

		public bool IsVector()
		{
			return _type == VtType.VT_FVEC;
		}

		public bool IsBool()
		{
			return _type == VtType.VT_BOOL;
		}

		public void CopyTo(Variant v)
		{
			v.Clear();

			v._type = _type;
			v._i = _i;
			v._o = _o;
			v._x = _x;
			v._y = _y;
			v._z = _z;

			if (_proplst != null)
			{
				v.EnsureProps();

				foreach (var prop in _proplst.Keys)
				{
					v[prop] = _proplst[prop].Dup();
				}
			}
		}

		// for new
		public Variant Dup()
		{
			Variant v = new Variant();
			CopyTo(v);
			return v;
		}

		public int AsInt()
		{
			return (int)this;
		}

		//public IIdentity AsIdentity()
		//{
		//	return _o as IIdentity;
		//}

		public void AsFloatVector(out float x, out float y, out float z)
		{
			if (_type == VtType.VT_FVEC)
			{
				x = _x;
				y = _y;
				z = _z;
				return;
			}

			x = 0f;
			y = 0f;
			z = 0f;
		}

		public void AsFloatVector(out float x, out float y)
		{
			float z;
			AsFloatVector(out x, out y, out z);
		}

		#region Conversions

		public static implicit operator Variant(string s)
		{
			return new Variant(s);
		}
		public static implicit operator string(Variant v)
		{
			return v.ToString();
		}

		public static implicit operator Variant(DateTime v)
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
		public static implicit operator DateTime(Variant v)
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
		public static implicit operator DateTime?(Variant v)
		{
			if (v._type == VtType.VT_DATETIME)
			{
				return (DateTime)v._o;
			}
			if (v._type == VtType.VT_STRING)
			{
				DateTime o;
				if (DateTime.TryParse(v, out o))
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
		public static implicit operator int(Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return v._i;
			}
			if (v._type == VtType.VT_REAL)
			{
				return (int)v._z;
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
		public static implicit operator int?(Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return v._i;
			}
			if (v._type == VtType.VT_REAL)
			{
				return (int)v._z;
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
		public static implicit operator float(Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return (float)v._i;
			}
			if (v._type == VtType.VT_REAL)
			{
				return v._z;
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
		public static implicit operator float?(Variant v)
		{
			if (v._type == VtType.VT_INT || v._type == VtType.VT_BOOL)
			{
				return (float)v._i;
			}
			if (v._type == VtType.VT_REAL)
			{
				return v._z;
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
				return v._i != 0;
			}
			if (v._type == VtType.VT_REAL)
			{
				return v._z != 0f;
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
				ret[count++] = s;
			}

			return ret;
		}
		public static implicit operator List<string>(Variant v)
		{
			var lst = new List<string>();

			if (v._proplst == null)
			{
				return lst;
			}

			foreach (var key in v._proplst.Keys)
			{
				if (StringHelper.IsInt(key))
				{
					lst.Add(v._proplst[key]);
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
				ret.Set(parts[0], parts[1]);
				return ret;
			}

			ret.Set(value);

			return ret;
		}

		public override string ToString()
		{
			if (_proplst != null && _proplst.Count > 0)
			{
				StringBuilder buf = new StringBuilder();
				bool addComma = false;

				buf.Append('[');

				foreach (var key in _proplst.Keys)
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
					buf.Append('=');
					buf.Append(_proplst[key].ToString());
				}

				buf.Append(']');

				return buf.ToString();
			}

			switch (_type)
			{
				case VtType.VT_BOOL:
					return _i == 0 ? "FALSE" : "TRUE";
				case VtType.VT_INT:
					return _i.ToString();
				case VtType.VT_REAL:
					return _z.ToString();
				case VtType.VT_STRING:
					return (string)_o;
				case VtType.VT_ERROR:
					return "UNDEFINED";
				case VtType.VT_IIDENTITY:
				case VtType.VT_DATETIME:
					return _o.ToString();
				case VtType.VT_KEYVALUE:
				{
					var key = _proplst.Keys.First();
					var val = _proplst[key];
					return key + "=" + val;
				}
				case VtType.VT_FVEC:
					return "[" + _x + "," + _y + "," + _z + "]";
			}

			return "NULL";
		}

		public string ToJSON()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append('{');
			buf.Append('"');
			buf.Append(ToString());
			buf.Append('"');
			buf.Append(":[{");

			bool comma = false;

			if (_proplst != null)
			{
				foreach(var key in _proplst.Keys)
				{
					if (comma)
					{
						buf.Append(',');
					}
					else
					{
						comma = true;
					}

					buf.Append('"');
					buf.Append(key);
					buf.Append('"');
					buf.Append(':');
					if (_proplst[key].Count > 0)
					{
						buf.Append(_proplst[key].ToJSON());
					}
					else
					{
						buf.Append('"');
						buf.Append(_proplst[key].ToString());
						buf.Append('"');
					}
				}
			}

			buf.Append("}]}");

			return buf.ToString();
		}

		public static Variant ParseJSON(string json)
		{
			JSONLex lex = new JSONLex(json);
			lex.Next();
			return ParseJSON(lex);
		}

		private static void Match(JSONLex lex, JSONLexToken token)
		{
			if (lex.Token != token)
			{
				throw new FormatException("Expected JSON " + token);
			}

			lex.Next();
		}

		static Variant ParseJSON(JSONLex lex)
		{
			Match(lex, JSONLexToken.LBRACE);

			Variant v = Variant.Parse(lex.Lexum);

			Match(lex, JSONLexToken.STRING);

			Match(lex, JSONLexToken.COLON);
			Match(lex, JSONLexToken.LBRACK);
			Match(lex, JSONLexToken.LBRACE);

			while (lex.Token != JSONLexToken.RBRACE)
			{
				string key = lex.Lexum;

				Match(lex, JSONLexToken.STRING);
				Match(lex, JSONLexToken.COLON);

				if (lex.Token == JSONLexToken.STRING)
				{
					if (StringHelper.IsInt(key))
					{
						v[Int32.Parse(key)] = Variant.Parse(lex.Lexum);
					}
					else
					{
						v.SetProp(key, Variant.Parse(lex.Lexum));
					}

					Match(lex, JSONLexToken.STRING);
				}
				else
				{
					if (StringHelper.IsInt(key))
					{
						v[Int32.Parse(key)] = ParseJSON(lex);
					}
					else
					{
						v.SetProp(key, ParseJSON(lex));
					}
				}

				if (lex.Token == JSONLexToken.COMMA)
				{
					lex.Next();
				}
			}

			Match(lex, JSONLexToken.RBRACE);
			Match(lex, JSONLexToken.RBRACK);
			Match(lex, JSONLexToken.RBRACE);

			return v;
		}

		public void ToBin(byte[] data, ref int pos)
		{
			data[pos++] = (byte)_type;

			switch (_type)
			{
				case VtType.VT_BOOL:
					BitConverterNoAlloc.GetBytes(_i, data, ref pos);
					break;
				case VtType.VT_INT:
					BitConverterNoAlloc.GetBytes(_i, data, ref pos);
					break;
				case VtType.VT_REAL:
					BitConverterNoAlloc.GetBytes(_z, data, ref pos);
					break;
				case VtType.VT_EMPTY:
					break;
				case VtType.VT_FVEC:
					BitConverterNoAlloc.GetBytes(_x, data, ref pos);
					BitConverterNoAlloc.GetBytes(_y, data, ref pos);
					BitConverterNoAlloc.GetBytes(_z, data, ref pos);
					break;
				case VtType.VT_KEYVALUE:
					break;
				case VtType.VT_ERROR:
					break;
				case VtType.VT_STRING:
					BitConverterNoAlloc.GetBytes((string)_o, data, ref pos);
					break;
				case VtType.VT_IIDENTITY:
					throw new InvalidOperationException("Not serializing Identity");
				case VtType.VT_DATETIME:
					BitConverterNoAlloc.GetBytes(((DateTime)_o).Ticks, data, ref pos);
					break;
			}

			int count = 0;
			if (_proplst != null)
			{
				count = _proplst.Count;
			}

			BitConverterNoAlloc.GetBytes((short)count, data, ref pos);

			if (count == 0)
			{
				return;
			}

			foreach (var key in _proplst.Keys)
			{
				BitConverterNoAlloc.GetBytes(key, data, ref pos);
				_proplst[key].ToBin(data, ref pos);
			}
		}

		public static Variant ParseBin(byte[] data, ref int pos)
		{
			return ParseBin(data, ref pos, new Variant());
		}

		public static Variant ParseBin(byte[] data, ref int pos, Variant v)
		{
			StringBuilder sbuf = null;
			v._type = (VtType)data[pos++];

			switch (v._type)
			{
				case VtType.VT_BOOL:
					v._i = BitConverter.ToInt32(data, pos);
					pos += 4;
					break;
				case VtType.VT_INT:
					v._i = BitConverter.ToInt32(data, pos);
					pos += 4;
					break;
				case VtType.VT_REAL:
					v._z = BitConverter.ToSingle(data, pos);
					pos += 8;
					break;
				case VtType.VT_EMPTY:
					break;
				case VtType.VT_FVEC:
					v._x = BitConverter.ToSingle(data, pos);
					pos += 4;
					v._y = BitConverter.ToSingle(data, pos);
					pos += 4;
					v._z = BitConverter.ToSingle(data, pos);
					pos += 8;
					break;
				case VtType.VT_KEYVALUE:
					break;
				case VtType.VT_ERROR:
					break;
				case VtType.VT_STRING:
					sbuf = new StringBuilder();
					v._o = BitConverterNoAlloc.ToString(data, ref pos, sbuf);
					break;
				case VtType.VT_IIDENTITY:
					throw new InvalidOperationException("Not serializing Identity");
				case VtType.VT_DATETIME:
					v._o = new DateTime(BitConverter.ToInt64(data, pos));
					pos += 8;
					break;
			}

			int count = BitConverter.ToInt16(data, pos);
			pos += 2;

			if (count == 0)
			{
				return v;
			}

			if (sbuf == null)
			{
				sbuf = new StringBuilder();
			}

			v._proplst = new Dictionary<string, Variant>();

			for (int x = 0; x < count; x++)
			{
				string key = BitConverterNoAlloc.ToString(data, ref pos, sbuf);
				v._proplst.Add(key, Variant.ParseBin(data, ref pos));
			}

			return v;
		}

		public void Dispose()
		{
			Clear();
			_proplst = null;
		}

		#region Compare

		public static bool operator ==(Variant a, Variant b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Variant a, Variant b)
		{
			return !(a == b);
		}

		public bool Equals(Variant obj)
		{
			if (_type != obj._type || _i != obj._i)
			{
				return false;
			}

			if (_o == null ^ obj._o == null)
			{
				return false;
			}

			if (_type == VtType.VT_FVEC)
			{
				if (_x != obj._x || _y != obj._y || _z != obj._z)
				{
					return false;
				}
			}

			if (_o != null && !_o.Equals(obj._o))
			{
				return false;
			}

			int pcnt1 = _proplst == null ? 0 : _proplst.Count;
			int pcnt2 = obj._proplst == null ? 0 : obj._proplst.Count;

			if (pcnt1 != pcnt2)
			{
				return false;
			}

			if (pcnt1 == 0)
			{
				return true;
			}

			foreach (var key in _proplst.Keys)
			{
				if (!obj._proplst.ContainsKey(key))
				{
					return false;
				}

				if (!_proplst[key].Equals(obj._proplst[key]))
				{
					return false;
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
			// TODO: not good
			if (_proplst != null && _proplst.Count > 0)
			{
				return _proplst.GetHashCode();
			}

			switch (_type)
			{
				case VtType.VT_BOOL:
				case VtType.VT_INT:
					return _i.GetHashCode();
				case VtType.VT_REAL:
					return _z.GetHashCode();
				case VtType.VT_EMPTY:
					return 0;
				case VtType.VT_FVEC:
					return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
				case VtType.VT_KEYVALUE:
					return Key.GetHashCode() ^ this[Key].GetHashCode();
				case VtType.VT_ERROR:
					return -1;
				case VtType.VT_STRING:
					return _o.GetHashCode();
				case VtType.VT_IIDENTITY:
					return _o.GetHashCode();
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
				return ((string)a).CompareTo((string)b) < 0;
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

		public static Variant operator +(Variant lhs, Variant rhs)
		{
			if (lhs._type == VtType.VT_STRING || rhs._type == VtType.VT_STRING)
			{
				return ((string)lhs) + ((string)rhs);
			}

			if (lhs.IsNullOrError() || lhs.IsNullOrError())
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

			if (lhs.IsNullOrError() || lhs.IsNullOrError())
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

			if (lhs.IsNullOrError() || lhs.IsNullOrError())
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

			if (lhs.IsNullOrError() || lhs.IsNullOrError())
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

			if (lhs.IsNullOrError() || lhs.IsNullOrError())
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

		public int CompareTo(Variant other)
		{
			if (_type != other._type)
			{
				return _type.CompareTo(other._type);
			}

			if (_type == VtType.VT_STRING)
			{
				return ((string)_o).CompareTo((string)other._o);
			}
			if (_type == VtType.VT_INT || _type == VtType.VT_BOOL)
			{
				return _i.CompareTo(other._i);
			}
			if (_type == VtType.VT_REAL)
			{
				return _z.CompareTo(other._z);
			}
			if (_type == VtType.VT_FVEC)
			{
				return (int)((MathF.Abs(_x) + Math.Abs(_y) + MathF.Abs(_z)) - (MathF.Abs(other._x) + Math.Abs(other._y) + MathF.Abs(other._z)));
			}

			return 0;
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
