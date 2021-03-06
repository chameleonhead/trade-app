﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var chart = new CandleChart(symbol, ChartRange.Hourly);
            chart.AddCandle(new Candle(date, 0, 48.70m, 47.79m, 48.16m, 0));

            var snapshot = chart.Snapshot;
            Assert.AreEqual(date, snapshot.Candles.First().Time);
        }

        [TestMethod]
        public void チャートにATRを設定しキャンドルを読み込むと自動的にATRが計算される()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var chart = new CandleChart(symbol, ChartRange.Hourly);
            chart.AddIndicator("ATR14", new AtrIndicator(14));
            chart.AddCandles(Seeds.ATR14_CANDLES.Item1);

            var snapshot = chart.Snapshot;
            var values = snapshot.Plot<SingleValue>("ATR14");
            var i = 0;
            foreach (var val in Seeds.ATR14_CANDLES.Item2)
            {
                if (val != null)
                {
                    var plot = values[i++];
                    Assert.AreEqual(val.Time, plot.Time);
                    Assert.AreEqual(val.Value, Math.Round(plot.Value, 4));
                }
            }
        }

        [TestMethod]
        public void データ登録済みのストアを指定してチャートを生成しインジケータを追加しインジケータが計算済みであること()
        {
            var date = DateTime.Now;
            var symbol = new TradingSymbol("USD_JPY");

            var chart = new CandleChart(symbol, ChartRange.Hourly, Seeds.ATR14_CANDLES.Item1);
            chart.AddIndicator("ATR14", new AtrIndicator(14));

            var snapshot = chart.Snapshot;
            CollectionAssert.AreEqual(
                Seeds.ATR14_CANDLES.Item2.Where(sv => sv != null).Select(sv => sv.Value).ToArray(),
                snapshot.Plot<SingleValue>("ATR14").Select(sv => Math.Round(sv.Value, 4)).ToArray()
            );
        }

        [TestMethod]
        public void チャートに保持するインジケータの計算結果が最大100件までとなる()
        {
            var date = DateTime.Now;
            var symbol = new TradingSymbol("USD_JPY");

            var chart = new CandleChart(symbol, ChartRange.Hourly);
            chart.AddIndicator("SMA5", new SmaIndicator(5));

            Enumerable.Range(1, 100).ToList()
                .ForEach(i =>
                {
                    chart.AddCandle(new Candle(date.AddDays(i), i, i, i, i, i));
                    var snapshot1 = chart.Snapshot;
                    Assert.AreEqual(i, snapshot1.Plot<SingleValue>("SMA5").Length);
                });

            chart.AddCandle(new Candle(date.AddDays(101), 101, 101, 101, 101, 101));
            var snapshot2 = chart.Snapshot;
            Assert.AreEqual(100, snapshot2.Candles.Length);
            Assert.AreEqual(100, snapshot2.Plot<SingleValue>("SMA5").Length);
        }

    }
}
