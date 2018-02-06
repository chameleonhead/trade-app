using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TradeApp.Charting.Data
{
    [TestClass]
    public class CandleChartUpdaterTest
    {
        [TestMethod]
        public void ストアにデータが存在しない場合に期間を指定してフェッチするとプロバイダーからデータを取得する()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Daily;
            var chart = new CandleChart(symbol, range);
            var from = new DateTime(2017, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddDays(10);
            var candles = Seeds.CreateRomdomCandles(from, to, range);

            // プロバイダーのセットアップ
            var provider = new MockCandleProvider();
            provider.SetCandle(range, candles);

            // ストアは空
            using (var store = new CandleChartStore(new DbContextOptionsBuilder()
                .UseInMemoryDatabase("CandleChartUpdaterTestDb")
                .Options))
            {
                var chartUpdater = new CandleChartUpdater(chart, store, provider);
                chartUpdater.Fetch(from, to);

                Assert.AreEqual(11, store.Candles.Count());
            }
        }

        [TestMethod]
        public void ストアに登録済みのデータが存在する場合に期間を指定してフェッチするとプロバイダーからデータを取得しない()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Daily;
            var chart = new CandleChart(symbol, range);
            var from = new DateTime(2017, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddDays(10);
            var candles = Seeds.CreateRomdomCandles(from, to, range);

            // プロバイダーのセットアップ
            var provider = new MockCandleProvider();
            provider.SetCandle(range, candles);

            // ストアは空
            using (var store = new CandleChartStore(new DbContextOptionsBuilder()
                .UseInMemoryDatabase("CandleChartUpdaterTestDb")
                .Options))
            {
                // ストアに保存
                var entry = store.FindOrCreateEntry(symbol, range);
                store.AddCandles(entry, from, to, candles.ToArray());
                var chartUpdater = new CandleChartUpdater(chart, store, provider);
                chartUpdater.Fetch(from, to);

                Assert.IsFalse(provider.ProvidedCandles.Any());
            }
        }
    }
}
