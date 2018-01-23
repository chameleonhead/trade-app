using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            return await GetResponse<AccountDetail>($"/v1/account/{accountId}");
        }

        public async Task<List<Price>> GetPrices(params string[] instruments)
        {
            if (instruments.Length > 0)
            {
                return (await GetResponse<PricesResponse>($"/v1/prices?instruments={Uri.EscapeDataString(String.Join(",", instruments))}")).Prices;
            }
            throw new ArgumentException($"{nameof(instruments)}は必ず指定してください");
        }

        public async Task<List<InstrumentInfo>> GetInstruments(params string[] instruments)
        {
            var fields = string.Join(",", new[] { "instrument", "displayName", "pip", "maxTradeUnits", "precision", "maxTrailingStop", "minTrailingStop", "marginRate", "halted" });
            if(instruments.Length > 0)
            {
                return (await GetResponse<InstrumentsResponse>($"/v1/instruments?accountId={accountId}&fields={Uri.EscapeDataString(fields)}&instruments={Uri.EscapeDataString(String.Join(",", instruments))}")).Instruments;
            }
            return (await GetResponse<InstrumentsResponse>($"/v1/instruments?accountId={accountId}&fields={Uri.EscapeDataString(fields)}")).Instruments;
        }

        private async Task<T> GetResponse<T>(string requestUri)
        {
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var errorInfo = await ConvertFromResponse<ErrorInfo>(response);
                if (errorInfo != null)
                {
                    throw OandaApiException.FromErrorInfo(errorInfo);
                }
                throw new HttpListenerException((int)response.StatusCode);
            }
            return await ConvertFromResponse<T>(response);
        }

        private async Task<T> ConvertFromResponse<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
