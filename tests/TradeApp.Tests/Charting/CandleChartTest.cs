using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TradeApp.Charting.Indicators;

namespace TradeApp.Charting
{
    [TestClass]
    public class CandleChartTest
    {
        [TestMethod]
        public void チャートにキャンドルを読み込むとストアに登録される()
        {
            var date = DateTime.Now;
            var symbol = new TradingSymbol("USD_JPY");
            var store = new MockCandleStore();
            var chart = new CandleChart(symbol, ChartRange.Hourly, store);
            chart.AddCandle(new Candle(date, 0, 48.70m, 47.79m, 48.16m, 0));
            Assert.AreEqual(date, store.Candles.First().Time);
        }

        [TestMethod]
        public void チャートにATRを設定しキャンドルを読み込むと自動的にATRが計算される()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var chart = new CandleChart(symbol, ChartRange.Hourly, new MockCandleStore());
            chart.AddIndicator("ATR14", new AtrIndicator(14));
            chart.AddCandles(ChartingSeedData.ATR_CANDLES.Item1);
            var values = chart.Plot<SingleValue>("ATR14");
            var i = 0;
            foreach (var val in ChartingSeedData.ATR_CANDLES.Item2)
            {
                if (val != null)
                {
                    var plot = values[i++];
                    Assert.AreEqual(val.Time, plot.Time);
                    Assert.AreEqual(val.Value, Math.Round(plot.Value, 4));
                }
            }
        }
    }
}
