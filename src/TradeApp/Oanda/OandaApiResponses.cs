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
}