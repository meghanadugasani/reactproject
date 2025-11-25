using Microsoft.Extensions.Configuration;
using Sharemarketsimulation.Interfaces.Repository;

namespace Sharemarketsimulation.Services
{
    public class CommissionService
    {
        private readonly IConfiguration _configuration;
        private readonly IStockRepository _stockRepository;
        private static readonly Dictionary<string, decimal> _stockCommissions = new();
        
        public CommissionService(IConfiguration configuration, IStockRepository stockRepository)
        {
            _configuration = configuration;
            _stockRepository = stockRepository;
            LoadFromAppSettings();
        }
        
        private void LoadFromAppSettings()
        {
            var commissionSection = _configuration.GetSection("StockCommissions");
            foreach (var item in commissionSection.GetChildren())
            {
                if (decimal.TryParse(item.Value, out var rate))
                {
                    _stockCommissions[item.Key] = rate;
                }
            }
        }
        
        public decimal GetCommissionRate(string stockSymbol)
        {
            if (_stockCommissions.ContainsKey(stockSymbol))
                return _stockCommissions[stockSymbol];
                
            return _configuration.GetValue<decimal>("StockSettings:CommissionRate", 0.001m);
        }
        
        public void SetCommissionRate(string stockSymbol, decimal rate)
        {
            _stockCommissions[stockSymbol] = rate;
        }
        
        public async Task<Dictionary<string, decimal>> GetAllCommissionRatesAsync()
        {
            var result = new Dictionary<string, decimal>();
            var stocks = await _stockRepository.GetAllAsync();
            
            foreach (var stock in stocks)
            {
                result[stock.Symbol ?? ""] = GetCommissionRate(stock.Symbol ?? "");
            }
            
            return result;
        }
        
        public bool RemoveCommissionRate(string stockSymbol)
        {
            return _stockCommissions.Remove(stockSymbol);
        }
    }
}