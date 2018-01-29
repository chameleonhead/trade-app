using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace TradeApp.Charting.Data
{
    [TestClass]
    public class CandleStoreTest
    {
        [TestInitialize]
        public void Setup()
        {
            using (var context = new CandleStore())
            {
                CandleStoreInitializer.Initialize(context);
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            using (var context = new CandleStore())
            {
                context.Database.EnsureDeleted();
            }
        }

        [TestMethod]
        public void チャートエントリを追加できること()
        {
            var entry = new ChartEntryEntity()
            {
                Symbol = "USD_JPY",
                Range = ChartRange.Hourly
            };
            using (var context = new CandleStore())
            {
                context.ChartEntries.Add(entry);
                context.SaveChanges();
            }

            using (var context = new CandleStore())
            {
                Assert.IsNotNull(context.ChartEntries.Find(entry.Id));
            }
        }

        [TestMethod]
        public void キャンドルを追加できること()
        {
            var entry = new ChartEntryEntity()
            {
                Symbol = "USD_JPY",
                Range = ChartRange.Hourly
            };

            entry.Candles.Add(new CandleEntity()
            {
                Time = DateTime.Now,
                Open = 2,
                High = 4,
                Low = 1,
                Close = 3,
                Volume = 5,
            });

            using (var context = new CandleStore())
            {
                context.ChartEntries.Add(entry);
                context.SaveChanges();
            }

            using (var context = new CandleStore())
            {
                var chart = context.ChartEntries.Find(entry.Id);
                context.Entry(chart).Collection(ce => ce.Candles).Load();
                Assert.IsNotNull(chart.Candles.FirstOrDefault());
            }
        }
    }
}
