using System;
using System.Collections.Generic;

using Portland.AI;
using Portland.Collections;
using Portland.RPG.Accounting;
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
		Empty = 0,
		Open,
		Filled,
		Canceled
	}

	public class ExchangeLocation
	{
		public AsciiId4 LocationCode;

		public Dictionary<String10, OrderBook> OrderBooks = new();

		public OrderBook GetOrCreateOrderBood(in String10 itemId)
		{
			if (OrderBooks.TryGetValue(itemId, out var orderBook))
			{
				return orderBook;
			}

			orderBook = new OrderBook() { ItemId = itemId };
			OrderBooks.Add(itemId, orderBook);
			return orderBook;
		}
	}

	public struct MsgResourceOrderDoCreate
	{
		public String10 ItemId;
		public AsciiId4 LocationCode;
		public TimeInForce TIF;
		public OrderAction BuyOrSell;
		public OrderType OrderType;
		public int Size;
		public int LimitPrice;
		public Date ExpireDate;
		public String10 CharId;
		public int AccountId;
	}

	public enum OrderCanceledReason
	{
		Unauthorized,
		InsufficientFunds
	}

	public struct MsgResourceOrderOnCanceled
	{
		public String10 CharId;
		public int AccountId;
		public AsciiId4 LocationCode;
		public String10 ItemId;
		public int Size;
		public int LimitPrice;
		public Date ExpireDate;
		public OrderCanceledReason Reason;

		public MsgResourceOrderOnCanceled(in MsgResourceOrderDoCreate msg, in OrderCanceledReason reason)
		{
			CharId = msg.CharId;
			AccountId = msg.AccountId;
			LocationCode = msg.LocationCode;
			ItemId = msg.ItemId;
			Size = msg.Size;
			LimitPrice = msg.LimitPrice;
			ExpireDate = msg.ExpireDate;
			Reason = reason;
		}
	}

	public class ResourceManager
	{
		Vector<Account> _accounts = new(65);
		Dictionary<int, int[]> _accountsByAgentId = new();
		Dictionary<String10, Ledger> _ledgers = new();
		Vector<Order> _orders = new();
		Vector<int> _orderPool = new();
		Dictionary<AsciiId4, ExchangeLocation> _exchangeLocations = new();
		IMessageBusTyped _bus;
		IClock _clock;

		public ResourceManager(IClock clock, IMessageBusTyped bus)
		{
			_bus = bus;
			_clock = clock;

			for (int i = 0; i < 10; ++i)
			{
				_orders.Add(new Order { OrderId = i, Status = OrderStatus.Empty });
				_orderPool.Add(i);
			}

			bus.Subscribe<MsgResourceOrderDoCreate>(DoOrderCreate);
		}

		public void Shutdown()
		{
			_bus.Unsubscribe<MsgResourceOrderDoCreate>(DoOrderCreate);
		}

		void DoOrderCreate(MsgResourceOrderDoCreate msg)
		{
			var account = _accounts[msg.AccountId];
			if (account.OwnerCharId != msg.CharId)
			{
				_bus.Send(new MsgResourceOrderOnCanceled(msg, OrderCanceledReason.Unauthorized));
				return;
			}
			
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
				AccountId = msg.AccountId, 
				CreatedDate = _clock.Now
			});

			var location = GetOrCreateLocation(msg.LocationCode);
			var orderBook = location.GetOrCreateOrderBood(msg.ItemId);
			var orderId = GetOrCreateOrder();

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

		int GetOrCreateOrder()
		{
			if (_orderPool.Count > 0)
			{
				return _orderPool.Pop();
			}

			return _orders.AddAndGetIndex(new Order { OrderId = _orders.Count, Status = OrderStatus.Empty });
		}
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
		public int AccountId;
		public ShortDateTime CreatedDate;
	}

	public struct OrderTransation
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

	public class OrderBook
	{
		public String10 ItemId;

		// ordered by bid asc, highest bid LAST
		Vector<int> BuyOrders = new();
		// ordered by offer desc, lowest price LAST 
		Vector<int> SellOrders = new();

		Vector<OrderTransation> ExecutionResults = new();

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
