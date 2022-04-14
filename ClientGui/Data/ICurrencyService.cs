using Common.Models;

namespace ClientGui.Data
{
    public static class CurrencyServiceType
    {
        public const string Fiat = "Fiat";
        public const string Crypto = "Crypto";
    }

    public interface ICurrencyService
    {
        string Type { get; init; }

        Task<Currency?> Get(ReferenceCurrency baseCurrency, string symbol);

        Task<CurrencySet?> Get(ReferenceCurrency baseCurrency);
    }
}
