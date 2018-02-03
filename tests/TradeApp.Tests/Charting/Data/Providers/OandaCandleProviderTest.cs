using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradeApp.FakeOandaSrver;
using TradeApp.Oanda;

namespace TradeApp.Charting.Data.Providers
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

            var oandaApi = new OandaApi(_server.BaseUri, _server.DefaultAccessToken);
            var sut = new OandaCandleProvider(oandaApi);
            var candles = sut.GetCandles(symbol, range, from, to);
            Assert.AreEqual(10, candles.Length);
        }
    }
}
