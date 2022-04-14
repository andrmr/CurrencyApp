namespace CurrencyApi.Services
{
    public abstract class CurrencyServiceBase : ICurrencyService
    {
        private const int DEFAULT_CACHE_TIMEOUT_H = 1;

        protected readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey;
        private readonly TimeSpan _cacheTimeout;
        private readonly SemaphoreSlim _cacheLock = new(1, 1);

        abstract protected Task<CurrencySet?> GetCurrencySetAsync(ReferenceCurrency referenceCurrency, CancellationToken cancellationToken);

        public CurrencyServiceBase(IConfiguration configuration, IMemoryCache cache, string cacheKey)
        {
            _configuration = configuration;
            _cache = cache;
            _cacheKey = cacheKey;

            _configuration.TryGetValue("AppSettings:CacheUpdateIntervalHours", out int cacheTimeoutH, DEFAULT_CACHE_TIMEOUT_H);
            _cacheTimeout = TimeSpan.FromHours(cacheTimeoutH);
        }

        public async Task<Currency?> Get(ReferenceCurrency referenceCurrency, string symbol)
        {
            var db = await GetFromCache();
            return db?.GetValueOrDefault(referenceCurrency)?.GetValueOrDefault(symbol);
        }

        public async Task<CurrencySet?> Get(ReferenceCurrency referenceCurrency)
        {
            var db = await GetFromCache();
            return db?.GetValueOrDefault(referenceCurrency);
        }

        private async Task<CurrencyDb?> GetFromCache()
        {
            if (!_cache.TryGetValue(_cacheKey, out CurrencyDb? db))
            {
                try
                {
                    await _cacheLock.WaitAsync();
                    if (!_cache.TryGetValue(_cacheKey, out db))
                    {
                        db = await BuildCurrencyDbAsync(CancellationToken.None);
                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheTimeout);
                        _cache.Set(_cacheKey, db, cacheEntryOptions);
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }
            }

            return db;
        }

        private async Task<CurrencyDb?> BuildCurrencyDbAsync(CancellationToken cancellationToken)
        {
            var getSet = async (ReferenceCurrency rc) =>
            {
                var set = await GetCurrencySetAsync(rc, cancellationToken);
                return (rc, set);
            };

            var tasks = Enum.GetValues(typeof(ReferenceCurrency)).Cast<ReferenceCurrency>().Select(rc => getSet(rc));
            var sets = await Task.WhenAll(tasks);

            return new CurrencyDb(sets.Where(p => p.set?.Count > 0).ToList().ToDictionary(p => p.rc, p => p.set));
        }
    }
}