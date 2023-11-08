using System;
using System.Collections.Generic;

using Portland.AI;
using Portland.Collections;
using Portland.Threading;

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

	public struct MsgExchangeOrderDoCreate
	{
		public String10 ItemId;
		public AsciiId4 LocationCode;
		public TimeInForce TIF;
		public OrderAction BuyOrSell;
		public OrderType OrderType;
		public int Size;
		public int LimitPrice;
		public Date ExpireDate;
		public int AgentId;
	}

	public class ExchangeManager
	{
		Vector<Order> _orders = new();
		Dictionary<AsciiId4, ExchangeLocation> _exchangeLocations = new();
		IMessageBusTyped _bus;
		IClock _clock;

		public ExchangeManager(IClock clock, IMessageBusTyped bus)
		{
			_bus = bus;
			_clock = clock;

			bus.Subscribe<MsgExchangeOrderDoCreate>(DoOrderCreate);
		}

		public void Shutdown()
		{
			_bus.Unsubscribe<MsgExchangeOrderDoCreate>(DoOrderCreate);
		}

		void DoOrderCreate(MsgExchangeOrderDoCreate msg)
		{
			int id = _orders.Count;
			_orders.Add(new Order { 
				OrderId = id, 
				ItemId = msg.ItemId, 
				LocationCode = msg.LocationCode, 
				TIF = msg.TIF, 
				BuyOrSell = msg.BuyOrSell, 
				OrderType = msg.OrderType, 
				Size = msg.Size, 
				LimitPrice = msg.LimitPrice, 
				ExpireDate = msg.ExpireDate, 
				Status = OrderStatus.Open, 
				AgentId = msg.AgentId, 
				CreatedDate = _clock.Now
			});

			var location = GetOrCreateLocation(msg.LocationCode);

		}

		ExchangeLocation GetOrCreateLocation(AsciiId4 locationCode)
		{
			if (_exchangeLocations.TryGetValue(locationCode, out var location))
			{
				return location;
			}

			location = new ExchangeLocation() { LocationCode = locationCode };
			_exchangeLocations.Add(locationCode, location);
			return location;
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
