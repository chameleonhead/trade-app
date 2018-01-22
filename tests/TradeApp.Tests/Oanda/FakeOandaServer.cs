using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TradeApp.Oanda
{
    public class FakeOandaContext
    {
        private static DateTime NOW = DateTime.Now;

        public class Account
        {
            public Account(int accountId, string accountName, string accountCurrency, decimal marginRate)
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
        }

        public class Instrument
        {
            public Instrument(
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
                Key = instrument;
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
            public string Key { get; private set; }
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

        public class Price
        {
            public Price(string instrument, DateTime time, decimal ask, decimal bid)
            {
                Instrument = instrument;
                Time = time;
                Ask = ask;
                Bid = bid;
            }

            public string Instrument { get; set; }
            public DateTime Time { get; private set; }
            public decimal Ask { get; private set; }
            public decimal Bid { get; private set; }
        }

        public FakeOandaContext()
        {
            Initialize();
        }

        public Account DefaultAccount { get; private set; }

        public Dictionary<int, Account> Accounts = new Dictionary<int, Account>();
        public Dictionary<string, Instrument> Instruments = new Dictionary<string, Instrument>();
        public Dictionary<string, Price> Prices = new Dictionary<string, Price>();

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
                Prices.Add(i.Key, i.CurrentPrice(NOW));
            });
        }

        private Instrument CreateInstrument(
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
            var inst = new Instrument(
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

        public Account CreateAccount(int accountId, string accountName, string accountCurrency, decimal marginRate)
        {
            var account = new Account(accountId, accountName, accountCurrency, marginRate);
            Accounts.Add(account.AccountId, account);
            return account;
        }
    }

    static class FakeOandaContextExtension
    {
        public static TValue Find<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            return default(TValue);
        }

        public static FakeOandaContext.Price CurrentPrice(this FakeOandaContext.Instrument instrument, DateTime time)
        {
            var diff = ((decimal)Math.Cos(2 * Math.PI * (time.Hour / 12.0))) * instrument.BaseAsk * 0.01m;
            var ask = instrument.BaseAsk + diff;
            var bid = instrument.BaseBid + diff;
            var exp = (int)Math.Abs(Math.Log10(instrument.Precision));
            return new FakeOandaContext.Price(instrument.Key, time, Math.Round(ask, exp), Math.Round(bid, exp));
        }
    }

    public class FakeOandaServer : TestServer
    {
        private static string ACCESS_TOKEN_GUID = Guid.NewGuid().ToString();

        private readonly FakeOandaContext context;

        public FakeOandaServer() : this(new FakeOandaContext())
        {
        }

        public FakeOandaServer(FakeOandaContext context)
            : base(new WebHostBuilder()
                  .ConfigureServices(services => services.Add(new ServiceDescriptor(typeof(FakeOandaContext), context)))
                  .UseStartup<FakeOandaStartup>())
        {
            this.context = context;
        }

        public FakeOandaContext Context { get { return context; } }
        public int DefaultAccountId { get { return context.DefaultAccount.AccountId; } }
        public string DefaultAccessToken { get { return ACCESS_TOKEN_GUID; } }
    }

    public class FakeOandaStartup
    {
        private readonly FakeOandaContext context;

        public FakeOandaStartup(FakeOandaContext context)
        {
            this.context = context;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<FakeOandaMiddleware>(context);
        }
    }

    public class FakeOandaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FakeOandaContext _context;

        public FakeOandaMiddleware(RequestDelegate next, FakeOandaContext context)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _context = context;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            if (request.Path.StartsWithSegments("/v1/accounts"))
            {
                await InvokeAccounts(context);
            }
            else if (request.Path.StartsWithSegments("/v1/account"))
            {
                await InvokeAccount(context);
            }
            else if (request.Path.StartsWithSegments("/v1/prices"))
            {
                await InvokePrices(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task InvokeAccounts(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new AccountsResponse
            {
                Accounts = _context.Accounts.Values.Select(obj => new Account() { AccountId = obj.AccountId, AccountName = obj.AccountName, AccountCurrency = obj.AccountCurrency, MarginRate = obj.MarginRate }).ToList()
            }));
        }

        private async Task InvokeAccount(HttpContext context)
        {
            if (!int.TryParse(context.Request.Path.Value.Remove(0, "/v1/account/".Length), out var accountId))
            {
                await _next.Invoke(context);
                return;
            }

            var account = _context.Accounts.Find(accountId);

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new AccountDetail()
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                Balance = account.Balance,
                UnrealizedPl = account.UnrealizedProfitLoss,
                RealizedPl = account.RealizedProfitLoss,
                MarginUsed = account.MarginUsed,
                MarginAvail = account.MarginAvail,
                OpenTrades = account.OpenTrades,
                OpenOrders = account.OpenOrders,
                MarginRate = account.MarginRate,
                AccountCurrency = account.AccountCurrency,
            }));
        }

        private async Task InvokePrices(HttpContext context)
        {
            var instruments = context.Request.Query["instruments"][0].Split(",");
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new PricesResponse()
            {
                Prices = _context.Prices.Values
                    .Where(p => instruments.Contains(p.Instrument))
                    .Select(price => new Price()
                    {
                        Instrument = price.Instrument,
                        Time = price.Time,
                        Ask = price.Ask,
                        Bid = price.Bid
                    }).ToList()
            }));
        }
    }
}