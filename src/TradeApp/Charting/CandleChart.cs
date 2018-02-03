using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeApp.Charting
{
    public class CandleChartSnapshot
    {
        private Dictionary<string, IPlotType[]> plots = new Dictionary<string, IPlotType[]>();

        public CandleChartSnapshot(Candle[] candles, Dictionary<string, IPlotType[]> plots)
        {
            this.Candles = candles;
            this.plots = plots;
        }

        public Candle[] Candles { get; }
        public T[] Plot<T>(string name)
            where T : IPlotType
        {
            if (plots.TryGetValue(name, out var value))
            {
                return value.Cast<T>().ToArray();
            }

            throw new InvalidOperationException();
        }
    }

    public class CandleChart
    {
        private interface IIndicatorPlot
        {
            void AddCandle(Candle candle);
            IPlotType[] Plot { get; }
        }

        private class IndicatorPlot : IIndicatorPlot
        {
            private Queue<IPlotType> plots = new Queue<IPlotType>();

            public IndicatorPlot(IChartIndicator<IPlotType> indicator)
            {
                Indicator = indicator;
                Plot = plots.ToArray();
            }

            public IChartIndicator<IPlotType> Indicator { get; }
            public IPlotType[] Plot { get; private set; }
            public void AddCandle(Candle candle)
            {
                var nextVal = Indicator.Next(candle);
                if (nextVal != null)
                {
                    plots.Enqueue(nextVal);
                    if (plots.Count > 100)
                    {
                        plots.Dequeue();
                    }
                    Plot = plots.ToArray();
                }
            }
        }

        private Queue<Candle> store = new Queue<Candle>();
        private Dictionary<string, IIndicatorPlot> indicators = new Dictionary<string, IIndicatorPlot>();
        private TradingSymbol symbol;
        private ChartRange range;
        private Candle latestCandle;

        public CandleChart(TradingSymbol symbol, ChartRange range) : this(symbol, range, new Candle[0])
        {
        }

        public CandleChart(TradingSymbol symbol, ChartRange range, Candle[] candles)
        {
            foreach (var candle in candles)
            {
                latestCandle = candle;
                store.Enqueue(candle);
            }
            this.symbol = symbol;
            this.range = range;
            updateSnapshot();
        }

        public void AddCandle(Candle candle)
        {
            addCandle(candle);
            updateSnapshot();
        }

        public void AddCandles(IEnumerable<Candle> candles)
        {
            foreach (var candle in candles)
            {
                AddCandle(candle);
            }
            updateSnapshot();
        }

        public void AddIndicator(string name, IChartIndicator<IPlotType> indicator)
        {
            var indicatorPlot = new IndicatorPlot(indicator);
            foreach (var candle in store.ToArray())
            {
                indicatorPlot.AddCandle(candle);
            }
            indicators.Add(name, indicatorPlot);
            updateSnapshot();
        }

        private void addCandle(Candle candle)
        {
            if (candle.Time <= latestCandle?.Time)
            {
                throw new InvalidOperationException($"{nameof(candle)} should be after last inserted candle.");
            }

            latestCandle = candle;
            store.Enqueue(candle);
            if (store.Count > 100)
            {
                store.Dequeue();
            }

            foreach (var indicatorPlot in indicators)
            {
                indicatorPlot.Value.AddCandle(candle);
            }
        }

        private void updateSnapshot()
        {
            var candles = store.ToArray();
            var plots = new Dictionary<string, IPlotType[]>();

            foreach (var indicatorPlot in indicators)
            {
                plots.Add(indicatorPlot.Key, indicatorPlot.Value.Plot);
            }

            Snapshot = new CandleChartSnapshot(candles, plots);
        }

        public CandleChartSnapshot Snapshot { get; private set; }
    }
}