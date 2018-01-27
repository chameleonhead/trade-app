using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using TradeApp.FakeOandaSrver;

namespace TradeApp.Oanda
{
    [TestClass]
    public class OandaApiTest_Account
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
        public void アカウントの詳細を取得する()
        {
            var account = _server.Context.DefaultAccount;

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var obj = oandaApi.GetAccount().Result;

            Assert.AreEqual(account.AccountId, obj.AccountId);
            Assert.AreEqual(account.AccountName, obj.AccountName);
            Assert.AreEqual(account.Balance, obj.Balance);
            Assert.AreEqual(account.UnrealizedProfitLoss, obj.UnrealizedPl);
            Assert.AreEqual(account.RealizedProfitLoss, obj.RealizedPl);
            Assert.AreEqual(account.MarginUsed, obj.MarginUsed);
            Assert.AreEqual(account.MarginAvail, obj.MarginAvail);
            Assert.AreEqual(account.OpenTrades, obj.OpenTrades);
            Assert.AreEqual(account.OpenOrders, obj.OpenOrders);
            Assert.AreEqual(account.MarginRate, obj.MarginRate);
            Assert.AreEqual(account.AccountCurrency, obj.AccountCurrency);
        }
    }
}
