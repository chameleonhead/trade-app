using System;
using System.Collections.Generic;
using System.Linq;
using TradeApp.Charting;
using TradeApp.Charting.Providers;

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

        public override Candle[] GetCandles(TradingSymbol symbol, DateTime from, DateTime to, ChartRange range)
        {
            var granularity = ConvertGranurality(range);
            var task = apiEndpoint.GetBidAskCandles(symbol.Symbol, granularity: granularity, start: from, end: to, includeFirst: true);
            return task.Result.Select(oandaCandle => new Candle(oandaCandle.Time, oandaCandle.OpenAsk, oandaCandle.HighAsk, oandaCandle.LowAsk, oandaCandle.CloseAsk, oandaCandle.Volume)).ToArray();
        }

        private Granularity ConvertGranurality(ChartRange range)
        {
            return (Granularity)(int)range;
        }
    }
}
