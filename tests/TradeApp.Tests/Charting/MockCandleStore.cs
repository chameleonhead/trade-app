using System.Collections.Generic;
using System.Linq;

namespace TradeApp.Charting
{
    class MockCandleStore : ICandleStore
    {
        private HashSet<Candle> candles;

        public MockCandleStore()
        {
            candles = new HashSet<Candle>();
        }

        public Candle LatestCandle { get; private set; }

        public IQueryable<Candle> Candles => new EnumerableQuery<Candle>(candles).OrderBy(candle => candle.Time);

        public void AddCandle(Candle candle)
        {
            LatestCandle = candle;
            candles.Add(candle);
        }

        public void Initialize(TradingSymbol symbol, ChartRange hourly)
        {
        }
    }
}
