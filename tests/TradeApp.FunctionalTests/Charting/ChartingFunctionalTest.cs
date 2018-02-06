using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradeApp.Charting.Data;
using TradeApp.Charting.Data.Providers;
using TradeApp.Charting.Indicators;
using TradeApp.FakeOandaSrver;
using TradeApp.Oanda;

namespace TradeApp.Charting
{
    [TestClass]
    public class ChartingFunctionalTest
    {
        private FakeOandaWebHost _server;

        [TestInitialize]
        public void Setup()
        {
            _server = new FakeOandaWebHost();
            _server.Start();

            using (var context = new CandleChartStore())
            {
                context.Database.EnsureDeleted();
                CandleStoreInitializer.Initialize(context);
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            _server.StopAsync().Wait();
            _server.Dispose();

            using (var context = new CandleChartStore())
            {
                context.Database.EnsureDeleted();
            }
        }

        [TestMethod]
        public void OANDAよりデータを取得してチャートが更新されることを確認する()
        {
            // 前提条件:
            // トレードをするクラスはチャートが常に勝手に最新に更新されていること
            // を想定して同期的にアクセス可能
            // データを取得する単位はシンボル・チャートの足単位とする
            // (TODO: 最適化できれば複数のシンボル・チャート足を同時に取得したいけど、
            // OANDA的に難しそうなのでとりあえず...分足から日足を生成するとか
            // -> ちなみに↑の最適化したい理由はリクエストが発生すると遅いから)
            // 
            // 1. 更新スケジュールを作り、タイミングでトリガーを発生
            // 2. トリガーでプロバイダよりデータを取得する
            // 3. プロバイダがデータを取得出来たらデータストアを更新
            // 4. データストアはチャートマネージャに通知を送信
            // 5. チャートマネージャは送られてきたデータによってチャートを更新する
            // 6. スレッドセーフになるように頑張る

            var symbol = new TradingSymbol("USD_JPY");
            var range = ChartRange.Daily;

            var from = new DateTime(2017, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2017, 12, 1, 0, 2, 0, DateTimeKind.Utc);

            var apiEndpoint = new OandaApi(_server.BaseUri, _server.DefaultAccessToken);
            var provider = new OandaCandleProvider(apiEndpoint) as CandleProvider;

            var chartManager = new CandleChartManager(from, (s, r) => provider);

            var chart = chartManager.GetChart(symbol, range);
            // この時点で100件分のチャートが取得済み
            chart.AddIndicator("SMA10", new SmaIndicator(10));
            var snapshot1 = chart.Snapshot;
            Assert.AreEqual(100, snapshot1.Candles.Length);
            Assert.AreEqual(100, snapshot1.Plot<SingleValue>("SMA10").Length);

            // これでチャートが更新される
            chartManager.Update(to);
            var snapshot2 = chart.Snapshot;
            Assert.AreEqual(100, snapshot2.Candles.Length);
            Assert.AreEqual(100, snapshot2.Plot<SingleValue>("SMA10").Length);

            // SNAPSHOT1とSNAPSHOT2のインスタンスが異なることを確認
            Assert.AreNotSame(snapshot1, snapshot2);
        }
    }
}
