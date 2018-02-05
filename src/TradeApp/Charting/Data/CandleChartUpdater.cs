using System;

namespace TradeApp.Charting.Data
{
    public class CandleChartUpdater
    {
        private CandleChart chart;
        private CandleChartStore store;
        private CandleProvider provider;

        private ChartEntryEntity entry;

        public CandleChartUpdater(CandleChart chart, CandleChartStore store, CandleProvider provider)
        {
            this.chart = chart;
            this.store = store;
            this.provider = provider;
            initialize();
        }

        private void initialize()
        {
            entry = store.FindOrCreateEntry(chart.Symbol, chart.Range);
        }

        public void Fetch(DateTime from, DateTime to)
        {
            if (!store.IsCacheAvailable(entry, from, to))
            {
                var candles = provider.GetCandles(chart.Symbol, chart.Range, from, to);
                store.AddCandles(entry, from, to, candles);
            }
        }
    }
}
