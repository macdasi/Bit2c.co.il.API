using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Bit2c.co.il.API.Client
{
    public class TradeConfig 
    {
        public decimal FeeOnTrade { get; set; }
        public decimal PtoRisk { get; set; }
        public int SecondsCheckOrders { get; set; }
        public bool ExecuteBtceTrades { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Site { get; set; }
        public int millisecondsSleep { get; set; }
        public decimal perRisk { get; set; }
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

    public class AddOrderResponse
    {
        public OrderResponse OrderResponse { get; set; }
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
        public decimal Price { get; set; }
        public decimal Total { get; set; }
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
        public decimal a { get; set; }
        public DateTime d { get; set; }
        public long id { get; set; }
        public decimal p { get; set; }
        public PairType pair { get; set; }
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
}
