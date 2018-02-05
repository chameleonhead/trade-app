using System;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    public class CandleChartManager
    {
        private Func<TradingSymbol, ChartRange, CandleProvider> candleProviderFactory;
        private CandleChartStore store = new CandleChartStore();
        private DateTime currentTime;

        public CandleChartManager(DateTime currentTime, Func<TradingSymbol, ChartRange, CandleProvider> candleProviderFactory)
        {
            this.candleProviderFactory = candleProviderFactory;
            this.currentTime = currentTime;
        }

        public CandleChart GetChart(TradingSymbol symbol, ChartRange range)
        {
            var provider = candleProviderFactory(symbol, range);
            CandleChart chart = new CandleChart(symbol, range);
            var updater = new CandleChartUpdater(chart, store, provider);
            var now = currentTime;
            updater.Fetch(now - TimeSpan.FromSeconds(200 * (int)range), now);
            return chart;
        }

        public void Update(DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}
