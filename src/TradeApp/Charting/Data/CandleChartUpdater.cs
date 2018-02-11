using System;

namespace TradeApp.Charting.Data
{
    public class CandleChartUpdater
    {
        private CandleChart chart;
        private CandleChartStore store;
        private ICandleProvider provider;

        private Lazy<ChartEntryEntity> entry;

        public CandleChartUpdater(CandleChart chart, CandleChartStore store, ICandleProvider provider)
        {
            this.chart = chart;
            this.store = store;
            this.provider = provider;
            this.entry = new Lazy<ChartEntryEntity>(() => store.FindOrCreateEntry(chart.Symbol, chart.Range));
        }

        public void Update(DateTime to, int takeCount)
        {
            Candle[] candles;
            if (store.IsCacheAvailable(entry.Value, to, takeCount))
            {
                candles = store.GetCandles(entry.Value, to, takeCount);
            }
            else
            {
                candles = provider.GetCandles(chart.Symbol, chart.Range, to, takeCount);
                store.AddCandles(entry.Value, to, candles);
            }
            chart.AddCandles(candles);
        }

        public void Update(DateTime to)
        {
            Candle[] candles;
            var from = chart.LatestCandleTime;
            if (store.IsCacheAvailable(entry.Value, from, to))
            {
                candles = store.GetCandles(entry.Value, from, to);
            }
            else
            {
                candles = provider.GetCandles(chart.Symbol, chart.Range, from, to);
                store.AddCandles(entry.Value, from, to, candles);
            }
            chart.AddCandles(candles);
        }
    }
}
