using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;

namespace TradeApp.FakeServer
{
    public class OandaFakeServer
    {
        private string accessToken;

        public OandaFakeServer(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public bool HasAccessedAccount { get; internal set; }

        public void Configuration(IApplicationBuilder app)
        {
            app.Run(Router);
        }

        private async Task Router(HttpContext context)
        {
            await Task.Yield();

            if (context.Request.Path.Value.StartsWith("/accounts/"))
            {
                StringValues value;
                if (!context.Request.Headers.TryGetValue("Authorization", out value) && value.Any(v => v == accessToken))
                {
                    HasAccessedAccount = true;
                }
            }
        }
    }
}