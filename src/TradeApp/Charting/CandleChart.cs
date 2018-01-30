using System;
using System.Collections.Generic;

namespace TradeApp.Charting
{
    public class CandleChart
    {
        private interface IIndicatorPlot
        {
            void AddCandle(Candle candle);
        }

        private class IndicatorPlot<T> : IIndicatorPlot
        {
            private List<T> plots = new List<T>();
            public IndicatorPlot(IChartIndicator<T> indicator)
            {
                Indicator = indicator;
                Plots = plots.ToArray();
            }

            public IChartIndicator<T> Indicator { get; }
            public T[] Plots { get; private set; }
            public void AddCandle(Candle candle)
            {
                var nextVal = Indicator.Next(candle);
                if (nextVal != null)
                {
                    plots.Add(nextVal);
                    Plots = plots.ToArray();
                }
            }
        }

        private Dictionary<string, IIndicatorPlot> indicators = new Dictionary<string, IIndicatorPlot>();
        private TradingSymbol symbol;
        private ChartRange range;

        private ICandleStore store;

        public CandleChart(TradingSymbol symbol, ChartRange range, ICandleStore store)
        {
            this.symbol = symbol;
            this.range = range;
            this.store = store;
            store.Initialize(symbol, range);
        }

        public void AddCandle(Candle candle)
        {
            if (candle.Time <= store.LatestCandle?.Time)
            {
                throw new InvalidOperationException($"{nameof(candle)} should be after last inserted candle.");
            }
            store.AddCandle(candle);
            foreach (var indicatorPlot in indicators.Values)
            {
                indicatorPlot.AddCandle(candle);
            }
        }

        public void AddCandles(IEnumerable<Candle> candles)
        {
            foreach (var candle in candles)
            {
                AddCandle(candle);
            }
        }

        public void AddIndicator<T>(string name, IChartIndicator<T> indicator)
        {
            indicators.Add(name, new IndicatorPlot<T>(indicator));
        }

        public T[] Plot<T>(string name)
        {
            if (indicators.TryGetValue(name, out var value))
            {
                return ((IndicatorPlot<T>)value).Plots;
            }

            throw new InvalidOperationException();
        }
    }
}