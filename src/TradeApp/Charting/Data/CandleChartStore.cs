using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace TradeApp.Charting.Data
{
    public class CandleChartStore : DbContext
    {
        public CandleChartStore() : base(new DbContextOptionsBuilder()
                .UseSqlite($"Data Source=candles.db")
                .Options)
        {
        }

        public CandleChartStore(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChartEntryEntity>().ToTable("ChartEntries");
            modelBuilder.Entity<CandleFetchHistory>().ToTable("FetchHistories");
            modelBuilder.Entity<CandleEntity>().ToTable("Candles");
        }

        public DbSet<ChartEntryEntity> ChartEntries { get; set; }
        public DbSet<CandleFetchHistory> FetchHistories { get; set; }
        public DbSet<CandleEntity> Candles { get; set; }

        public bool IsCacheAvailable(ChartEntryEntity entry, DateTime from, DateTime to)
        {
            return FetchHistories.Where(fh => fh.ChartEntry.Id == entry.Id && fh.From >= from && fh.To <= to).Any();
        }

        public Candle[] GetCandles(ChartEntryEntity entry, DateTime from, DateTime to)
        {
            return Candles
                .Where(c => c.ChartEntry.Id == entry.Id && c.Time >= from && c.Time <= to)
                .OrderBy(c => c.Time)
                .Select(c => new Candle(c.Time, c.Open, c.High, c.Low, c.Close, c.Volume))
                .ToArray();
        }

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

        public void AddCandles(ChartEntryEntity entry, DateTime from, DateTime to, Candle[] candles)
        {
            FetchHistories.Add(new CandleFetchHistory()
            {
                ChartEntry = entry,
                From = from,
                To = to
            });
            Candles.AddRangeAsync(
                candles.Select(candle => new CandleEntity()
                {
                    ChartEntry = entry,
                    Time = candle.Time,
                    Open = candle.Open,
                    High = candle.High,
                    Low = candle.Low,
                    Close = candle.Close,
                    Volume = candle.Volume
                }));
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
