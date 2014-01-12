using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bit2c.co.il.API.Client
{
    class Program
    {
        /// <summary>
        /// sample use of Bit2c.co.il calls with api client
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string url = "http://localhost:53825/"; //For prod -  https://www.bit2c.co.il/
            // key & secret you can find in account settings
            string key = "5692036b-23b9-48c7-aedf-17db72e23a5b";
            string secret = "b4f51ab9854e3ae37d7fbba7b65dd18df6b4f9608d61bfec2c8584e89fc23a90";
            Bit2cClient client = new Bit2cClient(url, key, secret);

            //public calls no need for "secret" or Key
            var trades = client.GetTrades(PairType.LtcBtc);
            var ticker = client.GetTicker(PairType.LtcBtc);
            var data = client.GetOrderBook(PairType.BtcNis);

            //merchant create checkout button
            CheckoutLinkModel model = new CheckoutLinkModel
            {
                CancelURL = string.Empty
                ,
                CoinType = Client.CoinType.BTC
                ,
                Description = "Mysale"
                ,
                NotifyByEmail = true
                ,
                Price = 0.123m
                ,
                ReturnURL = string.Empty
            };
            var checkout_button_id = client.CreateCheckout(model);


            //priavet calls - include secret & Key
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
