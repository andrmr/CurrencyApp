using Microsoft.AspNetCore.Mvc;

namespace CurrencyApi.Controllers
{
    [ApiController]
    [Route("api/currency/crypto")]
    public sealed class CryptoCurrencyController : CurrencyControllerBase
    {
        public CryptoCurrencyController(ICryptoCurrencyService cryptoCurrencyService) : base(cryptoCurrencyService) { }
    }
}
