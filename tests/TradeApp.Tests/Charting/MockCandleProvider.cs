using System;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    class MockCandleProvider : CandleProvider
    {
        public override Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}
