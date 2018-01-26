using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace TradeApp.Charting.Indicators
{
    [TestClass]
    public class SmaIndicatorTest
    {
        [TestMethod]
        public void _5日間単純移動平均で5日までの場合その時の平均が求められる()
        {
            var sut = new SmaIndicator(5);
            Assert.AreEqual(Enumerable.Range(1, 1).Sum() / 1m, sut.NextValue(1));
            Assert.AreEqual(Enumerable.Range(1, 2).Sum() / 2m, sut.NextValue(2));
            Assert.AreEqual(Enumerable.Range(1, 3).Sum() / 3m, sut.NextValue(3));
            Assert.AreEqual(Enumerable.Range(1, 4).Sum() / 4m, sut.NextValue(4));
            Assert.AreEqual(Enumerable.Range(1, 5).Sum() / 5m, sut.NextValue(5));
        }

        [TestMethod]
        public void _5日間単純移動平均で5日分以上値を入れた場合その時の5日間平均が求められる()
        {
            var sut = new SmaIndicator(5);
            Enumerable.Range(1, 5).ToList().ForEach(i => sut.NextValue(i));

            Assert.AreEqual(Enumerable.Range(2, 5).Sum() / 5m, sut.NextValue(6));
            Assert.AreEqual(Enumerable.Range(3, 5).Sum() / 5m, sut.NextValue(7));
            Assert.AreEqual(Enumerable.Range(4, 5).Sum() / 5m, sut.NextValue(8));
            Assert.AreEqual(Enumerable.Range(5, 5).Sum() / 5m, sut.NextValue(9));
            Assert.AreEqual(Enumerable.Range(6, 5).Sum() / 5m, sut.NextValue(10));
        }
    }
}
