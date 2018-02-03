using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradeApp.Charting.Data;

namespace TradeApp.Charting
{
    [TestClass]
    public class CandleChartManagerTest
    {
        [TestMethod]
        public void チャートが登録されている場合今保持しているチャートのデータを取りに行く()
        {
            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Daily;
            var chartManager = new CandleChartManager(DateTime.Now, (s, r) => new MockCandleProvider());
            chartManager.GetChart(symbol, range);
            chartManager.Update(DateTime.Now);
        }
    }
}
