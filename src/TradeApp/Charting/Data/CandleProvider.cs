using System;
using System.Collections.Generic;

namespace TradeApp.Charting.Data
{
    public interface ICandleProvider
    {
        Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime to, int count);
        Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime from, DateTime to);
    }

    public static class CandleProviderFactory
    {
        private static Dictionary<TradingSymbol, Func<ICandleProvider>> factories = new Dictionary<TradingSymbol, Func<ICandleProvider>>();

        public static void Register(TradingSymbol symbol, Func<ICandleProvider> factory)
        {
            factories.Add(symbol, factory);
        }

        public static void Unregister(TradingSymbol symbol)
        {
            factories.Remove(symbol);
        }

        public static ICandleProvider GetInstance(TradingSymbol symbol)
        {
            if (factories.TryGetValue(symbol, out var factory))
            {
                return factory.Invoke();
            }

            throw new ArgumentException($"provider for {symbol.Symbol} is not registered.");
        }
    }
}
