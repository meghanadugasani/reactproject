using Sharemarketsimulation.DTOs;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface IStockService
    {
        Task<StockDto?> GetStockByIdAsync(int id);
        Task<StockDto?> GetStockBySymbolAsync(string symbol);
        Task<IEnumerable<StockDto>> GetAllStocksAsync();
        Task<IEnumerable<StockDto>> GetActiveStocksAsync();
        Task<IEnumerable<StockDto>> SearchStocksAsync(StockSearchDto searchDto);
        Task<StockDto> CreateStockAsync(CreateStockDto createDto);
        Task<StockDto> UpdateStockAsync(int id, CreateStockDto updateDto);
        Task<bool> DeleteStockAsync(int id);
        Task<bool> UpdateStockPriceAsync(UpdateStockPriceDto updateDto);
        Task<IEnumerable<StockDto>> GetStocksByCategoryAsync(Models.StockCategory category);
        Task<IEnumerable<StockDto>> GetTopGainersAsync(int count = 10);
        Task<IEnumerable<StockDto>> GetTopLosersAsync(int count = 10);
        Task<IEnumerable<StockDto>> GetMostActiveAsync(int count = 10);
        Task<bool> ActivateStockAsync(int stockId);
        Task<bool> DeactivateStockAsync(int stockId);
        Task<decimal> GetMarketCapAsync();
        Task<int> GetTotalActiveStocksAsync();
        Task<bool> BulkUpdatePricesAsync(List<UpdateStockPriceDto> updates);
    }
}