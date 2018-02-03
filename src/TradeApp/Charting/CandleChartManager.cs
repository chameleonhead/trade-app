using System;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    public class CandleChartManager
    {
        private Func<TradingSymbol, ChartRange, CandleProvider> candleStoreFactory;

        public CandleChartManager(DateTime from, Func<TradingSymbol, ChartRange, CandleProvider> candleStoreFactory)
        {
            this.candleStoreFactory = candleStoreFactory;
        }

        public CandleChart GetChart(TradingSymbol symbol, ChartRange range)
        {
            throw new NotImplementedException();
        }

        public void Update(DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}
