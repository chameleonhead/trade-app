using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private class OrdersResponse
        {
            public Order[] Orders { get; set; }
        }

        private class OrderSideJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(OrderSide);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "buy":
                        return OrderSide.Buy;
                    case "sell":
                        return OrderSide.Sell;
                    default:
                        throw new InvalidCastException($"can't convert json to OrderSide: {value}");
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        private class OrderTypeJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(OrderType);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "limit":
                        return OrderType.Limit;
                    case "stop":
                        return OrderType.Stop;
                    case "marketIfTouched":
                        return OrderType.MarketIfTouched;
                    case "market":
                        return OrderType.Market;
                    default:
                        throw new InvalidCastException($"can't convert json to OrderType: {value}");
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        HttpClient client;
        int? accountId;
        private JsonConverter[] converters;

        public OandaApi(Uri baseUri, string accessToken) : this(CreateClient(baseUri, accessToken), null)
        {
        }

        public OandaApi(Uri baseUri, string accessToken, int accountId) : this(CreateClient(baseUri, accessToken), accountId)
        {
        }

        public OandaApi(HttpClient client, int? accountId)
        {
            this.client = client;
            this.accountId = accountId;
            this.converters = new JsonConverter[]
            {
                new OrderSideJsonConverter(),
            };
        }

        private static HttpClient CreateClient(Uri baseUri, string accessToken)
        {
            var client = new HttpClient();
            client.BaseAddress = baseUri;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }

        public async Task<Price[]> GetPrices(params string[] instruments)
        {
            if (instruments.Length > 0)
            {
                return (await GetResponse<PricesResponse>($"/v1/prices?instruments={Uri.EscapeDataString(String.Join(",", instruments))}")).Prices;
            }
            throw new ArgumentException($"{nameof(instruments)}は必ず指定してください");
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

        public async Task<InstrumentInfo[]> GetInstruments(params string[] instruments)
        {
            var accountId = RequireAccountId();
            var fields = string.Join(",", new[] { "instrument", "displayName", "pip", "maxTradeUnits", "precision", "maxTrailingStop", "minTrailingStop", "marginRate", "halted" });
            if (instruments.Length > 0)
            {
                return (await GetResponse<InstrumentsResponse>($"/v1/instruments?accountId={accountId}&fields={Uri.EscapeDataString(fields)}&instruments={Uri.EscapeDataString(String.Join(",", instruments))}")).Instruments;
            }
            return (await GetResponse<InstrumentsResponse>($"/v1/instruments?accountId={accountId}&fields={Uri.EscapeDataString(fields)}")).Instruments;
        }

        public async Task<AccountDetail> GetAccount()
        {
            var accountId = RequireAccountId();
            return await GetResponse<AccountDetail>($"/v1/accounts/{accountId}");
        }

        public async Task<Order> GetOrder(int id)
        {
            var accountId = RequireAccountId();
            return (await GetResponse<OrdersResponse>($"/v1/accounts/{accountId}/orders?ids={id}")).Orders.FirstOrDefault();
        }

        public async Task<Order[]> GetOrders(params int[] ids)
        {
            var accountId = RequireAccountId();
            if (ids.Length == 0)
            {
                throw new InvalidOperationException("must specify one ore more ids.");
            }
            return (await GetResponse<OrdersResponse>($"/v1/accounts/{accountId}/orders?ids={Uri.EscapeDataString(string.Join(",", ids))}")).Orders;
        }

        public async Task<Order[]> GetOrders(
            int? maxId = null,
            int count = 50,
            string instrument = null)
        {
            var accountId = RequireAccountId();
            if (count > 500)
            {
                throw new ArgumentException("count can't be more than 500.");
            }

            var query = new StringBuilder();
            if (maxId != null)
                query.Append($"maxId={maxId.Value.ToString()}");
            if (count != 50)
                query.Append(query.Length > 0 ? "&" : "").Append($"count={count}");
            if (!string.IsNullOrEmpty(instrument))
                query.Append(query.Length > 0 ? "&" : "").Append($"instrument={instrument}");

            return (await GetResponse<OrdersResponse>($"/v1/accounts/{accountId}/orders?{query.ToString()}")).Orders;
        }

        public async Task<CreatedOrder> PostOrder(
           string instrument,
           int units,
           OrderSide side,
           OrderType type,
           DateTime expiry,
           decimal price,
           decimal? lowerBound = null,
           decimal? upperBound = null,
           decimal? stopLoss = null,
           decimal? takeProfit = null,
           decimal? trailingStop = null)
        {
            if (type == OrderType.Market)
                throw new ArgumentException($"order type must not be market. try using another method: PostMarketOrder");
            var accountId = RequireAccountId();
            var param = new Dictionary<string, string>();
            param.Add("instrument", instrument);
            param.Add("units", units.ToString());
            param.Add("side", OrderSideToRequestRepresentation(side));
            param.Add("type", OrderTypeToRequestRepresentation(type));
            param.Add("expiry", XmlConvert.ToString(expiry, XmlDateTimeSerializationMode.Utc));
            param.Add("price", price.ToString());
            if (lowerBound != null)
                param.Add("lowerBound", lowerBound.ToString());
            if (upperBound != null)
                param.Add("upperBound", upperBound.ToString());
            if (stopLoss != null)
                param.Add("stopLoss", stopLoss.ToString());
            if (takeProfit != null)
                param.Add("takeProfit", takeProfit.ToString());
            if (trailingStop != null)
                param.Add("trailingStop", trailingStop.ToString());
            return (await PostResponse<CreatedOrder>($"/v1/accounts/{accountId}/orders", param));
        }

        public async Task<CreatedMarketOrder> PostMarketOrder(
           string instrument,
           int units,
           OrderSide side,
           decimal? lowerBound = null,
           decimal? upperBound = null,
           decimal? stopLoss = null,
           decimal? takeProfit = null,
           decimal? trailingStop = null)
        {
            var accountId = RequireAccountId();
            var param = new Dictionary<string, string>();
            param.Add("instrument", instrument);
            param.Add("units", units.ToString());
            param.Add("side", OrderSideToRequestRepresentation(side));
            param.Add("type", OrderTypeToRequestRepresentation(OrderType.Market));
            if (lowerBound != null)
                param.Add("lowerBound", lowerBound.ToString());
            if (upperBound != null)
                param.Add("upperBound", upperBound.ToString());
            if (stopLoss != null)
                param.Add("stopLoss", stopLoss.ToString());
            if (takeProfit != null)
                param.Add("takeProfit", takeProfit.ToString());
            if (trailingStop != null)
                param.Add("trailingStop", trailingStop.ToString());
            return (await PostResponse<CreatedMarketOrder>($"/v1/accounts/{accountId}/orders", param));
        }

        private static string OrderSideToRequestRepresentation(OrderSide side)
        {
            switch (side)
            {
                case OrderSide.Buy:
                    return "buy";
                case OrderSide.Sell:
                    return "sell";
                default:
                    throw new ArgumentException($"unknown order side: {side}");
            }
        }

        private static string OrderTypeToRequestRepresentation(OrderType type)
        {
            switch (type)
            {
                case OrderType.Limit:
                    return "limit";
                case OrderType.Stop:
                    return "stop";
                case OrderType.MarketIfTouched:
                    return "marketIfTouched";
                case OrderType.Market:
                    return "market";
                default:
                    throw new ArgumentException($"unknown order type: {type}");
            }
        }

        private int RequireAccountId()
        {
            return accountId ?? throw new InvalidOperationException("Must specify accountId with this operation");
        }

        private async Task<T> GetResponse<T>(string requestUri)
        {
            Debug.WriteLine(requestUri);
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                Trace.TraceError(response.Content.ReadAsStringAsync().Result);
                var errorInfo = await ConvertFromResponse<ErrorInfo>(response);
                if (errorInfo != null)
                {
                    throw OandaApiException.FromErrorInfo(errorInfo);
                }
                response.EnsureSuccessStatusCode();
            }
            return await ConvertFromResponse<T>(response);
        }

        private async Task<T> PostResponse<T>(string requestUri, IEnumerable<KeyValuePair<string, string>> namedValueCollection)
        {
            Debug.WriteLine(requestUri);
            HttpContent content = new FormUrlEncodedContent(namedValueCollection);
            var response = await client.PostAsync(requestUri, content).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                Trace.TraceError(response.Content.ReadAsStringAsync().Result);
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
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync(), this.converters);
        }
    }
}
