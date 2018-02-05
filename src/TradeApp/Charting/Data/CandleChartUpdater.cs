using System;

namespace TradeApp.Charting.Data
{
    public class CandleChartUpdater
    {
        private TradingSymbol symbol;
        private ChartRange range;
        private CandleChartStore store;
        private CandleProvider provider;

        private ChartEntryEntity entry;

        public CandleChartUpdater(TradingSymbol symbol, ChartRange range, CandleChartStore store, CandleProvider provider)
        {
            this.symbol = symbol;
            this.range = range;
            this.store = store;
            this.provider = provider;
            initialize();
        }

        private void initialize()
        {
            entry = store.FindOrCreateEntry(symbol, range);
        }

        public void Fetch(DateTime from, DateTime to)
        {
            if (!store.IsCacheAvailable(entry, from, to))
            {
                var candles = provider.GetCandles(symbol, range, from, to);
                store.AddCandles(entry, from, to, candles);
            }
        }
    }
}
