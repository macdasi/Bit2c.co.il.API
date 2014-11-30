using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Web;

namespace Bit2cPlatform.Client
{


    public abstract class ExchangeClient : IExcangeClient
    {
        private string Key;
        private string secret;
        private UInt32 _nonce;
        private string nonce
        {
            get
            {
                return (_nonce++).ToString();
            }
        }
        private string URL { get; set; }
        private WebClient client { get; set; }

        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        private string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + p.GetValue(obj, null).ToString();

            return String.Join("&", properties.ToArray());
        }

        private static string ToQueryString(NameValueCollection nv)
        {
            if (nv.Count == 0)
                return "";
            return "?" +
            String.Join(
                "&",
                nv.AllKeys.SelectMany(key => nv.GetValues(key)
                    .Select(value => String.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))).ToArray());
        }

        private T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

        private UserBalance DeserializeUserBalance(string json)
        {
            UserBalance userBalance = new UserBalance();
            Newtonsoft.Json.Linq.JObject results = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (string curr in allCoins)
            {
                if (results[curr] != null)
                    userBalance.Balance[curr] = (decimal)results[curr];
            }
            if (results["FeeMaker"] != null)
            {
                userBalance.FeeMaker = (decimal)results["FeeMaker"];
                userBalance.FeeTaker = (decimal)results["Fee"];
            }
            else
            {
                userBalance.FeeMaker = (decimal)results["Fee"];
                userBalance.FeeTaker = (decimal)results["Fee"];
            }
            userBalance.TradeVolume = (decimal)results["TradeVolume"];
            userBalance.TotalTradeVolume = (decimal)results["TotalTradeVolume"];
            return userBalance;
        }

        private Orders DeserializeOrders(string json)
        {
            Orders orders = new Orders();
            orders.bids = new List<TradeOrder>();
            orders.asks = new List<TradeOrder>();
            Newtonsoft.Json.Linq.JObject results = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var order in results["bids"])
            {
                TradeOrder add = new TradeOrder();
                orders.bids.Add(new TradeOrder() {
                    a = (decimal)order["a"],
                    d = (DateTime)order["d"],
                    id = (long)order["id"],
                    isBid = (bool)order["isBid"],
                    p = (decimal)order["p"],
                    pair = GetSupportedPairs()[(int)order["pair"]],
                    s = order["s"].ToObject<OrderStatusType>(),
                });
            }
            return orders;
        }

        private List<AccountRaw> DeserializeListAccountRaw(string json)
        {
            List<AccountRaw> ret = new List<AccountRaw>();
            Newtonsoft.Json.Linq.JArray results = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (Newtonsoft.Json.Linq.JToken line in results)
            {
                AccountRaw newLine = new AccountRaw();
                foreach (Newtonsoft.Json.Linq.JToken val in line.Children())
                {
                    if (!(val is Newtonsoft.Json.Linq.JProperty))
                        continue;
                    Newtonsoft.Json.Linq.JProperty prop = (Newtonsoft.Json.Linq.JProperty)val;
                    if (prop.Name.StartsWith("Balance"))
                    {
                        newLine.Balance[prop.Name.Substring("Balance".Length)] = (decimal)prop.Value;
                    }
                    if (prop.Name.StartsWith("FeeIn"))
                    {
                        newLine.FeeIn[prop.Name.Substring("FeeIn".Length)] = (decimal?)prop.Value;
                    }
                    if (allCoins.Contains(prop.Name))
                    {
                        newLine.Changed[prop.Name] = (decimal?)prop.Value;
                    }
                }
                newLine.Created = (DateTime)line["Created"];
                newLine.Fee = (decimal?)line["Fee"];
                newLine.id = (long)line["id"];
                newLine.OrderCreated = (DateTime?)line["OrderCreated"];
                newLine.PricePerCoin = (decimal?)line["PricePerCoin"];
                newLine.Ref = (string)line["Ref"];
                newLine.TypeId = (int)line["TypeId"];
                ret.Add(newLine);
            }
            return ret;
        }

        public string HistoryTypeIdString(int typeId)
        {
            return dicTypeIds[typeId];
        }

        private List<string> allCoins = new List<string>() { "BTC", "LTC", "NIS", "VND" };
        public abstract List<PairType> GetSupportedPairs();
        public abstract List<CoinType> GetSupportedCoins();
        public abstract List<CoinType> GetAcceptPaymentCoinTypes();

        protected abstract Dictionary<int, string> dicTypeIds { get; }

        private static string ComputeHash(string secret, string message)
        {
            var key = Encoding.ASCII.GetBytes(secret.ToUpper());
            string hashString;

            using (var hmac = new HMACSHA512(key))
            {
                var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(message));
                hashString = Convert.ToBase64String(hash);
            }

            return hashString;
        }

        public ExchangeClient(string key, string secret, string url)
        {
            this.URL = url;
            this.Key = key;
            this.secret = secret;
            client = new WebClient();
            client.Headers.Add("key", key);
            _nonce = UnixTime.Now;
        }

        // TODO: Consistency: get DateTime?
        public List<ExchangesTrade> GetTrades(PairType Pair, long? since = null, DateTime? date = null)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection nv = new NameValueCollection();
                if (since.HasValue) nv.Add("since", since.ToString());
                if (date.HasValue) nv.Add("date", ConvertToUnixTimestamp(date.Value).ToString());
                string qString = ToQueryString(nv);
                string result = client.DownloadString(URL + "Exchanges/" + Pair.ToString() + "/trades.json" + qString);
                List<ExchangesTrade> response = Deserialize<List<ExchangesTrade>>(result);
                return response;
            }
        }

        public Ticker GetTicker(PairType Pair)
        {
            using (WebClient client = new WebClient())
            {
                string result = client.DownloadString(URL + "Exchanges/" + Pair.ToString() + "/Ticker.json");
                Ticker response = Deserialize<Ticker>(result);
                return response;
            }
        }

        public OrderBook GetOrderBook(PairType Pair)
        {
            using (WebClient client = new WebClient())
            {
                string result = client.DownloadString(URL + "Exchanges/" + Pair.ToString() + "/orderbook.json");
                OrderBook response = Deserialize<OrderBook>(result);
                return response;
            }
        }

        public AddOrderResponse AddOrder(OrderData data)
        {
            data.Amount = decimal.Round(data.Amount, 4);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddOrder";
            string result = Query(qString, url, Key, sign, "POST");
            AddOrderResponse response = Deserialize<AddOrderResponse>(result);
            return response;
        }

        public Orders MyOrders(PairType pair)
        {
            string qString = "pair=" + pair.ToString() + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/MyOrders";
            string result = Query(qString, url, Key, sign, "POST");
            Orders response = DeserializeOrders(result);
            return response;
        }

        public void ClearMyOrders(PairType pair)
        {
            var response2 = client.DownloadString(URL + "Order/MyOrders?pair=" + pair.ToString());
            Orders myOrders = Deserialize<Orders>(response2);
            foreach (var item in myOrders.asks)
            {
                if (item.pair == pair)
                {
                    CancelOrder(item.id);
                }
            }
            foreach (var item in myOrders.bids)
            {
                if (item.pair == pair)
                {
                    CancelOrder(item.id);
                }
            }
        }

        public UserBalance Balance()
        {
            string qString = "nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Account/Balance/v2";
            string result = Query(qString, url, Key, sign, "POST");
            UserBalance response = DeserializeUserBalance(result);
            return response;
        }

        public List<AccountRaw> AccountHistory(DateTime? fromTime = null, DateTime? toTime = null)
        {
            string qString = "";
            string formatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            //formatString = "dd/MM/yyyy HH:mm:ss.fff";
            if (fromTime.HasValue) qString += "fromTime=" + fromTime.Value.ToString(formatString) + "&";
            if (toTime.HasValue) qString += "toTime=" + toTime.Value.ToString(formatString) + "&";
            qString += "nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AccountHistory";
            string result = Query(qString, url, Key, sign, "POST");
            List<AccountRaw> response = DeserializeListAccountRaw(result);
            return response;
        }
        public CancelOrderReponse CancelOrder(long id)
        {
            string qString = "id=" + id.ToString() + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/CancelOrder";
            string result = Query(qString, url, Key, sign, "POST");
            CancelOrderReponse response = Deserialize<CancelOrderReponse>(result);
            return response;
        }

        public Guid GetMyPaymentId()
        {
            string qString = string.Format("nonce={0}", nonce);
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Payment/GetMyId";
            string result = Query(qString, url, Key, sign, "POST");
            Guid response = Deserialize<Guid>(result);
            return response;
        }

        public SendPaymentResponse Send(Guid payTo, decimal Total)
        {
            /* Only BTC works */
            CoinType coin = CoinType.BTC;
            string qString = string.Format("payTo={0}&Total={1}&coin={2}&nonce={3}", payTo, Total, coin, nonce);
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Payment/Send";
            string result = Query(qString, url, Key, sign, "POST");
            SendPaymentResponse response = Deserialize<SendPaymentResponse>(result);
            return response;
        }

        public AskFundResponse AddFund(AskFund data)
        {
            System.Threading.Thread.Sleep(1000);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddFund";
            string result = Query(qString, url, Key, sign, "POST");
            AskFundResponse response = Deserialize<AskFundResponse>(result);
            return response;
        }

        public AddCoinFundResponse AddCoinFundsRequest(decimal total, CoinType Coin)
        {
            System.Threading.Thread.Sleep(1000);
            string qString = "total=" + total.ToString() + "&Coin=" + Coin.ToString() + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddCoinFundsRequest";
            string result = Query(qString, url, Key, sign, "POST");
            AddCoinFundResponse response = Deserialize<AddCoinFundResponse>(result);
            return response;
        }

        public AddOrderResponse AddOrderMarketPriceBuy(OrderBuy data)
        {
            System.Threading.Thread.Sleep(1000);
            data.Total = decimal.Round(data.Total, 4);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddOrderMarketPriceBuy";
            string result = Query(qString, url, Key, sign, "POST");
            AddOrderResponse response = Deserialize<AddOrderResponse>(result);
            return response;
        }

        public AddOrderResponse AddOrderMarketPriceSell(OrderSell data)
        {
            System.Threading.Thread.Sleep(1000);
            data.Amount = decimal.Round(data.Amount, 4);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddOrderMarketPriceSell";
            string result = Query(qString, url, Key, sign, "POST");
            AddOrderResponse response = Deserialize<AddOrderResponse>(result);
            return response;
        }

        public CheckoutResponse CreateCheckout(CheckoutLinkModel data)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Merchant/CreateCheckout";
            string result = Query(qString, url, Key, sign, "POST");
            CheckoutResponse response = Deserialize<CheckoutResponse>(result);
            return response;
        }

        private string Query(string qString, string url, string key, string sign, string method)
        {
            var data = Encoding.ASCII.GetBytes(qString);

            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            if (request == null)
                throw new Exception("Non HTTP WebRequest");

            request.Method = method;
            request.Timeout = 15000;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            request.Headers.Add("Key", key);
            request.Headers.Add("Sign", sign);
            var reqStream = request.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            try
            {
                var response = request.GetResponse();
                var resStream = response.GetResponseStream();
                var resStreamReader = new StreamReader(resStream);
                var resString = resStreamReader.ReadToEnd();

                return resString;
            }
            catch (WebException ex)
            {
                if ((ex.Response is HttpWebResponse) && ((int)((HttpWebResponse)ex.Response).StatusCode == 403))
                    throw new PermissionException("API key not enabled for write operations.");
                else
                    throw;
            }
        }
    }

    public static class UnixTime
    {
        static DateTime unixEpoch = new DateTime(1970, 1, 1);

        public static UInt32 Now
        {
            get
            {
                return GetFromDateTime(DateTime.UtcNow);
            }
        }

        public static UInt32 GetFromDateTime(DateTime d)
        {
            var dif = d - unixEpoch;
            return (UInt32)dif.TotalSeconds;
        }

        public static DateTime ConvertToDateTime(UInt32 unixtime)
        {
            return unixEpoch.AddSeconds(unixtime);
        }


    }
}
