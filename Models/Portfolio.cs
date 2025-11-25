using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int StockId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal AveragePrice { get; set; }
        
        public decimal TotalInvestment { get; set; }
        
        public decimal CurrentValue { get; set; }
        
        public decimal ProfitLoss { get; set; }
        
        public decimal ProfitLossPercent { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
    }
}