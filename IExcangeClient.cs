using System;
using System.Collections.Generic;
namespace Bit2cPlatform.Client
{
    interface IExcangeClient
    {
        System.Collections.Generic.List<AccountRaw> AccountHistory(DateTime? fromTime, DateTime? toTime);
        AddOrderResponse AddOrder(OrderData data);
        UserBalance Balance();
        CancelOrderReponse CancelOrder(long id);
        void ClearMyOrders(PairType pair);
        CheckoutResponse CreateCheckout(CheckoutLinkModel data);
        OrderBook GetOrderBook(PairType Pair);
        Ticker GetTicker(PairType Pair);
        List<ExchangesTrade> GetTrades(PairType Pair, long? since = null, DateTime? date = null);
        Orders MyOrders(PairType pair);

        List<PairType> GetSupportedPairs();
    }
}
