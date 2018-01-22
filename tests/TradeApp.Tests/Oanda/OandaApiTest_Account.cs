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
            var account = _server.Context.DefaultAccount;

            var oandaApi = new OandaApi(_client, _server.DefaultAccountId);
            var obj = oandaApi.GetAccount().Result;

            Assert.AreEqual(account.AccountId, obj.accountId);
            Assert.AreEqual(account.AccountName, obj.accountName);
            Assert.AreEqual(account.Balance, obj.balance);
            Assert.AreEqual(account.UnrealizedProfitLoss, obj.unrealizedPl);
            Assert.AreEqual(account.RealizedProfitLoss, obj.realizedPl);
            Assert.AreEqual(account.MarginUsed, obj.marginUsed);
            Assert.AreEqual(account.MarginAvail, obj.marginAvail);
            Assert.AreEqual(account.OpenTrades, obj.openTrades);
            Assert.AreEqual(account.OpenOrders, obj.openOrders);
            Assert.AreEqual(account.MarginRate, obj.marginRate);
            Assert.AreEqual(account.AccountCurrency, obj.accountCurrency);
        }
    }
}
