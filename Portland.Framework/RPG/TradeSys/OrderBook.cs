using System;
using System.Collections.Generic;

using Portland.Collections;

namespace Portland.RPG.Economics
{
	public enum OrderAction
	{
		Buy,
		Sell
	}

	public enum TimeInForce
	{
		DayOrder,
		GoodTilCancled,
		GoodTilDate,
		FillOrKill
	}

	public enum OrderType
	{
		Market,
		Limit,
	}

	public enum OrderStatus
	{
		Open,
		Filled,
		Canceled
	}

	public struct Order
	{
		public int OrderId;
		public String10 ItemId;
		public AsciiId4 LocationCode;
		public TimeInForce TIF;
		public OrderAction BuyOrSell;
		public OrderType OrderType;
		public int Size;
		public int LimitPrice;
		public Date ExpireDate;
		public OrderStatus Status;
		public int AgentId;
		public ShortDateTime CreatedDate;
	}

	public struct ItemTransation
	{
		public String10 ItemId;
		public DateTime When;
		public int Bid;
		public int BidAgentId;
		public int Ask;
		public int AskAgentId;
		public int Price;
		public int Size;
	}

	public class ExchangeLocation
	{
		public AsciiId4 LocationCode;

		public Vector<OrderBook> OrderBooks = new();
	}

	public class ExchangeManager
	{
		Vector<Order> _orders = new();
		Dictionary<AsciiId4, ExchangeLocation> _exchangeLocations = new();

		public ExchangeManager()
		{

		}

	}

	public class OrderBook
	{
		public event Action OnOrderExecution;

		public String10 ItemId;

		// ordered by bid asc, highest bid LAST
		Vector<int> BuyOrders = new();
		// ordered by offer desc, lowest price LAST 
		Vector<int> SellOrders = new();

		Vector<ItemTransation> ExecutionResults = new();

		Vector<Order> Orders = new();

		public OrderBook() 
		{
		}



		int SellInsertSortOrder(int aId, int bId)
		{
			ref Order soa = ref Orders.ElementAtRef(aId);
			ref Order sob = ref Orders.ElementAtRef(bId);

			if (soa.OrderType == OrderType.Market)
			{
				// Market before limit orders (place a on the right (end))
				return sob.OrderType == OrderType.Market ? 0 : 1;
			}
			if (sob.OrderType == OrderType.Market)
			{
				// offer a goes to the left
				return -1;
			}

			// Neither order is MKT

			return (int)(sob.LimitPrice - soa.LimitPrice);
		}

		int BuyInsertSortOrder(int aId, int bId)
		{
			ref Order boa = ref Orders.ElementAtRef(aId);
			if (boa.OrderType == OrderType.Market)
			{
				// Market before limit orders (place a on the right (end))
				return 1;
			}
			ref Order bob = ref Orders.ElementAtRef(bId);
			if (bob.OrderType == OrderType.Market)
			{
				// Order a goes to the left
				return -1;
			}

			// Neither order is MKT

			return (int)(boa.LimitPrice - bob.LimitPrice);
		}
	}
}
