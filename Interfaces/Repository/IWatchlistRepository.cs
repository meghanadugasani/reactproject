using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Repository
{
    public interface IWatchlistRepository
    {
        Task<Watchlist?> GetByIdAsync(int id);
        Task<IEnumerable<Watchlist>> GetByUserIdAsync(int userId);
        Task<Watchlist?> GetByUserAndStockAsync(int userId, int stockId);
        Task<IEnumerable<Watchlist>> GetActiveAlertsAsync();
        Task<Watchlist> CreateAsync(Watchlist watchlist);
        Task<Watchlist> UpdateAsync(Watchlist watchlist);
        Task<bool> DeleteAsync(int id);
        Task<bool> UserWatchingStockAsync(int userId, int stockId);
        Task<IEnumerable<Watchlist>> GetTriggeredAlertsAsync(int stockId, decimal currentPrice);
    }
}