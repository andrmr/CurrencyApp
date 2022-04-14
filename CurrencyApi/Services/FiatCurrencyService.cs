using System.Text.Json;

namespace CurrencyApi.Services
{
    public interface IFiatCurrencyService : ICurrencyService { }

    public sealed class FiatCurrencyService : CurrencyServiceBase, IFiatCurrencyService
    {
        private Dictionary<string, string>? _list;

        public FiatCurrencyService(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache, "fiat") { }

        protected override async Task<CurrencySet?> GetCurrencySetAsync(ReferenceCurrency referenceCurrency, CancellationToken cancellationToken)
        {
            if (!_configuration.TryGetValue("AppSettings:ApiKey:Getgeoapi", out string getgeoApiKey))
            {
                throw new ApplicationException("API key is not available.");
            }

            var listRequest = $"https://api.getgeoapi.com/v2/currency/list?api_key={getgeoApiKey}&format=json";

            var referenceCurrencyRoute = referenceCurrency.ToString().ToLower();
            var ratesLatestRequest = $"https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/{referenceCurrencyRoute}.json";
            var ratesOneDayRequest = $"https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/{DateTime.Now.AddDays(-1):yyyy-MM-dd}/currencies/{referenceCurrencyRoute}.json";
            var ratesSevenDaysRequest = $"https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/{DateTime.Now.AddDays(-7):yyyy-MM-dd}/currencies/{referenceCurrencyRoute}.json";
            var ratesThirtyDaysRequest = $"https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/{DateTime.Now.AddDays(-30):yyyy-MM-dd}/currencies/{referenceCurrencyRoute}.json";

            var httpClient = new HttpClient();
            var getAsync = async (string request) =>
            {
                var response = await httpClient.GetAsync(request);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : string.Empty;
            };

            var requests = new List<Task<string?>> {
                { getAsync(ratesLatestRequest) },
                { getAsync(ratesOneDayRequest) },
                { getAsync(ratesSevenDaysRequest) },
                { getAsync(ratesThirtyDaysRequest) },
            };

            if (_list is null)
            {
                requests.Add(getAsync(listRequest));
            }

            var responses = await Task.WhenAll(requests);
            if (responses?.Length == 0 /*|| responses.Any(r => r is null)*/)
            {
                // err
                return null;
            }

            var readRates = (string json) =>
            {
                var currencyRatesDocument = JsonDocument.Parse(json);
                return currencyRatesDocument.RootElement.GetProperty(referenceCurrencyRoute).Deserialize<OrdinalIgnoreCaseDictionary<decimal>>();
            };

            var calculatePriceChange = (decimal currentPrice, decimal previousPrice) => (double)(100 - (currentPrice / previousPrice) * 100);

            var latestRates = readRates(responses[0]);
            var oneDayRates = readRates(responses[1]);
            var sevenDaysRates = readRates(responses[2]);
            var thirtyDaysRates = readRates(responses[3]);

            if (_list is null)
            {
                _list = JsonDocument.Parse(responses[4]).RootElement.GetProperty("currencies").Deserialize<Dictionary<string, string>>();

                if (_list is null)
                {
                    // err
                    return null;
                }
            }

            var currencies = new CurrencySet();
            foreach (var (symbol, name) in _list)
            {
                if (latestRates.TryGetValue(symbol, out decimal currentPrice))
                {
                    var priceChangeOneDay = oneDayRates.TryGetValue(symbol, out decimal previousPrice) ? calculatePriceChange(previousPrice, currentPrice) : 0;
                    var priceChangeSevenDays = sevenDaysRates.TryGetValue(symbol, out previousPrice) ? calculatePriceChange(previousPrice, currentPrice) : 0;
                    var priceChangeThirtyDays = thirtyDaysRates.TryGetValue(symbol, out previousPrice) ? calculatePriceChange(previousPrice, currentPrice) : 0;

                    currencies[symbol] = new Currency
                    {
                        Symbol = symbol,
                        Name = name,
                        Price = currentPrice,
                        PriceChangeOneDay = priceChangeOneDay,
                        PriceChangeSevenDays = priceChangeSevenDays,
                        PriceChangeThirtyDays = priceChangeThirtyDays
                    };
                }
            }

            return currencies;
        }

        private class OrdinalIgnoreCaseDictionary<T> : Dictionary<string, T>
        {
            public OrdinalIgnoreCaseDictionary() : base(StringComparer.OrdinalIgnoreCase)
            {
            }
        }
    }
}
