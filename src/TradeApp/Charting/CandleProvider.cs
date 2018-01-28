using System;
using TradeApp.Oanda;

namespace TradeApp.Charting
{
    public abstract class CandleProvider
    {
        public abstract Candle[] GetCandles(TradingSymbol symbol, DateTime from, DateTime to, TimeSpan span);


        public static CandleProvider Get(string connectionString)
        {
            return OandaCandleProviderFactory.GetInstance(connectionString);
        }

        class OandaCandleProviderFactory
        {
            public static OandaCandleProvider GetInstance(string connectionString)
            {
                string server = null;
                string token = null;

                foreach (var item in connectionString.Split(";"))
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
