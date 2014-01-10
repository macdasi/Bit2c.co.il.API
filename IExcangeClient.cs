using System;
using System.Collections.Generic;
namespace Bit2c.co.il.API.Client
{
    interface IExcangeClient
    {
        AddOrderResponse AddOrder(OrderData item);
        UserBalance Balance();
        void CancelOrder(long orderid);
        Orders MyOrders(PairType pair);
        void ClearMyOrders(PairType pair);
        List<AccountRaw> AccountHistory(DateTime? fromTime, DateTime? toTime);
    }
}
