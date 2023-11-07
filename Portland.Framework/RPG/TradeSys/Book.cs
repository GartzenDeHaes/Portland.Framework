using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Maximum;

namespace Portland.RPG.TradeSys
{
	public sealed class Book
	{
		public String10 ProductId;

		public int TotalSold { get; private set; }

		public Money TotalSales { get; private set; }

		public int CurrentTimeStep { get; private set; }

		public enum EntryType
		{
			Bid = 0,
			Offer = 1,
		}

		public struct Entry
		{
			public int TempEntryId;
			public EntryType Type;
			public String10 AgentId;
			public int Price;
			public int Quntity;
		}

		public struct Transaction
		{
			public String10 Buyer;
			public String10 Seller;
			public int Price;
			public int Quntity;
			public int TimeSequence;
		}

		public readonly List<Transaction> Transactions = new List<Transaction>();
		readonly Vector<Entry> Bids = new Vector<Entry>();
		readonly Vector<Entry> Offers = new Vector<Entry>();

		public Book(in String10 productId)
		{
			ProductId = productId;
		}

		public void PostBid(in String10 agentId, in Money price, int qty, out int tempEntryId)
		{
			tempEntryId = -1;
		}

		public void PostOffer(in String10 agentId, in Money price, int qty, out int tempEntryId) 
		{
			tempEntryId = -1;
		}
	}
}
