using Microsoft.EntityFrameworkCore;
using Sharemarketsimulation.Data;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly StockMarketDbContext _context;

        public PortfolioRepository(StockMarketDbContext context)
        {
            _context = context;
        }

        public async Task<Portfolio?> GetByIdAsync(int id)
        {
            return await _context.Portfolios
                .Include(p => p.Stock)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Portfolio>> GetByUserIdAsync(int userId)
        {
            return await _context.Portfolios
                .Include(p => p.Stock)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Portfolio?> GetByUserAndStockAsync(int userId, int stockId)
        {
            return await _context.Portfolios
                .Include(p => p.Stock)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.StockId == stockId);
        }

        public async Task<IEnumerable<Portfolio>> GetByStockIdAsync(int stockId)
        {
            return await _context.Portfolios
                .Include(p => p.Stock)
                .Where(p => p.StockId == stockId)
                .ToListAsync();
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio> UpdateAsync(Portfolio portfolio)
        {
            _context.Portfolios.Update(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null) return false;
            
            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalPortfolioValueAsync(int userId)
        {
            return await _context.Portfolios
                .Where(p => p.UserId == userId)
                .SumAsync(p => p.CurrentValue);
        }

        public async Task<decimal> GetTotalInvestmentAsync(int userId)
        {
            return await _context.Portfolios
                .Where(p => p.UserId == userId)
                .SumAsync(p => p.TotalInvestment);
        }

        public async Task<bool> UserOwnsStockAsync(int userId, int stockId)
        {
            return await _context.Portfolios
                .AnyAsync(p => p.UserId == userId && p.StockId == stockId && p.Quantity > 0);
        }

        public async Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync()
        {
            return await _context.Portfolios
                .Include(p => p.Stock)
                .Include(p => p.User)
                .ToListAsync();
        }
    }
}