using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly IStockService _stockService;

        public ReportsController(IPortfolioService portfolioService, ITransactionService transactionService, 
            IUserService userService, IStockService stockService)
        {
            _portfolioService = portfolioService;
            _transactionService = transactionService;
            _userService = userService;
            _stockService = stockService;
        }

        [HttpGet("pnl/{userId}")]
        [Authorize(Roles = "User,Admin")] // Users can view their own P&L, Admin can view any
        public async Task<IActionResult> GetPnLReport(int userId)
        {
            var portfolio = await _portfolioService.GetPortfolioSummaryAsync(userId);
            var transactions = await _transactionService.GetTransactionSummaryAsync(userId);

            var report = new PnLReportDto
            {
                TotalInvestment = portfolio.TotalInvestment,
                CurrentValue = portfolio.CurrentValue,
                RealizedPnL = 0, // Calculate from sell transactions
                UnrealizedPnL = portfolio.TotalProfitLoss,
                TotalPnL = portfolio.TotalProfitLoss,
                TotalPnLPercent = portfolio.TotalProfitLossPercent,
                ReportDate = DateTime.UtcNow,
                StockBreakdown = portfolio.Holdings.Select(h => new StockPnLDto
                {
                    Symbol = h.StockSymbol,
                    CompanyName = h.CompanyName,
                    Quantity = h.Quantity,
                    AveragePrice = h.AveragePrice,
                    CurrentPrice = h.CurrentPrice,
                    Investment = h.TotalInvestment,
                    CurrentValue = h.CurrentValue,
                    PnL = h.ProfitLoss,
                    PnLPercent = h.ProfitLossPercent
                }).ToList()
            };

            return Ok(report);
        }

        [HttpGet("useractivity")]
        [Authorize(Roles = "Admin,Broker")] // Admin and Broker can view user activity reports
        public async Task<IActionResult> GetUserActivityReport()
        {
            var users = await _userService.GetAllUsersAsync();
            var activities = new List<UserActivityReportDto>();

            foreach (var user in users)
            {
                var transactions = await _transactionService.GetTransactionSummaryAsync(user.Id);
                activities.Add(new UserActivityReportDto
                {
                    UserId = user.Id,
                    UserName = $"{user.FirstName} {user.LastName}",
                    TotalOrders = transactions.TotalTransactions,
                    ExecutedOrders = transactions.TotalTransactions,
                    TotalVolume = transactions.TotalBuyAmount + transactions.TotalSellAmount,
                    TotalCommission = transactions.TotalCommission,
                    LastActivity = DateTime.UtcNow, // Would come from actual last transaction
                    Status = user.IsActive ? "Active" : "Inactive"
                });
            }

            return Ok(activities);
        }

        [HttpGet("market-summary")]
        [Authorize(Roles = "Admin,Broker")] // Admin and Broker can view market summary
        public async Task<IActionResult> GetMarketSummary()
        {
            var stocks = await _stockService.GetAllStocksAsync();
            var users = await _userService.GetAllUsersAsync();
            var marketCap = await _stockService.GetMarketCapAsync();

            var summary = new MarketSummaryDto
            {
                TotalStocks = stocks.Count(),
                ActiveStocks = await _stockService.GetTotalActiveStocksAsync(),
                TotalMarketCap = marketCap,
                TotalVolume = stocks.Sum(s => s.Volume),
                TotalUsers = users.Count(),
                ActiveUsers = users.Count(u => u.IsActive),
                TotalTransactionValue = 0, // Calculate from all transactions
                ReportDate = DateTime.UtcNow
            };

            return Ok(summary);
        }

        [HttpGet("monthly/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetMonthlyReport(int userId, [FromQuery] int year, [FromQuery] int month)
        {
            var report = await _transactionService.GetMonthlyReportAsync(userId, year, month);
            return Ok(report);
        }

        [HttpGet("yearly/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetYearlyReport(int userId, [FromQuery] int year)
        {
            var report = await _transactionService.GetYearlyReportAsync(userId, year);
            return Ok(report);
        }
    }
}