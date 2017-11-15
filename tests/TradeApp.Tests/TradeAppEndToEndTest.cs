using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeApp.Drivers;

namespace TradeApp
{
    [TestClass]
    public class TradeAppEndToEndTest
    {
        private static string OANDA_OAUTH_TOKEN = "TEST_TOKEN";
        private static OandaFakeServerDriver oanda;

        static TradeAppEndToEndTest()
        {
            oanda = new OandaFakeServerDriver(OANDA_OAUTH_TOKEN);
        }

        [TestInitialize]
        public void Setup()
        {
            oanda.Start();
        }

        [TestMethod]
        public void プログラムが起動したら口座残高が取得され終了コマンドを受けて終了する()
        {
            var app = new TradeAppRunner(oanda, OANDA_OAUTH_TOKEN);
            app.Start();
            app.WriteEventForStartup();
            oanda.HasReceivedAccountRequest();
            app.WriteEventForAccountBalanceUpdated();
            app.Stop();
        }

        [TestCleanup]
        public void Teardown()
        {
            oanda.Stop();
        }
    }
}
