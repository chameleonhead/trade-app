using System;
using System.Collections.Generic;

namespace TradeApp.Oanda
{
    /// <summary>
    /// APIエラーレスポンス
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// OANDAエラーコード。
        /// HTTPステータスコードと同じ場合と異なる場合があります
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 開発者向けのエラー詳細
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// (任意)エラーの詳細
        /// 考えられる原因とソリューションを記述したウェブページのリンク
        /// </summary>
        public string moreInfo { get; set; }
    }

    /// <summary>
    /// ユーザーのアカウントを取得する
    /// GET /v1/accounts
    /// </summary>
    public class Accounts
    {
        public List<Account> accounts { get; set; }
    }

    public class Account
    {
        public int accountId { get; set; }
        public string accountName { get; set; }
        public string accountCurrency { get; set; }
        public decimal marginRate { get; set; }
    }

    /// <summary>
    /// アカウント情報の取得
    /// GET /v1/accounts/:account_id
    /// </summary>
    public class AccountDetail
    {
        public int accountId { get; set; }
        public string accountName { get; set; }
        public decimal balance { get; set; }
        public decimal unrealizedPl { get; set; }
        public decimal realizedPl { get; set; }
        public decimal marginUsed { get; set; }
        public decimal marginAvail { get; set; }
        public decimal openTrades { get; set; }
        public decimal openOrders { get; set; }
        public decimal marginRate { get; set; }
        public string accountCurrency { get; set; }
    }

    /// <summary>
    /// 銘柄リストを取得する
    /// GET /v1/instruments
    /// </summary>
    public class Instruments
    {
        public List<Instrument> instruments { get; set; }
    }

    public class Instrument
    {
        /// <summary>
        /// 銘柄名
        /// </summary>
        public string instrument { get; set; }

        /// <summary>
        /// 銘柄の表示名
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// 当該銘柄の1pipの値
        /// </summary>
        public decimal pip { get; set; }

        /// <summary>
        /// 当該銘柄の最大取引単位
        /// </summary>
        public int maxTradeUnits { get; set; }

        /// <summary>
        /// 通貨ペアの小数点精度
        /// </summary>
        public decimal precision { get; set; }

        /// <summary>
        /// 当該銘柄を取引する際に設定できるトレーリングストップの最大値（pips)
        /// </summary>
        public decimal maxTrailingStop { get; set; }

        /// <summary>
        /// 当該銘柄を取引する際に設定できるトレーリングストップの最小値（pips)
        /// </summary>
        public decimal minTrailingStop { get; set; }

        /// <summary>
        /// 銘柄の必要証拠金率
        /// 3% margin rateは0.03と提示されます。
        /// </summary>        
        public decimal marginRate { get; set; }

        /// <summary>
        /// 銘柄の取引に関する現在の状態
        /// 取引が停止中の場合は、Trueが設定され、取引が可能な場合はfalseが設定されます。　このフィールドは　/v1/prices　エンドポイントの‘status’フィールドで受信する情報と同じです
        /// </summary>
        public bool halted { get; set; }
    }

    /// <summary>
    /// 現在のレートを取得する
    /// GET /v1/prices
    /// </summary>
    public class Prices
    {
        public List<Price> prices { get; set; }
    }

    public class Price
    {
        public string instrument { get; set; }
        public DateTime time { get; set; }
        public decimal bid { get; set; }
        public decimal ask { get; set; }
    }

}