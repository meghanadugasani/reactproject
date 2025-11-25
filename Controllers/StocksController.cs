using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _stockService.GetActiveStocksAsync();
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStock(int id)
        {
            var stock = await _stockService.GetStockByIdAsync(id);
            if (stock == null)
                return NotFound(new { message = "Stock not found" });

            return Ok(stock);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStock(CreateStockDto createDto)
        {
            try
            {
                var stock = await _stockService.CreateStockAsync(createDto);
                return CreatedAtAction(nameof(GetStock), new { id = stock.Id }, stock);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("updateprice")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> UpdatePrice(UpdateStockPriceDto updateDto)
        {
            var success = await _stockService.UpdateStockPriceAsync(updateDto);
            if (!success)
                return NotFound(new { message = "Stock not found" });

            return Ok(new { message = "Price updated successfully" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStocks([FromQuery] StockSearchDto searchDto)
        {
            var stocks = await _stockService.SearchStocksAsync(searchDto);
            return Ok(stocks);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetStocksByCategory(string category)
        {
            if (!Enum.TryParse<Models.StockCategory>(category, true, out var stockCategory))
                return BadRequest(new { message = "Invalid category. Valid values: Technology, Finance, Healthcare, Energy, ConsumerGoods, Telecommunications, RealEstate, Utilities, Materials, Industrials" });

            var stocks = await _stockService.GetStocksByCategoryAsync(stockCategory);
            return Ok(stocks);
        }
    }
}