using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStockRepository _stockRepository;

        public PortfolioService(IPortfolioRepository portfolioRepository, IUserRepository userRepository, IStockRepository stockRepository)
        {
            _portfolioRepository = portfolioRepository;
            _userRepository = userRepository;
            _stockRepository = stockRepository;
        }

        public async Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            var user = await _userRepository.GetByIdAsync(userId);

            var totalInvestment = portfolios.Sum(p => p.TotalInvestment);
            var currentValue = portfolios.Sum(p => p.CurrentValue);
            var totalProfitLoss = currentValue - totalInvestment;
            var totalProfitLossPercent = totalInvestment > 0 ? (totalProfitLoss / totalInvestment) * 100 : 0;

            return new PortfolioSummaryDto
            {
                TotalInvestment = totalInvestment,
                CurrentValue = currentValue,
                TotalProfitLoss = totalProfitLoss,
                TotalProfitLossPercent = totalProfitLossPercent,
                AvailableBalance = user?.Balance ?? 0,
                TotalStocks = portfolios.Count(),
                Holdings = portfolios.Select(MapToDto).ToList()
            };
        }

        public async Task<IEnumerable<PortfolioDto>> GetUserPortfolioAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.Select(MapToDto);
        }

        public async Task<PortfolioPerformanceDto> GetPortfolioPerformanceAsync(int userId)
        {
            
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            var totalProfitLoss = portfolios.Sum(p => p.ProfitLoss);

            return new PortfolioPerformanceDto
            {
                DayChange = totalProfitLoss,
                DayChangePercent = portfolios.Sum(p => p.ProfitLossPercent) / Math.Max(portfolios.Count(), 1),
                WeekChange = totalProfitLoss * 1.1m, 
                WeekChangePercent = 0,
                MonthChange = totalProfitLoss * 1.2m,
                MonthChangePercent = 0,
                YearChange = totalProfitLoss * 1.5m,
                YearChangePercent = 0
            };
        }

        public async Task UpdatePortfolioAsync(int userId, int stockId, int quantity, decimal price, bool isBuy)
        {
            var portfolio = await _portfolioRepository.GetByUserAndStockAsync(userId, stockId);
            var stock = await _stockRepository.GetByIdAsync(stockId);

            if (portfolio == null && isBuy)
            {
                // Create new portfolio entry
                portfolio = new Portfolio
                {
                    UserId = userId,
                    StockId = stockId,
                    Quantity = quantity,
                    AveragePrice = price,
                    TotalInvestment = quantity * price,
                    CreatedAt = DateTime.UtcNow
                };
                await _portfolioRepository.CreateAsync(portfolio);
            }
            else if (portfolio != null)
            {
                if (isBuy)
                {
                    // Update existing portfolio for buy
                    var newTotalInvestment = portfolio.TotalInvestment + (quantity * price);
                    var newQuantity = portfolio.Quantity + quantity;
                    portfolio.AveragePrice = newTotalInvestment / newQuantity;
                    portfolio.Quantity = newQuantity;
                    portfolio.TotalInvestment = newTotalInvestment;
                }
                else
                {
                    // Update existing portfolio for sell
                    portfolio.Quantity -= quantity;
                    if (portfolio.Quantity <= 0)
                    {
                        await _portfolioRepository.DeleteAsync(portfolio.Id);
                        return;
                    }
                    portfolio.TotalInvestment = portfolio.Quantity * portfolio.AveragePrice;
                }

                portfolio.UpdatedAt = DateTime.UtcNow;
                await _portfolioRepository.UpdateAsync(portfolio);
            }

            // Update current value and P&L
            if (portfolio != null && stock != null)
            {
                portfolio.CurrentValue = portfolio.Quantity * stock.CurrentPrice;
                portfolio.ProfitLoss = portfolio.CurrentValue - portfolio.TotalInvestment;
                portfolio.ProfitLossPercent = portfolio.TotalInvestment > 0 ? 
                    (portfolio.ProfitLoss / portfolio.TotalInvestment) * 100 : 0;
                await _portfolioRepository.UpdateAsync(portfolio);
            }
        }

        public async Task RecalculatePortfolioValuesAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            
            foreach (var portfolio in portfolios)
            {
                if (portfolio.Stock != null)
                {
                    portfolio.CurrentValue = portfolio.Quantity * portfolio.Stock.CurrentPrice;
                    portfolio.ProfitLoss = portfolio.CurrentValue - portfolio.TotalInvestment;
                    portfolio.ProfitLossPercent = portfolio.TotalInvestment > 0 ? 
                        (portfolio.ProfitLoss / portfolio.TotalInvestment) * 100 : 0;
                    await _portfolioRepository.UpdateAsync(portfolio);
                }
            }
        }

        public async Task<decimal> GetAvailableBalanceAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.Balance ?? 0;
        }

        public async Task<bool> HasSufficientBalanceAsync(int userId, decimal amount)
        {
            var balance = await GetAvailableBalanceAsync(userId);
            return balance >= amount;
        }

        public async Task<bool> HasSufficientStocksAsync(int userId, int stockId, int quantity)
        {
            var portfolio = await _portfolioRepository.GetByUserAndStockAsync(userId, stockId);
            return portfolio != null && portfolio.Quantity >= quantity;
        }

        public async Task<decimal> GetPortfolioValueAsync(int userId)
        {
            return await _portfolioRepository.GetTotalPortfolioValueAsync(userId);
        }

        public async Task<decimal> GetDayChangeAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.Sum(p => p.ProfitLoss);
        }

        public async Task<decimal> GetTotalProfitLossAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.Sum(p => p.ProfitLoss);
        }

        public async Task<IEnumerable<PortfolioDto>> GetTopHoldingsAsync(int userId, int count = 5)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.OrderByDescending(p => p.CurrentValue).Take(count).Select(MapToDto);
        }

        public async Task<decimal> GetDiversificationScoreAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.Count() > 0 ? Math.Min(portfolios.Count() * 10, 100) : 0;
        }

        public async Task<decimal> GetPortfolioRiskAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            var avgVolatility = portfolios.Average(p => Math.Abs(p.ProfitLossPercent));
            return Math.Min(avgVolatility, 100);
        }

        public async Task<bool> RebalancePortfolioAsync(int userId)
        {
            await RecalculatePortfolioValuesAsync(userId);
            return true;
        }

        public async Task<IEnumerable<PortfolioDto>> GetLosersAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.Where(p => p.ProfitLoss < 0).Select(MapToDto);
        }

        public async Task<IEnumerable<PortfolioDto>> GetGainersAsync(int userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.Where(p => p.ProfitLoss > 0).Select(MapToDto);
        }

        public async Task<object> GetAppPortfolioSummaryAsync()
        {
            var allPortfolios = await _portfolioRepository.GetAllPortfoliosAsync();
            var totalInvestment = allPortfolios.Sum(p => p.TotalInvestment);
            var totalCurrentValue = allPortfolios.Sum(p => p.CurrentValue);
            var totalProfitLoss = totalCurrentValue - totalInvestment;
            var totalProfitLossPercent = totalInvestment > 0 ? (totalProfitLoss / totalInvestment) * 100 : 0;

            return new
            {
                TotalInvestment = totalInvestment,
                TotalCurrentValue = totalCurrentValue,
                TotalProfitLoss = totalProfitLoss,
                TotalProfitLossPercent = totalProfitLossPercent,
                TotalUsers = allPortfolios.Select(p => p.UserId).Distinct().Count(),
                TotalHoldings = allPortfolios.Count(),
                TotalStocks = allPortfolios.Select(p => p.StockId).Distinct().Count()
            };
        }

        private static PortfolioDto MapToDto(Portfolio portfolio)
        {
            return new PortfolioDto
            {
                Id = portfolio.Id,
                UserId = portfolio.UserId,
                StockId = portfolio.StockId,
                StockSymbol = portfolio.Stock?.Symbol ?? "",
                CompanyName = portfolio.Stock?.CompanyName ?? "",
                Quantity = portfolio.Quantity,
                AveragePrice = portfolio.AveragePrice,
                CurrentPrice = portfolio.Stock?.CurrentPrice ?? 0,
                TotalInvestment = portfolio.TotalInvestment,
                CurrentValue = portfolio.CurrentValue,
                ProfitLoss = portfolio.ProfitLoss,
                ProfitLossPercent = portfolio.ProfitLossPercent,
                CreatedAt = portfolio.CreatedAt,
                UpdatedAt = portfolio.UpdatedAt
            };
        }
    }
}