using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TradeApp.Charting.Indicators
{
    [TestClass]
    public class SmaIndicatorTest
    {
        private SingleValue sv(decimal val)
        {
            return new SingleValue(new DateTime(), val);
        }

        [TestMethod]
        public void _5日間単純移動平均で5日までの場合その時の平均が求められる()
        {
            var sut = new SmaIndicator(5);
            Assert.AreEqual(Enumerable.Range(1, 1).Sum() / 1m, sut.Next(sv(1)).Value);
            Assert.AreEqual(Enumerable.Range(1, 2).Sum() / 2m, sut.Next(sv(2)).Value);
            Assert.AreEqual(Enumerable.Range(1, 3).Sum() / 3m, sut.Next(sv(3)).Value);
            Assert.AreEqual(Enumerable.Range(1, 4).Sum() / 4m, sut.Next(sv(4)).Value);
            Assert.AreEqual(Enumerable.Range(1, 5).Sum() / 5m, sut.Next(sv(5)).Value);
        }

        [TestMethod]
        public void _5日間単純移動平均で5日分以上値を入れた場合その時の5日間平均が求められる()
        {
            var sut = new SmaIndicator(5);
            Enumerable.Range(1, 5).ToList().ForEach(i => sut.Next(sv(i)));

            Assert.AreEqual(Enumerable.Range(2, 5).Sum() / 5m, sut.Next(sv(6)).Value);
            Assert.AreEqual(Enumerable.Range(3, 5).Sum() / 5m, sut.Next(sv(7)).Value);
            Assert.AreEqual(Enumerable.Range(4, 5).Sum() / 5m, sut.Next(sv(8)).Value);
            Assert.AreEqual(Enumerable.Range(5, 5).Sum() / 5m, sut.Next(sv(9)).Value);
            Assert.AreEqual(Enumerable.Range(6, 5).Sum() / 5m, sut.Next(sv(10)).Value);
        }
    }
}
