using System;
using System.Text;

using Portland.Collections;

#if UNITY_2019_1_OR_NEWER
using UnityEngine;
#endif

namespace Portland.AI.Semantics
{
	[Serializable]
	public struct ThematicRoleFlags
	{
#if UNITY_2019_1_OR_NEWER
		[SerializeField, HideInInspector]
#endif
		private BitSet16 _bits;

		//[property: SerializeField]
		public bool Agent { get { return _bits.IsSet(0); } set { _bits.SetTest(0, value); } }
		public bool Consumable { get { return _bits.IsSet(1); } set { _bits.SetTest(1, value); } }
		public bool Container { get { return _bits.IsSet(2); } set { _bits.SetTest(2, value); } }
		public bool Conveyance { get { return _bits.IsSet(3); } set { _bits.SetTest(3, value); } }
		public bool Enterable { get { return _bits.IsSet(4); } set { _bits.SetTest(4, value); } }
		public bool Interactable { get { return _bits.IsSet(5); } set { _bits.SetTest(5, value); } }
		public bool Location { get { return _bits.IsSet(6); } set { _bits.SetTest(6, value); } }
		public bool Resource { get { return _bits.IsSet(7); } set { _bits.SetTest(7, value); } }
		public bool Storeable { get { return _bits.IsSet(8); } set { _bits.SetTest(8, value); } }
		public bool Transformer { get { return _bits.IsSet(9); } set { _bits.SetTest(9, value); } }
		public bool Wearable { get { return _bits.IsSet(10); } set { _bits.SetTest(10, value); } }
		public bool Wieldable { get { return _bits.IsSet(11); } set { _bits.SetTest(11, value); } }

		public override string ToString()
		{
			return _bits.ToString();
		}

		public bool Equals(ThematicRoleFlags fl)
		{
			return fl._bits.RawBits == _bits.RawBits;
		}

		public override bool Equals(object obj)
		{
			if (obj is ThematicRoleFlags fl)
			{
				return fl._bits.RawBits == _bits.RawBits;
			}

			return _bits.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _bits.RawBits.GetHashCode();
		}
	}
}
