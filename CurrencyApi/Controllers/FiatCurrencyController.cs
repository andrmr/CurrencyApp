using Microsoft.AspNetCore.Mvc;

namespace CurrencyApi.Controllers
{
    [ApiController]
    [Route("api/currency/fiat")]
    public sealed class FiatCurrencyController : CurrencyControllerBase
    {
        public FiatCurrencyController(IFiatCurrencyService fiatCurrencyService) : base(fiatCurrencyService) { }
    }
}
