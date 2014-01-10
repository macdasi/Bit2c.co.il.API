﻿using System;
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

namespace Bit2c.co.il.API.Client
{
    public class HashHelper
    {
        public static string getHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }

    public class Bit2cClient : IExcangeClient
    {
        public string Key;
        public string secret;
        private UInt32 _nonce;
        public string nonce
        {
            get
            {
                return (_nonce++).ToString();
            }
        }
        public string URL { get; set; }
        public WebClient client { get; set; }

        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
        
        public T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

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

        public Bit2cClient(string url, string key, string secret)
        {
            try
            {
                this.URL = url; 
                this.Key = key; 
                this.secret = secret;
                client = new WebClient();
                client.Headers.Add("key", key);
                _nonce = UnixTime.Now;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        

        public AddOrderResponse AddOrder(OrderData data)
        {
            try
            {
                data.Amount = decimal.Round(data.Amount, 4);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                string qString = GetQueryString(data) + "&nonce=" + nonce;
                var sign = ComputeHash(this.secret,qString);
                var url = URL + "Order/AddOrder"; 
                string result = Query(qString, url, Key, sign,"POST");
                AddOrderResponse response = Deserialize<AddOrderResponse>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Orders MyOrders(PairType pair)
        {
            try
            {
                string qString = "nonce=" + nonce;
                var sign = ComputeHash(this.secret, qString);
                var url = URL + "Order/MyOrders";
                string result = Query(qString, url, Key, sign, "POST");
                Orders response = Deserialize<Orders>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ClearMyOrders(PairType pair)
        {
            try
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
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public UserBalance Balance()
        {
            try
            {
                string qString = "nonce=" + nonce;
                var sign = ComputeHash(this.secret, qString);
                var url = URL + "Account/Balance";
                string result = Query(qString, url, Key, sign, "POST");
                UserBalance response = Deserialize<UserBalance>(result);
                return response;
            }
            catch (Exception ex)
            {
                
                throw;
            }

        }

        public List<AccountRaw> AccountHistory(DateTime? fromTime, DateTime? toTime)
        {
            try
            {
                string ft = fromTime.HasValue ? fromTime.Value.ToString("dd/MM/yyyy HH:mm:ss.fff") : null;
                string tt = toTime.HasValue ? toTime.Value.ToString("dd/MM/yyyy HH:mm:ss.fff") : null;
                string qString = string.Format("fromTime={0}&toTime={1}&nonce={2}",ft,tt,nonce);
                var sign = ComputeHash(this.secret, qString);
                var url = URL + "Order/AccountHistory"; 
                string result = Query(qString, url, Key, sign, "POST");
                List<AccountRaw> response = Deserialize<List<AccountRaw>>(result);
                return response;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        public void CancelOrder(long id)
        {
            try
            {
                string qString = "id=" + id.ToString() + "&nonce=" + nonce;
                var sign = ComputeHash(this.secret, qString);
                var url = URL + "Order/CancelOrder";
                string result = Query(qString, url, Key, sign, "POST");
                OrderResponse response = Deserialize<OrderResponse>(result);
            }
            catch (Exception ex)
            {
                
                throw;
            }
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

            var response = request.GetResponse();
            var resStream = response.GetResponseStream();
            var resStreamReader = new StreamReader(resStream);
            var resString = resStreamReader.ReadToEnd();

            return resString;
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