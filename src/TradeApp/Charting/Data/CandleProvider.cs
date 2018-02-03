using System;
using System.Collections.Generic;

namespace TradeApp.Charting.Data
{
    public abstract class CandleProvider
    {
        public abstract Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime from, DateTime to);
    }

    public static class CandleProviderFactory
    {
        private static Dictionary<TradingSymbol, Func<CandleProvider>> factories = new Dictionary<TradingSymbol, Func<CandleProvider>>();

        public static void Register(TradingSymbol symbol, Func<CandleProvider> factory)
        {
            factories.Add(symbol, factory);
        }

        public static void Unregister(TradingSymbol symbol)
        {
            factories.Remove(symbol);
        }

        public static CandleProvider GetInstance(TradingSymbol symbol)
        {
            if (factories.TryGetValue(symbol, out var factory))
            {
                return factory.Invoke();
            }

            throw new ArgumentException($"provider for {symbol.Symbol} is not registered.");
        }
    }
}
