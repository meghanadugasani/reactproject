using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class StockPriceHistory
    {
        public int Id { get; set; }
        
        [Required]
        public int StockId { get; set; }
        
        [Required]
        public decimal OpenPrice { get; set; }
        
        [Required]
        public decimal HighPrice { get; set; }
        
        [Required]
        public decimal LowPrice { get; set; }
        
        [Required]
        public decimal ClosePrice { get; set; }
        
        public long Volume { get; set; }
        
        public DateTime Date { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual Stock Stock { get; set; } = null!;
    }
}