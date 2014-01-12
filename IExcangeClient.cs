using System;
using System.Collections.Generic;
namespace Bit2c.co.il.API.Client
{
    interface IExcangeClient
    {
        System.Collections.Generic.List<AccountRaw> AccountHistory(DateTime? fromTime, DateTime? toTime);
        AddOrderResponse AddOrder(OrderData data);
        UserBalance Balance();
        void CancelOrder(long id);
        void ClearMyOrders(PairType pair);
        CheckoutResponse CreateCheckout(CheckoutLinkModel data);
        OrderBook GetOrderBook(PairType Pair = PairType.BtcNis);
        Ticker GetTicker(PairType Pair = PairType.BtcNis);
        System.Collections.Generic.List<ExchangesTrade> GetTrades(PairType Pair = PairType.BtcNis, long since = 0, double date = 0);
        Orders MyOrders(PairType pair);
    }
}
