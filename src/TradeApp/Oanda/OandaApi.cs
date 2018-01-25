using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TradeApp.Oanda
{
    public class OandaApi
    {
        private class AccountsResponse
        {
            public Account[] Accounts { get; set; }
        }

        private class InstrumentsResponse
        {
            public InstrumentInfo[] Instruments { get; set; }
        }

        private class PricesResponse
        {
            public Price[] Prices { get; set; }
        }

        private class CandlesResponse<T>
        {
            public string Instrument { get; set; }
            public string Granularity { get; set; }
            public T[] Candles { get; set; }
        }

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

        public async Task<Price[]> GetPrices(params string[] instruments)
        {
            if (instruments.Length > 0)
            {
                return (await GetResponse<PricesResponse>($"/v1/prices?instruments={Uri.EscapeDataString(String.Join(",", instruments))}")).Prices;
            }
            throw new ArgumentException($"{nameof(instruments)}は必ず指定してください");
        }

        public async Task<InstrumentInfo[]> GetInstruments(params string[] instruments)
        {
            var fields = string.Join(",", new[] { "instrument", "displayName", "pip", "maxTradeUnits", "precision", "maxTrailingStop", "minTrailingStop", "marginRate", "halted" });
            if (instruments.Length > 0)
            {
                return (await GetResponse<InstrumentsResponse>($"/v1/instruments?accountId={accountId}&fields={Uri.EscapeDataString(fields)}&instruments={Uri.EscapeDataString(String.Join(",", instruments))}")).Instruments;
            }
            return (await GetResponse<InstrumentsResponse>($"/v1/instruments?accountId={accountId}&fields={Uri.EscapeDataString(fields)}")).Instruments;
        }

        public async Task<BidAskCandle[]> GetBidAskCandles(
            string instrument,
            Granularity granularity = Granularity.S5,
            int count = 500,
            DateTime? start = null,
            DateTime? end = null,
            bool includeFirst = true,
            int dailyAlignment = 0,
            string alignmentTimezone = "America/New_York",
            string weeklyAlignment = "Friday")
        {
            return await GetCandlesImpl<BidAskCandle>(instrument, granularity, count, start, end, includeFirst, dailyAlignment, alignmentTimezone, weeklyAlignment);
        }

        public async Task<MidpointCandle[]> GetMidpointCandles(
            string instrument,
            Granularity granularity = Granularity.S5,
            int count = 500,
            DateTime? start = null,
            DateTime? end = null,
            bool includeFirst = true,
            int dailyAlignment = 0,
            string alignmentTimezone = "America/New_York",
            string weeklyAlignment = "Friday")
        {
            return await GetCandlesImpl<MidpointCandle>(instrument, granularity, count, start, end, includeFirst, dailyAlignment, alignmentTimezone, weeklyAlignment);
        }

        private async Task<T[]> GetCandlesImpl<T>(
            string instrument,
            Granularity granularity,
            int count,
            DateTime? start,
            DateTime? end,
            bool includeFirst,
            int dailyAlignment,
            string alignmentTimezone,
            string weeklyAlignment)
        {
            var query = new StringBuilder();
            query.Append($"/v1/candles?instrument={instrument}");
            if (typeof(T) == typeof(BidAskCandle))
                query.Append("&candleFormat=bidask");
            if (typeof(T) == typeof(MidpointCandle))
                query.Append("&candleFormat=midpoint");

            if (granularity != Granularity.S5)
                query.Append($"&granularity={granularity.ToString()}");
            if (count != 500)
                query.Append($"&count={count}");
            if (start != null)
                query.Append($"&start={Uri.EscapeUriString(XmlConvert.ToString(start.Value, XmlDateTimeSerializationMode.Utc))}");
            if (end != null)
                query.Append($"&end={Uri.EscapeUriString(XmlConvert.ToString(end.Value, XmlDateTimeSerializationMode.Utc))}");
            if (!includeFirst)
                query.Append($"&includeFirst={includeFirst}");
            if (dailyAlignment != 0)
                query.Append($"&dailyAlignment={dailyAlignment}");
            if (alignmentTimezone != "America/New_York")
                query.Append($"&alignmentTimezone={Uri.EscapeUriString(alignmentTimezone)}");
            if (weeklyAlignment != "Friday")
                query.Append($"&weeklyAlignment={weeklyAlignment}");

            return (await GetResponse<CandlesResponse<T>>(query.ToString())).Candles;
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
                response.EnsureSuccessStatusCode();
            }
            return await ConvertFromResponse<T>(response);
        }

        private async Task<T> ConvertFromResponse<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
