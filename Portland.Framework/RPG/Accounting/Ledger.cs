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
	public enum TransactionType : short
	{
		Error = 0,
		Credit,
		Debit
	}

	public struct AccountType
	{
		public AsciiId4 AccountTypeCode;
		public TransactionType TransactionTypeCode;
		public Money Fee;
	}

	public enum AccountTransactionType : short
	{
		Error = 0,
		AdjustCr,
		AdjustDr,
		Deposit,
		Withdrawl,
		Fee
	}

	public struct AccountTransaction
	{
		public int LedgerId;
		public DateTime Dts;
		public TransactionType TransactionTypeCode;
		public AccountTransactionType LedgerTransactionTypeCode;
		public int AccountId;
		public Money Amount;
	}

	public class Account
	{
		public Int64Guid OwnerId;
		public int AccountId;
		public AsciiId4 AccountTypeCode;
		public Vector<AccountTransaction> AccountTransactions = new(16);
	}

	public class AccountStatement
	{
		public int AccountId;
		public Date Date;
		public Money ClosingBalance;
		public Money TotalDebit;
		public Money TotalCredit;
	}

	public enum LedgerType : short
	{
		Error = 0,
		Asset,
		Liability,
		Revenue,
		Expense,
		Gain,
		Loss
	}

	public struct LedgerTransaction
	{
		public int LedgerIdCr;
		public DateTime Dts;
		public int LedgerIdDr;
		public Money Amount;
	}

	public class Ledger
	{
		public int LedgerId;
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
