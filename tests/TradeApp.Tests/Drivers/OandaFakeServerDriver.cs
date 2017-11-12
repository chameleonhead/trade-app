using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using TradeApp.FakeServer;

namespace TradeApp.Drivers
{
    class OandaFakeServerDriver
    {
        private HttpRequestListener httpRequestListener;
        private OandaFakeServer server;

        public Uri BaseUri => server.BaseUri;

        public OandaFakeServerDriver(string accessToken)
        {
            httpRequestListener = new HttpRequestListener();
            server = new OandaFakeServer(accessToken, httpRequestListener);
        }

        public void Start()
        {
            server.Start();
        }

        public void Stop()
        {
            server.StopAsync();
        }

        public void HasReceivedAccountRequest()
        {
            httpRequestListener.Process((request, response) =>
            {
                if (request.Path.StartsWithSegments(PathString.FromUriComponent("/accounts/")))
                {
                    var responseBytes = Encoding.UTF8.GetBytes("");
                    var stream = new MemoryStream();
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    response.Body = stream;
                }
            }, TimeSpan.FromSeconds(5));
            Assert.Fail("時間内に口座残高照会のリクエストが来ていない");
        }
    }
}