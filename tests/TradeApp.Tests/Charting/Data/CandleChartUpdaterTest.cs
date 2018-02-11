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
        public void ストアにデータが存在しない場合に取得数を指定してフェッチするとプロバイダーからデータを取得する()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Daily;
            var chart = new CandleChart(symbol, range);
            var to = new DateTime(2017, 12, 10, 0, 0, 0, DateTimeKind.Utc);
            var candles = Seeds.CreateRomdomCandles(to.AddDays(-20), to, range);

            // プロバイダーのセットアップ
            var provider = new MockCandleProvider();
            provider.SetCandle(range, candles);

            // ストアは空
            using (var store = new CandleChartStore(new DbContextOptionsBuilder()
                .UseInMemoryDatabase("CandleChartUpdaterTestDb1")
                .Options))
            {
                var chartUpdater = new CandleChartUpdater(chart, store, provider);
                chartUpdater.Update(to, 10);

                Assert.AreEqual(10, store.Candles.Count());
            }
        }

        [TestMethod]
        public void ストアに登録済みのデータが存在する場合に取得数を指定してフェッチするとプロバイダーからデータを取得しない()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Daily;
            var chart = new CandleChart(symbol, range);
            var to = new DateTime(2017, 12, 10, 0, 0, 0, DateTimeKind.Utc);
            var candles = Seeds.CreateRomdomCandles(to.AddDays(-20), to, range);

            // プロバイダーのセットアップ
            var provider = new MockCandleProvider();
            provider.SetCandle(range, candles);

            // ストアは空
            using (var store = new CandleChartStore(new DbContextOptionsBuilder()
                .UseInMemoryDatabase("CandleChartUpdaterTestDb2")
                .Options))
            {
                // ストアに保存
                var entry = store.FindOrCreateEntry(symbol, range);
                store.AddCandles(entry, to, candles.ToArray());
                var chartUpdater = new CandleChartUpdater(chart, store, provider);
                chartUpdater.Update(to, 10);

                Assert.IsFalse(provider.ProvidedCandles.Any());
            }
        }

        [TestMethod]
        public void ストアにデータが存在しない場合に終了時刻のみを指定してフェッチするとプロバイダーからデータを取得する()
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
                .UseInMemoryDatabase("CandleChartUpdaterTestDb3")
                .Options))
            {
                var chartUpdater = new CandleChartUpdater(chart, store, provider);
                chartUpdater.Update(from, 1);
                chartUpdater.Update(to);

                Assert.AreEqual(11, store.Candles.Count());
                Assert.AreEqual(12, provider.ProvidedCandles.Count());
            }
        }

        [TestMethod]
        public void ストアに登録済みのデータが存在する場合に終了時刻のみを指定してフェッチするとプロバイダーからデータを取得しない()
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
                .UseInMemoryDatabase("CandleChartUpdaterTestDb4")
                .Options))
            {
                // ストアに保存
                var entry = store.FindOrCreateEntry(symbol, range);
                store.AddCandles(entry, from, to, candles.ToArray());
                var chartUpdater = new CandleChartUpdater(chart, store, provider);
                chartUpdater.Update(from, 1);
                chartUpdater.Update(to);

                Assert.AreEqual(11, store.Candles.Count());
                Assert.IsFalse(provider.ProvidedCandles.Any());
            }
        }
    }
}
