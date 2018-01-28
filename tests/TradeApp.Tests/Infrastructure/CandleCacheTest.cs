using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using TradeApp.Charting;

namespace TradeApp.Infrastructure
{
    [TestClass]
    public class CandleCacheTest
    {
        TradingSymbol symbol;
        DateTime from, to;
        TimeSpan span;
        Mock<CandleProvider> mock;

        [TestInitialize]
        public void Setup()
        {
            symbol = new TradingSymbol("USD_JPY");
            from = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            to = new DateTime(2017, 1, 1, 1, 1, 50, DateTimeKind.Utc);
            span = TimeSpan.FromSeconds(5);
            mock = new Mock<CandleProvider>();
            mock.Setup(m => m.GetCandles(
                It.Is<TradingSymbol>(o => o.Equals(symbol)),
                It.Is<DateTime>(o => o.Equals(from)),
                It.Is<DateTime>(o => o.Equals(to)),
                It.Is<TimeSpan>(o => o.Equals(span))))
                .Returns(new Candle[0]);
        }

        [TestMethod]
        public void キャッシュが存在しない場合にデータを自動的に取得する()
        {
            var sut = new CandleCache(mock.Object);
            sut.GetCandles(symbol, from, to, span);
            mock.VerifyAll();
        }

        [TestMethod]
        public void キャッシュが存在する場合にデータを自動的に取得する()
        {
            var sut = new CandleCache(mock.Object);
            sut.GetCandles(symbol, from, to, span);

            mock.Reset();
            sut.GetCandles(symbol, from, to, span);
            mock.Verify(m => m.GetCandles(
                It.IsAny<TradingSymbol>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>()), Times.Never);
        }
    }
}
