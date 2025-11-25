using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Repository
{
    public interface IStockRepository
    {
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<IEnumerable<Stock>> GetAllAsync();
        Task<IEnumerable<Stock>> GetActiveAsync();
        Task<IEnumerable<Stock>> GetByCategoryAsync(StockCategory category);
        Task<Stock> CreateAsync(Stock stock);
        Task<Stock> UpdateAsync(Stock stock);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdatePriceAsync(int stockId, decimal newPrice);
        Task<IEnumerable<Stock>> SearchAsync(string searchTerm);
    }
}