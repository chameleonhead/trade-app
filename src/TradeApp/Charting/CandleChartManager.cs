using System;
using System.Collections.Generic;
using System.Linq;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    public class CandleChartManager
    {
        private Func<TradingSymbol, ChartRange, ICandleProvider> candleProviderFactory;
        private CandleChartStore store;
        private DateTime currentTime;
        private Dictionary<Tuple<TradingSymbol, ChartRange>, Tuple<CandleChart, CandleChartUpdater>> charts
            = new Dictionary<Tuple<TradingSymbol, ChartRange>, Tuple<CandleChart, CandleChartUpdater>>();

        public CandleChartManager(DateTime currentTime, Func<TradingSymbol, ChartRange, ICandleProvider> candleProviderFactory) : this(currentTime, candleProviderFactory, new CandleChartStore())
        {
        }

        public CandleChartManager(DateTime currentTime, Func<TradingSymbol, ChartRange, ICandleProvider> candleProviderFactory, CandleChartStore store)
        {
            this.candleProviderFactory = candleProviderFactory;
            this.store = store;
            this.currentTime = currentTime;
        }

        public CandleChart GetChart(TradingSymbol symbol, ChartRange range)
        {
            var chartKey = Tuple.Create(symbol, range);
            if (charts.TryGetValue(chartKey, out var chartAndUpdater))
            {
                return chartAndUpdater.Item1;
            }

            var provider = candleProviderFactory(symbol, range);
            var chart = new CandleChart(symbol, range);
            var updater = new CandleChartUpdater(chart, store, provider);
            charts.Add(chartKey, Tuple.Create(chart, updater));
            var now = currentTime;
            updater.Update(now, 100);
            return chart;
        }

        public void Update(DateTime to)
        {
            charts.Values.ToList()
                .ForEach(cau => cau.Item2.Update(to));
            currentTime = to;
        }
    }
}
