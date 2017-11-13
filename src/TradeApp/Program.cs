using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TradeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static async Task RunAsync(TradeAppConfig config)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.OandaAccessToken);
            var response = await client.GetAsync(new Uri(config.OandaServerBaseUri, "/v1/accounts")).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var file = File.AppendText("log.txt"))
                {
                    file.WriteLine("Accounts:");
                }
            }
        }
    }
}
