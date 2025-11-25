using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class Watchlist
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int StockId { get; set; }
        
        public decimal? AlertPrice { get; set; }
        
        public AlertType? AlertType { get; set; }
        
        public bool IsAlertActive { get; set; } = false;
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        
        public string? Notes { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
    }

    public enum AlertType
    {
        Above = 1,
        Below = 2
    }
}