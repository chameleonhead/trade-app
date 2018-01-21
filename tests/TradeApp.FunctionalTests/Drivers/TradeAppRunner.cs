using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TradeApp.Drivers
{
    class TradeAppRunner
    {
        private string accessToken;
        private Uri oandaServerBaseUri;
        private string shutdownFilePath;
        private int accountId;
        private Task mainTask;

        public TradeAppRunner(OandaFakeServerDriver oandaServer, string accessToken, int accountId)
        {
            this.accessToken = accessToken;
            this.oandaServerBaseUri = oandaServer.BaseUri;
            this.shutdownFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            this.accountId = accountId;
        }

        public void Start()
        {
            mainTask = Task.Run(() =>
                Program.Main(new[] {
                    "--server", oandaServerBaseUri.ToString(),
                    "--token", accessToken,
                    "--accountId", accountId.ToString(),
                    "--gracefulshutdownfile", shutdownFilePath,
                }));
        }

        public void ApplicationEnded()
        {
            mainTask.Wait(TimeSpan.FromSeconds(5));
            Assert.IsTrue(mainTask.IsCompleted);
        }

        public void Stop()
        {
            File.WriteAllText(shutdownFilePath, null);
        }
    }
}