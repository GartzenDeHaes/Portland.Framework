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
			return Bits.GetHashCode();
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
				case "STAMINA_EMPTY": ret = 1; break;
				case "ALERT_CLOCK": ret = 2; break;
				case "ALERT_UTILITY": ret = 3; break;
				case "ALERT_HEALTH": ret = 4; break;
				case "ALERT_SOUND": ret = 5; break;
				case "ALERT_SIGHT": ret = 6; break;
				case "ENEMY_NEAR": ret = 7; break;
				case "ENEMY_VISIBLE": ret = 8; break;
				case "ENEMY_IN_AIM": ret = 9; break;
				case "INV_HAS_RANGED": ret = 10; break;
				case "INV_HAS_MELEE": ret = 11; break;
				case "INV_HAS_SPACE": ret = 12; break;
				case "EQUIPPED_ITEM": ret = 13; break;
				case "EQUIPPED_LOW_AMMO": ret = 14; break;
				case "EQUIPPED_RANGED": ret = 15; break;
				case "NAV_FAILED": ret = 16; break;
				case "NAV_ARRIVED": ret = 17; break;
				case "RESOURCE_VISIBLE": ret = 18; break;
				case "PICKUP_VISIBLE": ret = 19; break;
				case "IS_GROUNDED": ret = 20; break;
				case "NAV_HAS_PATH": ret = 21; break;
				case "ALLY_NEAR": ret = 22; break;
			}

			return ret;
		}
	}
}
