using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Broker,Admin")]
    public class BrokerController : ControllerBase
    {
        private readonly BrokerCommissionService _brokerCommissionService;

        public BrokerController(BrokerCommissionService brokerCommissionService)
        {
            _brokerCommissionService = brokerCommissionService;
        }

        [HttpGet("earnings/{brokerId}")]
        public async Task<IActionResult> GetBrokerEarnings(int brokerId)
        {
            var earnings = await _brokerCommissionService.GetBrokerCommissionEarningsAsync(brokerId);
            return Ok(new { brokerId, totalEarnings = earnings, currency = "INR" });
        }

        [HttpGet("all-earnings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBrokerEarnings()
        {
            var earnings = await _brokerCommissionService.GetAllBrokerEarningsAsync();
            return Ok(earnings);
        }
    }
}