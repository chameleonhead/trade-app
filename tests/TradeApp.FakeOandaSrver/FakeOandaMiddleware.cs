using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
                    }).ToArray()
            }));
        }
    }

}
