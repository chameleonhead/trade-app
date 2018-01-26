using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TradeApp.Charting.Indicators
{
    [TestClass]
    public class AtrIndicatorTest
    {
        private static DateTime date = DateTime.Now;
        private static int i = 0;
        private static Candle[] CANDLES = new[]
        {
            new Candle(date.AddDays(i++),  0, 48.70m, 47.79m, 48.16m, 0),
            new Candle(date.AddDays(i++),  0, 48.72m, 48.14m, 48.61m, 0),
            new Candle(date.AddDays(i++),  0, 48.90m, 48.39m, 48.75m, 0),
            new Candle(date.AddDays(i++),  0, 48.87m, 48.37m, 48.63m, 0),
            new Candle(date.AddDays(i++),  0, 48.82m, 48.24m, 48.74m, 0),
            new Candle(date.AddDays(i++),  0, 49.05m, 48.64m, 49.03m, 0),
            new Candle(date.AddDays(i++),  0, 49.20m, 48.94m, 49.07m, 0),
            new Candle(date.AddDays(i++),  0, 49.35m, 48.86m, 49.32m, 0),
            new Candle(date.AddDays(i++),  0, 49.92m, 49.50m, 49.91m, 0),
            new Candle(date.AddDays(i++),  0, 50.19m, 49.87m, 50.13m, 0),
            new Candle(date.AddDays(i++),  0, 50.12m, 49.20m, 49.53m, 0),
            new Candle(date.AddDays(i++),  0, 49.66m, 48.90m, 49.50m, 0),
            new Candle(date.AddDays(i++),  0, 49.88m, 49.43m, 49.75m, 0),
            new Candle(date.AddDays(i++),  0, 50.19m, 49.73m, 50.03m, 0),
            new Candle(date.AddDays(i++),  0, 50.36m, 49.26m, 50.31m, 0),
            new Candle(date.AddDays(i++),  0, 50.57m, 50.09m, 50.52m, 0),
            new Candle(date.AddDays(i++),  0, 50.65m, 50.30m, 50.41m, 0),
            new Candle(date.AddDays(i++),  0, 50.43m, 49.21m, 49.34m, 0),
            new Candle(date.AddDays(i++),  0, 49.63m, 48.98m, 49.37m, 0),
            new Candle(date.AddDays(i++),  0, 50.33m, 49.61m, 50.23m, 0),
            new Candle(date.AddDays(i++),  0, 50.29m, 49.20m, 49.24m, 0),
            new Candle(date.AddDays(i++),  0, 50.17m, 49.43m, 49.93m, 0),
            new Candle(date.AddDays(i++),  0, 49.32m, 48.08m, 48.43m, 0),
            new Candle(date.AddDays(i++),  0, 48.50m, 47.64m, 48.18m, 0),
            new Candle(date.AddDays(i++),  0, 48.32m, 41.55m, 46.57m, 0),
            new Candle(date.AddDays(i++),  0, 46.80m, 44.28m, 45.41m, 0),
            new Candle(date.AddDays(i++),  0, 47.80m, 47.31m, 47.77m, 0),
            new Candle(date.AddDays(i++),  0, 48.39m, 47.20m, 47.72m, 0),
            new Candle(date.AddDays(i++),  0, 48.66m, 47.90m, 48.62m, 0),
            new Candle(date.AddDays(i++),  0, 48.79m, 47.73m, 47.85m, 0),
        };

        [TestMethod]
        public void _14日間ATRでは14日目まで戻り値がNULLになる()
        {
            var sut = new AtrIndicator(14);
            for (var i = 0; i < 14; i++)
            {
                Assert.AreEqual(null, sut.NextValue(CANDLES[i]));
            }
        }

        [TestMethod]
        public void _14日間ATRでは15日目以降は戻り値が計算されたATRとなる()
        {
            var expected = new decimal[]
            {
                0.5933m,
                0.5852m,
                0.5684m,
                0.6149m,
                0.6174m,
                0.6419m,
                0.6739m,
                0.6922m,
                0.7749m,
                0.7810m,
                1.2088m,
                1.3024m,
                1.3801m,
                1.3665m,
                1.3361m,
                1.3163m
            };

            var sut = new AtrIndicator(14);
            for (var i = 0; i < 14; i++)
            {
                sut.NextValue(CANDLES[i]);
            }

            for (var i = 14; i < 14 + expected.Length; i++)
            {
                Assert.AreEqual(expected[i - 14], Math.Round(sut.NextValue(CANDLES[i]).Value, 4));
            }
        }
    }
}
