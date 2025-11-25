using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Repository
{
    public interface IStockPriceHistoryRepository
    {
        Task<StockPriceHistory?> GetByIdAsync(int id);
        Task<IEnumerable<StockPriceHistory>> GetByStockIdAsync(int stockId);
        Task<IEnumerable<StockPriceHistory>> GetByDateRangeAsync(int stockId, DateTime fromDate, DateTime toDate);
        Task<StockPriceHistory> CreateAsync(StockPriceHistory priceHistory);
        Task<StockPriceHistory?> GetLatestPriceAsync(int stockId);
        Task<IEnumerable<StockPriceHistory>> GetLastNDaysAsync(int stockId, int days);
        Task<bool> ExistsForDateAsync(int stockId, DateTime date);
    }
}