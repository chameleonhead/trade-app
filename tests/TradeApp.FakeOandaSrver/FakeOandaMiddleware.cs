﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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

        public Task Invoke(HttpContext context)
        {
            var request = context.Request;
            if (request.Method == "GET")
            {
                if (request.Path.StartsWithSegments("/v1/accounts", out var remaining))
                {
                    var match = Regex.Match(remaining, @"^/?(\d+)");
                    if (match.Success)
                    {
                        var accountId = int.Parse(match.Groups[1].Value);
                        if (remaining.StartsWithSegments($"/{accountId}/orders"))
                        {
                            return InvokeGetOrders(context, accountId);
                        }
                        else
                        {
                            return InvokeGetAccount(context, accountId);
                        }
                    }
                    else
                    {
                        return InvokeGetAccounts(context);
                    }
                }
                else if (request.Path.StartsWithSegments("/v1/instruments"))
                {
                    return InvokeGetInstruments(context);
                }
                else if (request.Path.StartsWithSegments("/v1/prices"))
                {
                    return InvokeGetPrices(context);
                }
                else if (request.Path.StartsWithSegments("/v1/candles"))
                {
                    return InvokeGetCandles(context);
                }
            }

            if (request.Method == "POST")
            {
                if (request.Path.StartsWithSegments("/v1/accounts", out var remaining))
                {
                    var match = Regex.Match(remaining, @"^/?(\d+)");
                    if (match.Success)
                    {
                        var accountId = int.Parse(match.Groups[1].Value);
                        if (remaining.StartsWithSegments($"/{accountId}/orders"))
                        {
                            return InvokePostOrders(context, accountId);
                        }
                    }
                }
            }
            return _next.Invoke(context);
        }

        private async Task InvokeGetAccounts(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Accounts = _context.Accounts.Values.Select(obj => new Account() { AccountId = obj.AccountId, AccountName = obj.AccountName, AccountCurrency = obj.AccountCurrency, MarginRate = obj.MarginRate }).ToArray()
            }));
        }

        private async Task InvokeGetAccount(HttpContext context, int accountId)
        {
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

        private async Task InvokeGetOrders(HttpContext context, int accountId)
        {
            var maxId = ParseQuery<int?>(context.Request.Query["maxId"]);
            var count = ParseQuery<int?>(context.Request.Query["count"]) ?? 50;
            var instrument = ParseQuery<string>(context.Request.Query["instrument"]);
            var ids = ParseQuery<int[]>(context.Request.Query["ids"]) ?? new int[0];

            var orders = _context.Accounts[accountId].Orders.Values as IEnumerable<FakeOandaContext.FakeOandaOrder>;

            if (ids.Length > 0)
            {
                orders = orders.Where(o => ids.Contains(o.Id));
            }
            else
            {
                if (maxId != null)
                    orders = orders.Where(o => o.Id <= maxId.Value);
                if (!string.IsNullOrEmpty(instrument))
                    orders = orders.Where(o => o.Instrument == instrument);
            }

            orders = orders.OrderBy(o => o.Id).Take(count);

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                orders = orders
                    .Select(order => new
                    {
                        id = order.Id,
                        instrument = order.Instrument,
                        units = order.Units,
                        side = order.Side.ToString(),
                        type = order.Type.ToString(),
                        time = order.Time,
                        price = order.Price,
                        takeProfit = order.TakeProfit,
                        stopLoss = order.StopLoss,
                        expiry = order.Expiry,
                        upperBound = order.UpperBound,
                        lowerBound = order.LowerBound,
                        trailingStop = order.TrailingStop,
                    }).ToArray()
            }));
        }

        private async Task InvokePostOrders(HttpContext context, int accountId)
        {
            var instrument = ParseQuery<string>(context.Request.Form["instrument"]);
            var units = ParseQuery<int>(context.Request.Form["units"]);
            var side = ParseQuery<FakeOandaContext.FakeOandaSide>(context.Request.Form["side"]);
            var type = ParseQuery<FakeOandaContext.FakeOandaOrderType>(context.Request.Form["type"]);
            var expiry = ParseQuery<DateTime?>(context.Request.Form["expiry"]);
            var price = ParseQuery<decimal?>(context.Request.Form["price"]);
            var lowerBound = ParseQuery<decimal?>(context.Request.Form["lowerBound"]);
            var upperBound = ParseQuery<decimal?>(context.Request.Form["upperBound"]);
            var stopLoss = ParseQuery<decimal?>(context.Request.Form["stopLoss"]);
            var takeProfit = ParseQuery<decimal?>(context.Request.Form["takeProfit"]);
            var trailingStop = ParseQuery<decimal?>(context.Request.Form["trailingStop"]);

            if (type == FakeOandaContext.FakeOandaOrderType.market)
            {
                var p = _context.Prices[instrument];
                var trade = _context.CreateTrade(
                    accountId,
                    instrument,
                    units,
                    side,
                    side == FakeOandaContext.FakeOandaSide.buy ? p.Ask : p.Bid,
                    stopLoss,
                    takeProfit,
                    trailingStop);

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    instrument = trade.Instrument,
                    time = trade.Time,
                    price = trade.Price,
                    tradeOpened = new
                    {
                        id = trade.Id,
                        units = trade.Units,
                        side = trade.Side.ToString(),
                        takeProfit = trade.TakeProfit ?? 0,
                        stopLoss = trade.StopLoss ?? 0,
                        trailingStop = trade.TrailingStop ?? 0,
                    },
                    tradesClosed = new object[0],
                    tradeReduced = new { },
                }));
            }
            else
            {
                var order = _context.CreateOrder(
                    accountId,
                    instrument,
                    units,
                    type,
                    side,
                    expiry.Value,
                    price.Value,
                    lowerBound,
                    upperBound,
                    stopLoss,
                    takeProfit,
                    trailingStop);

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    instrument = order.Instrument,
                    time = order.Time,
                    price = order.Price,
                    orderOpened = new
                    {
                        id = order.Id,
                        units = order.Units,
                        side = order.Side.ToString(),
                        takeProfit = order.TakeProfit ?? 0,
                        stopLoss = order.StopLoss ?? 0,
                        expiry = order.Expiry,
                        upperBound = order.UpperBound ?? 0,
                        lowerBound = order.LowerBound ?? 0,
                        trailingStop = order.TrailingStop ?? 0,
                    },
                }));
            }
        }

        private async Task InvokeGetInstruments(HttpContext context)
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
                instruments = fakeInstruments
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

        private async Task InvokeGetPrices(HttpContext context)
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

        private async Task InvokeGetCandles(HttpContext context)
        {
            var instrument = (string)context.Request.Query["instrument"];
            var candleFormat = ((string)context.Request.Query["candleFormat"]) ?? "bidask";
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

            if (start != null && end != null)
            {
                count = 0;
                for (var d = start; d <= end; d += interval) count++;
            }

            if (start == null && end == null)
            {
                end = new DateTime((_context.CurrentTime.Ticks / TimeSpan.FromSeconds((int)granularity).Ticks) * TimeSpan.FromSeconds((int)granularity).Ticks);
                start = end.Value - (interval * (count - 1));
            }

            if (start != null && end == null)
            {
                start = new DateTime((start.Value.Ticks / TimeSpan.FromSeconds((int)granularity).Ticks) * TimeSpan.FromSeconds((int)granularity).Ticks);
                end = (includeFirst ? start : (start + interval)) + (interval * count);
                if (end > _context.CurrentTime)
                {
                    count = (int)((end - _context.CurrentTime) / interval);
                    end = (includeFirst ? start : (start + interval)) + (interval * count);
                }
            }

            if (start == null && end != null)
            {
                end = new DateTime((end.Value.Ticks / TimeSpan.FromSeconds((int)granularity).Ticks) * TimeSpan.FromSeconds((int)granularity).Ticks);
                start = end - (interval * (count - 1));
            }
            if (candleFormat == "bidask")
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    instrument = instrument,
                    granularity = granularity.ToString(),
                    candles = CreateCandles(inst, interval, count, start, end, includeFirst, (d, open, high, low, close, volume) => new BidAskCandle()
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
                        Volume = volume,
                    })
                }));
            }
            else
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    instrument = instrument,
                    granularity = granularity.ToString(),
                    candles = CreateCandles(inst, interval, count, start, end, includeFirst, (d, open, high, low, close, volume) => new MidpointCandle()
                    {
                        Time = d,
                        OpenMid = (open.Ask + open.Bid) / 2,
                        HighMid = (high.Ask + high.Bid) / 2,
                        LowMid = (low.Ask + low.Bid) / 2,
                        CloseMid = (close.Ask + close.Bid) / 2,
                        Volume = volume,
                    })
                }));
            }

        }

        private static T[] CreateCandles<T>(FakeOandaContext.FakeOandaInstrument inst, TimeSpan interval, int count, DateTime? start, DateTime? end, bool includeFirst, Func<DateTime, FakeOandaContext.FakeOandaPrice, FakeOandaContext.FakeOandaPrice, FakeOandaContext.FakeOandaPrice, FakeOandaContext.FakeOandaPrice, int, T> factory)
        {
            var i = 0;
            var result = new T[count];
            for (var d = includeFirst ? start.Value : start.Value + interval; d <= end.Value; d += interval)
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

                result[i++] = factory(d, open, high, low, close, (d.Hour + 120) * 200);
            }

            return result;
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
                if (type.IsEnum)
                {
                    return (T)(object)Enum.Parse(type, s);
                }
                if (type == typeof(int?) || type == typeof(int))
                {
                    return (T)(object)int.Parse(s);
                }
                if (type == typeof(decimal?) || type == typeof(decimal))
                {
                    return (T)(object)decimal.Parse(s);
                }
                if (type == typeof(bool?) || type == typeof(bool))
                {
                    return bool.TryParse(s, out var b) ? (T)(object)b : default(T);
                }
                if (type == typeof(DateTime?) || type == typeof(DateTime))
                {
                    return (T)(object)XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.Utc);
                }
                if (type == typeof(int[]))
                {
                    return (T)(object)s.Split(",").Select(str => int.Parse(str)).ToArray();
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
