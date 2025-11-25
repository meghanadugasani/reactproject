using Microsoft.EntityFrameworkCore;
using Sharemarketsimulation.Data;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly StockMarketDbContext _context;

        public StockRepository(StockMarketDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.FindAsync(id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            return await _context.Stocks.ToListAsync();
        }

        public async Task<IEnumerable<Stock>> GetActiveAsync()
        {
            return await _context.Stocks.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Stock>> GetByCategoryAsync(StockCategory category)
        {
            return await _context.Stocks.Where(s => s.Category == category && s.IsActive).ToListAsync();
        }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<Stock> UpdateAsync(Stock stock)
        {
            _context.Stocks.Update(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var stock = await GetByIdAsync(id);
            if (stock == null) return false;
            
            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePriceAsync(int stockId, decimal newPrice)
        {
            var stock = await GetByIdAsync(stockId);
            if (stock == null) return false;

            stock.PreviousClose = stock.CurrentPrice;
            stock.CurrentPrice = newPrice;
            stock.PriceChange = newPrice - stock.PreviousClose;
            stock.PriceChangePercent = stock.PreviousClose != 0 ? (stock.PriceChange / stock.PreviousClose) * 100 : 0;
            stock.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Stock>> SearchAsync(string searchTerm)
        {
            return await _context.Stocks
                .Where(s => s.Symbol.Contains(searchTerm) || s.CompanyName.Contains(searchTerm))
                .Where(s => s.IsActive)
                .ToListAsync();
        }
    }
}