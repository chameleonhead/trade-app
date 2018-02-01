using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TradeApp.Charting.Data
{
    [TestClass]
    public class CandleProviderFactoryTest
    {
        [TestMethod]
        public void OANDAのキャンドルプロバイダーを指定する文字列を設定しOANDAのキャンドルプロバイダーが取得できる()
        {
            var sut = CandleProviderFactory.GetInstance("OANDA;server=https://servername;token=token");
            Assert.IsNotNull(sut);
            Assert.AreEqual("OandaCandleProvider", sut.GetType().Name);
        }
    }
}
