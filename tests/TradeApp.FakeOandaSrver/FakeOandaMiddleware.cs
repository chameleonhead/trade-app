using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using TradeApp.Oanda;

namespace TradeApp.FakeOandaSrver
{
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
            else if (request.Path.StartsWithSegments("/v1/instruments"))
            {
                await InvokeInstruments(context);
            }
            else if (request.Path.StartsWithSegments("/v1/prices"))
            {
                await InvokePrices(context);
            }
            else if (request.Path.StartsWithSegments("/v1/candles"))
            {
                await InvokeCandles(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task InvokeAccounts(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Accounts = _context.Accounts.Values.Select(obj => new Account() { AccountId = obj.AccountId, AccountName = obj.AccountName, AccountCurrency = obj.AccountCurrency, MarginRate = obj.MarginRate }).ToArray()
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

        private async Task InvokeInstruments(HttpContext context)
        {
            var fields = context.Request.Query.TryGetValue("fields", out var fieldsList) ? fieldsList[0].Split(",") : Array.Empty<String>();
            var instruments = context.Request.Query.TryGetValue("instruments", out var instrumentsList) ? instrumentsList[0].Split(",") : Array.Empty<String>();
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            var fakeInstruments = _context.Instruments.Values.AsEnumerable();
            if (instruments.Length > 0)
            {
                fakeInstruments = fakeInstruments.Where(inst => instruments.Contains(inst.Instrument));
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Instruments = fakeInstruments
                    .Select(inst =>
                    {
                        var dict = new Dictionary<string, object>();
                        dict.Add("instrument", inst.Instrument);
                        dict.Add("displayName", inst.DisplayName);
                        dict.Add("pip", inst.Pip);
                        dict.Add("maxTradeUnits", inst.MaxTradeUnits);
                        if (fields.Contains("precision"))
                            dict.Add("precision", inst.Precision);
                        if (fields.Contains("maxTrailingStop"))
                            dict.Add("maxTrailingStop", inst.MaxTrailingStop);
                        if (fields.Contains("minTrailingStop"))
                            dict.Add("minTrailingStop", inst.MinTrailingStop);
                        if (fields.Contains("marginRate"))
                            dict.Add("marginRate", inst.MarginRate);
                        if (fields.Contains("halted"))
                            dict.Add("halted", inst.Halted);
                        return dict;
                    }).ToList()
            }));
        }

        private async Task InvokePrices(HttpContext context)
        {
            var instruments = context.Request.Query["instruments"][0].Split(",");
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Prices = _context.Prices.Values
                    .Where(p => instruments.Contains(p.Instrument))
                    .Select(price => new Price()
                    {
                        Instrument = price.Instrument,
                        Time = price.Time,
                        Ask = price.Ask,
                        Bid = price.Bid
                    }).ToArray()
            }));
        }

        private async Task InvokeCandles(HttpContext context)
        {
            var instrument = (string)context.Request.Query["instrument"];
            var inst = _context.Instruments.Find(instrument);
            if (inst == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            if (!Enum.TryParse<Granularity>(context.Request.Query["granularity"], out var granularity))
            {
                granularity = Granularity.S5;
            }
            var interval = TimeSpan.FromSeconds((int)granularity);

            int count = ParseQuery<int?>(context.Request.Query["count"]) ?? 500;
            if (count > 5000)
            {
                count = 5000;
            }

            bool includeFirst = ParseQuery<bool?>(context.Request.Query["includeFirst"]) ?? true;
            int dailyAlignment = ParseQuery<int?>(context.Request.Query["dailyAlignment"]) ?? 0;
            string alignmentTimezone = ParseQuery<string>(context.Request.Query["alignmentTimezone"]) ?? "America/New_York";
            string weeklyAlignment = ParseQuery<string>(context.Request.Query["weeklyAlignment"]) ?? "Friday";

            var start = ParseQuery<DateTime?>(context.Request.Query["start"]);
            var end = ParseQuery<DateTime?>(context.Request.Query["end"]);

            if (start == null && end == null)
            {
                end = _context.CurrentTime;
            }

            if (start != null && end == null)
            {
                start = new DateTime((start.Value.Ticks / TimeSpan.FromSeconds(5).Ticks) * TimeSpan.FromSeconds(5).Ticks);
                end = (includeFirst ? start : (start + interval)) + (interval * count);
                if (end > _context.CurrentTime)
                {
                    count = (int)((end - _context.CurrentTime) / interval);
                    end = (includeFirst ? start : (start + interval)) + (interval * count);
                }
            }

            if (start == null && end != null)
            {
                end = new DateTime((end.Value.Ticks / TimeSpan.FromSeconds(5).Ticks) * TimeSpan.FromSeconds(5).Ticks);
                start = end - (interval * count);
            }
            var i = 0;
            var result = new BidAskCandle[count];
            for (var d = start.Value; d < end.Value; d += interval)
            {
                var open = inst.CurrentPrice(d);
                var close = inst.CurrentPrice(d + interval);

                FakeOandaContext.FakeOandaPrice high;
                FakeOandaContext.FakeOandaPrice low;
                if (open.Ask > close.Ask)
                {
                    high = open;
                    low = close;
                }
                else
                {
                    high = close;
                    low = open;
                }

                result[i++] = new BidAskCandle()
                {
                    Time = d,
                    OpenAsk = open.Ask,
                    OpenBid = open.Bid,
                    HighAsk = high.Ask,
                    HighBid = high.Bid,
                    LowAsk = low.Ask,
                    LowBid = low.Bid,
                    CloseAsk = close.Ask,
                    CloseBid = close.Bid,
                    Volume = (d.Hour + 120) * 200,
                };
            }
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                instrument = instrument,
                granularity = granularity.ToString(),
                candles = result
            }));
        }

        private T ParseQuery<T>(string s)
        {
            try
            {
                if (string.IsNullOrEmpty(s))
                    return default(T);

                var type = typeof(T);
                if (type == typeof(string))
                {
                    return (T)(object)s;
                }
                if (type == typeof(int?))
                {
                    return int.TryParse(s, out var i) ? (T)(object)i : default(T);
                }
                if (type == typeof(bool?))
                {
                    return bool.TryParse(s, out var b) ? (T)(object)b : default(T);
                }
                if (type == typeof(DateTime?))
                {
                    return (T)(object)XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.Utc);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine($"Couldnt parse as {typeof(T)} value: {s}");
            }
            return default(T);
        }
    }
}
