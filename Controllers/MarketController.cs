using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IOrderService _orderService;
        private readonly MarketTimingService _marketTimingService;

        public MarketController(IStockService stockService, IOrderService orderService, MarketTimingService marketTimingService)
        {
            _stockService = stockService;
            _orderService = orderService;
            _marketTimingService = marketTimingService;
        }

        [HttpPost("open")]
        [Authorize(Roles = "Admin")] 
        public IActionResult OpenMarket()
        {
            _marketTimingService.OpenMarketManually();
            return Ok(new { message = "Market opened manually - Virtual Money users can now trade", time = DateTime.UtcNow });
        }

        [HttpPost("close")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> CloseMarket()
        {
            
            _marketTimingService.CloseMarketManually();
            await _orderService.ProcessExpiredOrdersAsync();
            return Ok(new { message = "Market closed manually - Virtual Money trading blocked", time = DateTime.UtcNow });
        }

        [HttpGet("status")]
        [AllowAnonymous] 
        public IActionResult GetMarketStatus()
        {
            var status = _marketTimingService.GetMarketStatus();
            return Ok(status);
        }
        
        [HttpPost("reset-automatic")]
        [Authorize(Roles = "Admin")]
        public IActionResult ResetToAutomatic()
        {
            _marketTimingService.ResetToAutomatic();
            return Ok(new { message = "Market control reset to automatic time-based mode", time = DateTime.UtcNow });
        }

        [HttpGet("movers/gainers")]
        public async Task<IActionResult> GetTopGainers([FromQuery] int count = 10)
        {
            var gainers = await _stockService.GetTopGainersAsync(count);
            return Ok(gainers);
        }

        [HttpGet("movers/losers")]
        public async Task<IActionResult> GetTopLosers([FromQuery] int count = 10)
        {
            var losers = await _stockService.GetTopLosersAsync(count);
            return Ok(losers);
        }

        [HttpGet("movers/active")]
        public async Task<IActionResult> GetMostActive([FromQuery] int count = 10)
        {
            var active = await _stockService.GetMostActiveAsync(count);
            return Ok(active);
        }

        [HttpPost("simulate-prices")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> SimulatePriceChanges([FromQuery] int? count = null)
        {
            try
            {
                var stocks = await _stockService.GetActiveStocksAsync();
                var random = new Random();
                var updates = new List<Sharemarketsimulation.DTOs.UpdateStockPriceDto>();
                var stocksToUpdate = count.HasValue ? stocks.Take(count.Value) : stocks;

                foreach (var stock in stocksToUpdate)
                {
                    var changePercent = (decimal)(random.NextDouble() * 0.1 - 0.05);
                    var newPrice = stock.CurrentPrice * (1 + changePercent);
                    
                    // Ensure price doesn't go below 0.01
                    newPrice = Math.Max(0.01m, Math.Round(newPrice, 2));
                    
                    updates.Add(new Sharemarketsimulation.DTOs.UpdateStockPriceDto
                    {
                        StockId = stock.Id,
                        NewPrice = newPrice,
                        Volume = random.Next(1000, 100000)
                    });
                }

                var success = await _stockService.BulkUpdatePricesAsync(updates);
                if (!success)
                    return BadRequest(new { message = "Failed to update stock prices" });
                    
                return Ok(new { message = "Price simulation completed", updatedStocks = updates.Count, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}