using Microsoft.EntityFrameworkCore;
using Sharemarketsimulation.Data;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Repositories
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly StockMarketDbContext _context;

        public WatchlistRepository(StockMarketDbContext context)
        {
            _context = context;
        }

        public async Task<Watchlist?> GetByIdAsync(int id)
        {
            return await _context.Watchlists
                .Include(w => w.Stock)
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Watchlist>> GetByUserIdAsync(int userId)
        {
            return await _context.Watchlists
                .Include(w => w.Stock)
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<Watchlist?> GetByUserAndStockAsync(int userId, int stockId)
        {
            return await _context.Watchlists
                .Include(w => w.Stock)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.StockId == stockId);
        }

        public async Task<IEnumerable<Watchlist>> GetActiveAlertsAsync()
        {
            return await _context.Watchlists
                .Include(w => w.Stock)
                .Where(w => w.IsAlertActive && w.AlertPrice.HasValue)
                .ToListAsync();
        }

        public async Task<Watchlist> CreateAsync(Watchlist watchlist)
        {
            _context.Watchlists.Add(watchlist);
            await _context.SaveChangesAsync();
            return watchlist;
        }

        public async Task<Watchlist> UpdateAsync(Watchlist watchlist)
        {
            _context.Watchlists.Update(watchlist);
            await _context.SaveChangesAsync();
            return watchlist;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var watchlist = await _context.Watchlists.FindAsync(id);
            if (watchlist == null) return false;
            
            _context.Watchlists.Remove(watchlist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserWatchingStockAsync(int userId, int stockId)
        {
            return await _context.Watchlists
                .AnyAsync(w => w.UserId == userId && w.StockId == stockId);
        }

        public async Task<IEnumerable<Watchlist>> GetTriggeredAlertsAsync(int stockId, decimal currentPrice)
        {
            return await _context.Watchlists
                .Include(w => w.Stock)
                .Include(w => w.User)
                .Where(w => w.StockId == stockId && w.IsAlertActive && w.AlertPrice.HasValue &&
                    ((w.AlertType == AlertType.Above && currentPrice >= w.AlertPrice) ||
                     (w.AlertType == AlertType.Below && currentPrice <= w.AlertPrice)))
                .ToListAsync();
        }
    }
}