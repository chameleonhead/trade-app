using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradeApp.Charting;
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
            var from = new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2017, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var range = ChartRange.Hourly;

            var sut = new OandaCandleProvider(_server.BaseUri.ToString(), _server.DefaultAccessToken);
            var candles = sut.GetCandles(symbol, from, to, range);
            Assert.AreEqual(10, candles.Length);
        }
    }
}
