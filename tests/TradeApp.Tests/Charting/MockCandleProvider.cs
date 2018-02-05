using System;
using System.Collections.Generic;
using System.Linq;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    class MockCandleProvider : CandleProvider
    {
        public List<Candle> candles = new List<Candle>();

        public List<Candle> ProvidedCandles { get; } = new List<Candle>();

        public override Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime from, DateTime to)
        {
            var providingCandles = candles.Where(c => c.Time >= from && c.Time <= to).ToArray();
            ProvidedCandles.AddRange(providingCandles);
            return providingCandles;
        }

        public void SetCandle(IEnumerable<Candle> candles)
        {
            this.candles.AddRange(candles);
        }
    }
}
