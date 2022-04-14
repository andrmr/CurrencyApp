using System.Text.Json;

namespace GuiMudblazor.Data
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(IConfiguration configuration, string type)
        {
            Type = type;
            
            var baseAddress = configuration["AppSettings:APIs:Currency:BaseAddress"] + configuration[$"AppSettings:APIs:Currency:Routes:{type}"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

        }

        public string Type { get; init; }

        public async Task<Currency?> Get(ReferenceCurrency referenceCurrency, string symbol)
        {
            return await Get<Currency?>($"{referenceCurrency}/{symbol}");
        }

        public async Task<CurrencySet?> Get(ReferenceCurrency referenceCurrency)
        {
            return await Get<CurrencySet?>(referenceCurrency.ToString());
        }

        private async Task<T?> Get<T>(string route)
        {
            var response = await _httpClient.GetAsync(route);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), options);
            }

            return default;
        }
    }
}
