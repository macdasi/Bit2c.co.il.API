using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bit2c.co.il.API.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://www.bit2c.co.il/";
            string key = "[your key here]";
            string secret = "[your secret here]";
            Bit2cClient client = new Bit2cClient(url, key, secret);
            var data1 = client.Balance();
            var data2 = client.AddOrder(new OrderData { 
                 Amount = 1m , IsBid = true , Pair = PairType.BtcNis , Price = 1000m , Total = 1000m
            });
            var data3 = client.MyOrders( PairType.BtcNis);
            client.CancelOrder(data3.bids[0].id);
            var data4 = client.AccountHistory(null, null);
        }
    }
}
