﻿using Microsoft.EntityFrameworkCore;

namespace TradeApp.Charting.Data
{
    public class CandleStoreInitializer
    {
        public static void Initialize(CandleStore context)
        {
            context.Database.EnsureCreated();
        }
    }

    public class CandleStore : DbContext
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
        public DbSet<CandleEntity> Candles { get; set; }

        public void AddChart()
        {

        }
    }
}
