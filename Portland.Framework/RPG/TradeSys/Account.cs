using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

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

	public enum AccountTransactionStatus
	{
		Pending,
		Posted
	}

	public struct AccountTransaction
	{
		public int TransactionId;
		public DateTime Dts;
		public TransactionType TransactionTypeCode;
		public AccountTransactionType LedgerTransactionTypeCode;
		public AccountTransactionStatus Status;
		public int Amount;
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

	public class Account
	{
		public String10 OwnerCharId;
		public int AccountId;
		public AsciiId4 AccountTypeCode;
		public Vector<AccountBook> AccountBooks = new();

		Dictionary<String10, int> AccountBookIdx = new();

		public int GetBalance(in String10 itemId)
		{
			if (AccountBookIdx.TryGetValue(itemId, out var accountBookIdx))
			{
				return AccountBooks[accountBookIdx].Balance;
			}
			return 0;
		}
	}

	public enum AccountBookUnitType
	{
		Currency,
		Item
	}

	public class AccountBook
	{
		public String10 ItemId;
		public AccountBookUnitType Units;
		public int Balance;
		public int CreditBalance;
		public int DebitBalance;
		public Vector<AccountTransaction> Transactions = new();


	}
}
