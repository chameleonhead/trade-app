using System;
using System.Collections.Generic;
using TradeApp.Oanda;

namespace TradeApp.Charting.Data
{
    public abstract class CandleProvider
    {
        public abstract Candle[] GetCandles(TradingSymbol symbol, DateTime from, DateTime to, ChartRange range);
    }

    public static class CandleProviderFactory
    {
        private static Dictionary<string, Func<string, OandaCandleProvider>> factories = new Dictionary<string, Func<string, OandaCandleProvider>>();

        static CandleProviderFactory()
        {
            Register("OANDA", OandaCandleProviderFactory.GetInstance);
        }

        public static void Register(string providerName, Func<string, OandaCandleProvider> factory)
        {
            factories.Add(providerName, factory);
        }

        public static CandleProvider GetInstance(string connectionString)
        {
            var providerName = default(string);
            if (connectionString.IndexOf(";") < 0)
            {
                providerName = connectionString;
            }
            else
            {
                providerName = connectionString.Substring(0, connectionString.IndexOf(";"));
            }

            if (factories.TryGetValue(providerName, out var factory))
            {
                return factory.Invoke(connectionString);
            }

            throw new ArgumentException($"provider name {providerName} is not registered.");
        }

        class OandaCandleProviderFactory
        {
            public static OandaCandleProvider GetInstance(string connectionString)
            {
                string server = null;
                string token = null;

                foreach (var item in connectionString.Substring("OANDA;".Length).Split(";"))
                {
                    var propVal = item.Split("=");
                    if (propVal.Length != 2)
                    {
                        throw new ArgumentException("invalid connection string", nameof(connectionString));
                    }

                    switch (propVal[0].ToUpperInvariant())
                    {
                        case "TOKEN":
                            token = propVal[1];
                            break;
                        case "SERVER":
                            server = propVal[1];
                            break;
                        default:
                            break;
                    }
                }

                return new OandaCandleProvider(server, token);
            }
        }
    }
}
