using System.Linq;

namespace TradeApp.Charting.Data
{
    public class CandleStore : ICandleStore
    {
        private TradingSymbol symbol;
        private ChartRange range;
        private CandleChartStore store;
        private ChartEntryEntity entry;

        public CandleStore(CandleChartStore store)
        {
            this.store = store;
        }

        public void Initialize(TradingSymbol symbol, ChartRange range)
        {
            this.symbol = symbol;
            this.range = range;
            entry = store.ChartEntries.Where(ce => ce.Symbol == symbol.Symbol && ce.Range == range).FirstOrDefault();
            if (entry == null)
            {
                entry = new ChartEntryEntity()
                {
                    Symbol = symbol.Symbol,
                    Range = range,
                };
                this.store.ChartEntries.AddAsync(entry);
                store.SaveChangesAsync();
            }
        }

        public IQueryable<Candle> Candles => store
            .Candles
            .Where(c => c.ChartEntry.Symbol == symbol.Symbol && c.ChartEntry.Range == range)
            .OrderBy(c => c.Time)
            .Select(c => new Candle(c.Time, c.Open, c.High, c.Low, c.Close, c.Volume));

        public Candle LatestCandle => Candles.LastOrDefault();

        public void AddCandle(Candle candle)
        {
            store.Candles.AddAsync(new CandleEntity()
            {
                Time = candle.Time,
                Open = candle.Open,
                High = candle.High,
                Low = candle.Low,
                Close = candle.Close,
                Volume = candle.Volume
            });
            store.SaveChangesAsync();
        }
    }
}
