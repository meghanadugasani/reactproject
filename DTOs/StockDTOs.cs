using System.ComponentModel.DataAnnotations;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.DTOs
{
    public class StockDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal PreviousClose { get; set; }
        public long Volume { get; set; }
        public long MarketCap { get; set; }
        public decimal PriceChange { get; set; }
        public decimal PriceChangePercent { get; set; }
        public StockCategory Category { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal CommissionRate { get; set; }
    }

    public class CreateStockDto
    {
        [Required]
        [StringLength(10)]
        public string Symbol { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal CurrentPrice { get; set; }
        
        public StockCategory Category { get; set; }
        
        public string? LogoUrl { get; set; }
    }

    public class UpdateStockPriceDto
    {
        [Required]
        public int StockId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal NewPrice { get; set; }
        
        public long Volume { get; set; }
    }

    public class StockSearchDto
    {
        public string? Symbol { get; set; }
        public string? CompanyName { get; set; }
        public StockCategory? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}