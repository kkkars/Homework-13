using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DepsWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using DepsWebApp.Authentication;

namespace DepsWebApp.Controllers
{
    /// <summary>
    /// Rates controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RatesController : ControllerBase
    {
        private readonly ILogger<RatesController> _logger;
        private readonly IRatesService _rates;

#pragma warning disable CS1591 
        public RatesController(
            IRatesService rates,
            ILogger<RatesController> logger)
        {
            _rates = rates;
            _logger = logger;
        }
#pragma warning restore CS1591

        /// <summary>
        /// Get method that will convert given currency to the destination currency 
        /// </summary>
        /// <param name="srcCurrency">The source currency</param>
        /// <param name="dstCurrency">The destionation currency</param>
        /// <param name="amount">The exchange amount</param>
        /// <returns>Amount in destination currency</returns>
        [HttpGet("{srcCurrency}/{dstCurrency}")]
        public async Task<ActionResult<decimal>> Get(string srcCurrency, string dstCurrency, decimal? amount)
        {
            if (!(HttpContext.User.Identity is AccountIdentity identity))
            {
                return Unauthorized();
            }

            var exchange = await _rates.ExchangeAsync(srcCurrency, dstCurrency, amount ?? decimal.One);
            if (!exchange.HasValue)
            {
                _logger.LogDebug($"Can't exchange from '{srcCurrency}' to '{dstCurrency}'");
                return BadRequest("Invalid currency code");
            }
            return exchange.Value.DestinationAmount;
        }
    }
}