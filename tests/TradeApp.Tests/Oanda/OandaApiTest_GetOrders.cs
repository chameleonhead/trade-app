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
        }

        [TestCleanup]
        public void Teardown()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void 特定の口座における注文を取得する()
        {
            var instrument = "USD_JPY";
            var units = 500;
            var side = FakeOandaContext.FakeOandaSide.buy;
            var expected = _server.Context.CreateMarketOrder(_server.DefaultAccountId, instrument, units, side);
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var actual = oandaApi.GetOrders().Result.Single();

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
    }
}
