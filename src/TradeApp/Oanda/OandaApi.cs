using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TradeApp.Oanda
{
    public class OandaApi
    {
        HttpClient client;
        int accountId;

        public OandaApi(Uri baseUri, string accessToken, int accountId) : this(CreateClient(baseUri, accessToken), accountId)
        {
        }

        public OandaApi(HttpClient client, int accountId)
        {
            this.client = client;
            this.accountId = accountId;
        }

        private static HttpClient CreateClient(Uri baseUri, string accessToken)
        {
            var client = new HttpClient();
            client.BaseAddress = baseUri;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }

        public async Task<AccountDetail> GetAccount()
        {
            var response = await client.GetAsync($"/v1/account/{accountId}").ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorInfo = await ConvertFromResponse<ErrorInfo>(response);
                throw OandaApiException.FromErrorInfo(errorInfo);
            }
            return await ConvertFromResponse<AccountDetail>(response);
        }

        private async Task<T> ConvertFromResponse<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
