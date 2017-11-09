using System;
using System.IO;

namespace TradeApp
{
    class TradeAppRunner
    {
        private string accessToken;
        private Uri oandaServerBaseUri;
        private string shutdownFilePath;

        public TradeAppRunner(FakeOandaServer oandaServer, string accessToken)
        {
            this.accessToken = accessToken;
            this.oandaServerBaseUri = oandaServer.BaseUri;
            this.shutdownFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        public void Start()
        {
            var config = new TradeAppConfig()
            {
                OandaServerBaseUri = oandaServerBaseUri,
                OandaAccessToken = accessToken,
                ShutdownWatchFile = shutdownFilePath,
            };
            Program.Run(config);
        }

        public void Stop()
        {
            File.WriteAllText(shutdownFilePath, "");
        }

        public void WriteLogForAccountBalance()
        {
            throw new NotImplementedException();
        }
    }
}