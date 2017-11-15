using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace TradeApp.FakeServer
{
    public class OandaFakeServerMiddleware
    {
        private readonly RequestDelegate _next;
        private string _accessToken;

        public OandaFakeServerMiddleware(RequestDelegate next, string accessToken)
        {
            _next = next;
            _accessToken = accessToken;
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
            if (context.Request.Headers["Authorization"] != "Bearer " + _accessToken)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
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
        public static IApplicationBuilder UseOandaFakeServer(this IApplicationBuilder builder, string accessToken)
        {
            return builder.UseMiddleware<OandaFakeServerMiddleware>(accessToken);
        }
    }
}