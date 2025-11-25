using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.Services;
using Sharemarketsimulation.Extensions;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Broker,Admin")]
    public class CommissionController : ControllerBase
    {
        private readonly CommissionService _commissionService;

        public CommissionController(CommissionService commissionService)
        {
            _commissionService = commissionService;
        }

        [HttpPost("{stockSymbol}")]
        public IActionResult SetCommissionRate(string stockSymbol, [FromBody] decimal rate)
        {
            try
            {
                if (string.IsNullOrEmpty(stockSymbol)) return BadRequest(new { message = "Stock symbol is required" });
                _commissionService.SetCommissionRate(stockSymbol, rate);
                return Ok(new { message = "Commission rate set successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{stockSymbol}")]
        public IActionResult UpdateCommissionRate(string stockSymbol, [FromBody] decimal rate)
        {
            try
            {
                if (string.IsNullOrEmpty(stockSymbol)) return BadRequest(new { message = "Stock symbol is required" });
                _commissionService.UpdateCommissionRate(stockSymbol, rate);
                return Ok(new { message = "Commission rate updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommissionRates()
        {
            var rates = await _commissionService.GetAllCommissionRatesAsync();
            return Ok(rates);
        }

        [HttpGet("{stockSymbol}")]
        public IActionResult GetCommissionRate(string stockSymbol)
        {
            try
            {
                if (string.IsNullOrEmpty(stockSymbol)) return BadRequest(new { message = "Stock symbol is required" });
                var rate = _commissionService.GetCommissionRate(stockSymbol);
                return Ok(new { stockSymbol, rate });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{stockSymbol}")]
        public IActionResult RemoveCommissionRate(string stockSymbol)
        {
            try
            {
                if (string.IsNullOrEmpty(stockSymbol)) return BadRequest(new { message = "Stock symbol is required" });
                var removed = _commissionService.RemoveCommissionRate(stockSymbol);
                return removed ? Ok(new { message = "Commission rate removed successfully" }) : NotFound(new { message = "Commission rate not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("requests/approve")]
        public IActionResult ApproveCommissionRequest([FromBody] ApproveRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.StockSymbol)) return BadRequest(new { message = "Stock symbol is required" });
                _commissionService.SetCommissionRate(request.StockSymbol, request.Rate);
                return Ok(new { message = "Commission rate approved and updated", stockSymbol = request.StockSymbol, newRate = request.Rate });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("requests/reject")]
        public IActionResult RejectCommissionRequest([FromBody] RejectRequestDto request)
        {
            return Ok(new { message = "Commission rate request rejected", reason = request.Reason });
        }
    }

    public class ApproveRequestDto
    {
        public string StockSymbol { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }

    public class RejectRequestDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}