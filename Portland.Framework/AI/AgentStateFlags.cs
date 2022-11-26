using System;

using Portland.Collections;
using Portland.Text;

namespace Portland.AI
{
	[Serializable]
	public struct AgentStateFlags
	{
		public BitSet32 Bits;

		public bool Run { get { return Bits.IsSet(0); } set { Bits.SetTest(0, value); } }
		public bool StaminaEmpty { get { return Bits.IsSet(1); } set { Bits.SetTest(1, value); } }
		public bool AlertClock { get { return Bits.IsSet(2); } set { Bits.SetTest(2, value); } }
		public bool AlertUtility { get { return Bits.IsSet(3); } set { Bits.SetTest(3, value); } }
		public bool AlertHealth { get { return Bits.IsSet(4); } set { Bits.SetTest(4, value); } }
		public bool AlertSound { get { return Bits.IsSet(5); } set { Bits.SetTest(5, value); } }
		public bool AlertSight { get { return Bits.IsSet(6); } set { Bits.SetTest(6, value); } }
		public bool EnemyNear { get { return Bits.IsSet(7); } set { Bits.SetTest(7, value); } }            //!< Set in SensorProcessor
		public bool EnemyVisible { get { return Bits.IsSet(8); } set { Bits.SetTest(8, value); } }       //!< Set in SensorProcessor
		public bool EnemyInAim { get { return Bits.IsSet(9); } set { Bits.SetTest(9, value); } }        //!< Set in SensorProcessor
		public bool InvHasRanged { get { return Bits.IsSet(10); } set { Bits.SetTest(10, value); } }
		public bool InvHasMelee { get { return Bits.IsSet(11); } set { Bits.SetTest(11, value); } }
		public bool InvHasSpace { get { return Bits.IsSet(12); } set { Bits.SetTest(12, value); } }
		public bool EquippedItem { get { return Bits.IsSet(13); } set { Bits.SetTest(13, value); } }
		public bool EquippedLowAmmo { get { return Bits.IsSet(14); } set { Bits.SetTest(14, value); } }
		public bool EquippedRanged { get { return Bits.IsSet(15); } set { Bits.SetTest(15, value); } }
		public bool NavFailed { get { return Bits.IsSet(16); } set { Bits.SetTest(16, value); } }       //!< Set in AgentLocomotion
		public bool NavArrived { get { return Bits.IsSet(17); } set { Bits.SetTest(17, value); } }       //!< Set in AgentLocomotion
		public bool ResourceVisible { get { return Bits.IsSet(18); } set { Bits.SetTest(18, value); } } //!< Set in SensorProcessor
		public bool PickupVisible { get { return Bits.IsSet(19); } set { Bits.SetTest(19, value); } }      //!< Set in SensorProcessor
		public bool IsGrounded { get { return Bits.IsSet(20); } set { Bits.SetTest(20, value); } }      //!< Set in SensorProcessor
		public bool NavHasPath { get { return Bits.IsSet(21); } set { Bits.SetTest(21, value); } }
		public bool AllyNear { get { return Bits.IsSet(22); } set { Bits.SetTest(22, value); } }
		public bool AiReplanReq { get { return Bits.IsSet(23); } set { Bits.SetTest(23, value); } }

		public bool HasInterruptCondition { get { return AlertHealth || AlertUtility || AlertSound || AlertSight || EnemyNear || EnemyInAim || EnemyVisible || AiReplanReq; } }

		public override string ToString()
		{
			return Bits.ToString();
		}

		public bool Equals(AgentStateFlags fl)
		{
			return fl.Bits.RawBits == Bits.RawBits;
		}

		public override bool Equals(object obj)
		{
			if (obj is AgentStateFlags fl)
			{
				return fl.Bits.RawBits == Bits.RawBits;
			}

			return Bits.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Bits.RawBits.GetHashCode();
		}

		public void Reset()
		{
			Bits.RawBits = 0;
		}

		public void SetByName(string bitName, bool state)
		{
			int bit = BitNameToNum(bitName);
			if (bit < 0)
			{
				throw new ArgumentOutOfRangeException(bitName + " is not valid for AgentStateFlags");
			}

			Bits.SetTest(bit, state);
		}

		public static int BitNameToNum(string bitName)
		{
			if (!StringHelper.IsUpper(bitName))
			{
				bitName = bitName.ToUpper();
			}

			int ret = -1;
			switch (bitName)
			{
				case "RUN": ret = 0; break;
				case "STAMINAEMPTY": ret = 1; break;
				case "ALERTCLOCK": ret = 2; break;
				case "ALERTUTILITY": ret = 3; break;
				case "ALERTHEALTH": ret = 4; break;
				case "ALERTSOUND": ret = 5; break;
				case "ALERTSIGHT": ret = 6; break;
				case "ENEMYNEAR": ret = 7; break;
				case "ENEMYVISIBLE": ret = 8; break;
				case "ENEMYINAIM": ret = 9; break;
				case "INVHASRANGED": ret = 10; break;
				case "INVHASMELEE": ret = 11; break;
				case "INVHASSPACE": ret = 12; break;
				case "EQUIPPEDITEM": ret = 13; break;
				case "EQUIPPEDLOWAMMO": ret = 14; break;
				case "EQUIPPEDRANGED": ret = 15; break;
				case "NAVFAILED": ret = 16; break;
				case "NAVARRIVED": ret = 17; break;
				case "RESOURCEVISIBLE": ret = 18; break;
				case "PICKUPVISIBLE": ret = 19; break;
				case "ISGROUNDED": ret = 20; break;
				case "NAVHASPATH": ret = 21; break;
				case "ALLYNEAR": ret = 22; break;
			}

			return ret;
		}
	}
}
