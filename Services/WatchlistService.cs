using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly IWatchlistRepository _watchlistRepository;

        public WatchlistService(IWatchlistRepository watchlistRepository)
        {
            _watchlistRepository = watchlistRepository;
        }

        public async Task<IEnumerable<WatchlistDto>> GetUserWatchlistAsync(int userId)
        {
            var watchlists = await _watchlistRepository.GetByUserIdAsync(userId);
            return watchlists.Select(MapToDto);
        }

        public async Task<WatchlistDto> AddToWatchlistAsync(int userId, CreateWatchlistDto createDto)
        {
            var existing = await _watchlistRepository.GetByUserAndStockAsync(userId, createDto.StockId);
            if (existing != null)
                throw new InvalidOperationException("Stock already in watchlist");

            var watchlist = new Watchlist
            {
                UserId = userId,
                StockId = createDto.StockId,
                AlertPrice = createDto.AlertPrice,
                AlertType = createDto.AlertType,
                IsAlertActive = createDto.IsAlertActive,
                AddedAt = DateTime.UtcNow,
                Notes = createDto.Notes
            };

            var createdWatchlist = await _watchlistRepository.CreateAsync(watchlist);
            return MapToDto(createdWatchlist);
        }

        public async Task<WatchlistDto> UpdateWatchlistAsync(int watchlistId, UpdateWatchlistDto updateDto)
        {
            var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
            if (watchlist == null) throw new ArgumentException("Watchlist entry not found");

            if (updateDto.AlertPrice.HasValue) watchlist.AlertPrice = updateDto.AlertPrice.Value;
            if (updateDto.AlertType.HasValue) watchlist.AlertType = updateDto.AlertType.Value;
            if (updateDto.IsAlertActive.HasValue) watchlist.IsAlertActive = updateDto.IsAlertActive.Value;
            if (!string.IsNullOrEmpty(updateDto.Notes)) watchlist.Notes = updateDto.Notes;

            var updatedWatchlist = await _watchlistRepository.UpdateAsync(watchlist);
            return MapToDto(updatedWatchlist);
        }

        public async Task<bool> RemoveFromWatchlistAsync(int watchlistId, int userId)
        {
            var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
            if (watchlist == null || watchlist.UserId != userId) return false;

            return await _watchlistRepository.DeleteAsync(watchlistId);
        }

        public async Task<bool> IsStockInWatchlistAsync(int userId, int stockId)
        {
            return await _watchlistRepository.UserWatchingStockAsync(userId, stockId);
        }

        public async Task CheckAndTriggerAlertsAsync(int stockId, decimal currentPrice)
        {
            var triggeredAlerts = await _watchlistRepository.GetTriggeredAlertsAsync(stockId, currentPrice);
            
            foreach (var alert in triggeredAlerts)
            {
                
                alert.IsAlertActive = false;
                await _watchlistRepository.UpdateAsync(alert);
            }
        }

        public async Task<IEnumerable<WatchlistDto>> GetTriggeredAlertsAsync(int userId)
        {
            var userWatchlist = await _watchlistRepository.GetByUserIdAsync(userId);
            var triggeredAlerts = new List<Watchlist>();

            foreach (var watchlist in userWatchlist)
            {
                if (watchlist.IsAlertActive && watchlist.AlertPrice.HasValue && watchlist.Stock != null)
                {
                    var currentPrice = watchlist.Stock.CurrentPrice;
                    var isTriggered = (watchlist.AlertType == AlertType.Above && currentPrice >= watchlist.AlertPrice) ||
                                    (watchlist.AlertType == AlertType.Below && currentPrice <= watchlist.AlertPrice);
                    
                    if (isTriggered)
                        triggeredAlerts.Add(watchlist);
                }
            }

            return triggeredAlerts.Select(MapToDto);
        }

        public async Task<bool> SetPriceAlertAsync(int userId, int stockId, decimal alertPrice, Models.AlertType alertType)
        {
            var watchlist = await _watchlistRepository.GetByUserAndStockAsync(userId, stockId);
            if (watchlist == null) return false;
            
            watchlist.AlertPrice = alertPrice;
            watchlist.AlertType = alertType;
            watchlist.IsAlertActive = true;
            await _watchlistRepository.UpdateAsync(watchlist);
            return true;
        }

        public async Task<bool> DisableAlertAsync(int watchlistId)
        {
            var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
            if (watchlist == null) return false;
            
            watchlist.IsAlertActive = false;
            await _watchlistRepository.UpdateAsync(watchlist);
            return true;
        }

        public async Task<bool> EnableAlertAsync(int watchlistId)
        {
            var watchlist = await _watchlistRepository.GetByIdAsync(watchlistId);
            if (watchlist == null) return false;
            
            watchlist.IsAlertActive = true;
            await _watchlistRepository.UpdateAsync(watchlist);
            return true;
        }

        public async Task<IEnumerable<WatchlistDto>> GetWatchlistByCategoryAsync(int userId, Models.StockCategory category)
        {
            var watchlists = await _watchlistRepository.GetByUserIdAsync(userId);
            return watchlists.Where(w => w.Stock?.Category == category).Select(MapToDto);
        }

        public async Task<int> GetWatchlistCountAsync(int userId)
        {
            var watchlists = await _watchlistRepository.GetByUserIdAsync(userId);
            return watchlists.Count();
        }

        public async Task<bool> ClearWatchlistAsync(int userId)
        {
            var watchlists = await _watchlistRepository.GetByUserIdAsync(userId);
            foreach (var watchlist in watchlists)
            {
                await _watchlistRepository.DeleteAsync(watchlist.Id);
            }
            return true;
        }

        public async Task<IEnumerable<WatchlistDto>> GetActiveAlertsAsync(int userId)
        {
            var watchlists = await _watchlistRepository.GetByUserIdAsync(userId);
            return watchlists.Where(w => w.IsAlertActive).Select(MapToDto);
        }

        private static WatchlistDto MapToDto(Watchlist watchlist)
        {
            return new WatchlistDto
            {
                Id = watchlist.Id,
                UserId = watchlist.UserId,
                StockId = watchlist.StockId,
                StockSymbol = watchlist.Stock?.Symbol ?? "",
                CompanyName = watchlist.Stock?.CompanyName ?? "",
                CurrentPrice = watchlist.Stock?.CurrentPrice ?? 0,
                PriceChange = watchlist.Stock?.PriceChange ?? 0,
                PriceChangePercent = watchlist.Stock?.PriceChangePercent ?? 0,
                AlertPrice = watchlist.AlertPrice,
                AlertType = watchlist.AlertType,
                IsAlertActive = watchlist.IsAlertActive,
                AddedAt = watchlist.AddedAt,
                Notes = watchlist.Notes
            };
        }
    }
}