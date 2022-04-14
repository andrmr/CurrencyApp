using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CurrencyApi.Controllers
{
    [ApiController]
    [Route("api/currency/[controller]")]
    public abstract class CurrencyControllerBase : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyControllerBase(ICurrencyService currencyService) =>
            _currencyService = currencyService;

        [HttpGet("{referenceCurrency}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<CurrencySet?>> Get(ReferenceCurrency referenceCurrency)
        {
            var result = await _currencyService.Get(referenceCurrency);
            return Ok(result);
        }

        [HttpGet("{referenceCurrency}/{symbol}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Currency?>> Get(ReferenceCurrency referenceCurrency, string symbol)
        {
            var result = await _currencyService.Get(referenceCurrency, symbol);
            return Ok(result);
        }
    }
}
