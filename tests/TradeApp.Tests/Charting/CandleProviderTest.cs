using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeApp.Charting;

namespace TradeApp.Infrastructure
{
    [TestClass]
    public class CandleProviderTest
    {
        [TestMethod]
        public void OANDAのキャンドルプロバイダーを指定する文字列を設定しOANDAのキャンドルプロバイダーが取得できる()
        {
            var sut = CandleProvider.Get("provider=OANDA;server=https://servername;token=token");
            Assert.IsNotNull(sut);
            Assert.AreEqual("OandaCandleProvider", sut.GetType().Name);
        }
    }
}
