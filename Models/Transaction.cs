using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int StockId { get; set; }
        
        public int? OrderId { get; set; }
        
        [Required]
        public TransactionType TransactionType { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        public decimal Commission { get; set; }
        
        public decimal Tax { get; set; }
        
        public decimal NetAmount { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        public string? Notes { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
        public virtual Order? Order { get; set; }
    }

    public enum TransactionType
    {
        Buy = 1,
        Sell = 2,
        Dividend = 3,
        Bonus = 4,
        Split = 5
    }
}