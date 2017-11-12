using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace TradeApp.FakeServer
{
    class OandaFakeServer
    {
        private IWebHost webHost;
        private HttpRequestListener httpRequestListener;
        private string accessToken;

        public OandaFakeServer(string accessToken, HttpRequestListener httpRequestListener)
        {
            this.accessToken = accessToken;
            this.webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(URL_TO_LISTEN)
                .Configure(Configuration)
                .Build();
            this.httpRequestListener = httpRequestListener;
        }

        public readonly string URL_TO_LISTEN = "http://localhost:5000";
        public Uri BaseUri => new Uri(URL_TO_LISTEN);


        public void Start()
        {
            webHost.Start();
        }

        public Task StopAsync()
        {
            return webHost.StopAsync();
        }


        public void Configuration(IApplicationBuilder app)
        {
            app.Run(Router);
        }

        private async Task Router(HttpContext context)
        {
            await httpRequestListener.Dispatch(context);
        }
    }
}