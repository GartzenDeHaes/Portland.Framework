using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maximum;

using Portland.Collections;
using Portland.Mathmatics;

namespace Portland.RPG.Accounting
{
	public enum LedgerType
	{
		Error = 0,
		Trade,
		Asset,
		Liability,
		Revenue,
		Expense,
		Gain,
		Loss
	}

	public struct LedgerTransaction
	{
		public int DrAccountId;
		public int DrTransactionId;
		public int CrAccountId;
		public int CrTransactionId;
		public int Amount;
		public DateTime Dts;
	}

	public class Ledger
	{
		public int LedgerId;
		public String10 ItemId;
		public LedgerType LedgerTypeCode;

		public Vector<LedgerTransaction> LedgerTransactions = new(16);
	}

	public class LedgerStatement
	{
		public int LedgerId;
		public Date Date;
		public Money ClosingBalance;
	}
}
