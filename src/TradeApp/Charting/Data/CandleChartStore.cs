using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace TradeApp.Charting.Data
{
    public class CandleChartStore : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite($"Data Source=candles.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChartEntryEntity>().ToTable("ChartEntries");
            modelBuilder.Entity<CandleEntity>().ToTable("Candles");
        }

        public DbSet<ChartEntryEntity> ChartEntries { get; set; }

        internal bool IsCacheAvailable(ChartEntryEntity entry, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public DbSet<CandleEntity> Candles { get; set; }

        public ChartEntryEntity FindOrCreateEntry(TradingSymbol symbol, ChartRange range)
        {
            var entry = ChartEntries.Where(ce => ce.Symbol == symbol.Symbol && ce.Range == range).FirstOrDefault();
            if (entry == null)
            {
                entry = new ChartEntryEntity()
                {
                    Symbol = symbol.Symbol,
                    Range = range,
                };
                ChartEntries.AddAsync(entry);
                SaveChangesAsync();
            }
            return entry;
        }

        public void AddCandle(Candle candle)
        {
            Candles.AddAsync(new CandleEntity()
            {
                Time = candle.Time,
                Open = candle.Open,
                High = candle.High,
                Low = candle.Low,
                Close = candle.Close,
                Volume = candle.Volume
            });
            SaveChangesAsync();
        }
    }

    public class CandleStoreInitializer
    {
        public static void Initialize(CandleChartStore context)
        {
            context.Database.EnsureCreated();
        }
    }
}
