﻿using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_Prices
    {
        private FakeOandaServer _server;
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            _server = new FakeOandaServer();
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

            Assert.AreEqual(1, prices.Count);

            var actualPrice = prices.First();
            Assert.AreEqual("USD_JPY", actualPrice.Instrument);
            Assert.AreEqual(price.Time, actualPrice.Time);
            Assert.AreEqual(price.Ask, actualPrice.Ask);
            Assert.AreEqual(price.Bid, actualPrice.Bid);
        }
    }
}
