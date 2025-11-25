using Microsoft.EntityFrameworkCore;
using Sharemarketsimulation.Data;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly StockMarketDbContext _context;

        public TransactionRepository(StockMarketDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Stock)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Stock)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByStockIdAsync(int stockId)
        {
            return await _context.Transactions
                .Include(t => t.User)
                .Where(t => t.StockId == stockId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Transactions
                .Where(t => t.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Transactions
                .Include(t => t.Stock)
                .Include(t => t.User)
                .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .ToListAsync();
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionHistoryAsync(int userId, int page, int pageSize)
        {
            return await _context.Transactions
                .Include(t => t.Stock)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCommissionAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .SumAsync(t => t.Commission);
        }

        public async Task<decimal> GetTotalTaxAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .SumAsync(t => t.Tax);
        }
    }
}