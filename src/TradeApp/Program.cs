using CommandLine;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", option.OandaAccessToken);
                var response = await client.GetAsync(new Uri(new Uri(option.OandaServerBaseUri.ToString(), UriKind.Absolute), "/v1/accounts")).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var file = File.AppendText("log.txt"))
                    {
                        file.WriteLine("Accounts:");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return 0;
        }
    }
}
