using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using TradeApp.FakeServer;

namespace TradeApp.Drivers
{
    class OandaFakeServerDriver : IDisposable
    {
        private string accessToken;
        private OandaFakeServer server;
        private CancellationTokenSource cts;

        public Uri BaseUri { get; internal set; }

        public OandaFakeServerDriver(string accessToken)
        {
            this.accessToken = accessToken;
            BaseUri = new Uri("http://localhost:5000/");
        }

        public void Start()
        {
            cts = new CancellationTokenSource();
            server = new OandaFakeServer(accessToken);
            WebHost.CreateDefaultBuilder()
                .Configure(server.Configuration)
                .Build()
                .RunAsync(cts.Token);
        }

        public void Stop()
        {
            cts.Cancel();
        }

        public void HasReceivedAccountRequest()
        {
            // TODOもっときれいな実装に書き換え
            var time = DateTime.Now;
            while(DateTime.Now - time < TimeSpan.FromSeconds(5))
            {
                if (server.HasAccessedAccount)
                {
                    return;
                }
            }
            Assert.Fail("時間内に口座残高照会のリクエストが来ていない");
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cts.Cancel();
                }

                disposedValue = true;
            }
        }

        ~OandaFakeServerDriver()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}