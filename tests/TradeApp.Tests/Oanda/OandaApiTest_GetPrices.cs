﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_GetPrices
    {
        private FakeOandaTestServer _server;
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            _server = new FakeOandaTestServer();
            _client = _server.CreateClient();
        }

        [TestCleanup]
        public void Teardown()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void 現在のレートを取得する()
        {
            var price = _server.Context.Prices.Find("USD_JPY");

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);

            var prices = oandaApi.GetPrices("USD_JPY").Result;

            Assert.AreEqual(1, prices.Length);

            var actualPrice = prices.First();
            Assert.AreEqual("USD_JPY", actualPrice.Instrument);
            Assert.AreEqual(price.Time, actualPrice.Time);
            Assert.AreEqual(price.Ask, actualPrice.Ask);
            Assert.AreEqual(price.Bid, actualPrice.Bid);
        }

        [TestMethod]
        public void 複数のレートを取得する()
        {
            var expectedPrices = _server.Context.Prices.Where(p => p.Key == "USD_JPY" || p.Key == "EUR_USD").ToArray();
            Assert.IsTrue(expectedPrices.Length == 2);

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);

            var actualPrices = oandaApi.GetPrices("USD_JPY", "EUR_USD").Result;

            Assert.AreEqual(expectedPrices.Length, actualPrices.Length);
            CollectionAssert.AreEqual(expectedPrices.Select(p => p.Key).ToArray(),
                actualPrices.Select(p => p.Instrument).ToArray());
        }
    }
}
