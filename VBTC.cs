using System;
using System.Collections.Generic;

namespace Bit2cPlatform.Client
{
    public class VBTC : ExchangeClient
    {
        public VBTC(string key, string secret, string url = "https://www.vbtc.vn/")
            : base(key, secret, url)
        {
        }

        public class Pairs
        {
            public static PairType BtcVnd = new PairType() { C1 = new CoinType("BTC"), C2 = new CoinType("VND") };
            public static PairType LtcVnd = new PairType() { C1 = new CoinType("LTC"), C2 = new CoinType("VND") };
            public static PairType LtcBtc = new PairType() { C1 = new CoinType("LTC"), C2 = new CoinType("BTC") };
        }

        public override List<PairType> GetSupportedPairs()
        {
            /* Order is important! Used for desrializing */
            return new List<PairType>()
            {
                Pairs.BtcVnd,
                Pairs.LtcBtc,
                Pairs.LtcVnd,
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
                new CoinType("VND"),
            };
        }

        protected override Dictionary<int, string> dicTypeIds
        {
            get
            {
                return new Dictionary<int, string>
                {
                    {0, "SellBTC"},                     /* Sell BTC for VND */
                    {1, "BuyBTC"},                      /* Buy BTC in VND */
                    {2, "FeeBuyBTC"},                   
                    {3, "FeeSellBTC"},                  
                    {4, "DepositBTC"},                  /* Deposit of BTC */
                    {5, "DepositVND"},                  /* Deposit of VND */
                    {6, "WithdrawalBTC"},               /* Withdrawl of BTC */
                    {7, "WithdrawalVND"},               /* Withdrawl of VND */
                    {8, "FeeWithdrawalBTC"},            /* Fee for withdrawl of BTC */
                    {9, "FeeWithdrawalVND"},            /* Fee for withdrawl of VND */
                    {10, "Unknown"},
                    {11, "PayWithBTC"},                 /* Off-chain payment to exchange user sent */
                    {12, "ReceivedPaymentBTC"},         /* Off-chain payment from exchange user recived */
                    {13, "FeeReceivedPaymentBTC"},
                    {14, "DepositLTC"},                 /* Deposit of LTC */
                    {15, "WithdrawalLTC"},              /* Withdrawl of LTC */
                    {16, "FeeWithdrawalLTC"},           /* Fee for withdrawl of LTC */
                    {17, "BuyLTCBTC"},                  /* Buy LTC in BTC */
                    {18, "SellLTCBTC"},                 /* Sell LTC for BTC */
                    {19, "BuyLTCVND"},                  /* Buy LTC in VND */
                    {20, "SellLTCVND"},                 /* Sell LTC for VND */
                };
            }
        }
    }
}

