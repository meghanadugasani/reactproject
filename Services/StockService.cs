using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly CommissionService _commissionService;
        private readonly IPortfolioRepository _portfolioRepository;

        public StockService(IStockRepository stockRepository, CommissionService commissionService, IPortfolioRepository portfolioRepository)
        {
            _stockRepository = stockRepository;
            _commissionService = commissionService;
            _portfolioRepository = portfolioRepository;
        }

        public async Task<StockDto?> GetStockByIdAsync(int id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            return stock == null ? null : MapToDto(stock);
        }

        public async Task<StockDto?> GetStockBySymbolAsync(string symbol)
        {
            var stock = await _stockRepository.GetBySymbolAsync(symbol);
            return stock == null ? null : MapToDto(stock);
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            var stocks = await _stockRepository.GetAllAsync();
            return stocks.Select(MapToDto);
        }

        public async Task<IEnumerable<StockDto>> GetActiveStocksAsync()
        {
            var stocks = await _stockRepository.GetActiveAsync();
            return stocks.Select(MapToDto);
        }

        public async Task<IEnumerable<StockDto>> SearchStocksAsync(StockSearchDto searchDto)
        {
            var stocks = await _stockRepository.GetAllAsync();
            
            var query = stocks.Where(s => s.IsActive);

            if (!string.IsNullOrEmpty(searchDto.Symbol))
                query = query.Where(s => s.Symbol.Contains(searchDto.Symbol, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(searchDto.CompanyName))
                query = query.Where(s => s.CompanyName.Contains(searchDto.CompanyName, StringComparison.OrdinalIgnoreCase));

            if (searchDto.Category.HasValue)
                query = query.Where(s => s.Category == searchDto.Category.Value);

            if (searchDto.MinPrice.HasValue)
                query = query.Where(s => s.CurrentPrice >= searchDto.MinPrice.Value);

            if (searchDto.MaxPrice.HasValue)
                query = query.Where(s => s.CurrentPrice <= searchDto.MaxPrice.Value);

            return query.Select(MapToDto);
        }

        public async Task<StockDto> CreateStockAsync(CreateStockDto createDto)
        {
            var existingStock = await _stockRepository.GetBySymbolAsync(createDto.Symbol);
            if (existingStock != null)
                throw new InvalidOperationException("Stock symbol already exists");

            var stock = new Stock
            {
                Symbol = createDto.Symbol,
                CompanyName = createDto.CompanyName,
                Description = createDto.Description,
                CurrentPrice = createDto.CurrentPrice,
                OpenPrice = createDto.CurrentPrice,
                HighPrice = createDto.CurrentPrice,
                LowPrice = createDto.CurrentPrice,
                PreviousClose = createDto.CurrentPrice,
                Category = createDto.Category,
                LogoUrl = createDto.LogoUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            var createdStock = await _stockRepository.CreateAsync(stock);
            return MapToDto(createdStock);
        }

        public async Task<StockDto> UpdateStockAsync(int id, CreateStockDto updateDto)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            if (stock == null) throw new ArgumentException("Stock not found");

            stock.CompanyName = updateDto.CompanyName;
            stock.Description = updateDto.Description;
            stock.Category = updateDto.Category;
            stock.LogoUrl = updateDto.LogoUrl;
            stock.LastUpdated = DateTime.UtcNow;

            var updatedStock = await _stockRepository.UpdateAsync(stock);
            return MapToDto(updatedStock);
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            return await _stockRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateStockPriceAsync(UpdateStockPriceDto updateDto)
        {
            var success = await _stockRepository.UpdatePriceAsync(updateDto.StockId, updateDto.NewPrice);
            
            if (success)
            {
                // Update all portfolios that hold this stock
                var portfolios = await _portfolioRepository.GetByStockIdAsync(updateDto.StockId);
                foreach (var portfolio in portfolios)
                {
                    portfolio.CurrentValue = portfolio.Quantity * updateDto.NewPrice;
                    portfolio.ProfitLoss = portfolio.CurrentValue - portfolio.TotalInvestment;
                    portfolio.ProfitLossPercent = portfolio.TotalInvestment > 0 ? 
                        (portfolio.ProfitLoss / portfolio.TotalInvestment) * 100 : 0;
                    portfolio.UpdatedAt = DateTime.UtcNow;
                    await _portfolioRepository.UpdateAsync(portfolio);
                }
            }
            
            return success;
        }

        public async Task<IEnumerable<StockDto>> GetStocksByCategoryAsync(StockCategory category)
        {
            var stocks = await _stockRepository.GetByCategoryAsync(category);
            return stocks.Select(MapToDto);
        }

        public async Task<IEnumerable<StockDto>> GetTopGainersAsync(int count = 10)
        {
            var stocks = await _stockRepository.GetActiveAsync();
            return stocks.OrderByDescending(s => s.PriceChangePercent).Take(count).Select(MapToDto);
        }

        public async Task<IEnumerable<StockDto>> GetTopLosersAsync(int count = 10)
        {
            var stocks = await _stockRepository.GetActiveAsync();
            return stocks.OrderBy(s => s.PriceChangePercent).Take(count).Select(MapToDto);
        }

        public async Task<IEnumerable<StockDto>> GetMostActiveAsync(int count = 10)
        {
            var stocks = await _stockRepository.GetActiveAsync();
            return stocks.OrderByDescending(s => s.Volume).Take(count).Select(MapToDto);
        }

        public async Task<bool> ActivateStockAsync(int stockId)
        {
            var stock = await _stockRepository.GetByIdAsync(stockId);
            if (stock == null) return false;
            stock.IsActive = true;
            await _stockRepository.UpdateAsync(stock);
            return true;
        }

        public async Task<bool> DeactivateStockAsync(int stockId)
        {
            var stock = await _stockRepository.GetByIdAsync(stockId);
            if (stock == null) return false;
            stock.IsActive = false;
            await _stockRepository.UpdateAsync(stock);
            return true;
        }

        public async Task<decimal> GetMarketCapAsync()
        {
            var stocks = await _stockRepository.GetActiveAsync();
            return stocks.Sum(s => s.MarketCap);
        }

        public async Task<int> GetTotalActiveStocksAsync()
        {
            var stocks = await _stockRepository.GetActiveAsync();
            return stocks.Count();
        }

        public async Task<bool> BulkUpdatePricesAsync(List<UpdateStockPriceDto> updates)
        {
            foreach (var update in updates)
            {
                await _stockRepository.UpdatePriceAsync(update.StockId, update.NewPrice);
            }
            return true;
        }

        private StockDto MapToDto(Stock stock)
        {
            return new StockDto
            {
                Id = stock.Id,
                Symbol = stock.Symbol ?? string.Empty,
                CompanyName = stock.CompanyName ?? string.Empty,
                Description = stock.Description,
                CurrentPrice = stock.CurrentPrice,
                OpenPrice = stock.OpenPrice,
                HighPrice = stock.HighPrice,
                LowPrice = stock.LowPrice,
                PreviousClose = stock.PreviousClose,
                Volume = stock.Volume,
                MarketCap = stock.MarketCap,
                PriceChange = stock.PriceChange,
                PriceChangePercent = stock.PriceChangePercent,
                Category = stock.Category,
                LogoUrl = stock.LogoUrl,
                LastUpdated = stock.LastUpdated,
                CommissionRate = _commissionService.GetCommissionRate(stock.Symbol ?? string.Empty)
            };
        }
    }
}