using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int StockId { get; set; }
        
        [Required]
        public OrderType OrderType { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public decimal? StopLoss { get; set; }
        
        public decimal? Target { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExecutedAt { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        public string? Notes { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
    }

    public enum OrderType
    {
        Buy = 1,
        Sell = 2
    }

    public enum OrderStatus
    {
        Pending = 1,
        Executed = 2,
        Cancelled = 3,
        PartiallyExecuted = 4,
        Expired = 5
    }
}