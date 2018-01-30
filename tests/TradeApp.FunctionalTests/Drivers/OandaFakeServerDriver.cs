using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradeApp.FakeServer;

namespace TradeApp.Drivers
{
    class OandaFakeServerDriver
    {
        private HttpRequestListener httpRequestListener;
        private OandaFakeServer server;
        private int accountId;

        public Uri BaseUri => server.BaseUri;

        public OandaFakeServerDriver(string accessToken, int accountId)
        {
            httpRequestListener = new HttpRequestListener();
            server = new OandaFakeServer(accessToken, httpRequestListener);
            this.accountId = accountId;
        }

        public void Start()
        {
            server.Start();
        }

        public void Stop()
        {
            server.Stop();
        }

        public void HasReceivedAccountRequest()
        {
            if (!httpRequestListener.Process(context =>
            {
                Assert.IsTrue(context.Request.Path.StartsWithSegments(PathString.FromUriComponent($"/v1/accounts/{accountId}")));
            }))
            {
                Assert.Fail("時間内に口座残高照会のリクエストが来ていない");
            }
        }
    }
}