using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TradeApp.Charting.Data
{
    public class ChartEntryEntity
    {
        public ChartEntryEntity()
        {
            Candles = new List<CandleEntity>();
        }

        [Key]
        public int Id { get; set; }
        public string Symbol { get; set; }
        public ChartRange Range { get; set; }

        public virtual ICollection<CandleEntity> Candles { get; set; }
    }

    public class CandleFetchHistory
    {
        [Key]
        public int Id { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int FetchCount { get; set; }

        public virtual ChartEntryEntity ChartEntry { get; set; }
    }


    public class CandleEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }

        public virtual ChartEntryEntity ChartEntry { get; set; }
    }
}
