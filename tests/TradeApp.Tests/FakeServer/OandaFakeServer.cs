using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

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

        public void Stop()
        {
            webHost.StopAsync().ContinueWith(t => webHost.Dispose());
        }


        public void Configuration(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                httpRequestListener.Dispatch(context);
                await next();
            });
            app.UseOandaFakeServer();
        }
    }
}