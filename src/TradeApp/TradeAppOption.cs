using CommandLine;
using System;

namespace TradeApp
{
    public class TradeAppOption
    {
        [Option("server", HelpText = "OANDAサーバーのURLを指定します", Required = true)]
        public Uri OandaServerBaseUri { get; set; }
        [Option("token", HelpText = "OANDAサーバーのアクセストークンを指定します", Required = true)]
        public string OandaAccessToken { get; set; }
        [Option("accountId", HelpText = "OANDAのアカウントを指定します", Required = true)]
        public int OandaAccountId { get; set; }
        [Option("gracefulshutdownfile", HelpText = "シャットダウン用の監視ファイルを指定します", Required = true)]
        public string ShutdownWatchFile { get; set; }
    }
}