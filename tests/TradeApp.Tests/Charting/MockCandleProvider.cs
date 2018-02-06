using System;
using System.Collections.Generic;
using System.Linq;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    class MockCandleProvider : CandleProvider
    {
        public Dictionary<ChartRange, List<Candle>> candles = new Dictionary<ChartRange, List<Candle>>();

        public List<Candle> ProvidedCandles { get; } = new List<Candle>();

        public override Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime from, DateTime to)
        {
            if (candles.TryGetValue(range, out var value))
            {
                var providingCandles = value.Where(c => c.Time >= from && c.Time <= to).ToArray();
                ProvidedCandles.AddRange(providingCandles);
                return providingCandles;
            }
            return Array.Empty<Candle>();
        }

        public void SetCandle(ChartRange range, IEnumerable<Candle> candles)
        {
            if (this.candles.TryGetValue(range, out var value))
            {
                value.AddRange(candles);
            }
            else
            {
                this.candles[range] = candles.ToList();
            }
        }
    }
}
