using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_BidAskCandles
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
        public void 銘柄の過去データの取得()
        {
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);

            var candles = oandaApi.GetBidAskCandles("USD_JPY").Result;

            Assert.AreEqual(1, candles.Length);
        }
    }
}
