using System;

namespace TradeApp
{
    public class TradeAppConfig
    {
        public TradeAppConfig()
        {
        }

        public Uri OandaServerBaseUri { get; set; }
        public string OandaAccessToken { get; set; }
        public string ShutdownWatchFile { get; set; }
    }
}