using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Lazy<IEnumerable<Candle>> DailyCandles = new Lazy<IEnumerable<Candle>>(() => Seeds.CreateRomdomCandles(from, to, ChartRange.Daily));

        private MockCandleProvider mockProvider;
        private CandleChartStore store;

        [TestInitialize]
        public void Setup()
        {
            mockProvider = new MockCandleProvider();
            mockProvider.SetCandle(ChartRange.Daily, DailyCandles.Value);

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
            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Daily;
            var sut = new CandleChartManager(current, (s, r) => mockProvider, store);
            var chart = sut.GetChart(symbol, range);
            chart.AddIndicator("SMA5", new SmaIndicator(5));
            chart.AddIndicator("SMA10", new SmaIndicator(10));
            chart.AddIndicator("SMA20", new SmaIndicator(20));
            chart.AddIndicator("SMA60", new SmaIndicator(60));
            chart.AddIndicator("SMA120", new SmaIndicator(120));

            var snapshotBeforeUpdate = chart.Snapshot;
            sut.Update(to);
            var snapshotAfterUpdate = chart.Snapshot;
            Assert.IsTrue(snapshotBeforeUpdate.Candles.Last().Time < snapshotAfterUpdate.Candles.Last().Time);
        }

        [TestMethod]
        public void キャンドルマネージャーに2件のキャンドルを設定して更新した場合にキャンドルが正しく更新される()
        {
            var sut = new CandleChartManager(current, (s, r) => mockProvider, store);

            var symbol1 = new TradingSymbol("USD_JPY");
            var range1 = ChartRange.Daily;
            var chart1 = sut.GetChart(symbol1, range1);
            chart1.AddIndicator("SMA5", new SmaIndicator(5));
            chart1.AddIndicator("SMA10", new SmaIndicator(10));
            chart1.AddIndicator("SMA20", new SmaIndicator(20));
            chart1.AddIndicator("SMA60", new SmaIndicator(60));
            chart1.AddIndicator("SMA120", new SmaIndicator(120));

            var symbol2 = new TradingSymbol("EUR_USD");
            var range2 = ChartRange.Daily;
            var chart2 = sut.GetChart(symbol2, range2);
            chart2.AddIndicator("SMA5", new SmaIndicator(5));
            chart2.AddIndicator("SMA10", new SmaIndicator(10));
            chart2.AddIndicator("SMA20", new SmaIndicator(20));
            chart2.AddIndicator("SMA60", new SmaIndicator(60));
            chart2.AddIndicator("SMA120", new SmaIndicator(120));

            var snapshotBeforeUpdate1 = chart1.Snapshot;
            var snapshotBeforeUpdate2 = chart2.Snapshot;
            sut.Update(to);
            var snapshotAfterUpdate1 = chart1.Snapshot;
            var snapshotAfterUpdate2 = chart2.Snapshot;
            Assert.IsTrue(snapshotBeforeUpdate1.Candles.Last().Time < snapshotAfterUpdate1.Candles.Last().Time);
            Assert.IsTrue(snapshotBeforeUpdate2.Candles.Last().Time < snapshotAfterUpdate2.Candles.Last().Time);
        }
    }
}
