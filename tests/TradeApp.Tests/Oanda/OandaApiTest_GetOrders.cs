using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_GetOrders
    {
        private FakeOandaTestServer _server;
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            _server = new FakeOandaTestServer();
            _client = _server.CreateClient();
            var context = _server.Context;
            context.CreateMarketOrder(_server.DefaultAccountId, "USD_JPY", 1, FakeOandaContext.FakeOandaSide.buy);
            context.CreateMarketOrder(_server.DefaultAccountId, "EUR_USD", 2, FakeOandaContext.FakeOandaSide.buy);
            context.CreateMarketOrder(_server.DefaultAccountId, "USD_JPY", 3, FakeOandaContext.FakeOandaSide.buy);
            context.CreateMarketOrder(_server.DefaultAccountId, "EUR_USD", 4, FakeOandaContext.FakeOandaSide.buy);
        }

        [TestCleanup]
        public void Teardown()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void 特定IDを指定して注文を取得する()
        {
            var expected = _server.Context.DefaultAccount.Orders.Skip(1).First();
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var actual = oandaApi.GetOrder(expected.Id).Result;

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Instrument, actual.Instrument);
            Assert.AreEqual(expected.Units, actual.Units);
            Assert.AreEqual(expected.Side.ToString(), actual.Side);
            Assert.AreEqual(expected.Type.ToString(), actual.Type);
            Assert.AreEqual(expected.Time, actual.Time);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.TakeProfit, actual.TakeProfit);
            Assert.AreEqual(expected.StopLoss, actual.StopLoss);
            Assert.AreEqual(expected.Expiry, actual.Expiry);
            Assert.AreEqual(expected.UpperBound, actual.UpperBound);
            Assert.AreEqual(expected.LowerBound, actual.LowerBound);
            Assert.AreEqual(expected.TrailingStop, actual.TrailingStop);
        }

        [TestMethod]
        public void 複数のIDを指定して注文を取得する()
        {
            var expectedOrders = _server.Context.DefaultAccount.Orders.Take(2);
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var actualOrders = oandaApi.GetOrders(expectedOrders.Select(o => o.Id).ToArray()).Result;

            CollectionAssert.AreEqual(expectedOrders.Select(o => o.Id).ToArray(),
                actualOrders.Select(o => o.Id).ToArray());
        }

        [TestMethod]
        public void 最大のIDを指定して注文を取得する()
        {
            var maxId = _server.Context.DefaultAccount.Orders.OrderBy(o => o.Id).Skip(2).First().Id;
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var actualOrders = oandaApi.GetOrders(maxId: maxId).Result;

            Assert.AreEqual(3, actualOrders.Length);
        }

        [TestMethod]
        public void 最大の取得件数を指定して注文を取得する()
        {
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var actualOrders = oandaApi.GetOrders(count: 2).Result;

            Assert.AreEqual(2, actualOrders.Length);
        }

        [TestMethod]
        public void 通貨ペアを指定して注文を取得する()
        {
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var actualOrders = oandaApi.GetOrders(instrument: "USD_JPY").Result;

            Assert.AreEqual(2, actualOrders.Length);
        }
    }
}
