using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_Instruments
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
        public void 銘柄を取得する()
        {
            var instrument = _server.Context.Instruments.Find("USD_JPY");

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);

            var prices = oandaApi.GetInstruments("USD_JPY").Result;

            Assert.AreEqual(1, prices.Count);

            var actualPrice = prices.First();
            Assert.AreEqual("USD_JPY", actualPrice.Instrument);
            Assert.AreEqual(instrument.DisplayName, actualPrice.DisplayName);
            Assert.AreEqual(instrument.Pip, actualPrice.Pip);
            Assert.AreEqual(instrument.MaxTradeUnits, actualPrice.MaxTradeUnits);
            Assert.AreEqual(instrument.Precision, actualPrice.Precision);
            Assert.AreEqual(instrument.MaxTrailingStop, actualPrice.MaxTrailingStop);
            Assert.AreEqual(instrument.MinTrailingStop, actualPrice.MinTrailingStop);
            Assert.AreEqual(instrument.MarginRate, actualPrice.MarginRate);
            Assert.AreEqual(instrument.Halted, actualPrice.Halted);
        }

        [TestMethod]
        public void 複数の銘柄を取得する()
        {
            var expectedInstruments = _server.Context.Instruments.Where(p => p.Key == "USD_JPY" || p.Key == "EUR_USD").ToList();
            Assert.IsTrue(expectedInstruments.Count == 2);

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);

            var actualInstruments = oandaApi.GetInstruments("USD_JPY", "EUR_USD").Result;

            Assert.AreEqual(expectedInstruments.Count, actualInstruments.Count);
            CollectionAssert.AreEqual(expectedInstruments.Select(p => p.Key).ToArray(),
                actualInstruments.Select(p => p.Instrument).ToArray());
        }

        [TestMethod]
        public void 全銘柄を取得する()
        {
            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var actualInstruments = oandaApi.GetInstruments().Result;

            Assert.AreEqual(_server.Context.Instruments.Count, actualInstruments.Count);
        }
    }
}
