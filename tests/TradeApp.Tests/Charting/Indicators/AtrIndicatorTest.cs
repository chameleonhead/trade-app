using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TradeApp.Charting.Indicators
{
    [TestClass]
    public class AtrIndicatorTest
    {
        [TestMethod]
        public void _14日間ATRでは14日目まで戻り値がNULLになる()
        {
            var sut = new AtrIndicator(14);
            foreach (var candle in Seeds.ATR14_CANDLES.Item1.Take(14))
            {
                Assert.AreEqual(null, sut.Next(candle));
            }
        }

        [TestMethod]
        public void _14日間ATRでは15日目以降は戻り値が計算されたATRとなる()
        {
            var sut = new AtrIndicator(14);
            foreach (var candle in Seeds.ATR14_CANDLES.Item1.Take(14))
            {
                sut.Next(candle);
            }

            foreach (var candle in Seeds.ATR14_CANDLES
                .Item1
                .Select((value, index) => new { value, index })
                .Skip(14))
            {
                Assert.AreEqual(
                    Seeds.ATR14_CANDLES.Item2[candle.index].Value, 
                    Math.Round(sut.Next(candle.value).Value, 4)
                );
            }
        }
    }
}
