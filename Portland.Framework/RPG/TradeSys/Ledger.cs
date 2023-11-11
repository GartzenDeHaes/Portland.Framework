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
	public enum TransactionType
	{
		Credit = 1,
		Debit = -1
	}

	public enum AccountTransactionType
	{
		Error = 'E',
		AdjustCr = 'A',
		AdjustDr = 'a',
		Deposit = 'D',
		Withdrawl = 'W',
		WithdrawlAtm = 'w',
		Fee = 'F'
	}

	public struct AccountTransaction
	{
		public int TransactionId;
		public DateTime Dts;
		public TransactionType TransactionTypeCode;
		public AccountTransactionType LedgerTransactionTypeCode;
		public int Amount;
	}

	public class Account
	{
		public String10 OwnerCharId;
		public int AccountId;
		public AsciiId4 AccountTypeCode;
		public Vector<AccountBook> AccountBooks = new();

		public int Balance
	}

	public class AccountBook
	{
		public String10 ItemId;
		public Vector<AccountTransaction> Transactions = new();
	}

	public class AccountBookStatement
	{
		public String10 ItemId;
		public int ClosingBalance;
		public int TotalDebit;
		public int TotalCredit;
	}

	public class AccountStatement
	{
		public int AccountId;
		public ShortDateTime Created;
		public Vector<AccountBookStatement> ItemStatements = new();
	}

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
