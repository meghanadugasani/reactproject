using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Repository
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> GetByIdAsync(int id);
        Task<IEnumerable<Portfolio>> GetByUserIdAsync(int userId);
        Task<Portfolio?> GetByUserAndStockAsync(int userId, int stockId);
        Task<IEnumerable<Portfolio>> GetByStockIdAsync(int stockId);
        Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();
        Task<Portfolio> CreateAsync(Portfolio portfolio);
        Task<Portfolio> UpdateAsync(Portfolio portfolio);
        Task<bool> DeleteAsync(int id);
        Task<decimal> GetTotalPortfolioValueAsync(int userId);
        Task<decimal> GetTotalInvestmentAsync(int userId);
        Task<bool> UserOwnsStockAsync(int userId, int stockId);
    }
}