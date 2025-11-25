using Sharemarketsimulation.DTOs;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface IPortfolioService
    {
        Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(int userId);
        Task<IEnumerable<PortfolioDto>> GetUserPortfolioAsync(int userId);
        Task<PortfolioPerformanceDto> GetPortfolioPerformanceAsync(int userId);
        Task UpdatePortfolioAsync(int userId, int stockId, int quantity, decimal price, bool isBuy);
        Task RecalculatePortfolioValuesAsync(int userId);
        Task<decimal> GetAvailableBalanceAsync(int userId);
        Task<bool> HasSufficientBalanceAsync(int userId, decimal amount);
        Task<bool> HasSufficientStocksAsync(int userId, int stockId, int quantity);
        Task<decimal> GetPortfolioValueAsync(int userId);
        Task<decimal> GetDayChangeAsync(int userId);
        Task<decimal> GetTotalProfitLossAsync(int userId);
        Task<IEnumerable<PortfolioDto>> GetTopHoldingsAsync(int userId, int count = 5);
        Task<decimal> GetDiversificationScoreAsync(int userId);
        Task<decimal> GetPortfolioRiskAsync(int userId);
        Task<bool> RebalancePortfolioAsync(int userId);
        Task<IEnumerable<PortfolioDto>> GetLosersAsync(int userId);
        Task<IEnumerable<PortfolioDto>> GetGainersAsync(int userId);
        Task<object> GetAppPortfolioSummaryAsync();
    }
}