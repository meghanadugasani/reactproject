using Sharemarketsimulation.DTOs;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface IWatchlistService
    {
        Task<IEnumerable<WatchlistDto>> GetUserWatchlistAsync(int userId);
        Task<WatchlistDto> AddToWatchlistAsync(int userId, CreateWatchlistDto createDto);
        Task<WatchlistDto> UpdateWatchlistAsync(int watchlistId, UpdateWatchlistDto updateDto);
        Task<bool> RemoveFromWatchlistAsync(int watchlistId, int userId);
        Task<bool> IsStockInWatchlistAsync(int userId, int stockId);
        Task CheckAndTriggerAlertsAsync(int stockId, decimal currentPrice);
        Task<IEnumerable<WatchlistDto>> GetTriggeredAlertsAsync(int userId);
        Task<bool> SetPriceAlertAsync(int userId, int stockId, decimal alertPrice, Models.AlertType alertType);
        Task<bool> DisableAlertAsync(int watchlistId);
        Task<bool> EnableAlertAsync(int watchlistId);
        Task<IEnumerable<WatchlistDto>> GetWatchlistByCategoryAsync(int userId, Models.StockCategory category);
        Task<int> GetWatchlistCountAsync(int userId);
        Task<bool> ClearWatchlistAsync(int userId);
        Task<IEnumerable<WatchlistDto>> GetActiveAlertsAsync(int userId);
    }
}