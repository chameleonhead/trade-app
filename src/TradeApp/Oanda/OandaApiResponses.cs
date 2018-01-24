using System;
using System.Collections.Generic;

namespace TradeApp.Oanda
{
    public class AccountsResponse
    {
        public Account[] Accounts { get; set; }
    }

    public class InstrumentsResponse
    {
        public InstrumentInfo[] Instruments { get; set; }
    }

    public class PricesResponse
    {
        public Price[] Prices { get; set; }
    }

    public class CandleResponse<T>
    {
        public string Instrument { get; set; }
        public string Granularity { get; set; }
        public T[] Candles { get; set; }
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
    public class CandleAskBid
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
        public bool Complete { get; set; }
    }

}