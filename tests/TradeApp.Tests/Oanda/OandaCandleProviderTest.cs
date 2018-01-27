using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaCandleProviderTest
    {
        private FakeOandaWebHost _server;

        [TestInitialize]
        public void Setup()
        {
            _server = new FakeOandaWebHost();
            _server.Start();
        }

        [TestCleanup]
        public void Teardown()
        {
            _server.StopAsync().Wait();
            _server.Dispose();
        }

        [TestMethod]
        public void OANDAサーバーよりキャンドルが取得できる()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var from = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var to = new DateTime(2017, 1, 1, 1, 1, 50, DateTimeKind.Utc);
            var span = TimeSpan.FromSeconds(5);

            var sut = new OandaCandleProvider(_server.BaseUri.ToString(), _server.DefaultAccessToken);
            var candles = sut.GetCandles(symbol, from, to, span);
            Assert.AreEqual(10, candles.Length);
        }
    }
}
