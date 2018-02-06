using System;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    public class CandleChartManager
    {
        private Func<TradingSymbol, ChartRange, CandleProvider> candleProviderFactory;
        private CandleChartStore store;
        private DateTime currentTime;
        private CandleChartUpdater updater;
        private CandleChart chart;

        public CandleChartManager(DateTime currentTime, Func<TradingSymbol, ChartRange, CandleProvider> candleProviderFactory) : this(currentTime, candleProviderFactory, new CandleChartStore())
        {
        }

        public CandleChartManager(DateTime currentTime, Func<TradingSymbol, ChartRange, CandleProvider> candleProviderFactory, CandleChartStore store)
        {
            this.candleProviderFactory = candleProviderFactory;
            this.store = store;
            this.currentTime = currentTime;
        }

        public CandleChart GetChart(TradingSymbol symbol, ChartRange range)
        {
            var provider = candleProviderFactory(symbol, range);
            chart = new CandleChart(symbol, range);
            updater = new CandleChartUpdater(chart, store, provider);
            var now = currentTime;
            updater.Fetch(now - TimeSpan.FromSeconds(200 * (int)range), now);
            return chart;
        }

        public void Update(DateTime to)
        {
            updater.Fetch(currentTime, to);
            currentTime = to;
        }
    }
}
