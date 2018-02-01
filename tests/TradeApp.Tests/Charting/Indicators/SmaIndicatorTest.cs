using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TradeApp.Charting.Indicators
{
    [TestClass]
    public class SmaIndicatorTest
    {
        [TestMethod]
        public void SMA実装テスト_5区間の単純移動平均で5回以下の場合その時の平均が求められる()
        {
            var sut = new SmaIndicator.SmaImpl(5);
            Assert.AreEqual(Enumerable.Range(1, 1).Sum() / 1m, sut.Next(1));
            Assert.AreEqual(Enumerable.Range(1, 2).Sum() / 2m, sut.Next(2));
            Assert.AreEqual(Enumerable.Range(1, 3).Sum() / 3m, sut.Next(3));
            Assert.AreEqual(Enumerable.Range(1, 4).Sum() / 4m, sut.Next(4));
            Assert.AreEqual(Enumerable.Range(1, 5).Sum() / 5m, sut.Next(5));
        }

        [TestMethod]
        public void SMA実装テスト_5区間の単純移動平均で5回以上値を入れた場合直近の5回分の平均が求められる()
        {
            var sut = new SmaIndicator.SmaImpl(5);
            Enumerable.Range(1, 5).ToList().ForEach(i => sut.Next(i));

            Assert.AreEqual(Enumerable.Range(2, 5).Sum() / 5m, sut.Next(6));
            Assert.AreEqual(Enumerable.Range(3, 5).Sum() / 5m, sut.Next(7));
            Assert.AreEqual(Enumerable.Range(4, 5).Sum() / 5m, sut.Next(8));
            Assert.AreEqual(Enumerable.Range(5, 5).Sum() / 5m, sut.Next(9));
            Assert.AreEqual(Enumerable.Range(6, 5).Sum() / 5m, sut.Next(10));
        }

        [TestMethod]
        public void _5日間単純移動平均で5日分以上値を入れた場合その時の5日間平均終値より求められる()
        {
            var sut = new SmaIndicator(5);
            var date = DateTime.Now;
            Enumerable.Range(1, 5).ToList().ForEach(i => sut.Next(new Candle(date.AddDays(i), 0, 0, 0, i, 0)));

            Assert.AreEqual(Enumerable.Range(2, 5).Sum() / 5m, sut.Next(new Candle(date.AddDays(6), 0, 0, 0, 6, 0)).Value);
            Assert.AreEqual(Enumerable.Range(3, 5).Sum() / 5m, sut.Next(new Candle(date.AddDays(7), 0, 0, 0, 7, 0)).Value);
            Assert.AreEqual(Enumerable.Range(4, 5).Sum() / 5m, sut.Next(new Candle(date.AddDays(8), 0, 0, 0, 8, 0)).Value);
            Assert.AreEqual(Enumerable.Range(5, 5).Sum() / 5m, sut.Next(new Candle(date.AddDays(9), 0, 0, 0, 9, 0)).Value);
            Assert.AreEqual(Enumerable.Range(6, 5).Sum() / 5m, sut.Next(new Candle(date.AddDays(10), 0, 0, 0, 10, 0)).Value);
        }
    }
}
