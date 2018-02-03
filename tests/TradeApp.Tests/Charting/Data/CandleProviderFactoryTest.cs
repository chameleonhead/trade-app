using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TradeApp.Charting.Data
{
    [TestClass]
    public class CandleProviderFactoryTest
    {
        [TestMethod]
        public void 事前に登録したキャンドルプロバイダーをシンボルを指定して取得できる()
        {
            var symbol = new TradingSymbol("USD_JPY");
            CandleProviderFactory.Register(symbol, () => new MockCandleProvider());

            var sut = CandleProviderFactory.GetInstance(symbol);
            Assert.IsNotNull(sut);
            Assert.AreEqual("MockCandleProvider", sut.GetType().Name);
        }
    }
}
