using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_PostMarketOrder
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
        public void 必須項目を指定して成行注文を出す()
        {
            var now = _server.Context.CurrentTime;

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var instrument = "USD_JPY";
            var units = 100;
            var side = OrderSide.Buy;
            var orderResponse = oandaApi.PostMarketOrder(instrument, units, side).Result;

            Assert.AreEqual(instrument, orderResponse.Instrument);
            Assert.AreEqual(now, orderResponse.Time);
            Assert.AreEqual(units, orderResponse.TradeOpened.Units);
            Assert.AreEqual(side, orderResponse.TradeOpened.Side);
            Assert.AreEqual(0, orderResponse.TradeOpened.TakeProfit);
            Assert.AreEqual(0, orderResponse.TradeOpened.StopLoss);
            Assert.AreEqual(0, orderResponse.TradeOpened.TrailingStop);

            var context = _server.Context;
            var actual = context.Accounts[_server.DefaultAccountId].Trades[orderResponse.TradeOpened.Id];
            Assert.AreEqual(actual.Instrument, orderResponse.Instrument);
            Assert.AreEqual(actual.Units, orderResponse.TradeOpened.Units);
            Assert.AreEqual(actual.Side.ToString().ToUpper(), orderResponse.TradeOpened.Side.ToString().ToUpper());
            Assert.AreEqual(actual.Time, orderResponse.Time);
            Assert.AreEqual(actual.Price, orderResponse.Price);
            Assert.AreEqual(actual.TakeProfit ?? 0, orderResponse.TradeOpened.TakeProfit);
            Assert.AreEqual(actual.StopLoss ?? 0, orderResponse.TradeOpened.StopLoss);
            Assert.AreEqual(actual.TrailingStop ?? 0, orderResponse.TradeOpened.TrailingStop);
        }
    }
}
