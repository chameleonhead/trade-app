using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TradeApp.Charting.Data;
using TradeApp.Charting.Indicators;

namespace TradeApp.Charting
{
    [TestClass]
    public class CandleChartManagerTest
    {
        private static DateTime from = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime current = new DateTime(2017, 4, 3, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime to = new DateTime(2017, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        private static Lazy<IEnumerable<Candle>> HourlyCandles = new Lazy<IEnumerable<Candle>>(() => Seeds.CreateRomdomCandles(from, to, ChartRange.Hourly));
        private static Lazy<IEnumerable<Candle>> DailyCandles = new Lazy<IEnumerable<Candle>>(() => Seeds.CreateRomdomCandles(from, to, ChartRange.Daily));
        private static Lazy<IEnumerable<Candle>> WeeklyCandles = new Lazy<IEnumerable<Candle>>(() => Seeds.CreateRomdomCandles(from, to, ChartRange.Weekly));

        private MockCandleProvider mockProvider;
        private CandleChartStore store;

        [TestInitialize]
        public void Setup()
        {
            mockProvider = new MockCandleProvider();
            mockProvider.SetCandle(ChartRange.Hourly, HourlyCandles.Value);
            mockProvider.SetCandle(ChartRange.Daily, DailyCandles.Value);
            mockProvider.SetCandle(ChartRange.Weekly, WeeklyCandles.Value);

            store = new CandleChartStore(new DbContextOptionsBuilder()
                .UseInMemoryDatabase("CandleChartManagerTest")
                .Options);
            store.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Teardown()
        {
            store.Database.EnsureDeleted();
            store.Dispose();
        }

        [TestMethod]
        public void キャンドルマネージャーに1件のキャンドルを設定して更新した場合にキャンドルが正しく更新される()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Hourly;
            var sut = new CandleChartManager(current, (s, r) => mockProvider, store);
            var chart = sut.GetChart(symbol, range);
            chart.AddIndicator("SMA5", new SmaIndicator(5));
            chart.AddIndicator("SMA10", new SmaIndicator(10));
            chart.AddIndicator("SMA20", new SmaIndicator(20));
            chart.AddIndicator("SMA60", new SmaIndicator(60));
            chart.AddIndicator("SMA120", new SmaIndicator(120));

            sut.Update(to);
            stopwatch.Stop();
            Trace.Write(string.Format("{0}: {1}", DateTime.Now, stopwatch.Elapsed));
        }

    }
}
