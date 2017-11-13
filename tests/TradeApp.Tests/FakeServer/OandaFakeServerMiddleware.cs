using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace TradeApp.FakeServer
{
    public class OandaFakeServerMiddleware
    {
        private readonly RequestDelegate _next;

        public OandaFakeServerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/v1/accounts"))
            {
                return AccountContextHandler(context);
            }
            return this._next(context);
        }

        private async Task AccountContextHandler(HttpContext context)
        {
            var response = new
            {
                accounts = new[]
                {
                    new {
                        accountId = 8954947,
                        accountName = "Primary",
                        accountCurrency = "USD",
                        marginRate = 0.05,
                    },
                    new {
                        accountId = 8954950,
                        accountName = "SweetHome",
                        accountCurrency = "CAD",
                        marginRate = 0.02,
                    },
                }
            };
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }

    public static class OandaFakeServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseOandaFakeServer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OandaFakeServerMiddleware>();
        }
    }
}