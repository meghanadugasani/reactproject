using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class Stock
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string? Symbol { get; set; } 
        
        [Required]
        [StringLength(200)]
        public string? CompanyName { get; set; } 
        
        public string? Description { get; set; }
        
        [Required]
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
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
        public virtual ICollection<StockPriceHistory> PriceHistory { get; set; } = new List<StockPriceHistory>();
    }

    public enum StockCategory
    {
        Technology = 1,
        Finance = 2,
        Healthcare = 3,
        Energy = 4,
        ConsumerGoods = 5,
        Telecommunications = 6,
        RealEstate = 7,
        Utilities = 8,
        Materials = 9,
        Industrials = 10
    }
}