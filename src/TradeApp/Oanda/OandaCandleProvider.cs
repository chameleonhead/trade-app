using System;
using System.Collections.Generic;
using System.Linq;
using TradeApp.Charting;

namespace TradeApp.Oanda
{
    public class OandaCandleProvider : CandleProvider
    {
        private OandaApi apiEndpoint;
        private Dictionary<TradingSymbol, IEnumerable<Candle>> dictionay = new Dictionary<TradingSymbol, IEnumerable<Candle>>();

        public OandaCandleProvider(string server, string token)
        {
            this.apiEndpoint = new OandaApi(new Uri(server), token);
        }

        public override Candle[] GetCandles(TradingSymbol symbol, DateTime from, DateTime to, TimeSpan span)
        {
            var granularity = ConvertGranurality(span);
            var task = apiEndpoint.GetBidAskCandles(symbol.Symbol, start: from, end: to, includeFirst: true);
            return task.Result.Select(oandaCandle => new Candle(oandaCandle.Time, oandaCandle.OpenAsk, oandaCandle.HighAsk, oandaCandle.LowAsk, oandaCandle.CloseAsk, oandaCandle.Volume)).ToArray();
        }

        private Granularity ConvertGranurality(TimeSpan span)
        {
            return (Granularity)(int)span.TotalSeconds;
        }
    }
}
