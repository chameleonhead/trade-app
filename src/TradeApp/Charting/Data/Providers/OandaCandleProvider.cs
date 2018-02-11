using System;
using System.Linq;
using TradeApp.Oanda;

namespace TradeApp.Charting.Data.Providers
{
    public class OandaCandleProvider : ICandleProvider
    {
        private OandaApi apiEndpoint;

        public OandaCandleProvider(OandaApi apiEndpoint)
        {
            this.apiEndpoint = apiEndpoint;
        }

        public Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime from, DateTime to)
        {
            var granularity = ConvertGranurality(range);
            var task = apiEndpoint.GetBidAskCandles(symbol.Symbol, granularity: granularity, start: from, end: to, includeFirst: true);
            return task.Result.Select(oandaCandle => new Candle(oandaCandle.Time, oandaCandle.OpenAsk, oandaCandle.HighAsk, oandaCandle.LowAsk, oandaCandle.CloseAsk, oandaCandle.Volume)).ToArray();
        }

        public Candle[] GetCandles(TradingSymbol symbol, ChartRange range, DateTime to, int count)
        {
            var granularity = ConvertGranurality(range);
            var task = apiEndpoint.GetBidAskCandles(symbol.Symbol, granularity: granularity, count: count, end: to, includeFirst: true);
            return task.Result.Select(oandaCandle => new Candle(oandaCandle.Time, oandaCandle.OpenAsk, oandaCandle.HighAsk, oandaCandle.LowAsk, oandaCandle.CloseAsk, oandaCandle.Volume)).ToArray();
        }

        private Granularity ConvertGranurality(ChartRange range)
        {
            return (Granularity)(int)range;
        }
    }
}
