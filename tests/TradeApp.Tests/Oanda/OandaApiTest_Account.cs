using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_Account
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
        public void アカウントの詳細を取得する()
        {
            var testData = FakeOandaData.CreateAccount(1, "TEST1", "USD", 0.05m);
            var oandaApi = new OandaApi(_client, testData.AccountId);
            var obj = oandaApi.GetAccount().Result;

            Assert.AreEqual(testData.AccountId, obj.accountId);
            Assert.AreEqual(testData.AccountName, obj.accountName);
            Assert.AreEqual(testData.Balance, obj.balance);
            Assert.AreEqual(testData.UnrealizedProfitLoss, obj.unrealizedPl);
            Assert.AreEqual(testData.RealizedProfitLoss, obj.realizedPl);
            Assert.AreEqual(testData.MarginUsed, obj.marginUsed);
            Assert.AreEqual(testData.MarginAvail, obj.marginAvail);
            Assert.AreEqual(testData.OpenTrades, obj.openTrades);
            Assert.AreEqual(testData.OpenOrders, obj.openOrders);
            Assert.AreEqual(testData.MarginRate, obj.marginRate);
            Assert.AreEqual(testData.AccountCurrency, obj.accountCurrency);
        }
    }
}
