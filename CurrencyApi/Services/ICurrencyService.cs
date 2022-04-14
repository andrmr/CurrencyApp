namespace CurrencyApi.Services
{
    public interface ICurrencyService
    {
        Task<Currency?> Get(ReferenceCurrency referenceCurrency, string symbol);
        Task<CurrencySet?> Get(ReferenceCurrency referenceCurrency);
    }
}
