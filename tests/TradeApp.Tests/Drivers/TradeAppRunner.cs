using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TradeApp.Drivers
{
    class TradeAppRunner
    {
        private string accessToken;
        private Uri oandaServerBaseUri;
        private string shutdownFilePath;

        public TradeAppRunner(OandaFakeServerDriver oandaServer, string accessToken)
        {
            this.accessToken = accessToken;
            this.oandaServerBaseUri = oandaServer.BaseUri;
            this.shutdownFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        public void Start()
        {
            Task.Run(() =>
                Program.Main(new[] {
                    "--server", oandaServerBaseUri.ToString(),
                    "--token", accessToken,
                    "--gracefulshutdownfile", shutdownFilePath,
                }));
        }

        public void Stop()
        {
            File.WriteAllText(shutdownFilePath, null);
        }

        public void WriteEventForStartup()
        {
        }

        public void WriteEventForAccountBalanceUpdated()
        {
            while (!File.Exists("log.txt"))
            {
            }
            Assert.IsTrue(File.ReadAllLines("log.txt").Any(t => t.Contains("Accounts")));
        }
    }
}