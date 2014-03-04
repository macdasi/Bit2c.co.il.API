using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Bit2c.co.il.API.Client
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
        public decimal BalanceBTC { get; set; }
        public decimal BalanceLTC { get; set; }
        public decimal BalanceNIS { get; set; }
    }

    public enum AccountAction
    {
        SellBTC = 0,
        BuyBTC = 1,
        FeeBuyBTC = 2,
        FeeSellBTC = 3,
        DepositBTC = 4,
        DepositNIS = 5,
        WithdrawalBTC = 6,
        WithdrawalNIS = 7,
        FeeWithdrawalBTC = 8,
        FeeWithdrawalNIS = 9,
        Unknown = 10,
        PayWithBTC = 11,
        ReceivedPaymentBTC = 12,
        FeeReceivedPaymentBTC = 13,
        DepositLTC = 14,
        WithdrawalLTC = 15,
        FeeWithdrawalLTC = 16,
        BuyLTCBTC = 17,
        SellLTCBTC = 18,
        BuyLTCNIS = 19,
        SellLTCNIS = 20,
    }

    public class AccountRaw
    {
        public decimal BalanceBTC { get; set; }
        public decimal BalanceLTC { get; set; }
        public decimal BalanceNIS { get; set; }
        public decimal? BTC { get; set; }
        public decimal? LTC { get; set; }
        public decimal? NIS { get; set; }
        public DateTime Created { get; set; }
        public decimal? Fee { get; set; }
        public decimal? FeeInNIS { get; set; }
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
        public decimal TotalInNIS { get; set; }

        public string Reference { get; set; }

        public bool IsDeposit { get; set; }
    }

    public class AddCoinFundResponse {
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

    public enum PairType
    {
        BtcNis = 0,
        LtcBtc = 1,
        LtcNis = 2
    }

    public enum CoinType
    {
        BTC = 0,
        LTC = 1,
        NIS = 2
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
