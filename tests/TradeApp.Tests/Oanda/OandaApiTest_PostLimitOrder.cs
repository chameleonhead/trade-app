using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_PostLimitOrder
    {
        private FakeOandaTestServer _server;
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            _server = new FakeOandaTestServer();
            _client = _server.CreateClient();
            var context = _server.Context;
        }

        [TestCleanup]
        public void Teardown()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void 必須項目を指定して指値注文を出す()
        {
            var now = _server.Context.CurrentTime;

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var instrument = "USD_JPY";
            var units = 100;
            var side = OrderSide.Buy;
            var type = OrderType.Limit;
            var expiry = now.AddDays(10);
            var price = 120.05m;
            var orderResponse = oandaApi.PostOrder(instrument, units, side, type, expiry, price).Result;

            Assert.AreEqual(instrument, orderResponse.Instrument);
            Assert.AreEqual(now, orderResponse.Time);
            Assert.AreEqual(price, orderResponse.Price);
            Assert.AreEqual(units, orderResponse.OrderOpened.Units);
            Assert.AreEqual(side, orderResponse.OrderOpened.Side);
            Assert.AreEqual(null, orderResponse.OrderOpened.TakeProfit);
            Assert.AreEqual(null, orderResponse.OrderOpened.StopLoss);
            Assert.AreEqual(expiry, orderResponse.OrderOpened.Expiry);
            Assert.AreEqual(null, orderResponse.OrderOpened.UpperBound);
            Assert.AreEqual(null, orderResponse.OrderOpened.LowerBound);
            Assert.AreEqual(null, orderResponse.OrderOpened.TrailingStop);

            var context = _server.Context;
            var actual = context.Accounts[_server.DefaultAccountId].Orders.Find(orderResponse.OrderOpened.Id);
            Assert.AreEqual(actual.Instrument, orderResponse.Instrument);
            Assert.AreEqual(actual.Units, orderResponse.OrderOpened.Units);
            Assert.AreEqual(actual.Side, orderResponse.OrderOpened.Side);
            Assert.AreEqual(actual.Type, OrderType.Limit);
            Assert.AreEqual(actual.Time, orderResponse.Time);
            Assert.AreEqual(actual.Price, orderResponse.Price);
            Assert.AreEqual(actual.TakeProfit, orderResponse.OrderOpened.TakeProfit);
            Assert.AreEqual(actual.StopLoss, orderResponse.OrderOpened.StopLoss);
            Assert.AreEqual(actual.Expiry, orderResponse.OrderOpened.Expiry);
            Assert.AreEqual(actual.UpperBound, orderResponse.OrderOpened.UpperBound);
            Assert.AreEqual(actual.LowerBound, orderResponse.OrderOpened.LowerBound);
            Assert.AreEqual(actual.TrailingStop, orderResponse.OrderOpened.TrailingStop);
        }
    }
}
