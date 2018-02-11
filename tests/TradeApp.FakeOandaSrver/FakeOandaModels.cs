using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeApp.FakeOandaSrver
{
    public class FakeOandaContext
    {
        private static DateTime NOW;
        static FakeOandaContext()
        {
            var d = DateTime.Now;
            NOW = new DateTime((d.Ticks / TimeSpan.FromSeconds(1).Ticks) * TimeSpan.FromSeconds(1).Ticks);
        }

        public class FakeOandaAccount
        {
            public FakeOandaAccount(int accountId, string accountName, string accountCurrency, decimal marginRate)
            {
                AccountId = accountId;
                AccountName = accountName;
                AccountCurrency = accountCurrency;
                MarginRate = marginRate;
                Balance = 3000;
            }

            public int AccountId { get; private set; }
            public string AccountName { get; private set; }
            public string AccountCurrency { get; private set; }
            public decimal MarginRate { get; private set; }
            public decimal Balance { get; private set; }

            public decimal UnrealizedProfitLoss { get { return 0; } }
            public decimal RealizedProfitLoss { get { return 0; } }
            public decimal MarginUsed { get { return 0; } }
            public decimal MarginAvail { get { return Balance - MarginUsed; } }
            public int OpenTrades { get { return 0; } }
            public int OpenOrders { get { return 0; } }

            public Dictionary<int, FakeOandaOrder> Orders { get; } = new Dictionary<int, FakeOandaOrder>();
            public Dictionary<int, FakeOandaTrade> Trades { get; } = new Dictionary<int, FakeOandaTrade>();
        }

        public class FakeOandaInstrument
        {
            public FakeOandaInstrument(
                string instrument,
                string displayName,
                decimal pip,
                int maxTradeUnits,
                double precision,
                decimal maxTrailingStop,
                decimal minTrailingStop,
                decimal marginRate,
                bool halted,
                decimal baseAsk,
                decimal baseBid)
            {
                Instrument = instrument;
                DisplayName = displayName;
                Pip = pip;
                MaxTradeUnits = maxTradeUnits;
                Precision = precision;
                MaxTrailingStop = maxTrailingStop;
                MinTrailingStop = minTrailingStop;
                MarginRate = marginRate;
                Halted = halted;
                BaseAsk = baseAsk;
                BaseBid = baseBid;
            }
            public string Instrument { get; private set; }
            public string DisplayName { get; private set; }
            public decimal Pip { get; private set; }
            public int MaxTradeUnits { get; private set; }
            public double Precision { get; private set; }
            public decimal MaxTrailingStop { get; private set; }
            public decimal MinTrailingStop { get; private set; }
            public decimal MarginRate { get; private set; }
            public bool Halted { get; private set; }
            public decimal BaseAsk { get; set; }
            public decimal BaseBid { get; set; }
        }

        public class FakeOandaPrice
        {
            public FakeOandaPrice(string instrument, DateTime time, decimal ask, decimal bid)
            {
                Instrument = instrument;
                Time = time;
                Ask = ask;
                Bid = bid;
            }

            public string Instrument { get; private set; }
            public DateTime Time { get; private set; }
            public decimal Ask { get; private set; }
            public decimal Bid { get; private set; }
        }

        public enum FakeOandaSide
        {
            buy,
            sell
        }

        public enum FakeOandaOrderType
        {
            limit,
            stop,
            marketIfTouched,
            market
        }

        public class FakeOandaOrder
        {
            public FakeOandaOrder(
                int id,
                string instrument,
                int units,
                FakeOandaSide side,
                FakeOandaOrderType type,
                DateTime time,
                decimal? price,
                decimal? takeProfit,
                decimal? stopLoss,
                DateTime? expiry,
                decimal? upperBound,
                decimal? lowerBound,
                decimal? trailingStop)
            {
                Id = id;
                Instrument = instrument;
                Units = units;
                Side = side;
                Type = type;
                Time = time;
                Price = price;
                TakeProfit = takeProfit;
                StopLoss = stopLoss;
                Expiry = expiry;
                UpperBound = upperBound;
                LowerBound = lowerBound;
                TrailingStop = trailingStop;
            }

            public int Id { get; private set; }
            public string Instrument { get; private set; }
            public int Units { get; private set; }
            public FakeOandaSide Side { get; private set; }
            public FakeOandaOrderType Type { get; private set; }
            public DateTime Time { get; private set; }
            public decimal? Price { get; private set; }
            public decimal? TakeProfit { get; private set; }
            public decimal? StopLoss { get; private set; }
            public DateTime? Expiry { get; private set; }
            public decimal? UpperBound { get; private set; }
            public decimal? LowerBound { get; private set; }
            public decimal? TrailingStop { get; private set; }
        }

        public class FakeOandaTrade
        {
            public FakeOandaTrade(
                int id,
                string instrument,
                int units,
                FakeOandaSide side,
                DateTime time,
                decimal price,
                decimal? takeProfit,
                decimal? stopLoss,
                decimal? trailingStop)
            {
                Id = id;
                Instrument = instrument;
                Units = units;
                Side = side;
                Time = time;
                Price = price;
                TakeProfit = takeProfit;
                StopLoss = stopLoss;
                TrailingStop = trailingStop;
            }

            public int Id { get; private set; }
            public string Instrument { get; private set; }
            public int Units { get; private set; }
            public FakeOandaSide Side { get; private set; }
            public DateTime Time { get; private set; }
            public decimal Price { get; private set; }
            public decimal? TakeProfit { get; private set; }
            public decimal? StopLoss { get; private set; }
            public decimal? TrailingStop { get; private set; }
        }

        public FakeOandaContext()
        {
            Initialize();
        }

        public FakeOandaAccount DefaultAccount { get; private set; }
        public DateTime CurrentTime { get { return NOW; } }

        public Dictionary<int, FakeOandaAccount> Accounts = new Dictionary<int, FakeOandaAccount>();
        public Dictionary<string, FakeOandaInstrument> Instruments = new Dictionary<string, FakeOandaInstrument>();
        public Dictionary<string, FakeOandaPrice> Prices = new Dictionary<string, FakeOandaPrice>();
        private int _orderId;
        private int _tradeId;

        public void Initialize()
        {
            Accounts.Clear();
            Instruments.Clear();

            CreateInstrument("AUD_CAD", "AUD/CAD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 0.99833m, 0.99815m);
            CreateInstrument("AUD_CHF", "AUD/CHF", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 0.77144m, 0.77119m);
            CreateInstrument("AUD_HKD", "AUD/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 6.27109m, 6.26978m);
            CreateInstrument("AUD_JPY", "AUD/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.04m, 88.889m, 88.875m);
            CreateInstrument("AUD_NZD", "AUD/NZD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 1.09473m, 1.09445m);
            CreateInstrument("AUD_SGD", "AUD/SGD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.05789m, 1.05756m);
            CreateInstrument("AUD_USD", "AUD/USD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 0.80211m, 0.80195m);
            CreateInstrument("CAD_CHF", "CAD/CHF", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 0.77277m, 0.77254m);
            CreateInstrument("CAD_HKD", "CAD/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 6.28191m, 6.28091m);
            CreateInstrument("CAD_JPY", "CAD/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.04m, 89.047m, 89.032m);
            CreateInstrument("CAD_SGD", "CAD/SGD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.05972m, 1.05942m);
            CreateInstrument("CHF_HKD", "CHF/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 8.13055m, 8.12882m);
            CreateInstrument("CHF_JPY", "CHF/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 115.251m, 115.238m);
            CreateInstrument("CHF_ZAR", "CHF/ZAR", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 12.57495m, 12.5662m);
            CreateInstrument("EUR_AUD", "EUR/AUD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 1.52789m, 1.52769m);
            CreateInstrument("EUR_CAD", "EUR/CAD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 1.52516m, 1.52498m);
            CreateInstrument("EUR_CHF", "EUR/CHF", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.1784m, 1.17823m);
            CreateInstrument("EUR_CZK", "EUR/CZK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 25.40681m, 25.39895m);
            CreateInstrument("EUR_DKK", "EUR/DKK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 7.44487m, 7.44348m);
            CreateInstrument("EUR_GBP", "EUR/GBP", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 0.8774m, 0.8773m);
            CreateInstrument("EUR_HKD", "EUR/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 9.58005m, 9.57874m);
            CreateInstrument("EUR_HUF", "EUR/HUF", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 309.824m, 309.595m);
            CreateInstrument("EUR_JPY", "EUR/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.04m, 135.798m, 135.785m);
            CreateInstrument("EUR_NOK", "EUR/NOK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 9.63396m, 9.63125m);
            CreateInstrument("EUR_NZD", "EUR/NZD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 1.67248m, 1.67216m);
            CreateInstrument("EUR_PLN", "EUR/PLN", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 4.16914m, 4.16725m);
            CreateInstrument("EUR_SEK", "EUR/SEK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 9.8383m, 9.83597m);
            CreateInstrument("EUR_SGD", "EUR/SGD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.61612m, 1.61572m);
            CreateInstrument("EUR_TRY", "EUR/TRY", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 4.62882m, 4.62704m);
            CreateInstrument("EUR_USD", "EUR/USD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 1.22531m, 1.22526m);
            CreateInstrument("EUR_ZAR", "EUR/ZAR", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 14.82069m, 14.8108m);
            CreateInstrument("GBP_AUD", "GBP/AUD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.74153m, 1.7412m);
            CreateInstrument("GBP_CAD", "GBP/CAD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.73846m, 1.73811m);
            CreateInstrument("GBP_CHF", "GBP/CHF", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.34311m, 1.34293m);
            CreateInstrument("GBP_HKD", "GBP/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 10.91949m, 10.91781m);
            CreateInstrument("GBP_JPY", "GBP/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 154.788m, 154.764m);
            CreateInstrument("GBP_NZD", "GBP/NZD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.90626m, 1.90589m);
            CreateInstrument("GBP_PLN", "GBP/PLN", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 4.75235m, 4.74943m);
            CreateInstrument("GBP_SGD", "GBP/SGD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.84204m, 1.84156m);
            CreateInstrument("GBP_USD", "GBP/USD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.39664m, 1.39651m);
            CreateInstrument("GBP_ZAR", "GBP/ZAR", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 16.89212m, 16.88099m);
            CreateInstrument("HKD_JPY", "HKD/JPY", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 14.1763m, 14.17411m);
            CreateInstrument("NZD_CAD", "NZD/CAD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 0.91209m, 0.91185m);
            CreateInstrument("NZD_CHF", "NZD/CHF", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 0.70465m, 0.70441m);
            CreateInstrument("NZD_HKD", "NZD/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 5.72873m, 5.72737m);
            CreateInstrument("NZD_JPY", "NZD/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.04m, 81.216m, 81.186m);
            CreateInstrument("NZD_SGD", "NZD/SGD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 0.9664m, 0.96607m);
            CreateInstrument("NZD_USD", "NZD/USD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 0.73273m, 0.73259m);
            CreateInstrument("SGD_CHF", "SGD/CHF", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 0.72925m, 0.72903m);
            CreateInstrument("SGD_HKD", "SGD/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 5.92865m, 5.92752m);
            CreateInstrument("SGD_JPY", "SGD/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 84.043m, 84.021m);
            CreateInstrument("TRY_JPY", "TRY/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 29.382m, 29.302m);
            CreateInstrument("USD_CAD", "USD/CAD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 1.24473m, 1.24456m);
            CreateInstrument("USD_CHF", "USD/CHF", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 0.96163m, 0.96157m);
            CreateInstrument("USD_CNH", "USD/CNH", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 6.40567m, 6.405m);
            CreateInstrument("USD_CZK", "USD/CZK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 20.73612m, 20.7283m);
            CreateInstrument("USD_DKK", "USD/DKK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 6.0764m, 6.07487m);
            CreateInstrument("USD_HKD", "USD/HKD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 7.81834m, 7.81798m);
            CreateInstrument("USD_HUF", "USD/HUF", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 252.86m, 252.68m);
            CreateInstrument("USD_INR", "USD/INR", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 63.882m, 63.777m);
            CreateInstrument("USD_JPY", "USD/JPY", 0.01m, 3000000, 0.001, 10000, 5, 0.04m, 110.827m, 110.823m);
            CreateInstrument("USD_MXN", "USD/MXN", 0.0001m, 10000000, 0.00001, 10000, 5, 0.08m, 18.70343m, 18.6991m);
            CreateInstrument("USD_NOK", "USD/NOK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 7.86297m, 7.85986m);
            CreateInstrument("USD_PLN", "USD/PLN", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 3.40279m, 3.40104m);
            CreateInstrument("USD_SAR", "USD/SAR", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 3.75259m, 3.74782m);
            CreateInstrument("USD_SEK", "USD/SEK", 0.0001m, 10000000, 0.00001, 10000, 5, 0.04m, 8.02987m, 8.02751m);
            CreateInstrument("USD_SGD", "USD/SGD", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 1.31894m, 1.31877m);
            CreateInstrument("USD_THB", "USD/THB", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 31.891m, 31.793m);
            CreateInstrument("USD_TRY", "USD/TRY", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 3.77627m, 3.77523m);
            CreateInstrument("USD_ZAR", "USD/ZAR", 0.0001m, 10000000, 0.00001, 10000, 5, 0.05m, 12.09474m, 12.08811m);
            CreateInstrument("ZAR_JPY", "ZAR/JPY", 0.01m, 10000000, 0.001, 10000, 5, 0.05m, 9.173m, 9.16m);

            DefaultAccount = CreateAccount(8954947, "Primary", "USD", 0.05m);
            CreateAccount(8954950, "SweetHome", "CAD", 0.02m);

            Instruments.Values.ToList().ForEach(i =>
            {
                Prices.Add(i.Instrument, i.CurrentPrice(NOW));
            });
        }

        private FakeOandaInstrument CreateInstrument(
                string instrument,
                string displayName,
                decimal pip,
                int maxTradeUnits,
                double precision,
                decimal maxTrailingStop,
                decimal minTrailingStop,
                decimal marginRate,
                decimal ask,
                decimal bid)
        {
            var inst = new FakeOandaInstrument(
                instrument,
                displayName,
                pip,
                maxTradeUnits,
                precision,
                maxTrailingStop,
                minTrailingStop,
                marginRate,
                false,
                ask,
                bid);
            Instruments.Add(instrument, inst);
            return inst;
        }

        public FakeOandaAccount CreateAccount(int accountId, string accountName, string accountCurrency, decimal marginRate)
        {
            var account = new FakeOandaAccount(accountId, accountName, accountCurrency, marginRate);
            Accounts.Add(account.AccountId, account);
            return account;
        }

        public FakeOandaOrder CreateBuyOrder(int accountId, string instrument, int units, FakeOandaOrderType type, DateTime expiry, decimal price, decimal? lowerBound = null, decimal? upperBound = null, decimal? stopLoss = null, decimal? takeProfit = null, decimal? trailingStop = null)
        {
            return CreateOrder(accountId, instrument, units, type, FakeOandaSide.buy, expiry, price, lowerBound, upperBound, stopLoss, takeProfit, trailingStop);
        }

        public FakeOandaOrder CreateSellOrder(int accountId, string instrument, int units, FakeOandaOrderType type, DateTime expiry, decimal price, decimal? lowerBound = null, decimal? upperBound = null, decimal? stopLoss = null, decimal? takeProfit = null, decimal? trailingStop = null)
        {
            return CreateOrder(accountId, instrument, units, type, FakeOandaSide.sell, expiry, price, lowerBound, upperBound, stopLoss, takeProfit, trailingStop);
        }

        public FakeOandaOrder CreateOrder(
            int accountId,
            string instrument,
            int units,
            FakeOandaOrderType type,
            FakeOandaSide side,
            DateTime expiry,
            decimal price,
            decimal? lowerBound = null,
            decimal? upperBound = null,
            decimal? stopLoss = null,
            decimal? takeProfit = null,
            decimal? trailingStop = null)
        {
            var order = new FakeOandaOrder(
                _orderId++,
                instrument,
                units,
                side,
                type,
                CurrentTime,
                price,
                takeProfit,
                stopLoss,
                expiry,
                upperBound,
                lowerBound,
                trailingStop);
            Accounts[accountId].Orders.Add(order.Id, order);
            return order;
        }

        public FakeOandaTrade CreateTrade(
            int accountId,
            string instrument,
            int units,
            FakeOandaSide side,
            decimal price,
            decimal? stopLoss = null,
            decimal? takeProfit = null,
            decimal? trailingStop = null)
        {
            var trade = new FakeOandaTrade(
                _tradeId++,
                instrument,
                units,
                side,
                CurrentTime,
                price,
                takeProfit,
                stopLoss,
                trailingStop);
            Accounts[accountId].Trades.Add(trade.Id, trade);
            return trade;
        }
    }

    public static class FakeOandaContextExtension
    {
        public static TValue Find<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            return default(TValue);
        }

        public static FakeOandaContext.FakeOandaPrice CurrentPrice(this FakeOandaContext.FakeOandaInstrument instrument, DateTime time)
        {
            var diff = ((decimal)Math.Cos(2 * Math.PI * (time.Hour / 12.0))) * instrument.BaseAsk * 0.01m;
            var ask = instrument.BaseAsk + diff;
            var bid = instrument.BaseBid + diff;
            var exp = (int)Math.Abs(Math.Log10(instrument.Precision));
            return new FakeOandaContext.FakeOandaPrice(instrument.Instrument, time, Math.Round(ask, exp), Math.Round(bid, exp));
        }
    }
}
