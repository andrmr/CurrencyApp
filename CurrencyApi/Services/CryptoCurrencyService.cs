using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CurrencyApi.Services
{
    public interface ICryptoCurrencyService : ICurrencyService { }

    public sealed class CryptoCurrencyService : CurrencyServiceBase, ICryptoCurrencyService
    {
        public CryptoCurrencyService(IConfiguration configuration, IMemoryCache cache) : base (configuration, cache, "crypto") { }

        protected override async Task<CurrencySet?> GetCurrencySetAsync(ReferenceCurrency referenceCurrency, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();

            if (!_configuration.TryGetValue("AppSettings:ApiKey:Nomics", out string nomicsApiKey))
            {
                throw new ApplicationException("API key is not available.");
            }

            var referenceCurrencyRoute = referenceCurrency.ToString().ToUpper();
            var request = $"https://api.nomics.com/v1/currencies/ticker?key={nomicsApiKey}&interval=1h,1d,7d,30d&convert={referenceCurrencyRoute}&per-page=100&page=1";
            var response = await httpClient.GetAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // err
                //throw new HttpRequestException($"Request did not receive successful response. Status code: {response.StatusCode}. Reason: {response.ReasonPhrase}");
                return null;
            }

            var ratesJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var ratesDocument = JsonDocument.Parse(ratesJson);
            var ratesList = ratesDocument.RootElement.Deserialize<List<NomicsCryptoCurrency>>();

            if (ratesList is null || ratesList.Count == 0)
            {
                // err
                return null;
            }

            var currencySet = new CurrencySet();
            foreach (var c in ratesList)
            {
                currencySet[c.Symbol] = (Currency) c;
            }

            return currencySet;
        }

        private record NomicsCryptoCurrency
        {
            [JsonPropertyName("id")]
            public string Symbol { get; init; } = default!;

            [JsonPropertyName("name")]
            public string Name { get; init; } = default!;

            [JsonPropertyName("price")]
            public string Price { get; init; } = default!;

            [JsonIgnore, JsonPropertyName("1h")]
            public PriceChange? PriceChangeOneHour { get; init; } = default;

            [JsonPropertyName("1d")]
            public PriceChange? PriceChangeOneDay { get; init; } = default;

            [JsonPropertyName("7d")]
            public PriceChange? PriceChangeSevenDays { get; init; } = default;

            [JsonPropertyName("30d")]
            public PriceChange? PriceChangeThirtyDays { get; init; } = default;

            public record PriceChange
            {
                [JsonIgnore, JsonPropertyName("price_change")]
                public string Absolute { get; init; } = default!;

                [JsonPropertyName("price_change_pct")]
                public string Percentage { get; init; } = default!;
            }

            public static explicit operator Currency(NomicsCryptoCurrency c)
            {
                var numberFormatInfo = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = "." };

                return new Currency
                {
                    Symbol = c.Symbol,
                    Name = c.Name,
                    Price = Convert.ToDecimal(c.Price, numberFormatInfo),
                    PriceChangeOneDay = Convert.ToDouble(c.PriceChangeOneDay?.Percentage, numberFormatInfo),
                    PriceChangeSevenDays = Convert.ToDouble(c.PriceChangeOneDay?.Percentage, numberFormatInfo),
                    PriceChangeThirtyDays = Convert.ToDouble(c.PriceChangeThirtyDays?.Percentage, numberFormatInfo)
                };
            }
        }
    }
}