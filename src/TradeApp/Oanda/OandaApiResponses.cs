using System;

namespace TradeApp.Oanda
{
    /// <summary>
    /// それぞれのキャンドルスティックがカバーしている時間の範囲。　指定された値が、最初のキャンドルスティックのアラインメントを決定します。　
    /// </summary>
    public enum Granularity
    {
        //１分の初めにアライン
        /// <summary>
        ///5 秒
        /// </summary>
        S5 = 5,
        /// <summary>
        ///10 秒
        /// </summary>
        S10 = 10,
        /// <summary>
        ///15 秒
        /// </summary>
        S15 = 15,
        /// <summary>
        ///30 秒
        /// </summary>
        S30 = 30,
        /// <summary>
        ///1 分
        /// </summary>
        M1 = 60,
        //1時間の初めにアライン
        /// <summary>
        ///2 分
        /// </summary>
        M2 = 2 * 60,
        /// <summary>
        ///3 分
        /// </summary>
        M3 = 3 * 60,
        /// <summary>
        ///5 分
        /// </summary>
        M5 = 5 * 60,
        /// <summary>
        ///10 分
        /// </summary>
        M10 = 10 * 60,
        /// <summary>
        ///15 分
        /// </summary>
        M15 = 15 * 60,
        /// <summary>
        ///30 分
        /// </summary>
        M30 = 30 * 60,
        /// <summary>
        ///1 時間
        /// </summary>
        H1 = 60 * 60,
        //1日の初めにアライン(17:00, 米国東部標準時)
        /// <summary>
        ///2 時間
        /// </summary>
        H2 = 2 * 60 * 60,
        /// <summary>
        ///3 時間
        /// </summary>
        H3 = 3 * 60 * 60,
        /// <summary>
        ///4 時間
        /// </summary>
        H4 = 4 * 60 * 60,
        /// <summary>
        ///6 時間
        /// </summary>
        H6 = 6 * 60 * 60,
        /// <summary>
        ///8 時間
        /// </summary>
        H8 = 8 * 60 * 60,
        /// <summary>
        ///12 時間
        /// </summary>
        H12 = 12 * 60 * 60,
        /// <summary>
        ///1 日
        /// </summary>
        D = 24 * 60 * 60,
        //1週間の初めにアライン (土曜日)
        /// <summary>
        ///1 週
        /// </summary>
        W = 7 * 24 * 60 * 60,
        //1か月の初めにアライン (その月の最初の日)
        /// <summary>
        ///1 か月
        /// </summary>
        M = -1,
    }

    /// <summary>
    /// APIエラーレスポンス
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// OANDAエラーコード。
        /// HTTPステータスコードと同じ場合と異なる場合があります
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 開発者向けのエラー詳細
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// (任意)エラーの詳細
        /// 考えられる原因とソリューションを記述したウェブページのリンク
        /// </summary>
        public string MoreInfo { get; set; }
    }

    /// <summary>
    /// ユーザーのアカウントを取得する
    /// GET /v1/accounts
    /// </summary>
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCurrency { get; set; }
        public decimal MarginRate { get; set; }
    }

    /// <summary>
    /// アカウント情報の取得
    /// GET /v1/accounts/:account_id
    /// </summary>
    public class AccountDetail
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public decimal UnrealizedPl { get; set; }
        public decimal RealizedPl { get; set; }
        public decimal MarginUsed { get; set; }
        public decimal MarginAvail { get; set; }
        public decimal OpenTrades { get; set; }
        public decimal OpenOrders { get; set; }
        public decimal MarginRate { get; set; }
        public string AccountCurrency { get; set; }
    }

    /// <summary>
    /// 銘柄リストを取得する
    /// GET /v1/instruments
    /// </summary>
    public class InstrumentInfo
    {
        /// <summary>
        /// 銘柄名
        /// </summary>
        public string Instrument { get; set; }

        /// <summary>
        /// 銘柄の表示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 当該銘柄の1pipの値
        /// </summary>
        public decimal Pip { get; set; }

        /// <summary>
        /// 当該銘柄の最大取引単位
        /// </summary>
        public int MaxTradeUnits { get; set; }

        /// <summary>
        /// 通貨ペアの小数点精度
        /// </summary>
        public double Precision { get; set; }

        /// <summary>
        /// 当該銘柄を取引する際に設定できるトレーリングストップの最大値（pips)
        /// </summary>
        public decimal MaxTrailingStop { get; set; }

        /// <summary>
        /// 当該銘柄を取引する際に設定できるトレーリングストップの最小値（pips)
        /// </summary>
        public decimal MinTrailingStop { get; set; }

        /// <summary>
        /// 銘柄の必要証拠金率
        /// 3% margin rateは0.03と提示されます。
        /// </summary>        
        public decimal MarginRate { get; set; }

        /// <summary>
        /// 銘柄の取引に関する現在の状態
        /// 取引が停止中の場合は、Trueが設定され、取引が可能な場合はfalseが設定されます。　このフィールドは　/v1/prices　エンドポイントの‘status’フィールドで受信する情報と同じです
        /// </summary>
        public bool Halted { get; set; }
    }

    /// <summary>
    /// 現在のレートを取得する
    /// GET /v1/prices
    /// </summary>
    public class Price
    {
        public string Instrument { get; set; }
        public DateTime Time { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
    }

    /// <summary>
    /// 銘柄の過去データの取得
    /// GET /v1/candles
    /// </summary>
    public class BidAskCandle
    {
        public DateTime Time { get; set; }
        public decimal OpenBid { get; set; }
        public decimal OpenAsk { get; set; }
        public decimal HighBid { get; set; }
        public decimal HighAsk { get; set; }
        public decimal LowBid { get; set; }
        public decimal LowAsk { get; set; }
        public decimal CloseBid { get; set; }
        public decimal CloseAsk { get; set; }
        public int Volume { get; set; }
        public DateTime? Complete { get; set; }
    }

    public class MidCandle
    {
        public DateTime Time { get; set; }
        public decimal OpenMid { get; set; }
        public decimal HighMid { get; set; }
        public decimal LowMid { get; set; }
        public decimal CloseMid { get; set; }
        public int Volume { get; set; }
        public bool Complete { get; set; }
    }
}