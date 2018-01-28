using System;
using System.Collections.Generic;
using System.Linq;
using TradeApp.Charting;

namespace TradeApp.Infrastructure
{
    public class CandleCache
    {
        class CandleCachRegistry
        {
            private static object lockObject = new object();
            private static int MAX_ID = 0;
            public static CandleCachRegistry New(DateTime start, DateTime end)
            {
                int nextId;
                lock(lockObject)
                {
                    nextId = ++MAX_ID;
                }
                return new CandleCachRegistry(nextId, start, end);
            }

            public CandleCachRegistry(int id, DateTime start, DateTime end)
            {
                Id = id;
                Start = start;
                End = end;
            }

            public int Id { get; }
            public DateTime Start { get; }
            public DateTime End { get; }
        }

        private Dictionary<Tuple<TradingSymbol, TimeSpan>, List<CandleCachRegistry>> registries = new Dictionary<Tuple<TradingSymbol, TimeSpan>, List<CandleCachRegistry>>();
        private Dictionary<int, List<Candle>> caches = new Dictionary<int, List<Candle>>();
        private CandleProvider provider;

        public CandleCache(CandleProvider provider)
        {
            this.provider = provider;
        }

        public Candle[] GetCandles(TradingSymbol symbol, DateTime from, DateTime to, TimeSpan span)
        {
            if (!registries.TryGetValue(Tuple.Create(symbol, span), out var candleRegistries))
            {
                candleRegistries = new List<CandleCachRegistry>();
                registries.Add(Tuple.Create(symbol, span), candleRegistries);
            }

            var candleRegistry = candleRegistries.Where(r => r.Start <= from && r.End >= to).FirstOrDefault();
            if (candleRegistry != null)
            {
                return caches[candleRegistry.Id].Where(r => r.Time >= from && r.Time <= to).ToArray();
            }

            var candles = provider.GetCandles(symbol, from, to, span);
            var newRegistry = CandleCachRegistry.New(from, to);
            candleRegistries.Add(newRegistry);
            caches[newRegistry.Id] = candles.ToList();

            return candles;
        }
    }
}
