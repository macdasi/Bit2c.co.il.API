using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bit2cPlatform.Client
{
    class Program
    {
        /// <summary>
        /// sample use of Bit2c.co.il calls with api client
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            /*string url = "http://localhost:53825/";*/
            // key & secret you can find in account settings
            string key = "f095a305-45f1-461c-b024-c0ede7af62dc";
            string secret = "4989fe13e20a607cedb9caca33f9a233765829da5bcf4b7ffd85a9813b2f0794";
            ExchangeClient client = new Bit2c(key, secret/*, url*/);

            //public calls no need for "secret" or Key
            var trades = client.GetTrades(Bit2c.Pairs.LtcBtc);
            var ticker = client.GetTicker(Bit2c.Pairs.LtcBtc);
            var data = client.GetOrderBook(Bit2c.Pairs.LtcBtc);

            var _MyPaymentId = client.GetMyPaymentId();

            //this should alert error can not send self payment
            var sendresp = client.Send(_MyPaymentId, 1.4m);
            

            var account = client.AccountHistory(new DateTime(2014, 1, 14), new DateTime(2014, 1, 16,14,00,00));

            var addfundr = client.AddFund(new AskFund { IsDeposit = true, Reference = "12324", Total = 2000 });

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
                 Amount = 1m , IsBid = true , Pair = Bit2c.Pairs.BtcNis , Price = 1000m , Total = 1000m
            });
            var data3 = client.MyOrders( Bit2c.Pairs.BtcNis);
            client.CancelOrder(data3.bids[0].id);
            var data4 = client.AccountHistory(null, null);
        }
    }
}
