using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace TradeApp.Oanda
{
    public class FakeOandaData
    {
        private static string ACCESS_TOKEN_GUID = Guid.NewGuid().ToString();

        public string AccessToken { get { return ACCESS_TOKEN_GUID; } }
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

        static FakeOandaData()
        {
            CreateAccount(8954947, "Primary", "USD", 0.05m);
            CreateAccount(8954950, "SweetHome", "CAD", 0.02m);
        }

        public static Dictionary<int, FakeOandaData> dictionary = new Dictionary<int, FakeOandaData>();

        public static void Initialize()
        {
            dictionary.Clear();
        }

        public static FakeOandaData CreateAccount(int accountId, string accountName, string accountCurrency, decimal marginRate)
        {
            var data = new FakeOandaData()
            {
                AccountId = accountId,
                AccountName = accountName,
                AccountCurrency = accountCurrency,
                MarginRate = marginRate,
                Balance = 3000,
            };
            dictionary.Add(data.AccountId, data);
            return data;
        }
    }

    public class FakeOandaServer : TestServer
    {
        public FakeOandaServer()
            : base(new WebHostBuilder()
                  .UseStartup<FakeOandaStartup>())
        {
        }
    }

    public class FakeOandaStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<FakeOandaMiddleware>();
        }
    }

    public class FakeOandaMiddleware
    {
        private readonly RequestDelegate _next;

        public FakeOandaMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
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
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task InvokeAccounts(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new Accounts
            {
                accounts = FakeOandaData.dictionary.Values.Select(obj => new Account() { accountId = obj.AccountId, accountName = obj.AccountName, accountCurrency = obj.AccountCurrency, marginRate = obj.MarginRate }).ToList()
            }));
        }

        private async Task InvokeAccount(HttpContext context)
        {
            if (!int.TryParse(context.Request.Path.Value.Remove(0, "/v1/account/".Length), out var accountId) || !FakeOandaData.dictionary.TryGetValue(accountId, out var data))
            {
                await _next.Invoke(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new AccountDetail()
            {
                accountId = data.AccountId,
                accountName = data.AccountName,
                balance = data.Balance,
                unrealizedPl = data.UnrealizedProfitLoss,
                realizedPl = data.RealizedProfitLoss,
                marginUsed = data.MarginUsed,
                marginAvail = data.MarginAvail,
                openTrades = data.OpenTrades,
                openOrders = data.OpenOrders,
                marginRate = data.MarginRate,
                accountCurrency = data.AccountCurrency,
            }));
        }
    }
}