using CommandLine;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TradeApp.Oanda;

namespace TradeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<TradeAppOption>(args);
            result.MapResult(o => RunAsync(o).Result,
                _ => 1);
        }

        private async static Task<int> RunAsync(TradeAppOption option)
        {
            using (var file = File.AppendText("log.txt"))
            {
                file.WriteLine("Application Started.");
            }

            try
            {
                var oandaApi = new OandaApi(option.OandaServerBaseUri, option.OandaAccessToken, option.OandaAccountId);
                var account = await oandaApi.GetAccount();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return 0;
        }
    }
}
