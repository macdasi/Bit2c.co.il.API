using System;
using System.Collections.Generic;

namespace Bit2cPlatform.Client
{
    public class Bit2c : ExchangeClient
    {
        public Bit2c(string key, string secret, string url = "https://www.bit2c.co.il/")
            : base(key, secret, url)
        {
        }

        public class Pairs
        {
            public static PairType BtcNis = new PairType() { C1 = new CoinType("BTC"), C2 = new CoinType("NIS") };
            public static PairType LtcNis = new PairType() { C1 = new CoinType("LTC"), C2 = new CoinType("NIS") };
            public static PairType LtcBtc = new PairType() { C1 = new CoinType("LTC"), C2 = new CoinType("BTC") };
        }

        public override List<PairType> GetSupportedPairs()
        {
            /* Order is important! Used for desrializing */
            return new List<PairType>()
            {
                Pairs.BtcNis,
                Pairs.LtcBtc,
                Pairs.LtcNis,
            };
        }

        public override List<CoinType> GetSupportedCoins()
        {
            return new List<CoinType>() {
                new CoinType("BTC"),
                new CoinType("LTC"),
            };
        }

        public override List<CoinType> GetAcceptPaymentCoinTypes()
        {
            return new List<CoinType>() {
                new CoinType("BTC"),
                new CoinType("NIS"),
            };
        }

        protected override Dictionary<int, string> dicTypeIds
        {
            get
            {
                return new Dictionary<int, string>
                {
                    {0, "SellBTC"},                     /* Sell BTC for NIS */
                    {1, "BuyBTC"},                      /* Buy BTC in NIS */
                    {2, "FeeBuyBTC"},                   
                    {3, "FeeSellBTC"},                  
                    {4, "DepositBTC"},                  /* Deposit of BTC */
                    {5, "DepositNIS"},                  /* Deposit of NIS */
                    {6, "WithdrawalBTC"},               /* Withdrawl of BTC */
                    {7, "WithdrawalNIS"},               /* Withdrawl of NIS */
                    {8, "FeeWithdrawalBTC"},            /* Fee for withdrawl of BTC */
                    {9, "FeeWithdrawalNIS"},            /* Fee for withdrawl of NIS */
                    {10, "Unknown"},
                    {11, "PayWithBTC"},                 /* Off-chain payment to exchange user sent */
                    {12, "ReceivedPaymentBTC"},         /* Off-chain payment from exchange user recived */
                    {13, "FeeReceivedPaymentBTC"},
                    {14, "DepositLTC"},                 /* Deposit of LTC */
                    {15, "WithdrawalLTC"},              /* Withdrawl of LTC */
                    {16, "FeeWithdrawalLTC"},           /* Fee for withdrawl of LTC */
                    {17, "BuyLTCBTC"},                  /* Buy LTC in BTC */
                    {18, "SellLTCBTC"},                 /* Sell LTC for BTC */
                    {19, "BuyLTCNIS"},                  /* Buy LTC in NIS */
                    {20, "SellLTCNIS"},                 /* Sell LTC for NIS */
                };
            }
        }
    }
}

