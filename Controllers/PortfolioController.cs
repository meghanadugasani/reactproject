using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Broker,Admin")] 
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IWatchlistService _watchlistService;
        private readonly IStockService _stockService;

        public PortfolioController(IPortfolioService portfolioService, IWatchlistService watchlistService, IStockService stockService)
        {
            _portfolioService = portfolioService;
            _watchlistService = watchlistService;
            _stockService = stockService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPortfolio(int userId)
        {
            var portfolio = await _portfolioService.GetPortfolioSummaryAsync(userId);
            return Ok(portfolio);
        }

        [HttpGet("{userId}/holdings")]
        public async Task<IActionResult> GetHoldings(int userId)
        {
            var holdings = await _portfolioService.GetUserPortfolioAsync(userId);
            return Ok(holdings);
        }

        [HttpGet("{userId}/performance")]
        public async Task<IActionResult> GetPerformance(int userId)
        {
            var performance = await _portfolioService.GetPortfolioPerformanceAsync(userId);
            return Ok(performance);
        }

        [HttpGet("{userId}/balance")]
        public async Task<IActionResult> GetBalance(int userId)
        {
            var balance = await _portfolioService.GetAvailableBalanceAsync(userId);
            return Ok(new { balance });
        }

        [HttpGet("{userId}/analytics")]
        public async Task<IActionResult> GetPortfolioAnalytics(int userId)
        {
            var dayChange = await _portfolioService.GetDayChangeAsync(userId);
            var diversificationScore = await _portfolioService.GetDiversificationScoreAsync(userId);
            var riskScore = await _portfolioService.GetPortfolioRiskAsync(userId);

            return Ok(new 
            {
                dayChange,
                diversificationScore,
                riskScore,
                generatedAt = DateTime.UtcNow
            });
        }

        [HttpGet("{userId}/top-holdings")]
        public async Task<IActionResult> GetTopHoldings(int userId, [FromQuery] int count = 5)
        {
            var topHoldings = await _portfolioService.GetTopHoldingsAsync(userId, count);
            return Ok(topHoldings);
        }

        [HttpGet("{userId}/gainers")]
        public async Task<IActionResult> GetGainers(int userId)
        {
            var gainers = await _portfolioService.GetGainersAsync(userId);
            return Ok(gainers);
        }

        [HttpGet("{userId}/losers")]
        public async Task<IActionResult> GetLosers(int userId)
        {
            var losers = await _portfolioService.GetLosersAsync(userId);
            return Ok(losers);
        }

        [HttpPost("{userId}/rebalance")]
        public async Task<IActionResult> RebalancePortfolio(int userId)
        {
            var success = await _portfolioService.RebalancePortfolioAsync(userId);
            if (!success)
                return BadRequest(new { message = "Rebalancing failed" });

            return Ok(new { message = "Portfolio rebalanced successfully" });
        }

        [HttpPost("dashboard")]
        public async Task<IActionResult> GetDashboardData([FromBody] DashboardRequest request)
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            
            var portfolio = await _portfolioService.GetPortfolioSummaryAsync(request.UserId);
            var holdings = await _portfolioService.GetUserPortfolioAsync(request.UserId);
            var watchlist = await _watchlistService.GetUserWatchlistAsync(request.UserId);
            var stocks = await _stockService.GetActiveStocksAsync();
            
            return Ok(new 
            {
                portfolio,
                holdings,
                watchlist,
                availableStocks = stocks
            });
        }

        public class DashboardRequest
        {
            public int UserId { get; set; }
            public string RequestName { get; set; } = "UserDashboard";
        }

        [HttpGet("app-summary")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> GetAppPortfolioSummary()
        {
            var summary = await _portfolioService.GetAppPortfolioSummaryAsync();
            return Ok(summary);
        }
    }
}