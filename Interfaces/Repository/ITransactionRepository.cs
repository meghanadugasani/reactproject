using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Repository
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Transaction>> GetByStockIdAsync(int stockId);
        Task<IEnumerable<Transaction>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetUserTransactionHistoryAsync(int userId, int page, int pageSize);
        Task<decimal> GetTotalCommissionAsync(int userId);
        Task<decimal> GetTotalTaxAsync(int userId);
    }
}