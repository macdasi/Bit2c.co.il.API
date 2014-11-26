using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Bit2cPlatform.Client
{
    public class ExchangesTrade
    {
        /// <summary>
        /// date is the unixtimestamp of the trade
        /// </summary>
        public double date { get; set; }
        /// <summary>
        /// price is the price as in your markets currency (e.g. USD). 
        /// Make sure it's not a float but a real decimal. You may add quotes if that makes things easier for you.
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// amount is the amount of bitcoins exchanged in that trade.
        /// Don't use floats here either. You may add quotes, too.
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// tid is a unique trade id (monotonically increasing integer) for each trade
        /// </summary>
        public long tid { get; set; }
    }

    public class Ticker
    {
        /// <summary>
        /// high bid
        /// </summary>
        public decimal h { get; set; }
        /// <summary>
        /// low ask
        /// </summary>
        public decimal l { get; set; }
        /// <summary>
        /// trade last
        /// </summary>
        public decimal ll { get; set; }
        /// <summary>
        /// last 24 hours volume
        /// </summary>
        public decimal a { get; set; }
        /// <summary>
        /// last 24 hours average
        /// </summary>
        public decimal av { get; set; }
    }

    public class OrderBook
    {
        public List<List<decimal>> asks { get; set; }
        public List<List<decimal>> bids { get; set; }
    }


    public class UserBalance
    {
        public Dictionary<string, decimal> Balance = new Dictionary<string, decimal>();
        public decimal FeeMaker;
        public decimal FeeTaker;
        public decimal TradeVolume;
        public decimal TotalTradeVolume;
    }

    public class AccountRaw
    {
        public Dictionary<string, decimal> Balance = new Dictionary<string, decimal>();
        public Dictionary<string, decimal?> Changed = new Dictionary<string, decimal?>();
        public Dictionary<string, decimal?> FeeIn = new Dictionary<string, decimal?>();
        public DateTime Created { get; set; }
        public decimal? Fee { get; set; }
        public long id { get; set; }
        public DateTime? OrderCreated { get; set; }
        public decimal? PricePerCoin { get; set; }
        public string Ref { get; set; }
        public int TypeId { get; set; }
    }

    public class SendPaymentResponse
    {
        public string reff { get; set; }
        public string error { get; set; }
    }

    public class AskFundResponse
    {
        public string message { get; set; }
        public string error { get; set; }
    }

    public class AskFund
    {
        // TODO: Make generic
        public decimal TotalInNIS { get; set; }

        public string Reference { get; set; }

        public bool IsDeposit { get; set; }
    }

    public class AddCoinFundResponse
    {
        public string address { get; set; }
    }

    public class AddOrderResponse
    {
        public OrderResponse OrderResponse { get; set; }
        /// <summary>
        /// New order response after:
        /// 1.validation of engine 
        /// 2.set status to order according to user balance(open/no funds) 
        /// 3.added to orderbook 
        /// 4.found a match for trade
        /// </summary>
        public od NewOrder { get; set; }
    }

    public class OrderResponse
    {
        public bool HasError { get; set; }
        public string Error { get; set; }
    }

    public class CancelOrderReponse
    {
        public OrderResponse OrderResponse;
    }

    /// <summary>
    /// order details
    /// </summary>
    public class od
    {
        /// <summary>
        /// date created
        /// </summary>
        public double d { get; set; }
        /// <summary>
        /// type sell or buy
        /// </summary>
        public bool t { get; set; }
        /// <summary>
        /// status
        /// </summary>
        public int s { get; set; }
        /// <summary>
        /// amount
        /// </summary>
        public decimal a { get; set; }
        /// <summary>
        /// price
        /// </summary>
        public decimal p { get; set; }
        /// <summary>
        /// pair
        /// </summary>
        public decimal p1 { get; set; }
        /// <summary>
        ///id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// account action type 
        /// </summary>
        public int aa { get; set; }
    }

    public class OrderData
    {
        public decimal Amount { get; set; }
        /// <summary>
        /// price with 5 figures after point is valid, after that it is rounded
        /// </summary>
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        /// <summary>
        /// true for buy false for sell
        /// </summary>
        public bool IsBid { get; set; }
        public PairType Pair { get; set; }
    }

    public enum OrderStatusType
    {
        New = 0,
        Open = 1,
        NoFunds = 2,
        Wait = 3
    }

    public class PairType
    {
        public PairType()
        {
        }

        public PairType(string pairText)
        {
            if (pairText == null || pairText.Length != 6)
                throw new ArgumentException("Invalid pair string", "pairText");
            C1 = new CoinType(pairText.Substring(0, 3));
            C2 = new CoinType(pairText.Substring(3, 3));
        }

        public CoinType C1;
        public CoinType C2;
        public override string ToString()
        {
            if (C1 == null || C2 == null || C1.ShortName == null || C2.ShortName == null)
                return "<null PairType>";
            return string.Format(
                "{0}{1}",
                CultureInfo.InvariantCulture.TextInfo.ToTitleCase(C1.ShortName.ToLower()),
                CultureInfo.InvariantCulture.TextInfo.ToTitleCase(C2.ShortName.ToLower()));
        }
    }

    public class CoinType
    {
        public CoinType(string shortName)
        {
            ShortName = shortName;
        }
        public override string ToString()
        {
            return ShortName;
        }
        public readonly string ShortName;
        public static readonly CoinType BTC = new CoinType("BTC");
    }

    public class TradeOrder
    {
        /// <summary>
        /// amount
        /// </summary>
        public decimal a { get; set; }
        /// <summary>
        /// date created utc
        /// </summary>
        public DateTime d { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// price
        /// </summary>
        public decimal p { get; set; }
        /// <summary>
        /// pair
        /// </summary>
        public PairType pair { get; set; }
        /// <summary>
        /// true buy - false sell
        /// </summary>
        public bool isBid { get; set; }
        public OrderStatusType s { get; set; }
    }

    public class Orders
    {
        public List<TradeOrder> bids { get; set; }
        public List<TradeOrder> asks { get; set; }
    }

    public class NewOrders
    {
        public List<OrderData> bids { get; set; }
        public List<OrderData> asks { get; set; }
        public NewOrders()
        {
            bids = new List<OrderData>();
            asks = new List<OrderData>();
        }
    }

    //for merchants 
    public class CheckoutLinkModel
    {
        public decimal Price { get; set; }
        public string Description { get; set; }
        public CoinType CoinType { get; set; }
        public string ReturnURL { get; set; }
        public string CancelURL { get; set; }
        public bool NotifyByEmail { get; set; }
    }

    public class CheckoutResponse
    {
        public string error { get; set; }
        public Guid id { get; set; }
    }

    public class OrderSell
    {
        public decimal Amount { get; set; }
        public PairType Pair { get; set; }
    }
    public class OrderBuy
    {
        public decimal Total { get; set; }
        public PairType Pair { get; set; }
    }
}
